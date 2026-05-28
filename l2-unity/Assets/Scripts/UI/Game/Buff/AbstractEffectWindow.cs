using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class AbstractEffectWindow : L2Window
{
    protected VisualTreeAsset _buffSlot;
    protected VisualElement[] _buffRowContainers;
    protected EffectType _effectType;
    private List<BuffSlot> _effectSlots;
    private List<BuffSlot> _specialEffectSlots;
    private Coroutine _effectCoroutine;

    protected enum EffectType
    {
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

        _effectSlots = new List<BuffSlot>();
        _specialEffectSlots = new List<BuffSlot>();

        _effectCoroutine = StartCoroutine(UpdateBuffTimers());
    }

    protected void HideWindowIfNeeded()
    {
        if (_effectSlots.Count == 0 && _specialEffectSlots.Count == 0)
        {
            if (!_isWindowHidden)
            {
                HideWindow(true);
            }
        }
    }

    private void CleanupAllNormalEffects()
    {
        for (int i = _effectSlots.Count - 1; i >= 0; i--)
        {
            _buffRowContainers[0].RemoveAt(i);
            _effectSlots.RemoveAt(i);
        }
    }

    private void CleanupAllSpecialEffects()
    {
        for (int i = _specialEffectSlots.Count - 1; i >= 0; i--)
        {
            _buffRowContainers[1].RemoveAt(i);
            _specialEffectSlots.RemoveAt(i);
        }
    }

    public void SetEffects(BuffEffect[] buffEffects)
    {
        CleanupAllNormalEffects();

        foreach (BuffEffect buffEffect in buffEffects)
        {
            Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(buffEffect.SkillId, buffEffect.SkillLvl);
            if (skillgrp == null) continue;

            if (skillgrp.IconType == SkillType.Buff && _effectType == EffectType.Buff ||
                skillgrp.IconType == SkillType.Debuff && _effectType == EffectType.Debuff)
            {
                // newEffects.Add(buffEffect);
                AddEffect(buffEffect.SkillId, buffEffect.SkillLvl, buffEffect.Duration, _effectType == EffectType.Buff ? BuffType.Normal : BuffType.Debuff);
            }
        }

        HideWindowIfNeeded();
    }

    public void AddEffect(int effectId, int effectLevel, int duration, BuffType type)
    {
        Debug.Log($"Received buff: {effectId}, duration: {duration}");

        List<BuffSlot> buffSlots = type == BuffType.Special ? _specialEffectSlots : _effectSlots;
        int containerIndex = type == BuffType.Special ? 1 : 0;

        VisualElement visualElement = _buffSlot.Instantiate()[0];
        BuffSlot buffSlot = new BuffSlot(visualElement);
        _buffRowContainers[containerIndex].Add(buffSlot.SlotElement);
        buffSlots.Add(buffSlot);

        buffSlot.AssignEffect(effectId, effectLevel, duration, type);

        if (_isWindowHidden)
        {
            ShowWindow();
        }
    }

    private IEnumerator UpdateBuffTimers()
    {
        while (true)
        {
            if (_isWindowHidden)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            foreach (BuffSlot buffSlot in _effectSlots)
            {
                buffSlot.UpdateRemainingDuration();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDestroy()
    {
        if (_effectCoroutine != null)
        {
            StopCoroutine(_effectCoroutine);
        }
    }

    public virtual void SetEtcEffects(PlayerBuffStatus status)
    {
        CleanupAllSpecialEffects();
    }
}
