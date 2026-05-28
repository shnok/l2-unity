using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SkillbarSlot : SkillSlot
{
    private L2Slot _innerSlot;
    private Shortcut _shortcut;
    [SerializeField] private int _skillbarId;
    [SerializeField] private int _slot;
    private VisualElement _keyElement;

    public int Slot { get => _slot; }

    public SkillbarSlot(VisualElement slotElement, int position, int skillbarId, int slot) : base(position, slotElement, SlotType.SkillBar)
    {
        _slotElement = slotElement;
        _position = position;
        _skillbarId = skillbarId;
        _slot = slot;
        _keyElement = _slotElement.Q<VisualElement>("Key");
        _toggled = false;
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearManipulators();

        _shortcut = shortcut;
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);

        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ACTION:
                AssignAction(shortcut.Id);
                break;
            case Shortcut.TYPE_ITEM:
                AssignItem(shortcut.Id);
                break;
            case Shortcut.TYPE_MACRO:
                break;
            case Shortcut.TYPE_RECIPE:
                break;
            case Shortcut.TYPE_SKILL:
                AssignSkill(shortcut.Id, shortcut.Level);
                break;
        }
    }

    public void AssignItem(int objectId)
    {
        _empty = false;

        ItemInstance item = PlayerInventory.Instance.GetItemByObjectId(objectId);
        _innerSlot = new InventorySlot(_position, _slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        _slotAnimationManipulator = new SlotAnimationManipulator(_slotEffect, this);

        UpdateInputInfo();

        if (PlayerShortcuts.Instance.IsItemToggled(item.ItemId))
        {
            _toggled = true;
            SkillbarWindow.Instance.AddToggledSlot(this);
        }
        else
        {
            _toggled = false;
            SkillbarWindow.Instance.RemoveToggledSlot(this);
        }
    }

    public void AssignAction(int objectId)
    {
        _empty = false;

        _innerSlot = new ActionSlot(_slotElement, _position, SlotType.SkillBar);
        ((ActionSlot)_innerSlot).AssignAction((ActionType)objectId);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
    }

    public void AssignSkill(int skillId, int level)
    {
        SkillWindowInfo skill = new SkillWindowInfo(skillId, level);
        if (skill.IsPassiveSkill()) return;

        _slotAnimationManipulator = new SlotAnimationManipulator(_slotEffect, this);

        _innerSlot = new SkillSlot(_position, _slotElement, SlotType.SkillBar);
        ((SkillSlot)_innerSlot).AssignSkill(skill);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        _skillInfo = PlayerSkill.Instance.GetSkillInfo(skillId);

        if (_skillInfo.IsSkillOnCooldown) //TODO: FIX CAN CRASH HERE
        {
            // CooldownStartTime = skillInfo.CooldownStartTime;
            // CooldownEndTime = skillInfo.CooldownEndTime;
            SkillbarWindow.Instance.AddSkillOnCooldown(this);
        }

        UpdateInputInfo();
    }

    private void UpdateInputInfo()
    {
        _slotElement.RemoveFromClassList("empty");

        string key = PlayerShortcuts.Instance.GetKeybindForShortcut(_skillbarId, _slot);

        Texture2D inputTexture = SkillbarImageTable.Instance.LoadTextureByKey(key);

        if (inputTexture != null)
        {
            _keyElement.style.backgroundImage = inputTexture;
            _keyElement.style.width = inputTexture.width;
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();
    }

    protected override void HandleLeftClick()
    {
        if (_shortcut != null)
        {
            Debug.LogWarning($"Use bar slot {_position}.");
            PlayerShortcuts.Instance.UseShortcut(_shortcut);
        }
    }

    protected override void HandleRightClick()
    {
        if (_shortcut != null)
        {
            if (_shortcut.Type == Shortcut.TYPE_ITEM)
            {
                ItemName itemName = ((InventorySlot)_innerSlot).ItemName;
                if (itemName.DefaultAction == "action_soulshot" || itemName.DefaultAction == "action_bless_spiritshot" || itemName.DefaultAction == "action_spiritshot")
                {
                    Debug.LogWarning($"Toggle bar slot {_position}.");
                    PlayerShortcuts.Instance.RequestToggleShortcutItem(itemName.Id, _toggled);
                }
            }
        }
    }

    protected override void HandleMiddleClick()
    {
    }
}