using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2ToolTip : L2PopupWindow
{

    private VisualElement _skillTooltip;
    private VisualElement _labelTooltip;
    private VisualElement _value;
    private VisualElement _tooltipTarget;
    private Coroutine _updateStyleCoroutine;

    private static L2ToolTip _instance;
    public static L2ToolTip Instance { get { return _instance; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Components/L2Tooltip/L2Tooltip");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _value = GetElementById("Content");
        _skillTooltip = GetElementById("SkillTooltip");
        _labelTooltip = GetElementById("LabelTooltip");
    }

    public void UpdateTooltip<T>(L2Slot.SlotType type, T value, VisualElement target)
    {
        _windowEle.style.left = -1000;
        _windowEle.style.opacity = 0;

        _tooltipTarget = target;

        switch (type)
        {
            case L2Slot.SlotType.Skill:
                SkillWindowInfo val = value as SkillWindowInfo;
                if (val is null) break;
                AddSkillTooltip(val);
                _skillTooltip.style.display = DisplayStyle.Flex;
                _labelTooltip.style.display = DisplayStyle.None;
                break;

            default:
                _skillTooltip.style.display = DisplayStyle.None;

                string stringVal;
                if (value is SkillWindowInfo info)
                {
                    stringVal = info.Name;
                }
                else
                {
                    stringVal = value as string;
                }

                if (stringVal != string.Empty)
                {
                    GetLabelById("Content").text = stringVal;
                    _labelTooltip.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _labelTooltip.style.display = DisplayStyle.None;
                }

                break;
        }

        ShowWindow();

        if (_updateStyleCoroutine != null)
        {
            StopCoroutine(_updateStyleCoroutine);
        }

        _updateStyleCoroutine = StartCoroutine(UpdateToolTipCoroutine(target));
    }

    IEnumerator UpdateToolTipCoroutine(VisualElement target)
    {
        while (true)
        {

            yield return new WaitForEndOfFrame();

            _windowEle.style.left = target.worldBound.x;
            _windowEle.style.top = target.worldBound.y - _windowEle.resolvedStyle.height;

            _windowEle.style.opacity = 1;
        }
    }

    public void HideWindow(VisualElement exitElement)
    {
        if (exitElement == _tooltipTarget)
        {
            base.HideWindow(false);

            if (_updateStyleCoroutine != null)
            {
                StopCoroutine(_updateStyleCoroutine);
            }
        }
    }

    public void AddSkillTooltip(SkillWindowInfo skill)
    {
        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skill.Icon));
        GetElementById("SkillTooltipIcon").style.backgroundImage = background;
        GetLabelById("SkillTooltipName").text = skill.Name;
        GetLabelById("SkillTooltipLevelValue").text = skill.Level.ToString();
        GetLabelById("SkillTooltipType").text = skill.GetSkillType();
        Label desc = GetLabelById("SkillTooltipDescription");
        desc.text = skill.ComposeDescription();

        if (skill.MpCost > 0)
        {
            GetLabelById("SkillTooltipMpCostValue").text = skill.MpCost.ToString();
            GetElementById("SkillTooltipMpCost").style.display = DisplayStyle.Flex;
        }
        else
        {
            GetElementById("SkillTooltipMpCost").style.display = DisplayStyle.None;
        }

        if (skill.Range > 0)
        {
            GetLabelById("SkillTooltipRangeValue").text = skill.Range.ToString();
            GetElementById("SkillTooltipRange").style.display = DisplayStyle.Flex;
        }
        else
        {
            GetElementById("SkillTooltipRange").style.display = DisplayStyle.None;
        }

        if (skill.HitTime > 0)
        {
            GetLabelById("SkillTooltipCastingTimeValue").text = skill.HitTime.ToString(CultureInfo.InvariantCulture);
            GetElementById("SkillTooltipCastingTime").style.display = DisplayStyle.Flex;
        }
        else
        {
            GetElementById("SkillTooltipCastingTime").style.display = DisplayStyle.None;
        }

        if (skill.IsPassiveSkill() || skill.Type == SkillIconType.CraftAndItems)
        {
            GetElementById("SkillTooltipReuseTime").style.display = DisplayStyle.None;
        }
        else
        {
            GetLabelById("SkillTooltipReuseTimeValue").text = skill.ReuseDelay.ToString(CultureInfo.InvariantCulture);
            GetElementById("SkillTooltipReuseTime").style.display = DisplayStyle.Flex;
        }
    }
}
