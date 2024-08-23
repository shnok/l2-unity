using UnityEngine.UIElements;

public class SKillbarSlot : L2DraggableSlot
{
    private int _page;
    private long _remainingTime;
    private ButtonClickSoundManipulator _slotClickSoundManipulator;
    private bool _empty;

    public SKillbarSlot(int page, int position, VisualElement slotElement, SlotType slotType) :
    base(position, slotElement, slotType)
    {
        _page = page;
        _slotElement.AddToClassList("inventory-slot");
        _empty = true;

        if (_slotClickSoundManipulator == null)
        {
            _slotClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        }
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

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotClickSoundManipulator);
            _slotClickSoundManipulator = null;
        }
    }

    protected override void HandleLeftClick()
    {
    }

    protected override void HandleRightClick()
    {
    }
}