using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static NativeFunctions;

public class L2GameUI : MonoBehaviour {
    public bool mouseEnabled = true;
    public bool mouseOverUI = false;
    public NativeCoords lastMousePosition;
    public static L2GameUI instance;
    VisualElement rootElement;
    public bool uiLoaded = false;
    public Focusable focusedElement;

    public static L2GameUI GetInstance() {
        return instance;
    }

    void Awake() {
        if(instance == null) {
            instance = this;
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

    public VisualElement GetRootElement() {
        if(rootElement != null) {
            return rootElement;
        }
        return null;
    }

    public void BlurFocus() {
        if(focusedElement != null) {
            focusedElement.Blur();
        }
    }

    private void LoadUI() {
        VisualElement rootVisualContainer = rootElement.Q<VisualElement>("UIContainer");

        StatusWindow.GetInstance().AddWindow(rootVisualContainer);
        ChatWindow.GetInstance().AddWindow(rootVisualContainer);
        TargetWindow.GetInstance().AddWindow(rootVisualContainer);
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
