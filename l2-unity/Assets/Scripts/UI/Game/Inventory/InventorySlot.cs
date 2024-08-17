using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : L2Slot
{
    protected L2Tab _currentTab;
    private int _count;
    private long _remainingTime;
    private TooltipManipulator _tooltipManipulator;
    private SlotDragManipulator _slotDragManipulator;
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    private int _objectId;
    private ItemCategory _itemCategory;
    protected bool _empty = true;

    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }
    public ItemCategory ItemCategory { get { return _itemCategory; } }

    public InventorySlot(int position, VisualElement slotElement, L2Tab tab, bool handleMouseOver)
    : base(slotElement, position, handleMouseOver) {
        _currentTab = tab;
        _slotElement.AddToClassList("inventory-slot");
        _empty = true;
        
        if(_slotClickSoundManipulator == null) {
            _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
            _slotElement.AddManipulator(_slotClickSoundManipulator);
        }
    }

    public void AssignItem(ItemInstance item) {
        if (item.ItemData != null) {
            _id = item.ItemData.Id;
            _name = item.ItemData.ItemName.Name;
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
            _objectId = item.ObjectId;
            _empty = false;
            _itemCategory = item.Category;
        } else {
            Debug.LogWarning($"Item data is null for item {item.ItemId}.");
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
            _objectId = -1;
            _itemCategory = ItemCategory.Item;
        }

        _count = item.Count;
        _remainingTime = item.RemainingTime;

        StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
        _slotBg.style.backgroundImage = background;

        AddTooltip(item);

        if(_slotDragManipulator == null) {
            _slotDragManipulator = new SlotDragManipulator(_slotElement, this);
            _slotElement.AddManipulator(_slotDragManipulator);
        }
    }

    private void AddTooltip(ItemInstance item) { 
        string tooltipText =  $"{_name} ({_count})";
        if(item.Category == ItemCategory.Weapon || 
            item.Category == ItemCategory.Jewel || 
            item.Category == ItemCategory.ShieldArmor) {
            tooltipText = _name;
        }
        
        if(_tooltipManipulator == null) {
            _tooltipManipulator = new TooltipManipulator(_slotElement, tooltipText);
            _slotElement.AddManipulator(_tooltipManipulator);
        } else {
            _tooltipManipulator.SetText(tooltipText);
        }
    }

    public void ClearSlot() {
        if(_tooltipManipulator != null) {
            _tooltipManipulator.Clear();
            _slotElement.RemoveManipulator(_tooltipManipulator);
            _tooltipManipulator = null;
        }

        if(_slotDragManipulator != null) {
            _slotElement.RemoveManipulator(_slotDragManipulator);
            _slotDragManipulator = null;
        }

        if(_slotClickSoundManipulator != null) {
            _slotElement.RemoveManipulator(_slotClickSoundManipulator);
            _slotClickSoundManipulator = null;
        }
    }

    protected override void HandleLeftClick() {
        _currentTab.SelectSlot(_position);
    }

    protected override void HandleRightClick() {
        UseItem();
    }

    public virtual void UseItem() {
        if(!_empty) {
            GameClient.Instance.ClientPacketHandler.UseItem(_objectId);
        }
    }
}
