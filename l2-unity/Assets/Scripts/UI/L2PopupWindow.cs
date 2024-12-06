using UnityEngine;
using UnityEngine.UIElements;

public abstract class L2PopupWindow : L2Window
{
    protected void RegisterCloseWindowEvent(string closeButtonClass)
    {
        Button closeButton = (Button)GetElementByClass(closeButtonClass);
        if (closeButton == null)
        {
            Debug.LogWarning($"Cant find close button with className: {closeButtonClass}.");
            return;
        }

        ButtonClickSoundManipulator buttonClickSoundManipulator = new ButtonClickSoundManipulator(closeButton);
        closeButton.AddManipulator(buttonClickSoundManipulator);

        closeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AudioManager.Instance.PlayUISound("window_close");
            HideWindow(false);
        });
    }

    public void RegisterClickWindowEvent(VisualElement windowEle, VisualElement dragEle)
    {
        if (windowEle != null)
        {
            windowEle.RegisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }

        if (dragEle != null)
        {
            dragEle.RegisterCallback<MouseDownEvent>(evt =>
            {
                BringToFront();
            }, TrickleDown.TrickleDown);
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        BringToFront();
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);
    }

    public override void BringToFront()
    {
        _windowEle.BringToFront();
    }

    public override void SendToBack()
    {
        _windowEle.SendToBack();
    }

    public void CenterWindow()
    {
        float root_width = _root.worldBound.width / 2;
        float window_width = _windowEle.worldBound.width / 2;
        float width = root_width - window_width;

        float root_height = _root.worldBound.height / 2;
        float window_height = _windowEle.worldBound.height / 2;
        float height = root_height - window_height;
        Vector2 center = new Vector2(_root.worldBound.x + width, _root.worldBound.y + height);
        _windowEle.transform.position = center;
    }
}
