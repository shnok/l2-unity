using UnityEngine.UIElements;
using static L2Slot;

public class SkillbarSlot : L2Slot
{
    private ButtonClickSoundManipulator _buttonClickSoundManipulator;
    private L2Slot _innerSlot;

    public SkillbarSlot(VisualElement slotElement, int position) : base(slotElement, position, SlotType.SkillBar)
    {
        _slotElement = slotElement;
        _position = position;
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearManipulators();

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

    public void AssignItem(int itemId)
    {
        ItemInstance item = PlayerInventory.Instance.GetItemById(itemId);
        _innerSlot = new InventorySlot(_position, _slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);

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
}