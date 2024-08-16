using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryTab : L2Tab {
    private InventorySlot[] _inventorySlots;
    [SerializeField] private int _selectedSlot = -1;
    private int _itemCount = 0;

    private VisualElement _contentContainer;
    public List<ItemCategory> _filteredCategories;
    public bool MainTab { get; internal set; }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);

        _selectedSlot = -1;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
    }

    public IEnumerator UpdateItemList(List<ItemInstance> items) {
        // Clear slots
        if(_inventorySlots != null) {
            foreach(InventorySlot slot in _inventorySlots) {
                slot.ClearSlot();
            }
        }
        
        _contentContainer.Clear();
        _itemCount = 0;

        // Create empty slots
        int slotCount = InventoryWindow.Instance.SlotCount;
        _inventorySlots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++) {
            VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            _contentContainer.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, this, MainTab);
            _inventorySlots[i] = slot;
        }

        // Add disabled slot to fill up the window
        int rowLength = 9;
        if(InventoryWindow.Instance.Expanded) {
            rowLength = 12;
        }

        int padSlot = 0;
        if (slotCount < 8 * rowLength) {
            padSlot = 8 * rowLength - slotCount;
        } else if (slotCount % rowLength != 0) {
            padSlot = rowLength - slotCount % rowLength;
        }

        for (int i = 0; i < padSlot; i++) {
            VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            slotElement.AddToClassList("inventory-slot");
            slotElement.AddToClassList("disabled");
            _contentContainer.Add(slotElement);
        }

        // Assign items to slots
        items.ForEach(item => {
            if(item.Location == ItemLocation.Inventory) {
                if (_filteredCategories == null || _filteredCategories.Count == 0) {
                    _inventorySlots[item.Slot].AssignItem(item);
                    _itemCount++;
                } else if(_filteredCategories.Contains(item.Category)) {
                    _inventorySlots[_itemCount++].AssignItem(item);
                }
            }
        });

        yield return new WaitForEndOfFrame();
        
        if(_selectedSlot != -1) {
            SelectSlot(_selectedSlot);
        }
    }

    public override void SelectSlot(int slotPosition) {
        if(_selectedSlot != -1) {
            _inventorySlots[_selectedSlot].UnSelect();
        }
        _inventorySlots[slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }

    protected override void OnGeometryChanged() {
    }

    protected override void OnSwitchTab() {
        if (InventoryWindow.Instance.SwitchTab(this)) {
            AudioManager.Instance.PlayUISound("window_open");
        }
    }

    protected override void RegisterAutoScrollEvent() {
    }
}
