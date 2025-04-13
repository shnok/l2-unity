using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[System.Serializable]
public class SkillTab : L2Tab
{
    public enum SkillTabType { ACTIVE, PASSIVE }

    [SerializeField] private SkillTabType _tabType;
    private SkillSlot[] _skillSlots;
    private VisualElement _contentContainer;
    private short skillSlotIx;

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(tabView, tabContainer, tabHeader);
        _contentContainer = TabContainer.Q<VisualElement>("Content");
    }

    public void UpdateSkills(List<SkillWindowInfo>[] skills)
    {
        if (_skillSlots != null)
        {
            foreach (SkillSlot slot in _skillSlots)
            {
                slot.UnregisterClickableCallback();
                slot.ClearManipulators();
            }
        }

        skillSlotIx = 0;
        _contentContainer.Clear();
        if (_tabType == SkillTabType.PASSIVE)
        {
            _skillSlots = new SkillSlot[skills[1].Count];
            ShowPassiveSkills(skills[1]);
        }
        else if (_tabType == SkillTabType.ACTIVE)
        {
            _skillSlots = new SkillSlot[skills[0].Count];
            ShowActiveSkills(skills[0]);
        }
    }

    private void ShowPassiveSkills(List<SkillWindowInfo> skills)
    {
        List<SkillWindowInfo> equipmentSkills = new List<SkillWindowInfo>(5);
        List<SkillWindowInfo> abilitySkills = new List<SkillWindowInfo>(6);
        List<SkillWindowInfo> raceSkills = new List<SkillWindowInfo>();
        // List<SkillWindowInfo> occupationSkills = new List<SkillWindowInfo>(1); // should be for 3rd class + subclass
        List<SkillWindowInfo> clanHeroMentoringSkills = new List<SkillWindowInfo>(5);
        List<SkillWindowInfo> itemSkills = new List<SkillWindowInfo>(10);

        for (var i = 0; i < skills.Count; i++)
        {
            SkillWindowInfo skill = skills[i];
            switch (skill.Type)
            {
                case SkillIconType.EquipmentPassive:
                    equipmentSkills.Add(skill);
                    break;

                case SkillIconType.AbilityPassive or SkillIconType.Passive:
                    abilitySkills.Add(skill);
                    break;

                case SkillIconType.Clan or SkillIconType.NoblessOrHero:
                    clanHeroMentoringSkills.Add(skill);
                    break;

                case SkillIconType.CraftAndItems:
                    itemSkills.Add(skill);
                    break;

                case SkillIconType.WeightLimit:
                    raceSkills.Add(skill);
                    break;
            }
        }

        if (equipmentSkills.Any())
        {
            VisualElement section = AddSection(equipmentSkills, "Equipment Skills");
            _contentContainer.Add(section);
        }

        if (abilitySkills.Any())
        {
            VisualElement section = AddSection(abilitySkills, "Ability Skills");
            _contentContainer.Add(section);
        }

        if (clanHeroMentoringSkills.Any())
        {
            VisualElement section = AddSection(clanHeroMentoringSkills, "Clan/Hero/Mentoring Skills");
            _contentContainer.Add(section);
        }

        if (itemSkills.Any())
        {
            VisualElement section = AddSection(itemSkills, "Item Skills");
            _contentContainer.Add(section);
        }

        if (raceSkills.Any())
        {
            VisualElement section = AddSection(raceSkills, "Race Skills");
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
            if (skill.Type is SkillIconType.Physical or SkillIconType.Magic)
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
                    case SkillIconType.Buff:
                        reinforcementSkills.Add(skill);
                        break;

                    case SkillIconType.Debuff:
                        weakenSkills.Add(skill);
                        break;

                    case SkillIconType.CraftAndItems:
                        itemSkills.Add(skill);
                        break;

                    case SkillIconType.NoblessOrHero or SkillIconType.Clan:
                        clanHeroMentoringSkills.Add(skill);
                        break;

                    case SkillIconType.Magic:
                        toggleSkills.Add(skill);
                        break;

                    case SkillIconType.Toggle:
                        toggleSkills.Add(skill);
                        break;

                    case SkillIconType.TransformationOrMount:
                        transformSkills.Add(skill);
                        break;
                }
            }
        }

        if (physicalSkills.Any())
        {
            VisualElement section = AddSection(physicalSkills, "Physical Skills");
            _contentContainer.Add(section);
        }
        if (magicSkills.Any())
        {
            VisualElement section = AddSection(magicSkills, "Magical Skills");
            _contentContainer.Add(section);
        }
        if (reinforcementSkills.Any())
        {
            VisualElement section = AddSection(reinforcementSkills, "Reinforcement Skills");
            _contentContainer.Add(section);
        }
        if (weakenSkills.Any())
        {
            VisualElement section = AddSection(weakenSkills, "Weaken Skills");
            _contentContainer.Add(section);
        }
        if (clanHeroMentoringSkills.Any())
        {
            VisualElement section = AddSection(clanHeroMentoringSkills, "Clan/Hero/Mentoring Skills");
            _contentContainer.Add(section);
        }
        if (itemSkills.Any())
        {
            VisualElement section = AddSection(itemSkills, "Item Skills");
            _contentContainer.Add(section);
        }
        if (toggleSkills.Any())
        {
            VisualElement section = AddSection(toggleSkills, "Toggle Skills");
            _contentContainer.Add(section);
        }
        if (transformSkills.Any())
        {
            VisualElement section = AddSection(transformSkills, "Transform Skills");
            _contentContainer.Add(section);
        }
    }

    public VisualElement AddSection(List<SkillWindowInfo> skills, string name)
    {
        VisualElement section = SkillWindow.Instance.SkillSectionTemplate.Instantiate()[0];
        section.Q<Label>("SkillsSectionHeaderLabel").text = name;
        VisualElement sectionContainer = section.Q<VisualElement>("SkillsSectionBarContainer");

        //TODO: Use L2SlotContainers

        // maybe use onclick on whole header
        Button btn = section.Q<Button>("PlusMinusBtn");
        btn.RegisterCallback<ClickEvent>(HandleSlotClick, TrickleDown.TrickleDown);

        for (var i = 0; i < skills.Count; ++i)
        {
            VisualElement slotElement = L2SlotManager.Instance.SkillSlotTemplate.Instantiate()[0];
            SkillSlot skillSlot = new SkillSlot(i, slotElement, L2Slot.SlotType.Skill);
            _skillSlots[skillSlotIx++] = skillSlot;
            skillSlot.AssignSkill(skills[i]);
            sectionContainer.Add(slotElement);
        }

        int rowLength = 6;
        int padSlot = 0;
        if (skills.Count < 8 * rowLength)
        {
            padSlot = 8 * rowLength - skills.Count;
        }
        else if (skills.Count % rowLength != 0)
        {
            padSlot = rowLength - skills.Count % rowLength;
        }

        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = L2SlotManager.Instance.SkillSlotTemplate.Instantiate()[0];
            slotElement.AddToClassList("skillbar-slot.empty");
            // slotElement.AddToClassList("disabled");
            sectionContainer.Add(slotElement);
        }
        return section;
    }

    public override void SelectSlot(int slotPosition)
    {
        // use skill

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