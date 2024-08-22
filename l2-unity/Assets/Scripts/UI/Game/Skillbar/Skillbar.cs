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
    private float _currentHeight;

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
            HideBar();
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

    public IEnumerator Expand()
    {
        ShowBar();

        AdjustHeight(0);
        while (_currentHeight < 46)
        {


            AdjustHeight(Mathf.Min(46, _currentHeight + 6f));
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator Minimize()
    {
        AdjustHeight(46);

        while (_currentHeight > 0)
        {
            AdjustHeight(Mathf.Max(0, _currentHeight - 6f));
            yield return new WaitForFixedUpdate();
        }

        HideBar();
    }

    private void AdjustHeight(float value)
    {
        _currentHeight = value;
        _windowEle.style.height = value;
    }

    public void HideBar()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    private void ShowBar()
    {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}
