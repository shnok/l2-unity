using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryTab : L2Tab {
    private InventorySlot[] _inventorySlots;
    private int _selectedSlot = -1;

    private VisualElement _contentContainer;

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);

        _selectedSlot = -1;
        _contentContainer = tabContainer.Q<VisualElement>("Content");

        int slotCount = 40;
        _inventorySlots = new InventorySlot[slotCount];
        for(int i = 0; i < slotCount; i++) {
            VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            _contentContainer.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, ItemTable.Instance.GetWeapon(16), slotElement, this);
            _inventorySlots[i] = slot;
        }
    }

    public void SelectSlot(int slotPosition) {
        if(_selectedSlot != slotPosition) {
            if(_selectedSlot != -1) {
                _inventorySlots[_selectedSlot].UnSelect();
            }
            _inventorySlots[slotPosition].SetSelected();
            _selectedSlot = slotPosition;
        }
    }

    protected override void OnGeometryChanged() {
    }

    protected override void OnSwitchTab() {
    }

    protected override void RegisterAutoScrollEvent() {
    }
}
