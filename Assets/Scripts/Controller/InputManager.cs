using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CameraController cameraController;
    public PlayerController playerController;
    private bool axisPressed = false;
    private bool mouseMoving = false;
    private bool jumpPressed = false;
    private bool dodgePressed = false;
    private bool attackPressed = false;
    private bool holsterWeaponsPressed = false;

    private static InputManager _instance;
    public static InputManager GetInstance() {
        return _instance;
    }
    void Awake() {
        if(_instance == null) {
            _instance = this;
        } else {
            Object.Destroy(gameObject);
        }
    }

    void Update() {
        if(!playerController || !cameraController) {
            axisPressed = false;
            jumpPressed = false;
            dodgePressed = false;
            attackPressed = false;
            mouseMoving = false;
            return;
        }

        cameraController.UpdateZoom(Input.GetAxis("Mouse ScrollWheel"));
        if(Input.GetMouseButton(1)) {
            if(mouseMoving) {
                cameraController.UpdateInputs(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                L2GameUI.GetInstance().DisableMouse();
            }
        }
        if(Input.GetMouseButtonUp(1)) {
            L2GameUI.GetInstance().EnableMouse();
        }

        playerController.UpdateInputs(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        mouseMoving = Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0;
        axisPressed = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
    }

    public void SetCameraController(CameraController controller) {
        cameraController = controller;
    }
    public void SetPlayerController(PlayerController controller) {
        playerController = controller;
    }

    public bool AxisPressed() {
        return axisPressed;
    }

    public bool Jump() {
        return jumpPressed;
    }

    public bool Dodge() {
        return dodgePressed;
    }

    public bool Attack() {
        return attackPressed;
    }

    public bool HolsterWeapons() {
        return holsterWeaponsPressed;
    }
}
