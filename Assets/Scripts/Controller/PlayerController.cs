using UnityEngine;

public class PlayerController : MonoBehaviour {

    /* Components */
    public CharacterController controller;

    /*Rotate*/
    private float finalAngle;

    /* Move */
    public Vector3 moveDirection;
    public float currentSpeed;
    public float defaultSpeed = 4;
    public Vector2 axis;
    public bool canMove = true;

    /* Gravity */
    private float _verticalVelocity = 0;
    public float jumpForce = 10;
    public float gravity = 28;

    /* Target */
    public Vector3 flatTransformPos;
    public Vector3 targetPosition;
    public float followDistance;
    public bool runToTarget = false;

    public float measuredSpeed;
    private Vector3 current_pos;
    private Vector3 last_pos;

    public static PlayerController _instance;
    public static PlayerController GetInstance() {
        return _instance;
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        flatTransformPos = new Vector3(transform.position.x, 0, transform.position.z);

        if(!runToTarget) {
            /* Update input axis */
            axis = GetAxis();

            /* Speed */
            currentSpeed = GetInputMoveSpeed(currentSpeed);

            /* Angle */
            finalAngle = GetInputRotationValue(finalAngle);

            /* Direction */
            moveDirection = GetInputDirection(currentSpeed);
        } else {
            if(Vector3.Distance(flatTransformPos, targetPosition) < followDistance) {
                ResetTargetPosition();
            } else {
                MoveToTargetPosition();
            }

            if(InputManager.GetInstance().IsInputPressed(InputType.Move)) {
                ResetTargetPosition();
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * finalAngle), Time.deltaTime * 7.5f);

        moveDirection = ApplyGravity(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        MeasureSpeed();
    }

    private void MeasureSpeed() {
        current_pos = transform.position;
        measuredSpeed = (current_pos - last_pos).magnitude / Time.deltaTime;
        last_pos = current_pos;
    }

    public void SetTargetPosition(Vector3 position, float distance) {
        runToTarget = true;
        followDistance = distance;
        targetPosition = new Vector3(position.x, 0, position.z);
    }

    public void ResetTargetPosition() {
        runToTarget = false;
        targetPosition = Vector3.zero;
        ClickManager.GetInstance().HideLocator();
    }

    private void MoveToTargetPosition() {
        Vector3 relativeDirection = targetPosition - flatTransformPos;

        Vector3 relativeAxis = new Vector2(relativeDirection.x, relativeDirection.z);

        // Use Atan2 to calculate the angle in radians
        float angleInRadians = Mathf.Atan2(relativeDirection.x, relativeDirection.z);

        // Convert radians to degrees and adjust for Unity's coordinate system
        float angleInDegrees = Mathf.Rad2Deg * angleInRadians;

        // Ensure the angle is between 0 and 360 degrees
        angleInDegrees = (angleInDegrees + 360) % 360;

        axis = relativeAxis;
        finalAngle = angleInDegrees;
        currentSpeed = defaultSpeed;
        moveDirection = relativeDirection.normalized * currentSpeed;
    }

    public Vector2 GetAxis() {
        Vector2 localAxis;
        if(InputManager.GetInstance().IsInputPressed(InputType.MoveForward)) {
            LookForward(true);
            localAxis = Vector2.up;
        } else {
            localAxis = Vector2.zero;
        }
        localAxis = (localAxis + InputManager.GetInstance().inputAxis);
        localAxis = new Vector2(Mathf.Clamp(localAxis.x, -1f, 1f), Mathf.Clamp(localAxis.y, -1f, 1f));

        return localAxis;
    }

    private float GetInputRotationValue(float angle) {
        if(InputManager.GetInstance().IsInputPressed(InputType.InputAxis) && canMove) {
            angle = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 45f);
            angle *= 45f;
            angle += Camera.main.transform.eulerAngles.y;
        }

        return angle;
    }

    private Vector3 GetInputDirection(float speed) {
        /* Handle input direction */
        Vector3 direction;
        if(controller.isGrounded && canMove) {
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            forward.y = 0;
            direction = axis.x * right + axis.y * forward;
        } else if(!controller.isGrounded) {
            direction = transform.forward;
        } else {
            direction = Vector3.zero;
        }
        direction = direction.normalized * speed;

        return direction;
    }

    private Vector3 ApplyGravity(Vector3 dir) {
        /* Handle gravity */
        if(controller.isGrounded) {
            if(_verticalVelocity < -1.25f) {
                _verticalVelocity = -1.25f;
            }
        } else {
            _verticalVelocity -= gravity * Time.deltaTime;
        }
        dir.y = _verticalVelocity;

        return dir;
    }

    private float GetInputMoveSpeed(float speed) {
        float smoothDuration = 0.2f;

        if(InputManager.GetInstance().IsInputPressed(InputType.Move)) {
            speed = defaultSpeed;
        } else if(speed > 0 && controller.isGrounded) {
            speed -= (defaultSpeed / smoothDuration) * Time.deltaTime;
        }

        return speed < 0 ? 0 : speed;
    }

    public void Jump() {
        if(controller.isGrounded && canMove) {
            _verticalVelocity = jumpForce;
        }
    }

    public void LookForward(bool followCamera) {
        if(followCamera) {
            finalAngle = Camera.main.transform.eulerAngles.y;
        }

        transform.rotation = Quaternion.Euler(Vector3.up * finalAngle);
    }
}
