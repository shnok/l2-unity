using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class BuffSlot : L2Slot
{
    public Buff Buff { get; private set; }
    private Label _durationLabel;

    public BuffSlot(VisualElement slotElement) : base(slotElement, 0, SlotType.Effect)
    {
        _durationLabel = slotElement.Q<Label>("Duration");
    }

    public void AssignEffect(int effectId, int effectLevel, int duration, BuffType buffType)
    {
        Id = effectId;
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(effectId, effectLevel);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(effectId, effectLevel);

        // string desc = string.IsNullOrEmpty(skillNameData.Desc) ? SkillNameTable.Instance.GetDescription(effectId, effectLevel) : skillNameData.Desc;
        string description = ComposeDescription(skillNameData.Desc, skillNameData.DescParams);

        Buff = new Buff(skillNameData.Name, description, skillgrp.Icon, effectLevel, duration, buffType, Time.unscaledTime);

        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skillgrp.Icon));
        _slotBg.style.backgroundImage = background;

        AddTooltip();
    }

    private string ComposeDescription(string desc, string[] descParams)
    {
        for (var i = 0; i < descParams.Length; i++)
        {
            desc = desc.Replace($"$s{i + 1}", descParams[i]);
        }
        return desc;
    }

    public void UpdateRemainingDuration()
    {
        if (Buff.Duration < -1)
        {
            return;
        }

        float remainingDuration = GetRemainingDuration();

        if (remainingDuration < 60)
        {
            TogglePulse();

            if (Buff.Duration != -1)
            {
                ShowRemainingDuration(remainingDuration);
            }
        }
        else
        {
            HideRemainingDuration();
        }
    }

    public float GetRemainingDuration()
    {
        return Buff.StartTime + Buff.Duration - Time.unscaledTime;
    }

    private void AddTooltip()
    {
        _tooltipManipulator?.SetValue(Buff);
    }

    private void TogglePulse()
    {
        if (_slotElement.ClassListContains("fade-low"))
        {
            _slotElement.RemoveFromClassList("fade-low");
            _slotElement.AddToClassList("fade-high");
        }
        else
        {
            _slotElement.RemoveFromClassList("fade-high");
            _slotElement.AddToClassList("fade-low");
        }
    }

    private void ShowRemainingDuration(float duration)
    {
        _durationLabel.text = Mathf.Ceil(duration).ToString(CultureInfo.InvariantCulture);
        if (_durationLabel.style.display != DisplayStyle.Flex) _durationLabel.style.display = DisplayStyle.Flex;
    }

    private void HideRemainingDuration()
    {
        if (_durationLabel.style.display != DisplayStyle.None) _durationLabel.style.display = DisplayStyle.None;
    }
}