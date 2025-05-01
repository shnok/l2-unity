using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static NativeFunctions;

public class L2GameUI : L2UI
{
    private NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private List<L2PopupWindow> _openedWindows;

    private static L2GameUI _instance;
    public static L2GameUI Instance { get { return _instance; } }
    public bool IsTyping { get; set; }

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

        _openedWindows = new List<L2PopupWindow>();
    }

    private void Start()
    {
        MouseOverUI = false;
        _windowsLoaded = 0;
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

        CheckUIInputs();
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
            InventoryWindow.Instance.HideWindow(true);
        }
        if (CharacterInfoWindow.Instance != null)
        {
            CharacterInfoWindow.Instance.AddWindow(_rootVisualContainer);
            CharacterInfoWindow.Instance.HideWindow(true);
        }
        if (ActionWindow.Instance != null)
        {
            ActionWindow.Instance.AddWindow(_rootVisualContainer);
            ActionWindow.Instance.HideWindow(true);
        }
        if (NpcHtmlWindow.Instance != null)
        {
            NpcHtmlWindow.Instance.AddWindow(_rootVisualContainer);
        }
        if (ShopWindow.Instance != null)
        {
            ShopWindow.Instance.AddWindow(_rootVisualContainer);
            ShopWindow.Instance.HideWindow(true);
        }
        if (RestartLocationWindow.Instance != null)
        {
            RestartLocationWindow.Instance.AddWindow(_rootVisualContainer);
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
            L2ToolTip.Instance.HideWindow(true);
        }
        if (L2SlotManager.Instance != null)
        {
            L2SlotManager.Instance.AddWindow(_slotVisualContainer);
            L2SlotManager.Instance.HideWindow(true);
        }
        if (SkillWindow.Instance != null)
        {
            SkillWindow.Instance.AddWindow(_rootVisualContainer);
            SkillWindow.Instance.HideWindow(true);
        }
        if (SkillLearnWindow.Instance != null)
        {
            SkillLearnWindow.Instance.AddWindow(_rootVisualContainer);
            SkillLearnWindow.Instance.HideWindow(true);
        }
        if (BuffWindow.Instance != null)
        {
            BuffWindow.Instance.AddWindow(_rootVisualContainer);
            BuffWindow.Instance.HideWindow(true);
        }
        if (DebuffWindow.Instance != null)
        {
            DebuffWindow.Instance.AddWindow(_rootVisualContainer);
            DebuffWindow.Instance.HideWindow(true);
        }
        if (L2ConfirmWindow.Instance != null)
        {
            L2ConfirmWindow.Instance.AddWindow(_popupVisualContainer);
            L2ConfirmWindow.Instance.HideWindow(true);
        }
        if (L2InputAmountWindow.Instance != null)
        {
            L2InputAmountWindow.Instance.AddWindow(_popupVisualContainer);
            L2InputAmountWindow.Instance.HideWindow(true);
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
        else
        {
            NativeFunctions.SetCursorPos(_lastMousePosition.X, _lastMousePosition.Y);
        }
    }

    public void CheckUIInputs()
    {
        if (InputManager.Instance.OpenCharacerStatus)
        {
            if (CharacterInfoWindow.Instance != null)
            {
                CharacterInfoWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenInventory)
        {
            if (InventoryWindow.Instance != null)
            {
                InventoryWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenSystemMenu)
        {
            if (SystemMenuWindow.Instance != null)
            {
                SystemMenuWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenActions)
        {
            if (ActionWindow.Instance != null)
            {
                ActionWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.CloseWindow)
        {
            if (PlayerStateMachine.Instance?.State == PlayerState.SKILL) // Prioritize skill cast cancel
            {
                return;
            }

            if (ChatWindow.Instance != null && ChatWindow.Instance.ChatOpened)
            {
                ChatWindow.Instance.CloseChat(false);
                return;
            }

            if (_openedWindows != null && _openedWindows.Count > 0)
            {
                _openedWindows[_openedWindows.Count - 1].HideWindow(false);
            }
            else
            {
                SystemMenuWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenCharacerStatus)
        {
            if (CharacterInfoWindow.Instance != null)
            {
                CharacterInfoWindow.Instance.ToggleHideWindow();
            }
        }

        if (InputManager.Instance.OpenSkills)
        {
            if (SkillWindow.Instance != null)
            {
                SkillWindow.Instance.ToggleHideWindow();
            }
        }
    }

    public void WindowOpened(L2PopupWindow popupWindow)
    {
        _openedWindows.Add(popupWindow);
    }

    public void WindowClosed(L2PopupWindow popupWindow)
    {
        _openedWindows.Remove(popupWindow);
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
