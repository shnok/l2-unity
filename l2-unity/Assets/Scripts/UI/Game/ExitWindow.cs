using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitWindow : L2PopupWindow
{
    private Button _exitButton;
    private Button _restartButton;
    private Button _cancelButton;
    private Label _windowName;
    private Label _expAcquired;
    private Label _adenaAcquired;
    private Label _itemAcquired;

    private static ExitWindow _instance;
    public static ExitWindow Instance
    {
        get { return _instance; }
    }

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

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ExitWindow/ExitWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _windowName = (Label)GetElementById("windows-name-label");
        _expAcquired = (Label)GetElementById("CountExp");
        _adenaAcquired = (Label)GetElementById("CountAdena");
        _itemAcquired = (Label)GetElementById("CountItem");
        _restartButton = GetElementById("RestartButton").Q<Button>("L2Button");
        _exitButton = GetElementById("ExitButton").Q<Button>("L2Button");
        _cancelButton = GetElementById("CancelButton").Q<Button>("L2Button");
        _restartButton.AddManipulator(new ButtonClickSoundManipulator(_restartButton));
        _exitButton.AddManipulator(new ButtonClickSoundManipulator(_exitButton));
        _cancelButton.AddManipulator(new ButtonClickSoundManipulator(_cancelButton));

        Button initializeButton = GetElementById("InitializeButton").Q<Button>("L2Button");
        initializeButton.AddManipulator(new ButtonClickSoundManipulator(initializeButton));

        var _boxHeader = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(_boxHeader, _windowEle, this);
        _boxHeader.AddManipulator(drag);

        yield return new WaitForEndOfFrame();

        CenterWindow();

        HideWindow(true);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, _boxHeader);

        _restartButton.RegisterCallback<ClickEvent>((evt) => HandleRestartButtonClick());
        _exitButton.RegisterCallback<ClickEvent>((evt) => HandleExitButtonClick());
        _cancelButton.RegisterCallback<ClickEvent>((evt) => HandleCancelButtonClick());

        L2GameUI.Instance.WindowLoadComplete();
    }

    public void OpenWindow(bool exit)
    {
        if (exit)
        {
            _windowName.text = "Exit Game";
            _restartButton.style.display = DisplayStyle.None;
            _exitButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            _windowName.text = "Restart Game";
            _restartButton.style.display = DisplayStyle.Flex;
            _exitButton.style.display = DisplayStyle.None;
        }

        ShowWindow();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("window_close");

        L2GameUI.Instance.WindowClosed(this);
    }

    private void HandleRestartButtonClick()
    {
        HideWindow(false);
        GameClient.Instance.ClientPacketHandler.RequestRestart();
    }

    private void HandleExitButtonClick()
    {
        HideWindow(false);
        GameClient.Instance.ClientPacketHandler.RequestDisconnect();
    }

    private void HandleCancelButtonClick()
    {
        HideWindow(false);
    }
}