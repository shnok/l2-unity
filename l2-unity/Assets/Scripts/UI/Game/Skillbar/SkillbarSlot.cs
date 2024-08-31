using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public class SkillbarSlot : L2ClickableSlot
{
    private ButtonClickSoundManipulator _buttonClickSoundManipulator;
    private L2Slot _innerSlot;
    private Shortcut _shortcut;
    private int _skillbarId;
    private int _slot;
    private VisualElement _keyElement;

    public SkillbarSlot(VisualElement slotElement, int position, int skillbarId, int slot) : base(slotElement, position, SlotType.SkillBar, true, false)
    {
        _slotElement = slotElement;
        _position = position;
        _skillbarId = skillbarId;
        _slot = slot;
        _keyElement = _slotElement.Q<VisualElement>("Key");
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
                break;
        }
    }

    public void AssignItem(int objectId)
    {
        ItemInstance item = PlayerInventory.Instance.GetItemByObjectId(objectId);
        _innerSlot = new InventorySlot(_position, _slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
    }

    public void AssignAction(int objectId)
    {
        _innerSlot = new ActionSlot(_slotElement, _position, SlotType.SkillBar);
        ((ActionSlot)_innerSlot).AssignAction((ActionType)objectId);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
    }

    private void UpdateInputInfo() {
        _slotElement.RemoveFromClassList("empty");

        string key = PlayerShortcuts.Instance.GetKeybindForShortcut(_skillbarId, _slot);

        Texture2D inputTexture = KeyImageTable.Instance.LoadTextureByKey(key);

        if(inputTexture != null) {
            _keyElement.style.backgroundImage = inputTexture;
            _keyElement.style.width = inputTexture.width;
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }
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
    }

    protected override void HandleMiddleClick()
    {
    }
}