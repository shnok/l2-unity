using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillbarMin : AbstractSkillbar
{
    protected float _currentHeight;

    public SkillbarMin(VisualElement skillbarWindowElement, int skillbarIndex) : base(skillbarWindowElement, skillbarIndex)
    {
    }

    protected override void UpdateVisuals()
    {
        _windowEle.Q<VisualElement>("Footer").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("ExpandBtn").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("MinimizeBtn").style.display = DisplayStyle.None;
        _windowEle.style.marginBottom = -4;
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
        _windowEle.style.height = value;
    }

}
