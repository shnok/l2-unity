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

        if (InputManager.Instance != null && InputManager.Instance.IsInputPressed(InputType.TurnCamera)) {
            DisableMouse();
        } else {
            EnableMouse();
        }
    }
  
    protected override void LoadUI() {
        base.LoadUI();

        if(L2LoginUI.Instance != null) {
            StartLoading();
        }

        // SKillbar needs updates
        //ShortCutPanelMinimal.Instance.AddWindow(rootVisualContainer);
        //ShortCutPanel.Instance.AddWindow(rootVisualContainer);
        if (MenuWindow.Instance != null) {
            MenuWindow.Instance.AddWindow(_rootVisualContainer);
        }
        //if (IconOverlay.Instance != null) {
        //    IconOverlay.Instance.AddWindow(_rootVisualContainer);
        //}
        if (StatusWindow.Instance != null) {
            StatusWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (InventoryWindow.Instance != null) {
            InventoryWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (CharacterInfoWindow.Instance != null) {
            CharacterInfoWindow.Instance.AddWindow(_rootVisualContainer);
            CharacterInfoWindow.Instance.HideWindow();
        }
        // if (ActionWindow.Instance != null) {
        //     ActionWindow.Instance.AddWindow(_rootVisualContainer);
        // }
        // if (SkillLearn.Instance != null) {
        //     SkillLearn.Instance.AddWindow(_rootVisualContainer);
        // }
        if (ChatWindow.Instance != null) {
            ChatWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (TargetWindow.Instance != null) {
            TargetWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (L2ToolTip.Instance != null) {
            L2ToolTip.Instance.AddWindow(_tooltipVisualContainer);
          //  L2ToolTip.Instance.HideWindow();
        }
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
