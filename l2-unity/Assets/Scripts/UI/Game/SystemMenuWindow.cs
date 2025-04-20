using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
public class SystemMenuWindow : L2PopupWindow
{
    private float _windowHeight = 0f;
    private static SystemMenuWindow _instance;
    public static SystemMenuWindow Instance
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

    private void Update()
    {
        if (_isWindowHidden)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!MouseOverThisWindow() && !MenuWindow.Instance.MouseOverThisWindow())
            {
                HideWindow(false);
            }
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SystemMenuWindow/SystemMenuWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        // root.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement exitButton = GetElementByClass("exit-btn");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>((evt) => HandleExitButtonClick());
        VisualElement restartButton = GetElementByClass("restart-btn");
        restartButton.AddManipulator(new ButtonClickSoundManipulator(restartButton));
        restartButton.RegisterCallback<ClickEvent>((evt) => HandleRestartButtonClick());

        _windowHeight = _windowEle.worldBound.height;
        RegisterClickWindowEvent(_windowEle, null);

        HideWindow(true);

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void HandleExitButtonClick()
    {
        HideWindow(false);
        ExitWindow.Instance.OpenWindow(true);
    }

    private void HandleRestartButtonClick()
    {
        HideWindow(false);
        ExitWindow.Instance.OpenWindow(false);
    }

    public override void ToggleHideWindow()
    {
        Vector2 basePosition = MenuWindow.Instance.GetWindowPosition();
        _windowEle.transform.position = new Vector2(basePosition.x, basePosition.y - _windowHeight);
        base.ToggleHideWindow();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("system_open_01");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("system_close_01");

        L2GameUI.Instance.WindowClosed(this);
    }
}