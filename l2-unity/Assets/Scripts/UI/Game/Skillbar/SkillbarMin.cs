using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillbarMin : AbstractSkillbar
{
    protected float _currentHeight;

    public SkillbarMin(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    : base(skillbarWindowElement, skillbarIndex, page, horizontalBar)
    {
    }

    protected override void UpdateVisuals()
    {
        _windowEle.Q<VisualElement>("Footer").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("ExpandBtn").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("MinimizeBtn").style.display = DisplayStyle.None;

        if (_horizontalBar)
        {
            _windowEle.style.marginBottom = -3;
        }
        else
        {
            _windowEle.style.marginRight = -3;
        }

        HideBar();
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

        if (_horizontalBar)
        {
            _windowEle.style.height = value;
        }
        else
        {
            _windowEle.style.width = value;
        }
    }

}
