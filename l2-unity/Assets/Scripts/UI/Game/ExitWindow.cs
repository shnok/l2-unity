using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitWindow : L2PopupWindow
{
    private Button _exitButton;
    private Button _restartButton;
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ExitWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        _windowName = (Label)GetElementById("windows-name-label");
        _expAcquired = (Label)GetElementById("CountExp");
        _adenaAcquired = (Label)GetElementById("CountAdena");
        _itemAcquired = (Label)GetElementById("CountItem");
        _restartButton = (Button)GetElementByClass("restart-button");
        _exitButton = (Button)GetElementByClass("exit-button");

        Button initializeButton = (Button)GetElementByClass("initialize-button");
        initializeButton.AddManipulator(new ButtonClickSoundManipulator(initializeButton));


        var _boxHeader = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(_boxHeader, _windowEle);
        _boxHeader.AddManipulator(drag);

        float root_width = root.worldBound.width / 2;
        float exit_width = _windowEle.worldBound.width / 2;
        float width = root_width - exit_width;
        Vector2 center = new Vector2(root.worldBound.x + width, root.worldBound.y);
        _windowEle.transform.position = center;

        HideWindow();

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEvent("cancel-button");
        RegisterClickWindowEvent(_windowEle, _boxHeader);

        _restartButton.RegisterCallback<ClickEvent>((evt) => HandleRestartButtonClick());
        _exitButton.RegisterCallback<ClickEvent>((evt) => HandleExitButtonClick());
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
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioManager.Instance.PlayUISound("window_close");
    }

    private void HandleRestartButtonClick()
    {
        HideWindow();
        GameClient.Instance.ClientPacketHandler.RequestRestart();
    }

    private void HandleExitButtonClick()
    {
        HideWindow();
        GameClient.Instance.ClientPacketHandler.RequestDisconnect();
    }
}