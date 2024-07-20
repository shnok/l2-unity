using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : L2Slot
{
    private InventoryTab _currentTab;
    private int _count;
    private long _remainingTime;

    public int Count { get { return _count; } }
    public long RemainingTime { get { return _remainingTime; } }

    public InventorySlot(int position, AbstractItem item, VisualElement slotElement, InventoryTab tab) 
        : base(slotElement, position, item.Id, item.ItemName.Name, item.ItemName.Description, item.Icon) {
        _currentTab = tab;
        _slotElement.AddToClassList("inventory-slot");
    }

    public InventorySlot(int position, VisualElement slotElement, InventoryTab tab)
    : base(slotElement, position) {
        _currentTab = tab;
        _slotElement.AddToClassList("inventory-slot");
    }

    public void AssignItem(ItemInstance item) {
        _id = item.ItemData.Id;
        if (item.ItemData != null) {
            _name = item.ItemData.ItemName.Name;
            _description = item.ItemData.ItemName.Description;
            _icon = item.ItemData.Icon;
        } else {
            _name = "Unkown";
            _description = "Unkown item.";
            _icon = "";
        }
        _count = item.Count;
        _remainingTime = item.RemainingTime;

        Debug.Log(_slotBg);

        StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
        _slotBg.style.backgroundImage = background;
    }

    protected override void HandleLeftClick() {
        AudioManager.Instance.PlayUISound("click_03");
        _currentTab.SelectSlot(_position);
    }

    protected override void HandleRightClick() {

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
