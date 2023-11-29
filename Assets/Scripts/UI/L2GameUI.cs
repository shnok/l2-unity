using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static NativeFunctions;

public class L2GameUI : MonoBehaviour {
    public bool mouseEnabled = true;
    public bool mouseOverUI = false;
    public NativeCoords lastMousePosition;
    public static L2GameUI _instance;
    VisualElement rootElement;
    bool uiLoaded = false;
    public Focusable focusedElement;

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

    public void Update() {
        if(rootElement != null && !float.IsNaN(rootElement.resolvedStyle.width) && uiLoaded == false) {
            LoadUI();
            uiLoaded = true;
        } else {
            rootElement = GetComponent<UIDocument>().rootVisualElement;
        }

        if(uiLoaded) {
            focusedElement = rootElement.focusController.focusedElement;
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }

    public void BlurFocus() {
        if(focusedElement != null) {
            focusedElement.Blur();
        }
    }

    private void LoadUI() {
        VisualElement rootVisualContainer = rootElement[0];

        StatusWindow.GetInstance().AddWindow(rootVisualContainer);
        ChatWindow.GetInstance().AddWindow(rootVisualContainer);
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
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        } else {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public static void BlinkingCursor(VisualElement tf) {
        tf.schedule.Execute(() => {
            if(tf.ClassListContains("transparent-cursor"))
                tf.RemoveFromClassList("transparent-cursor");
            else
                tf.AddToClassList("transparent-cursor");
        }).Every(500);
    }
}
