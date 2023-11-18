﻿using UnityEngine;

public class PlayerController : MonoBehaviour {

    /* Components */
    public CharacterController controller;

    /* RootBone */
    public Transform rootBone;
    public float rootBoneOffsetMultiplier = 5f;
    public bool stickToRootBone = true;
    public CharacterController rootController;

    /*Rotate*/
    private float _finalAngle;

    /* Move */
    public Vector3 moveDirection;
    public float currentSpeed;
    public float defaultSpeed = 4;
    public Vector2 axis;
    public bool canMove = true;

    /* Gravity */
    private float _verticalVelocity = 0;
    public float _jumpForce = 10;
    public float _gravity = 28;

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
        /* Update input axis */
        if(InputManager.GetInstance().IsInputPressed(InputType.MoveForward)) {
            LookForward(true);
            axis = Vector2.up;
        } else {
            axis = Vector2.zero;
        }
        axis = (axis + InputManager.GetInstance().inputAxis).normalized;

        /* Speed */
        currentSpeed = GetMoveSpeed(currentSpeed);

        /* Angle */
        _finalAngle = GetRotationValue(_finalAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.up * _finalAngle), Time.deltaTime * 7.5f);

        /* Direction */
        moveDirection = GetMoveDirection(moveDirection, currentSpeed);
        controller.Move(moveDirection * Time.deltaTime);

        current_pos = transform.position;
        measuredSpeed = (current_pos - last_pos).magnitude / Time.deltaTime;
        last_pos = current_pos;
    }

    private float GetRotationValue(float angle) {
        float startAngle = angle;
        if(InputManager.GetInstance().IsInputPressed(InputType.InputAxis) && canMove) {
            angle = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 45f);
            angle *= 45f;
            angle += Camera.main.transform.eulerAngles.y;
        }

        return angle;
    }

    private Vector3 GetMoveDirection(Vector3 direction, float speed) {
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        /* Handle gravity */
        if(controller.isGrounded) {
            if(_verticalVelocity < -1.25f) {
                _verticalVelocity = -1.25f;
            }
        } else {
            _verticalVelocity -= _gravity * Time.deltaTime;
        }

        /* Handle input direction */
        if(controller.isGrounded && canMove) {
            direction = axis.x * right + axis.y * forward;
        } else if(!controller.isGrounded) {
            direction = transform.forward;
        } else {
            direction = Vector3.zero;
        }

        direction = direction.normalized * speed;

        direction.y = _verticalVelocity;

        return direction;
    }

    private float GetMoveSpeed(float speed) {
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
            _verticalVelocity = _jumpForce;
        }
    }

    public void LookForward(bool followCamera) {
        if(followCamera) {
            _finalAngle = Camera.main.transform.eulerAngles.y;
        }

        transform.rotation = Quaternion.Euler(Vector3.up * _finalAngle);
    }
}