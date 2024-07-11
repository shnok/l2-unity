using Assets.Scripts.UI;
using UnityEngine;
using static NativeFunctions;

public class L2GameUI : L2UI {
    private NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

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

    protected override void Update() {
        base.Update();

        if (InputManager.Instance.IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }
  
    protected override void LoadUI() {
        base.LoadUI();

        StartLoading();

        MenuWindow.Instance.AddWindow(_rootVisualContainer);
        // SKillbar needs updates
        //ShortCutPanelMinimal.Instance.AddWindow(rootVisualContainer);
        //ShortCutPanel.Instance.AddWindow(rootVisualContainer);
        IconOverlay.Instance.AddWindow(_rootVisualContainer);
        StatusWindow.Instance.AddWindow(_rootVisualContainer);
        CharacterInventoryWindow.Instance.AddWindow(_rootVisualContainer);
        CharacterInfoWindow.Instance.AddWindow(_rootVisualContainer);
        CharacterInfoWindow.Instance.HideWindow();
        ActionWindow.Instance.AddWindow(_rootVisualContainer);
        ChatWindow.Instance.AddWindow(_rootVisualContainer);
        TargetWindow.Instance.AddWindow(_rootVisualContainer);
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
}
