using UnityEngine;
using static NativeFunctions;

public class L2GameUI : MonoBehaviour {
    public bool mouseEnabled = true;
    public NativeCoords lastMousePosition;
    public static L2GameUI _instance;

    public static L2GameUI GetInstance() {
        return _instance;
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        } else {
            Object.Destroy(gameObject);
        }
    }

    private void Start() {
        
    }

    public void Update() {
        if(InputManager.GetInstance().IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }

    public void ToggleMouse() {
        mouseEnabled = !mouseEnabled;
    }

    public void EnableMouse() {
        if(!mouseEnabled) {
            mouseEnabled = true;
            NativeFunctions.SetCursorPos(lastMousePosition.X, lastMousePosition.Y);
        }
    }
    public void DisableMouse() {
        if(mouseEnabled) {
            NativeFunctions.GetCursorPos(out lastMousePosition);
            mouseEnabled = false;
        }
    }

    public void OnGUI() {
        if(mouseEnabled) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
