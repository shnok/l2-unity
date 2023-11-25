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

    [SerializeField]
    private VisualTreeAsset statusWindow;

    public void Update() {
        if(rootElement != null && !float.IsNaN(rootElement.resolvedStyle.width) && uiLoaded == false) {
            LoadUI();
            uiLoaded = true;
        } else {
            rootElement = GetComponent<UIDocument>().rootVisualElement;
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }

    private void LoadUI() {
        VisualElement rootVisualContainer = rootElement[0];

        var statusWindowEle = statusWindow.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(statusWindowEle);
        statusWindowEle.AddManipulator(mouseOverDetection);

        var statusWindowDragArea = statusWindowEle.Q<VisualElement>(null, "drag-area");
        DragAndDropManipulator manipulator = new DragAndDropManipulator(statusWindowDragArea, statusWindowEle);
        statusWindowDragArea.AddManipulator(manipulator);

        rootVisualContainer.Add(statusWindowEle);

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
}
