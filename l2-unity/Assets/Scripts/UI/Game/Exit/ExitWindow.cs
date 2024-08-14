using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExitWindow : L2PopupWindow
{
    private static ExitWindow _instance;
    private bool _isShow;
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Exit/ExitWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
        Button exit_row = (Button) GetElementByClass("exit-button");
        var _boxHeader = GetElementByClass("drag-area");
        float root_width = root.worldBound.width / 2;
        float exit_width = _windowEle.worldBound.width / 2;
        float width = root_width - exit_width;
        Vector2 center = new Vector2(root.worldBound.x + width, root.worldBound.y);
        _windowEle.transform.position = center;
        HideWindow();
        yield return new WaitForEndOfFrame();

        DragManipulator drag = new DragManipulator(_boxHeader, _windowEle);
        _boxHeader.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEvent("cancel-button");
        RegisterClickWindowEvent(_windowEle, _boxHeader);

        RegisterExitEvent(exit_row);


    }

    private void RegisterExitEvent(VisualElement exit_row)
    {
        exit_row.RegisterCallback<MouseUpEvent>(evt => {
            HideWindow();
            GameClient.Instance.Disconnect();
        }, TrickleDown.TrickleDown);
    }


}
