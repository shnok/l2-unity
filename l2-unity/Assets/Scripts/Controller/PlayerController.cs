using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    /* Components */
    private CharacterController _controller;
    /*Rotate*/
    private float _finalAngle;

    /* Movement */
    [SerializeField] private bool _canMove = true;
    [SerializeField] private Vector3 _moveDirection;
    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _defaultSpeed = 4;
    [SerializeField] private float _measuredSpeed;
    private Vector3 _currentPos;
    private Vector3 _lastPos;
    private Vector2 _axis;

    /* Gravity */
    private float _verticalVelocity = 0;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private float _gravity = 28;

    /* Target */
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private bool _runningToDestination = false;
    [SerializeField] private Transform _lookAtTarget;
    private float _stopAtRange;
    private Vector3 _flatTransformPos;

    public float CurrentSpeed { get { return _currentSpeed; } }
    public float DefaultSpeed { get { return _defaultSpeed; } set { _defaultSpeed = value; } }
    public bool RunningToDestination { get { return _runningToDestination; } }
   // public bool CanMove { get { return _canMove; } }
    public Vector3 MoveDirection { get { return _moveDirection; } }

    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    public void Initialize() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }

    void Start() {
        _controller = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        _flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);

        if(_runningToDestination) {
            if (InputManager.Instance.IsInputPressed(InputType.Move)) {
                ResetDestination();
            }

            if(ShouldRunToDestination(_stopAtRange)) {
                MoveToTargetPosition();
            }
        } else {
            ListenToInputs();
        }

        if(_lookAtTarget != null) {
            UpdateFinalAngleToLookAt(_lookAtTarget);
        } 

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * _finalAngle), Time.deltaTime * 7.5f);

        _moveDirection = ApplyGravity(_moveDirection);
        _controller.Move(_moveDirection * Time.deltaTime);

        MeasureSpeed();
    }

    private bool ShouldRunToDestination(float stopAtRange) {
        return Vector3.Distance(_flatTransformPos, _targetPosition) > stopAtRange; 
    }

    public void SetDestination(Vector3 position, float distance) {
        _runningToDestination = true;
        _stopAtRange = distance;
        _targetPosition = VectorUtils.To2D(position);
    }

    public void ResetDestination() {
        _runningToDestination = false;
        _targetPosition = _flatTransformPos;
        ClickManager.Instance.HideLocator();
    }

    public void ListenToInputs() {
        /* Update input axis */
        _axis = GetAxis();

        /* Speed */
        _currentSpeed = GetInputMoveSpeed(_currentSpeed);

        /* Angle */
        _finalAngle = GetInputRotationValue(_finalAngle);

        /* Direction */
        _moveDirection = GetInputDirection(_currentSpeed);
    }

    private void MeasureSpeed() {
        _currentPos = transform.position;
        _measuredSpeed = (_currentPos - _lastPos).magnitude / Time.deltaTime;
        _lastPos = _currentPos;
    }

    private void MoveToTargetPosition() {
        Vector3 relativeDirection = _targetPosition - _flatTransformPos;

        if(PlayerStateMachine.Instance.CanMove()) {         
            Vector3 relativeAxis = new Vector2(relativeDirection.x, relativeDirection.z);

            // Use Atan2 to calculate the angle in radians
            float angleInRadians = Mathf.Atan2(relativeDirection.x, relativeDirection.z);

            // Convert radians to degrees and adjust for Unity's coordinate system
            float angleInDegrees = Mathf.Rad2Deg * angleInRadians;

            // Ensure the angle is between 0 and 360 degrees
            angleInDegrees = (angleInDegrees + 360) % 360;

            _axis = relativeAxis;
            _finalAngle = angleInDegrees;
            _currentSpeed = _defaultSpeed;
        } else {
            relativeDirection = Vector3.zero;
        }

        _moveDirection = relativeDirection.normalized * _currentSpeed;
    }

    public Vector2 GetAxis() {
        Vector2 localAxis;
        if(InputManager.Instance.IsInputPressed(InputType.MoveForward) && PlayerStateMachine.Instance.CanMove()) {
            LookForward(true);
            localAxis = Vector2.up;
        } else {
            localAxis = Vector2.zero;
        }
        localAxis = (localAxis + InputManager.Instance.inputAxis);
        localAxis = new Vector2(Mathf.Clamp(localAxis.x, -1f, 1f), Mathf.Clamp(localAxis.y, -1f, 1f));

        return localAxis;
    }

    private float GetInputRotationValue(float angle) {
        if(InputManager.Instance.IsInputPressed(InputType.InputAxis) && PlayerStateMachine.Instance.CanMove()) {
            angle = Mathf.Atan2(_axis.x, _axis.y) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 45f);
            angle *= 45f;
            angle += Camera.main.transform.eulerAngles.y;
        }

        return angle;
    }

    private Vector3 GetInputDirection(float speed) {
        /* Handle input direction */
        Vector3 direction;
        if(_controller.isGrounded && PlayerStateMachine.Instance.CanMove()) {
            //Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            Vector3 rotationAxis = Vector3.up; // Axis of rotation (e.g., upwards)
            // Create a Quaternion representing the rotation
            Quaternion rotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, rotationAxis);
            // Rotate the vector based on camera angle
            Vector3 forward = rotation * Vector3.forward;
            // Calculate vector based on keyboard inputs + camera angle
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            forward.y = 0;
            direction = _axis.x * right + _axis.y * forward;
        } else if(!_controller.isGrounded) {
            direction = transform.forward;
        } else {
            direction = Vector3.zero;
        }
        direction = direction.normalized * speed;

        return direction;
    }

    private Vector3 ApplyGravity(Vector3 dir) {
        /* Handle gravity */
        if(_controller.isGrounded) {
            if(_verticalVelocity < -1.25f) {
                _verticalVelocity = -1.25f;
            }
        } else {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }
        dir.y = _verticalVelocity;

        return dir;
    }

    private float GetInputMoveSpeed(float speed) {
        float smoothDuration = 0.2f;

        if(InputManager.Instance.IsInputPressed(InputType.Move)) {
            speed = _defaultSpeed;
        } else if(speed > 0 && _controller.isGrounded) {
            speed -= (_defaultSpeed / smoothDuration) * Time.deltaTime;
        }

        return speed < 0 ? 0 : speed;
    }

    public void Jump() {
        if(_controller.isGrounded && PlayerStateMachine.Instance.CanMove()) {
            _verticalVelocity = _jumpForce;
        }
    }

    public void LookForward(bool followCamera) {
        if(followCamera) {
            _finalAngle = Camera.main.transform.eulerAngles.y;
        }

        transform.rotation = Quaternion.Euler(Vector3.up * _finalAngle);
    }

    public void StartLookAt(Transform target) {
        UpdateFinalAngleToLookAt(target);

        // Wait for a small delay to lock on to target
        _lookAtTarget = target;
    }

    public void StopLookAt() {
        UpdateFinalAngleToLookAt(_lookAtTarget);
        _lookAtTarget = null;
    }

    public void UpdateFinalAngleToLookAt(Transform target) {
        if (target == null) {
            return;
        }

        float angle = Mathf.Atan2(target.position.x - transform.position.x, target.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f);
        angle *= 45f;
        _finalAngle = angle;
    }

    //public void SetCanMove(bool canMove) {
    //    _moveDirection = new Vector3(0, _moveDirection.y, 0);
    //    _canMove = canMove;
    //}

    public bool IsMoving() {
        return !VectorUtils.IsVectorZero2D(_moveDirection) && PlayerStateMachine.Instance.CanMove();
    }

    public void StopMoving() {
        //ResetDestination();
        _moveDirection = new Vector3(0, _moveDirection.y, 0);
    }
}
