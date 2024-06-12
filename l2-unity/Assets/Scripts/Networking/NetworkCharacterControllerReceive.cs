using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NetworkAnimationController), typeof(NetworkTransformReceive), typeof(CharacterController))]
public class NetworkCharacterControllerReceive : MonoBehaviour
{
    private CharacterController _characterController;
    private NetworkAnimationController _animationReceive;
    private NetworkTransformReceive _networkTransformReceive;
    private Entity _entity;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _destination;
    [SerializeField] private float _gravity = 28f;
    private float _moveSpeedMultiplier = 1f;

    public Vector3 MoveDirection { get { return _direction; } set { _direction = value; } }
    public NetworkAnimationController NetworkAnimationController { get { return _animationReceive; } }

    void Start() {
        if(World.Instance.OfflineMode) {
            this.enabled = false;
        }
        _entity = GetComponent<Entity>();
        _networkTransformReceive = GetComponent<NetworkTransformReceive>();
        _animationReceive = GetComponent<NetworkAnimationController>();
        _characterController = GetComponent<CharacterController>();

        //adjust movespeed for player entities
        //TODO: Should not need this for players to be synced...
        if (_entity.Identity.EntityType == EntityType.User) {
            _moveSpeedMultiplier = 1.1f;
        }

        if(_characterController == null || World.Instance.OfflineMode || _animationReceive == null) {
            this.enabled = false;
        }

        _direction = Vector3.zero;
        _destination = Vector3.zero;
    }

    private void FixedUpdate() {

        if (!_networkTransformReceive.IsPositionSynced()) {
            /* pause script during position sync */
            return;
        }

        if (_destination != null && _destination != Vector3.zero) {
            SetMoveDirectionToDestination();
        }

        Vector3 ajustedDirection = _direction * _speed * _moveSpeedMultiplier + Vector3.down * _gravity;
        _characterController.Move(ajustedDirection * Time.deltaTime);
    }

    public void UpdateMoveDirection(Vector3 direction) {
        _speed = _entity.Stats.ScaledSpeed;
        _direction = direction;

        if (direction.x != 0 || direction.z != 0) {
            _networkTransformReceive.SetFinalRotation(VectorUtils.CalculateMoveDirectionAngle(direction.x, direction.z));
        }
    }

    public void SetDestination(Vector3 destination) {
        _speed = _entity.Stats.ScaledSpeed;
        _destination = destination;
    }

    public void SetMoveDirectionToDestination() {
        Vector3 transformFlat = VectorUtils.To2D(transform.position);
        Vector3 destinationFlat = VectorUtils.To2D(_destination);

        if(Vector3.Distance(transformFlat, destinationFlat) > Geodata.Instance.NodeSize / 20f) {
            _networkTransformReceive.PausePositionSync();
            _direction = (destinationFlat - transformFlat).normalized;
        } else {
            if(_direction != Vector3.zero) {
                _entity.OnStopMoving();
                //TODO check if has target and is attacking
            }

            _direction = Vector3.zero;
            _networkTransformReceive.ResumePositionSync();
        }
    }

    public bool IsMoving() {
        return !VectorUtils.IsVectorZero2D(_direction);
    }
}
