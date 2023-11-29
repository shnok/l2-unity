using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum InputType {
    InputAxis,
    MoveForward,
    Move,
    MouseMoving,
    Zoom,
    LeftMouseButtonDown,
    LeftMouseButton,
    RightMouseButton,
    Attack,
    Sit,
    Jump,
    TurnCamera,
    SendMessage,
    Escape
}

public class InputManager : MonoBehaviour {
    public Dictionary<InputType, bool> inputsPressed = new Dictionary<InputType, bool>();
    public Vector2 inputAxis;
    public Vector2 mouseAxis;
    public float scrollAxis;

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
        if(!L2GameUI.GetInstance().mouseOverUI) {
            if(IsInputPressed(InputType.RightMouseButton) && IsInputPressed(InputType.MouseMoving)) {
                UpdateInput(InputType.TurnCamera, true);
                L2GameUI.GetInstance().DisableMouse();
            }

            scrollAxis = Input.GetAxis("Mouse ScrollWheel");
            UpdateInput(InputType.Zoom, scrollAxis != 0);

            UpdateInput(InputType.LeftMouseButtonDown, Input.GetMouseButtonDown(0));
            UpdateInput(InputType.LeftMouseButton, Input.GetMouseButton(0));
            UpdateInput(InputType.RightMouseButton, Input.GetMouseButton(1));
        }

        if(Input.GetMouseButtonUp(1)) {
            UpdateInput(InputType.TurnCamera, false);
            L2GameUI.GetInstance().EnableMouse();
        }

        mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        UpdateInput(InputType.MouseMoving, mouseAxis.x != 0 || mouseAxis.y != 0);

        UpdateInput(InputType.SendMessage, Input.GetKeyDown(KeyCode.Return));
        UpdateInput(InputType.Escape, Input.GetKeyDown(KeyCode.Escape));

        if(!ChatWindow.GetInstance().chatOpened) {
            inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            UpdateInput(InputType.Jump, Input.GetKeyDown(KeyCode.Space));
            UpdateInput(InputType.Sit, Input.GetKeyDown(KeyCode.E));
        } else {
            inputAxis = Vector2.zero;
        }

        UpdateInput(InputType.InputAxis, inputAxis.x != 0 || inputAxis.y != 0);
        UpdateInput(InputType.Move, IsInputPressed(InputType.InputAxis) || IsInputPressed(InputType.MoveForward));
        UpdateInput(InputType.MoveForward, IsInputPressed(InputType.LeftMouseButton) && IsInputPressed(InputType.RightMouseButton));
    }

    public bool IsInputPressed(InputType type) {
        return inputsPressed.ContainsKey(type) && inputsPressed[type] != false;
    }

    public void UpdateInput(InputType type, bool pressed) {
        if(!inputsPressed.ContainsKey(type)) {
            inputsPressed.Add(type, pressed);
        } else {
            inputsPressed[type] = pressed;
        }
    }
}
