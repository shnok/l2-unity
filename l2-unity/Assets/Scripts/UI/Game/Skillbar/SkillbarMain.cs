using UnityEngine.UIElements;

public class SkillbarMain : AbstractSkillbar
{
    private Button _expandButton;
    private Button _minimizeButton;
    private Button _tooltipButton;
    private Button _lockButton;
    private Button _rotateButton;

    public SkillbarMain(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    : base(skillbarWindowElement, skillbarIndex, page, horizontalBar)
    {
    }

    protected override void UpdateVisuals()
    {
        _expandButton = _windowEle.Q<Button>("ExpandBtn");
        _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
        _expandButton.RegisterCallback<ClickEvent>((evt) => SkillbarWindow.Instance.AddSkillbar());

        _minimizeButton = _windowEle.Q<Button>("MinimizeBtn");
        _minimizeButton.AddManipulator(new ButtonClickSoundManipulator(_minimizeButton));
        _minimizeButton.RegisterCallback<ClickEvent>((evt) => SkillbarWindow.Instance.ResetSkillbar());

        _rotateButton = _windowEle.Q<Button>("RotateBtn");
        _rotateButton.AddManipulator(new ButtonClickSoundManipulator(_rotateButton));

        _lockButton = _windowEle.Q<Button>("LockBtn");
        _lockButton.AddManipulator(new ButtonClickSoundManipulator(_lockButton));

        _tooltipButton = _windowEle.Q<Button>("TooltipBtn");
        _tooltipButton.AddManipulator(new ButtonClickSoundManipulator(_tooltipButton));

        _rotateButton.RegisterCallback<ClickEvent>((evt) => HandleRotateClick());
        _lockButton.RegisterCallback<ClickEvent>((evt) => HandleLockClick());
        _tooltipButton.RegisterCallback<ClickEvent>((evt) => HandleTooltipClick());

        UpdateExpandInput(0);
    }

    public void UpdateExpandInput(int mode)
    {
        if (mode == 0)
        {
            _minimizeButton.style.display = DisplayStyle.None;
            _expandButton.style.display = DisplayStyle.Flex;
            return;
        }

        _minimizeButton.style.display = DisplayStyle.Flex;
        _expandButton.style.display = DisplayStyle.None;
    }

    private void HandleTooltipClick()
    {
        SkillbarWindow.Instance.ToggleDisableTooltip();
    }

    public void ToggleDisableTooltip()
    {
        if (!SkillbarWindow.Instance.TooltipDisabled)
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
    }

    private void HandleLockClick()
    {
        SkillbarWindow.Instance.ToggleLockSkillBar();
    }

    public void ToggleLockSkillBar()
    {
        if (!SkillbarWindow.Instance.Locked)
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
    }

    private void HandleRotateClick()
    {
        SkillbarWindow.Instance.ToggleRotate();
    }
}
