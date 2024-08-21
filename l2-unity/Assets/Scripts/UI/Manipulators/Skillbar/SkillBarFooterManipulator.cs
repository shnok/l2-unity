using UnityEngine.UIElements;

public class SkillBarFooterManipulator : PointerManipulator
{
    private Button _tooltipButton;
    private Button _lockButton;
    private Button _rotateButton;

    private VisualElement _root;
    public SkillBarFooterManipulator(VisualElement target)
    {
        _root = target;

        _rotateButton = _root.Q<Button>("RotateBtn");
        _rotateButton.AddManipulator(new ButtonClickSoundManipulator(_rotateButton));

        _lockButton = _root.Q<Button>("LockBtn");
        _lockButton.AddManipulator(new ButtonClickSoundManipulator(_lockButton));

        _tooltipButton = _root.Q<Button>("TooltipBtn");
        _tooltipButton.AddManipulator(new ButtonClickSoundManipulator(_tooltipButton));

        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        _rotateButton.RegisterCallback<ClickEvent>((evt) => HandleRotateClick());
        _lockButton.RegisterCallback<ClickEvent>((evt) => HandleLockClick());
        _tooltipButton.RegisterCallback<ClickEvent>((evt) => HandleTooltipClick());
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _rotateButton.UnregisterCallback<ClickEvent>((evt) => HandleRotateClick());
        _lockButton.UnregisterCallback<ClickEvent>((evt) => HandleLockClick());
        _tooltipButton.UnregisterCallback<ClickEvent>((evt) => HandleTooltipClick());
    }

    private void HandleTooltipClick()
    {
        if (SkillbarWindow.Instance.TooltipDisabled)
        {
            if (_tooltipButton.ClassListContains("min"))
            {
                _tooltipButton.RemoveFromClassList("min");

            }
        }
        else
        {
            if (!_tooltipButton.ClassListContains("min"))
            {

                _tooltipButton.AddToClassList("min");
            }
        }
        SkillbarWindow.Instance.TooltipDisabled = !SkillbarWindow.Instance.TooltipDisabled;
    }

    private void HandleLockClick()
    {
        if (SkillbarWindow.Instance.Locked)
        {
            if (_lockButton.ClassListContains("locked"))
            {
                _lockButton.RemoveFromClassList("locked");
            }
        }
        else
        {
            if (!_lockButton.ClassListContains("locked"))
            {
                _lockButton.AddToClassList("locked");
            }
        }
        SkillbarWindow.Instance.Locked = !SkillbarWindow.Instance.Locked;
    }

    private void HandleRotateClick()
    {

    }
}