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
    DebugAttack,
    Sit,
    Jump,
    TurnCamera,
    SendMessage,
    Escape
}

/**
 * 
 * 
 *  TODO: Need to switch to Unity InputManager
 *  ==================================
 * 
 */
public class InputManager : MonoBehaviour {
    public Dictionary<InputType, bool> inputsPressed = new Dictionary<InputType, bool>();
    public Vector2 inputAxis;
    public Vector2 mouseAxis;
    public float scrollAxis;

    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        inputsPressed = null;
        _instance = null;
    }

    void Update() {
        if(!L2GameUI.Instance.MouseOverUI) {
            if(IsInputPressed(InputType.RightMouseButton) && IsInputPressed(InputType.MouseMoving)) {
                UpdateInput(InputType.TurnCamera, true);
                L2GameUI.Instance.DisableMouse();
            }

            scrollAxis = Input.GetAxis("Mouse ScrollWheel");
            UpdateInput(InputType.Zoom, scrollAxis != 0);

            UpdateInput(InputType.LeftMouseButtonDown, Input.GetMouseButtonDown(0));
            UpdateInput(InputType.LeftMouseButton, Input.GetMouseButton(0));
            UpdateInput(InputType.RightMouseButton, Input.GetMouseButton(1));
        }

        if(Input.GetMouseButtonUp(1)) {
            UpdateInput(InputType.TurnCamera, false);
            L2GameUI.Instance.EnableMouse();
        }

        mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        UpdateInput(InputType.MouseMoving, mouseAxis.x != 0 || mouseAxis.y != 0);

        UpdateInput(InputType.SendMessage, Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter));
        UpdateInput(InputType.Escape, Input.GetKeyDown(KeyCode.Escape));

        if(!ChatWindow.Instance.ChatOpened) {
            inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            UpdateInput(InputType.Jump, Input.GetKeyDown(KeyCode.Space));
            UpdateInput(InputType.Sit, Input.GetKeyDown(KeyCode.E));
        } else {
            inputAxis = Vector2.zero;
        }

        UpdateInput(InputType.InputAxis, inputAxis.x != 0 || inputAxis.y != 0);
        UpdateInput(InputType.Move, IsInputPressed(InputType.InputAxis) || IsInputPressed(InputType.MoveForward));
        UpdateInput(InputType.MoveForward, IsInputPressed(InputType.LeftMouseButton) && IsInputPressed(InputType.RightMouseButton));
        UpdateInput(InputType.Attack, Input.GetKeyDown(KeyCode.F));
        UpdateInput(InputType.DebugAttack, Input.GetKeyDown(KeyCode.C));
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
