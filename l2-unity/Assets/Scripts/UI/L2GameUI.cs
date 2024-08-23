using UnityEngine;
using UnityEngine.UIElements;
using static NativeFunctions;

public class L2GameUI : L2UI
{
    private NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private static L2GameUI _instance;
    public static L2GameUI Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void Update()
    {
        base.Update();

        if (InputManager.Instance != null && InputManager.Instance.TurnCamera)
        {
            DisableMouse();
        }
        else
        {
            EnableMouse();
        }
    }

    protected override void LoadUI()
    {
        base.LoadUI();

        if (L2LoginUI.Instance != null)
        {
            StartLoading();
        }

        if (ChatWindow.Instance != null)
        {
            ChatWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (SystemMenuWindow.Instance != null)
        {
            SystemMenuWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (MenuWindow.Instance != null)
        {
            MenuWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (StatusWindow.Instance != null)
        {
            StatusWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (SkillbarWindow.Instance != null)
        {
            SkillbarWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (InventoryWindow.Instance != null)
        {
            InventoryWindow.Instance.AddWindow(_rootVisualContainer);
            InventoryWindow.Instance.HideWindow();
        }
        if (CharacterInfoWindow.Instance != null)
        {
            CharacterInfoWindow.Instance.AddWindow(_rootVisualContainer);
            CharacterInfoWindow.Instance.HideWindow();
        }
        if (TargetWindow.Instance != null)
        {
            TargetWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (ExitWindow.Instance != null)
        {
            ExitWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (L2ToolTip.Instance != null)
        {
            L2ToolTip.Instance.AddWindow(_tooltipVisualContainer);
            L2ToolTip.Instance.HideWindow();
        }
        if (L2SlotManager.Instance != null)
        {
            L2SlotManager.Instance.AddWindow(_slotVisualContainer);
            L2SlotManager.Instance.HideWindow();
        }
    }

    public void EnableMouse()
    {
        if (!_mouseEnabled)
        {
            _mouseEnabled = true;
            NativeFunctions.SetCursorPos(_lastMousePosition.X, _lastMousePosition.Y);
        }
    }

    public void DisableMouse()
    {
        if (_mouseEnabled)
        {
            NativeFunctions.GetCursorPos(out _lastMousePosition);
            _mouseEnabled = false;
        }
    }

    public void OnGUI()
    {
        if (_mouseEnabled)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
