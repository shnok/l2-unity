using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : L2Slot
{
    private InventoryTab _currentTab;

    public InventorySlot(int position, AbstractItem item, VisualElement slotElement, InventoryTab tab) 
        : base(slotElement, position, item.Id, item.ItemName.Name, item.ItemName.Description, item.Icon) {
        _currentTab = tab;
        _slotElement.AddToClassList("inventory-slot");
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
