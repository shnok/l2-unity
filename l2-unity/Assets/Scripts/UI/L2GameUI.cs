using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static NativeFunctions;

public class L2GameUI : MonoBehaviour {
    private NativeCoords _lastMousePosition;
    private VisualElement _rootElement;

    [SerializeField] private bool _uiLoaded = false;
    [SerializeField] private Focusable _focusedElement;
    [SerializeField] private bool _mouseEnabled = true;
    [SerializeField] private bool _mouseOverUI = false;

    public bool MouseOverUI { get { return _mouseOverUI; } set { _mouseOverUI = value; } }
    public bool UILoaded { get { return _uiLoaded; } set { _uiLoaded = value; } }

    private static L2GameUI _instance;
    public static L2GameUI Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    public void Update() {
        if(_rootElement != null && !float.IsNaN(_rootElement.resolvedStyle.width) && _uiLoaded == false) {
            LoadUI();
            _uiLoaded = true;
        } else {
            _rootElement = GetComponent<UIDocument>().rootVisualElement;
        }

        if(_uiLoaded) {
            _focusedElement = _rootElement.focusController.focusedElement;
        }

        if(InputManager.Instance.IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }

    public VisualElement GetRootElement() {
        if(_rootElement != null) {
            return _rootElement;
        }
        return null;
    }

    public void BlurFocus() {
        if(_focusedElement != null) {
            _focusedElement.Blur();
        }
    }

    private void LoadUI() {
        VisualElement rootVisualContainer = _rootElement.Q<VisualElement>("UIContainer");


        
        MenuWindow.Instance.AddWindow(rootVisualContainer);
        ShortCutPanelMinimal.Instance.AddWindow(rootVisualContainer);
        ShortCutPanel.Instance.AddWindow(rootVisualContainer);
        IconOverlay.Instance.AddWindow(rootVisualContainer);
        StatusWindow.Instance.AddWindow(rootVisualContainer);
        CharacterInventory.Instance.AddWindow(rootVisualContainer);
        CharacterInfo.Instance.AddWindow(rootVisualContainer);
        ChatWindow.Instance.AddWindow(rootVisualContainer);
        TargetWindow.Instance.AddWindow(rootVisualContainer);
       


    }

    public void EnableMouse() {
        if(!_mouseEnabled) {
            _mouseEnabled = true;
            NativeFunctions.SetCursorPos(_lastMousePosition.X, _lastMousePosition.Y);
        }
    }

    public void DisableMouse() {
        if(_mouseEnabled) {
            NativeFunctions.GetCursorPos(out _lastMousePosition);
            _mouseEnabled = false;
        }
    }

    public void OnGUI() {
        if(_mouseEnabled) {
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
