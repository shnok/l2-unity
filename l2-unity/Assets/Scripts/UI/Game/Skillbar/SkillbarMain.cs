using UnityEngine.UIElements;

public class SkillbarMain : AbstractSkillbar
{
    private Button _expandButton;
    private Button _minimizeButton;
    private bool _expanded = false;
    private SkillBarFooterManipulator _footerManipulator;

    public SkillbarMain(VisualElement skillbarWindowElement, int skillbarIndex) : base(skillbarWindowElement, skillbarIndex)
    {
    }

    // Start is called before the first frame update
    protected override void UpdateVisuals()
    {

        _expandButton = _windowEle.Q<Button>("ExpandBtn");
        _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
        _expandButton.RegisterCallback<ClickEvent>((evt) => SkillbarWindow.Instance.AddSkillbar());

        _minimizeButton = _windowEle.Q<Button>("MinimizeBtn");
        _minimizeButton.AddManipulator(new ButtonClickSoundManipulator(_minimizeButton));
        _minimizeButton.RegisterCallback<ClickEvent>((evt) => SkillbarWindow.Instance.ResetSkillbar());
        _footerManipulator = new SkillBarFooterManipulator(_windowEle);

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
}
