using UnityEngine;

[RequireComponent(typeof(NetworkTransformReceive), typeof(CharacterController))]
public class NetworkCharacterControllerReceive : MonoBehaviour
{
    private CharacterController _characterController;
    private NetworkTransformReceive _networkTransformReceive;
    private Entity _entity;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _distanceToDestination;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _destination;
    [SerializeField] private Vector3 _lastDestination;
    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _moveSpeedMultiplier = 1f;
    [SerializeField] private float _stopAtRange = 0;
    [SerializeField] private float _stopAtRangeDefault = 0.05f;



    public long lastUpdateTimestamp = 0;
    [SerializeField] private float _verticalVelocity = 0;
    [SerializeField] public bool _isJumping = false;
    public Vector3 MoveDirection { get { return _direction; } set { _direction = value; } }

    void Start()
    {
        if (World.Instance.OfflineMode)
        {
            this.enabled = false;
        }
        _entity = GetComponent<Entity>();
        _networkTransformReceive = GetComponent<NetworkTransformReceive>();
        _characterController = GetComponent<CharacterController>();

        _direction = Vector3.zero;
        _destination = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // if (!_networkTransformReceive.IsPositionSynced())
        // {
        //     /* pause script during position sync */
        //     return;
        // }

        if (_destination != null && _destination != Vector3.zero)
        {
            CheckIfDestinationReached();
        }
        Vector3 ajustedDirection = _direction * _speed * _moveSpeedMultiplier + Vector3.down;
        ajustedDirection = ApplyGravity(ajustedDirection);

        ManageJump();

        _characterController.Move(ajustedDirection * Time.fixedDeltaTime);
    }
    private Vector3 ApplyGravity(Vector3 dir)
    {
        /* Handle gravity */
        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < -1.25f)
            {
                _verticalVelocity = -1.25f;

            }
            // _isJumping = false;

        }
        else
        {
            _verticalVelocity -= _gravity * Time.deltaTime;
            // _isJumping = true;
        }
        dir.y = _verticalVelocity;

        return dir;
    }

    // Player move direction packets
    public void UpdateMoveDirection(Vector3 position, Vector3 direction, float verticalVelocity, long timestamp)
    {
        if (lastUpdateTimestamp > timestamp)
        {
            Debug.LogWarning("UpdateMoveDirection: is outdated " + direction + " timestamp: " + timestamp + " lastUpdateTimestamp: " + lastUpdateTimestamp);
            return;
        }
        _networkTransformReceive.SetNewPosition(position);
        lastUpdateTimestamp = timestamp;

        _speed = _entity.Running ? _entity.Stats.ScaledRunSpeed : _entity.Stats.ScaledWalkSpeed;
        _direction = direction;

        if (direction.x != 0 || direction.z != 0)
        {
            _networkTransformReceive.SetFinalRotation(VectorUtils.CalculateMoveDirectionAngle(direction.x, direction.z));
        }

        if (_characterController.isGrounded)
        {
            if (direction.x != 0 || direction.z != 0)
                _entity.AnimationController.Move();
            else
                _entity.AnimationController.Wait();

            _networkTransformReceive.ResumePositionSync();

            if (verticalVelocity >= 4.0f)
            {
                _isJumping = true;
                _entity.AnimationController.Jump();
                _verticalVelocity = verticalVelocity;
            }
        }
    }

    private void ManageJump()
    {
        if (_isJumping && _verticalVelocity <= 0 && _characterController.isGrounded) // jump is done
        {
            _isJumping = false;
            if (_direction.x != 0 || _direction.z != 0)
            {
                _entity.AnimationController.Move();
            }
            else
            {
                _entity.AnimationController.Wait();
            }
        }
    }

    // Move to destination packets
    public void SetDestination(Vector3 destination, float stopAtRange)
    {
        // Debug.LogWarning("Set destination: " + destination);
        if (_lastDestination == destination)
        {
            return;
        }

        _lastDestination = destination;
        // Debug.LogWarning("SetDestination: " + destination);
        _stopAtRange = stopAtRange > 0 ? stopAtRange + 0.28f : 0; //TODO: Change 0.28f based on entities collision width
        _destination = destination;
        _speed = _entity.Running ? _entity.Stats.ScaledRunSpeed : _entity.Stats.ScaledWalkSpeed;

        Vector3 transformFlat = VectorUtils.To2D(transform.position);
        Vector3 destinationFlat = VectorUtils.To2D(_destination);
        _distanceToDestination = Vector3.Distance(transformFlat, destinationFlat);
        _direction = (destinationFlat - transformFlat).normalized;


        if (_distanceToDestination > _stopAtRangeDefault + stopAtRange)
        {
            _entity.AnimationController.Move();
            _networkTransformReceive.PausePositionSync();
        }
    }

    private void CheckIfDestinationReached()
    {
        _distanceToDestination = Vector3.Distance(VectorUtils.To2D(transform.position), VectorUtils.To2D(_destination));

        if (_distanceToDestination < _stopAtRangeDefault)
        {
            if (_direction != Vector3.zero)
            {
                _entity.OnStopMoving();
                //TODO check if has target and is attacking
            }

            _direction = Vector3.zero;
            _networkTransformReceive.ResumePositionSync();


            // adjust the network position with the attack range
            if (_stopAtRange > 0)
            {
                _networkTransformReceive.SetNewPosition(transform.position);
            }
        }
    }

    public void ResetDestination()
    {
        _destination = transform.position;
        _direction = Vector3.zero;
    }

    public bool IsMoving()
    {
        return !VectorUtils.IsVectorZero2D(_direction);
    }
}
