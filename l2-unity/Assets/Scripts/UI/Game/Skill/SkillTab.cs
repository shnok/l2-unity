using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SkillTab : L2Tab
{
    public enum SkillTabType { ACTIVE, PASSIVE }

    [SerializeField] private SkillTabType _tabType;
    private VisualElement _contentContainer;

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(tabView, tabContainer, tabHeader);
        _contentContainer = TabContainer.Q<VisualElement>("Content");
    }

    public void UpdateSkills(List<SkillWindowInfo>[] skills)
    {
        _contentContainer.Clear();

        if (_tabType == SkillTabType.PASSIVE)
        {
            ShowPassiveSkills(skills[1]);
        }
        else if (_tabType == SkillTabType.ACTIVE)
        {
            ShowActiveSkills(skills[0]);
        }
    }

    private void ShowPassiveSkills(List<SkillWindowInfo> skills)
    {
        List<SkillWindowInfo> equipmentSkills = new List<SkillWindowInfo>();
        List<SkillWindowInfo> abilitySkills = new List<SkillWindowInfo>();
        List<SkillWindowInfo> raceSkills = new List<SkillWindowInfo>();
        // List<SkillWindowInfo> occupationSkills = new List<SkillWindowInfo>(1); // should be for 3rd class + subclass
        List<SkillWindowInfo> clanHeroMentoringSkills = new List<SkillWindowInfo>();
        List<SkillWindowInfo> itemSkills = new List<SkillWindowInfo>();

        for (var i = 0; i < skills.Count; i++)
        {
            SkillWindowInfo skill = skills[i];
            switch (skill.Type)
            {
                case SkillType.EquipmentPassive:
                    equipmentSkills.Add(skill);
                    break;

                case SkillType.AbilityPassive or SkillType.Passive:
                    abilitySkills.Add(skill);
                    break;

                case SkillType.Clan or SkillType.NoblessOrHero:
                    clanHeroMentoringSkills.Add(skill);
                    break;

                case SkillType.CraftAndItems:
                    itemSkills.Add(skill);
                    break;

                case SkillType.WeightLimit:
                    raceSkills.Add(skill);
                    break;
            }
        }

        if (equipmentSkills.Any())
        {
            VisualElement section = AddSection(equipmentSkills, SysStringTable.Instance.GetSysString(1702).Name);
            _contentContainer.Add(section);
        }

        if (abilitySkills.Any())
        {
            VisualElement section = AddSection(abilitySkills, SysStringTable.Instance.GetSysString(1703).Name);
            _contentContainer.Add(section);
        }

        if (clanHeroMentoringSkills.Any())
        {
            VisualElement section = AddSection(clanHeroMentoringSkills, SysStringTable.Instance.GetSysString(1699).Name);
            _contentContainer.Add(section);
        }

        if (itemSkills.Any())
        {
            VisualElement section = AddSection(itemSkills, SysStringTable.Instance.GetSysString(1700).Name);
            _contentContainer.Add(section);
        }

        if (raceSkills.Any())
        {
            VisualElement section = AddSection(raceSkills, SysStringTable.Instance.GetSysString(1704).Name);
            _contentContainer.Add(section);
        }
    }

    private void ShowActiveSkills(List<SkillWindowInfo> skills)
    {
        List<SkillWindowInfo> physicalSkills = new List<SkillWindowInfo>(10);
        List<SkillWindowInfo> magicSkills = new List<SkillWindowInfo>(10);
        List<SkillWindowInfo> reinforcementSkills = new List<SkillWindowInfo>();
        List<SkillWindowInfo> weakenSkills = new List<SkillWindowInfo>(5);
        List<SkillWindowInfo> clanHeroMentoringSkills = new List<SkillWindowInfo>(6);
        List<SkillWindowInfo> itemSkills = new List<SkillWindowInfo>(1);
        List<SkillWindowInfo> toggleSkills = new List<SkillWindowInfo>(4);
        List<SkillWindowInfo> transformSkills = new List<SkillWindowInfo>(2);

        for (var i = 0; i < skills.Count; i++)
        {
            SkillWindowInfo skill = skills[i];
            if (skill.Type is SkillType.Physical or SkillType.Magic)
            {
                if (skill.IsMagicSkill())
                {
                    magicSkills.Add(skill);
                }
                else
                {
                    physicalSkills.Add(skill);
                }
            }
            else
            {
                switch (skill.Type)
                {
                    case SkillType.Buff:
                        reinforcementSkills.Add(skill);
                        break;

                    case SkillType.Debuff:
                        weakenSkills.Add(skill);
                        break;

                    case SkillType.CraftAndItems:
                        itemSkills.Add(skill);
                        break;

                    case SkillType.NoblessOrHero or SkillType.Clan:
                        clanHeroMentoringSkills.Add(skill);
                        break;

                    case SkillType.Magic:
                        toggleSkills.Add(skill);
                        break;

                    case SkillType.Toggle:
                        toggleSkills.Add(skill);
                        break;

                    case SkillType.TransformationOrMount:
                        transformSkills.Add(skill);
                        break;
                }
            }
        }

        if (physicalSkills.Any())
        {
            VisualElement section = AddSection(physicalSkills, SysStringTable.Instance.GetSysString(1694).Name);
            _contentContainer.Add(section);
        }
        if (magicSkills.Any())
        {
            VisualElement section = AddSection(magicSkills, SysStringTable.Instance.GetSysString(1695).Name);
            _contentContainer.Add(section);
        }
        if (reinforcementSkills.Any())
        {
            VisualElement section = AddSection(reinforcementSkills, SysStringTable.Instance.GetSysString(1696).Name);
            _contentContainer.Add(section);
        }
        if (weakenSkills.Any())
        {
            VisualElement section = AddSection(weakenSkills, SysStringTable.Instance.GetSysString(1697).Name);
            _contentContainer.Add(section);
        }
        if (clanHeroMentoringSkills.Any())
        {
            VisualElement section = AddSection(clanHeroMentoringSkills, SysStringTable.Instance.GetSysString(1699).Name);
            _contentContainer.Add(section);
        }
        if (itemSkills.Any())
        {
            VisualElement section = AddSection(itemSkills, SysStringTable.Instance.GetSysString(1700).Name);
            _contentContainer.Add(section);
        }
        if (toggleSkills.Any())
        {
            VisualElement section = AddSection(toggleSkills, SysStringTable.Instance.GetSysString(1698).Name);
            _contentContainer.Add(section);
        }
        if (transformSkills.Any())
        {
            VisualElement section = AddSection(transformSkills, SysStringTable.Instance.GetSysString(1701).Name);
            _contentContainer.Add(section);
        }
    }

    public VisualElement AddSection(List<SkillWindowInfo> skills, string name)
    {
        VisualElement section = SkillWindow.Instance.SkillSectionTemplate.Instantiate()[0];
        section.Q<Label>("SkillsSectionHeaderLabel").text = name;
        VisualElement sectionContainer = section.Q<VisualElement>("SkillsSectionBarContainer");

        // maybe use onclick on whole header
        Button btn = section.Q<Button>("PlusMinusBtn");
        btn.RegisterCallback<ClickEvent>(HandleSlotClick, TrickleDown.TrickleDown);

        L2SlotContainer basicSlotContainer = new L2SlotContainer();
        basicSlotContainer.Initialize(sectionContainer, 6, 6);
        basicSlotContainer.CreateSlots(skills.Count, L2Slot.SlotType.Skill);

        for (var i = 0; i < skills.Count; ++i)
        {
            basicSlotContainer.AssignSkill(i, skills[i]);
            SkillWindow.Instance.AddSlot(basicSlotContainer.Slots[i]);
        }

        return section;
    }

    private void HandleSlotClick(ClickEvent evt)
    {
        ToggleShrink((VisualElement)evt.currentTarget);
        AudioManager.Instance.PlayUISound("click_01");
    }

    protected virtual void ToggleShrink(VisualElement btn)
    {
        VisualElement container = btn.parent.parent.Children().ElementAt(1);
        if (btn.ClassListContains("max"))
        {
            btn.RemoveFromClassList("max");
            container.RemoveFromClassList("shrink");
        }
        else
        {
            btn.AddToClassList("max");
            container.AddToClassList("shrink");
        }
    }
}