using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractSkillbar
{
    protected VisualElement _windowEle;
    protected VisualElement _skillbarWindow;

    protected int _skillbarIndex;

    public AbstractSkillbar(VisualElement skillbarWindowElement, int skillbarIndex)
    {
        _skillbarWindow = skillbarWindowElement;
        _skillbarIndex = skillbarIndex;
    }

    public IEnumerator BuildWindow(VisualTreeAsset template, VisualElement container)
    {
        _windowEle = template.Instantiate()[0];
        container.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = _windowEle.Q<VisualElement>("DragArea");
        _windowEle.AddManipulator(new DragManipulator(dragArea, _skillbarWindow));

        UpdateVisuals();
    }

    protected abstract void UpdateVisuals();

    public void HideBar()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowBar()
    {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}
