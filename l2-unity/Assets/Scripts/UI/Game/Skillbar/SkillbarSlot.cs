using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public class SkillbarSlot : L2ClickableSlot
{
    private ButtonClickSoundManipulator _buttonClickSoundManipulator;
    private L2Slot _innerSlot;
    private Shortcut _shortcut;

    public SkillbarSlot(VisualElement slotElement, int position) : base(slotElement, position, SlotType.SkillBar, true, false)
    {
        _slotElement = slotElement;
        _position = position;
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearManipulators();

        _shortcut = shortcut;
        _slotElement.AddToClassList("skillbar-slot");
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);

        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ACTION:
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

        _slotElement.RemoveFromClassList("empty");
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