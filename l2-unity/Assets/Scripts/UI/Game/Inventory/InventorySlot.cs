using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : L2Slot
{
    protected L2Tab _currentTab;
    private int _count;
    private long _remainingTime;
    private TooltipManipulator _tooltipManipulator;
    private int _objectId;

    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }

    public InventorySlot(int position, VisualElement slotElement, L2Tab tab)
    : base(slotElement, position) {
        _currentTab = tab;
        _slotElement.AddToClassList("inventory-slot");
    }

    public void AssignItem(ItemInstance item) {
        if (item.ItemData != null) {
            _id = item.ItemData.Id;
            _name = item.ItemData.ItemName.Name;
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
            _objectId = item.ObjectId;
        } else {
            Debug.LogWarning($"Item data is null for item {item.ItemId}.");
            _id = 0;
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
            _objectId = -1;
        }
        _count = item.Count;
        _remainingTime = item.RemainingTime;

        StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
        _slotBg.style.backgroundImage = background;

        string tooltipText =  $"{_name} ({_count})";
        if(item.Category == ItemCategory.Weapon || item.Category == ItemCategory.Jewel || item.Category == ItemCategory.ShieldArmor) {
            tooltipText = _name;
        }

        if(_tooltipManipulator == null) {
            _tooltipManipulator = new TooltipManipulator(_slotElement, tooltipText);
            _slotElement.AddManipulator(_tooltipManipulator);
        } else {
            _tooltipManipulator.SetText(tooltipText);
        }
    }

    protected override void HandleLeftClick() {
        AudioManager.Instance.PlayUISound("click_03");
        _currentTab.SelectSlot(_position);
    }

    protected override void HandleRightClick() {
        GameClient.Instance.ClientPacketHandler.UseItem(_objectId);
    }

    public void SetSelected() {
        Debug.Log($"Slot {_position} selected.");
        _slotElement.AddToClassList("selected");
    }

    public void UnSelect() {
        Debug.Log($"Slot {_position} unselected.");
        _slotElement.RemoveFromClassList("selected");
    }
}
