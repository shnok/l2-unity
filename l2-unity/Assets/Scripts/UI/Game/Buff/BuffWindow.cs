using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BuffWindow : L2Window
{
    private BuffSlot[][] _buffs;
    private int _prevNormalBuffsIx = -1;
    private int _prevSpecialBuffsIx = -1;
    private int _prevToggleBuffsIx = -1;
    private int _prevDebuffBuffsIx = -1;
    private int _normalBuffsIx = -1;
    private int _specialBuffsIx = -1;
    private int _toggleBuffsIx = -1;
    private int _debuffBuffsIx = -1;
    [SerializeField] private int normalBuffsCount = 24;
    [SerializeField] private int specialBuffsCount = 12;
    [SerializeField] private int toggleBuffsCount = 10;
    [SerializeField] private int debuffBuffsCount = 10;
    [SerializeField] private int maxRowNormalBuffs = 12;
    private VisualTreeAsset _buffSlot;
    private VisualElement _normalBuffsContainer;
    private VisualElement _specialBuffsContainer;
    private VisualElement _toggleBuffsContainer;
    private VisualElement _debuffBuffsContainer;
    private int _buffCount;
    private bool isUpdating = false;
    private static BuffWindow _instance;
    public static BuffWindow Instance { get { return _instance; } }


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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/BuffWindow/BuffWindow");
        _buffSlot = LoadAsset("Data/UI/_Elements/Game/BuffWindow/BuffSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        _windowEle.style.left = 182;

        VisualElement dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        MouseOverDetectionManipulator mouseOverDetectionManipulator = new MouseOverDetectionManipulator(_windowEle);
        _windowEle.AddManipulator(mouseOverDetectionManipulator);

        _buffs = new BuffSlot[3][];
        _buffs[0] = new BuffSlot[normalBuffsCount];
        _buffs[1] = new BuffSlot[toggleBuffsCount];
        _buffs[2] = new BuffSlot[specialBuffsCount];
        _normalBuffsContainer = _windowEle.Q<VisualElement>("NormalBuffs");
        _specialBuffsContainer = _windowEle.Q<VisualElement>("SpecialBuffs");
        _toggleBuffsContainer = _windowEle.Q<VisualElement>("ToggleBuffs");
        _debuffBuffsContainer = _windowEle.Q<VisualElement>("DebuffBuffs");

        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
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

    public void UpsertPlayerStatus(PlayerBuffStatus status)
    {
        // Buff buff;
        // foreach (BuffSlot b in _buffs[(int)BuffType.Special])
        // {
        // to fix: skillid - duelist sonic focus = 8 or tyrant focus force = 50 
        if (status.Charges > 0) UpsertBuff(8, status.Charges, 600);

        // treat it as debuff
        if (status.IsInsideDangerZone) UpsertBuff(4268, 1, -100);
        if (status.IsBlockingAllPlayers) UpsertBuff(4269, 1, -100);
        if (status.WeightPenalty > 0) UpsertBuff(4270, status.WeightPenalty, -50);
        if (status.HasCharmOfCourage) UpsertBuff(5041, 1, -100);
        if (status.DeathPenaltyLvl > 0) // needs fix
            if (status.HasGradePenalty) UpsertBuff(6209, 1, -100); //needs fix: 6209 for armor, 6213 for weapon
        // }
    }

    public void UpsertPlayerBuffs(BuffEffect[] buffs)
    {
        _normalBuffsIx = -1;
        _specialBuffsIx = -1;
        _toggleBuffsIx = -1;
        _debuffBuffsIx = -1;

        foreach (BuffEffect e in buffs)
        {
            // this almost never happens, but just to be safe..
            if (e.Duration == 0)
            {
                for (int i = 0; i < _buffs.Length; i++)
                {
                    for (int k = 0; k < _buffs[i].Length; k++)
                    {
                        if (e.SkillId == _buffs[i][k].Id)
                        {
                            if (k < _buffs[i].Length - 1)
                            {
                                RemoveBuff(i, k);
                            }
                            else
                            {
                                RemoveLastBuff(i, k);
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                UpsertBuff(e.SkillId, e.SkillLvl, e.Duration);
            }
        }
        CleanupOldBuffs();
        UpdateBuffsCount();

        if (_buffCount == 0) _windowEle.style.display = DisplayStyle.None;
    }

    public void UpsertBuff(int skillId, int skillLvl, int duration)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, skillLvl);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, skillLvl);
        string desc = string.IsNullOrEmpty(skillNameData.Desc) ? SkillNameTable.Instance.GetDescription(skillId) : skillNameData.Desc;

        BuffType type = Buff.ResolveBuffType(skillgrp);

        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skillgrp.Icon));
        string description = ComposeDescription(desc, skillNameData.DescParams);
        BuffSlot buffSlot;
        VisualElement visualBuff;
        (VisualElement visualContainer, int ix) = GetContainerAndBuffIndex(type, increment: true);
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
            buffSlot = _buffs[(int)type][ix];
            buffSlot.Buff.Level = skillLvl;
            buffSlot.Buff.Type = type;
            buffSlot.Buff.StartTime = Time.unscaledTime;
            if (buffSlot.Id != skillId || duration > buffSlot.Buff.Duration + 1)
            {
                buffSlot.Buff.Duration = duration;
            }
            if (duration >= 60)
            {
                buffSlot.SlotElement.Q<Label>("Duration").style.display = DisplayStyle.None;
                buffSlot.SlotElement.style.opacity = 1;
            }
            else if (duration > -0.5f && duration < 60)
            {
                ShowRemainingDuration(buffSlot.SlotElement, duration);
            }
            if (buffSlot.SlotElement.style.display == DisplayStyle.None)
            {
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

        _buffs[(int)type][ix] = buffSlot;

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
            for (var i = 0; i < _buffs.Length - 1; ++i)
            {
                (VisualElement visualContainer, int buffIndexes) = GetContainerAndBuffIndex((BuffType)i, increment: false);
                for (var k = 0; k <= buffIndexes; ++k)
                {
                    BuffSlot buff = _buffs[i][k];
                    if (buff is null) continue;

                    VisualElement effect = visualContainer[buff.Position];
                    float duration = buff.Buff.StartTime + buff.Buff.Duration - Time.unscaledTime;

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

            yield return new WaitForSeconds(0.5f);
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

    private (VisualElement, int) GetContainerAndBuffIndex(BuffType type, bool increment) => type switch
    {
        BuffType.Normal => (_normalBuffsContainer, increment ? ++_normalBuffsIx : _normalBuffsIx),
        BuffType.Special => (_specialBuffsContainer, increment ? ++_specialBuffsIx : _specialBuffsIx),
        BuffType.Toggle => (_toggleBuffsContainer, increment ? ++_toggleBuffsIx : _toggleBuffsIx),
        BuffType.Debuff => (_debuffBuffsContainer, increment ? ++_debuffBuffsIx : _debuffBuffsIx),
        _ => (_normalBuffsContainer, increment ? ++normalBuffsCount : _normalBuffsIx)
    };

    private void TogglePulse(VisualElement element)
    {
        if (element.ClassListContains("fade-low"))
        {
            element.RemoveFromClassList("fade-low");
            element.AddToClassList("fade-high");
        }
        else
        {
            element.RemoveFromClassList("fade-high");
            element.AddToClassList("fade-low");
        }
    }

    private void CleanupOldBuffs()
    {
        for (int i = _normalBuffsIx + 1; i < _prevNormalBuffsIx + 1; i++)
        {
            RemoveBuff((int)BuffType.Normal, i);
        }

        for (int i = _specialBuffsIx + 1; i < _prevSpecialBuffsIx + 1; i++)
        {
            RemoveBuff((int)BuffType.Special, i);
        }

        for (int i = _toggleBuffsIx + 1; i < _prevToggleBuffsIx + 1; i++)
        {
            RemoveBuff((int)BuffType.Toggle, i);
        }

        for (int i = _debuffBuffsIx + 1; i < _prevDebuffBuffsIx + 1; i++)
        {
            RemoveBuff((int)BuffType.Debuff, i);
        }
    }

    private void UpdateBuffsCount()
    {
        _prevNormalBuffsIx = _normalBuffsIx;
        _prevSpecialBuffsIx = _specialBuffsIx;
        _prevToggleBuffsIx = _toggleBuffsIx;
        _prevDebuffBuffsIx = _debuffBuffsIx;
    }
}
