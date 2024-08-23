using UnityEngine.UIElements;
using static L2Slot;

public class SkillbarSlot
{
    private ButtonClickSoundManipulator _buttonClickSoundManipulator;
    private VisualElement _slotElement;
    private L2Slot _innerSlot;

    public SkillbarSlot(VisualElement slotElement)
    {
        _slotElement = slotElement;
        _slotElement.AddToClassList("skillbar-slot");
        _slotElement.AddToClassList("empty");
    }

    public void AssignShortcut(Shortcut shortcut)
    {
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
        _innerSlot = new InventorySlot(_slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);

        _slotElement.RemoveFromClassList("empty");
    }

    // public void AssignItem(ItemInstance item)
    // {
    //     if (item.ItemData != null)
    //     {
    //         _id = item.ItemData.Id;
    //         _name = item.ItemData.ItemName.Name;
    //         _description = item.ItemData.ItemName.Description;
    //         _icon = item.ItemData.Icon;
    //         _objectId = item.ObjectId;
    //         _empty = false;
    //         _itemCategory = item.Category;
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Item data is null for item {item.ItemId}.");
    //         _id = 0;
    //         _name = "Unkown";
    //         _description = "Unkown item.";
    //         _icon = "";
    //         _objectId = -1;
    //         _itemCategory = ItemCategory.Item;
    //     }

    //     _count = item.Count;
    //     _remainingTime = item.RemainingTime;

    //     StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
    //     _slotBg.style.backgroundImage = background;

    //     AddTooltip(item);

    //     _slotDragManipulator.enabled = true;
    // }

    // private void AddTooltip(ItemInstance item)
    // {
    //     string tooltipText = $"{_name} ({_count})";
    //     if (item.Category == ItemCategory.Weapon ||
    //         item.Category == ItemCategory.Jewel ||
    //         item.Category == ItemCategory.ShieldArmor)
    //     {
    //         tooltipText = _name;
    //     }

    //     if (_tooltipManipulator != null)
    //     {
    //         _tooltipManipulator.SetText(tooltipText);
    //     }
    // }

    public void ClearManipulators()
    {
        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }
    }
}