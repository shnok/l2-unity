using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ShopTabSmall : L2Tab
{
    private InventorySlot[] _slots;
    [SerializeField] private int _selectedSlot = -1;
    private int _itemCount = 0;

    private VisualElement _contentContainer;

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);

        _selectedSlot = -1;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
    }

    public void UpdateItemList(List<ItemInstance> items)
    {
        // Clear slots
        if (_slots != null)
        {
            foreach (InventorySlot slot in _slots)
            {
                slot.UnregisterClickableCallback();
                slot.ClearManipulators();
            }
        }

        _contentContainer.Clear();
        _itemCount = 0;

        // Create empty slots
        int slotCount = PlayerInventory.Instance.InventorySize;
        _slots = new InventorySlot[slotCount];

        L2Slot.SlotType slotType = L2Slot.SlotType.Inventory;

        for (int i = 0; i < slotCount; i++)
        {
            VisualElement slotElement = ShopWindow.Instance.ShopSlotTemplate.Instantiate()[0];
            _contentContainer.Add(slotElement);

            InventorySlot slot = new InventorySlot(i, slotElement, this, slotType);
            _slots[i] = slot;
        }

        // Add disabled slot to fill up the window
        int rowLength = 6;
        int colLenght = 7;

        int padSlot = 0;
        if (slotCount < colLenght * rowLength)
        {
            padSlot = colLenght * rowLength - slotCount;
        }
        else if (slotCount % rowLength != 0)
        {
            padSlot = rowLength - slotCount % rowLength;
        }

        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = ShopWindow.Instance.ShopSlotTemplate.Instantiate()[0];
            slotElement.AddToClassList("inventory-slot");
            slotElement.AddToClassList("disabled");
            _contentContainer.Add(slotElement);
        }

        // Assign items to slots
        items?.ForEach(item =>
        {
            _slots[item.Slot].AssignItem(item);
        });

        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }

    public override void SelectSlot(int slotPosition)
    {
        if (_selectedSlot != -1)
        {
            _slots[_selectedSlot].UnSelect();
        }
        _slots[slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }

    protected override void OnGeometryChanged()
    {
    }

    protected override void OnSwitchTab()
    {
    }

    protected override void RegisterAutoScrollEvent()
    {
    }
}
