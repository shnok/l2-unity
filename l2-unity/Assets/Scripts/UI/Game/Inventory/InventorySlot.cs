using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : L2DraggableSlot
{
    protected L2Tab _currentTab;
    private int _count;
    private long _remainingTime;
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    private int _objectId;
    private ItemName _assignedItem;
    private ItemType1 _type1;
    private ItemType2 _type2;
    protected bool _empty = true;
    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }
    public ItemType1 Type1 { get { return _type1; } }
    public ItemType2 Type2 { get { return _type2; } }
    public int ObjectId { get { return _objectId; } }

    public ItemName ItemName { get { return _assignedItem; } }

    public InventorySlot(int position, VisualElement slotElement, L2Tab tab, SlotType slotType)
    : base(position, slotElement, slotType, false, true)
    {
        _currentTab = tab;
        _empty = true;

        if (_slotClickSoundManipulator == null)
        {
            _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
            _slotElement.AddManipulator(_slotClickSoundManipulator);
        }
    }

    public InventorySlot(int position, VisualElement slotElement, SlotType slotType)
    : base(position, slotElement, slotType, true, false)
    {
        _empty = true;
    }

    public void AssignItem(ItemInstance item)
    {
        _slotElement.RemoveFromClassList("empty");

        if (item.ItemData != null)
        {
            _id = item.ItemData.Id;
            _name = item.ItemData.ItemName.Name;
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
            _objectId = item.ObjectId;
            _empty = false;
            _type1 = item.Type1;
            _assignedItem = item.ItemData.ItemName;
        }
        else
        {
            Debug.LogWarning($"Item data is null for item {item.ItemId}.");
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
            _objectId = -1;
            _type1 = Type1;
            _assignedItem = new ItemName();
        }

        _count = item.Count;
        _remainingTime = item.RemainingTime;

        if (_slotElement != null)
        {
            StyleBackground background = new StyleBackground(IconTable.Instance.GetIcon(_id));
            _slotBg.style.backgroundImage = background;

            AddTooltip(item);

            _slotDragManipulator.enabled = true;
        }
    }

    private void AddTooltip(ItemInstance item)
    {
        string tooltipText = $"{_name} ({_count})";
        if (item.Type2 == ItemType2.TYPE2_WEAPON ||
            item.Type2 == ItemType2.TYPE2_ACCESSORY ||
            item.Type2 == ItemType2.TYPE2_SHIELD_ARMOR)
        {
            tooltipText = _name;
        }

        if (_tooltipManipulator != null)
        {
            _tooltipManipulator.SetText(tooltipText);
        }
    }

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
        if (_currentTab != null)
        {
            _currentTab.SelectSlot(_position);
        }
    }

    protected override void HandleRightClick()
    {
        UseItem();
    }

    protected override void HandleMiddleClick()
    {
        if (!_empty)
        {
            PlayerInventory.Instance.DestroyItem(_objectId, 1);
        }
    }

    public virtual void UseItem()
    {
        if (!_empty)
        {
            PlayerInventory.Instance.UseItem(_objectId);
        }
    }
}
