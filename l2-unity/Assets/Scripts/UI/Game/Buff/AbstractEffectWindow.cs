using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class AbstractEffectWindow : L2Window
{
    protected VisualTreeAsset _buffSlot;
    protected VisualElement[] _buffRowContainers;
    protected int[] _buffRows;
    protected EffectType _effectType;
    protected int[] _prevBuffsIx;
    protected int[] _buffsIx;
    private BuffSlot[][] _buffs;
    private int _buffCount;
    private bool isUpdating = false;

    protected enum EffectType {
        Buff, Debuff
    }

    protected void Init(VisualElement root)
    {
        InitWindow(root);
        _windowEle.style.left = 182;

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        MouseOverDetectionManipulator mouseOverDetectionManipulator = new MouseOverDetectionManipulator(_windowEle);
        _windowEle.AddManipulator(mouseOverDetectionManipulator);

        _prevBuffsIx = new int[_buffRows.Length];
        _buffsIx = new int[_buffRows.Length];
        Array.Fill(_buffsIx, -1);
        Array.Fill(_prevBuffsIx, -1);
        
        _buffs = new BuffSlot[_buffRows.Length][];
        for (int i = 0; i < _buffs.Length; i++) {
            _buffs[i] = new BuffSlot[_buffRows[i]];
        }
    }
    
    public void UpsertBuffs(int objectId, TargetType type, BuffEffect[] buffs)
    {
        // update target buffs/debuffs
    }
    
    public void RemoveLastBuff(int typeIndex, int index)
    {
        _buffs[typeIndex][index].SlotElement.style.display = DisplayStyle.None;
        _buffs[typeIndex][index].SlotElement.Q<Label>("Duration").style.display = DisplayStyle.None;
        _buffCount--;
    }

    public void RemoveBuff(int typeIndex, int index)
    {
        _buffs[typeIndex][index].SlotElement.style.display = DisplayStyle.None;
        _buffCount--;
    }

    public void UpsertPlayerBuffs(BuffEffect[] buffs)
    {
        for (int i = 0; i < _buffsIx.Length; i++) {
            _buffsIx[i] = -1;
        }

        foreach (BuffEffect e in buffs)
        {
            UpsertEffect(e.SkillId, e.SkillLvl, e.Duration);
        }
        CleanupOldBuffs();
        UpdateBuffsCount();

        if (_buffCount == 0) _windowEle.style.display = DisplayStyle.None;
    }

    public void UpsertEffect(int skillId, int skillLvl, int duration)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, skillLvl);
        BuffType type = Buff.ResolveBuffType(skillgrp);
        if ((type == BuffType.Debuff && _effectType != EffectType.Debuff) ||
            (type != BuffType.Debuff && _effectType == EffectType.Debuff)) {
            return;
        }

        UpsertBuff(skillId, skillLvl, duration, skillgrp, type);
    }
    
    public void UpsertEffect(int skillId, int skillLvl, int duration, BuffType type)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, skillLvl);
        if ((type == BuffType.Debuff && _effectType != EffectType.Debuff) ||
            (type != BuffType.Debuff && _effectType == EffectType.Debuff)) {
            return;
        }

        UpsertBuff(skillId, skillLvl, duration, skillgrp, type);
    }

    public void UpsertBuff(int skillId, int skillLvl, int duration, Skillgrp skillgrp, BuffType type)
    {
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, skillLvl);
        Debug.Log($"Received buff: {skillNameData.Name}, duration: {duration}");
        string desc = string.IsNullOrEmpty(skillNameData.Desc) ? SkillNameTable.Instance.GetDescription(skillId) : skillNameData.Desc;

        int buffTypeIndex = type == BuffType.Debuff ? 0 : (int)type;
        
        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skillgrp.Icon));
        string description = ComposeDescription(desc, skillNameData.DescParams);
        BuffSlot buffSlot;
        VisualElement visualBuff;
        (VisualElement visualContainer, int ix) = GetContainerAndBuffIndex(buffTypeIndex, increment: true);
        // insert new slot
        if (visualContainer.childCount <= ix)
        {
            visualBuff = _buffSlot.Instantiate()[0];
            visualBuff.Q<VisualElement>("SlotBg").style.backgroundImage = background;
            buffSlot = new BuffSlot(visualBuff, ix, new Buff(skillNameData.Name, description, skillgrp.Icon, skillLvl, duration, type, Time.unscaledTime));
            visualContainer.Add(visualBuff);
            _buffCount++;
        }
        else
        {
            // update old slot
            visualBuff = visualContainer[ix];
            visualBuff.Q<VisualElement>("SlotBg").style.backgroundImage = background;
            visualContainer.Children().ToArray()[ix] = visualBuff;
            buffSlot = _buffs[buffTypeIndex][ix];
            buffSlot.Buff.Level = skillLvl;
            buffSlot.Buff.Type = type;
            float remainingDuration = GetRemainingDuration(buffSlot.Buff);
            if (buffSlot.Id != skillId || duration > remainingDuration + 1)
            {
                buffSlot.Buff.StartTime = Time.unscaledTime;
                buffSlot.Buff.Duration = duration;
                remainingDuration = GetRemainingDuration(buffSlot.Buff);
            }
            if (remainingDuration >= 60)
            {
                buffSlot.SlotElement.Q<Label>("Duration").style.display = DisplayStyle.None;
                buffSlot.SlotElement.style.opacity = 1;
            }
            else if (remainingDuration > -0.5f && remainingDuration < 60)
            {
                ShowRemainingDuration(buffSlot.SlotElement, remainingDuration);
            }
            if (buffSlot.SlotElement.style.display == DisplayStyle.None)
            {
                _buffCount++;
                buffSlot.SlotElement.style.display = DisplayStyle.Flex;
            }
        }
        buffSlot.Position = ix;
        buffSlot.Id = skillId;
        buffSlot.Icon = skillgrp.Icon;
        buffSlot.Buff.Name = skillNameData.Name;
        buffSlot.Buff.Description = description;
        buffSlot.Buff.Icon = skillgrp.Icon;
        buffSlot.AssignEffect();

        _buffs[buffTypeIndex][ix] = buffSlot;

        if (_windowEle.style.display == DisplayStyle.None)
        {
            _windowEle.style.display = DisplayStyle.Flex;
        }

        if (!isUpdating)
        {
            StartCoroutine(UpdateBuffTimers());
        }
    }

    private IEnumerator UpdateBuffTimers()
    {
        isUpdating = true;

        while (_buffCount > 0)
        {
            int removedBuffs = 0;
            for (var i = 0; i < _buffs.Length; ++i)
            {
                (VisualElement visualContainer, int buffIndexes) = GetContainerAndBuffIndex(i, increment: false);
                for (var k = 0; k <= buffIndexes; ++k)
                {
                    BuffSlot buff = _buffs[i][k];
                    if (buff is null) continue;

                    VisualElement effect = visualContainer[buff.Position];
                    float duration = GetRemainingDuration(buff.Buff);

                    switch (duration)
                    {
                        case > -0.5f and <= 30f:
                            TogglePulse(effect);
                            ShowRemainingDuration(effect, duration);
                            break;
                        case > 30 and < 60:
                            ShowRemainingDuration(effect, duration);
                            break;
                        case -1: //toggle
                            TogglePulse(effect);
                            break;
                        case -100: //infinite
                            break;
                        case <= -0.5f when removedBuffs == _buffCount:
                            effect.style.opacity = 1;
                            RemoveLastBuff(i, k);
                            break;
                    }
                }
            }
            if (_buffCount == 0) _windowEle.style.display = DisplayStyle.None;

            yield return new WaitForSeconds(1f);
        }

        isUpdating = false;
    }

    private void ShowRemainingDuration(VisualElement effect, float duration)
    {
        Label remaining = effect.Q<Label>("Duration");
        remaining.text = Mathf.Ceil(duration).ToString(CultureInfo.InvariantCulture);
        if (remaining.style.display != DisplayStyle.Flex) remaining.style.display = DisplayStyle.Flex;
    }

    private string ComposeDescription(string desc, string[] descParams)
    {
        for (var i = 0; i < descParams.Length; i++)
        {
            desc = desc.Replace($"$s{i + 1}", descParams[i]);
        }
        return desc;
    }

    private (VisualElement, int) GetContainerAndBuffIndex(int typeIndex, bool increment)
    {
        VisualElement row = _buffRowContainers[typeIndex];
        return (row, increment ? ++_buffsIx[typeIndex] : _buffsIx[typeIndex]);
    }

    protected abstract void TogglePulse(VisualElement element);

    private void CleanupOldBuffs()
    {
        for (int i = 0; i < _buffsIx.Length; i++) {
            for (int k = _buffsIx[i] + 1; k < _prevBuffsIx[i] + 1; k++) {
                RemoveBuff(i, k);
            }
        }
    }

    private void UpdateBuffsCount()
    {
        for (int i = 0; i < _prevBuffsIx.Length; i++) {
            _prevBuffsIx[i] = _buffsIx[i];
        }
    }

    private float GetRemainingDuration(Buff buff) {
        return buff.StartTime + buff.Duration - Time.unscaledTime;
    }
}
