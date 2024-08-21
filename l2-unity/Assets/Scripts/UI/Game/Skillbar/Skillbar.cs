using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Skillbar
{
    private SkillBarFooterManipulator _footerManipulator;
    private Button _expandButton;
    private Button _minimizeButton;
    private VisualElement _windowEle;
    private VisualElement _skillbarWindow;
    private bool _expanded = false;
    private bool _mainSkillbar;
    private int _skillbarIndex;

    public Skillbar(VisualElement skillbarWindowElement, int skillbarIndex)
    {
        _skillbarWindow = skillbarWindowElement;
        _mainSkillbar = skillbarIndex == 0;
        _skillbarIndex = skillbarIndex;
    }

    public IEnumerator BuildWindow(VisualTreeAsset template, VisualElement container)
    {
        _windowEle = template.Instantiate()[0];
        container.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement dragArea = _windowEle.Q<VisualElement>("DragArea");
        _windowEle.AddManipulator(new DragManipulator(dragArea, _skillbarWindow));

        if (_mainSkillbar)
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
        else
        {
            _windowEle.Q<VisualElement>("Footer").style.display = DisplayStyle.None;
            _windowEle.Q<VisualElement>("ExpandBtn").style.display = DisplayStyle.None;
            _windowEle.Q<VisualElement>("MinimizeBtn").style.display = DisplayStyle.None;
            _windowEle.style.marginBottom = -4;
            Minimize();
        }
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

    public void Expand()
    {
        _windowEle.style.display = DisplayStyle.Flex;
    }

    public void Minimize()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    // public void Animate()
    // {

    // }

    // private IEnumerator WaitAndStart(Vector2 target_postion, VisualElement activeElement)
    // {
    //     while (true)
    //     {
    //         Vector2 source = activeElement.transform.position;
    //         Vector2 tempVector = Vector2.MoveTowards(source, target_postion, Time.deltaTime * 500);
    //         if (source.Equals(target_postion))
    //         {
    //             break;
    //         }

    //         activeElement.transform.position = tempVector;
    //         yield return new WaitForSeconds(0.01f);
    //     }
    // }

}
