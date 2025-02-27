using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2SlotContainer : L2Scrollable
{
    private VisualElement _slotContainerElement;
    private L2Slot[] _slots;
    public L2Slot[] Slots { get => _slots; }
    private int _currentSelectedSlotId;
    private int _rowLength;

    public virtual void Initialize(VisualElement container, int rowLength)
    {
        base.Initialize(container, false);
        _container = container;
        _slotContainerElement = container.Q<VisualElement>("Content");
        _currentSelectedSlotId = -1;
        _rowLength = rowLength;
    }


    private void ClearSlots()
    {
        // Clear slots
        if (_slots != null)
        {
            foreach (InventorySlot slot in _slots)
            {
                slot?.UnregisterClickableCallback();
                slot?.ClearManipulators();
            }
        }

        _slotContainerElement.Clear();
    }

    private void PadSlots(int slotCount)
    {
        // Add disabled slot to fill up the window
        int padSlot = 0;
        if (slotCount % _rowLength != 0)
        {
            padSlot = _rowLength - slotCount % _rowLength;
        }

        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = L2SlotManager.Instance.InventorySlotTemplate.Instantiate()[0];
            slotElement.AddToClassList("inventory-slot");
            slotElement.AddToClassList("disabled");
            _slotContainerElement.Add(slotElement);
        }
    }

    private void SelectDefaultSlot()
    {
        if (_currentSelectedSlotId != -1)
        {
            SelectSlot(_currentSelectedSlotId);
        }
    }

    public void SelectSlot(int slotPosition)
    {
        if (_currentSelectedSlotId != -1)
        {
            _slots[_currentSelectedSlotId].UnSelect();
        }
        _slots[slotPosition].SetSelected();
        _currentSelectedSlotId = slotPosition;
    }

    public void UpdateSlots(int slotCount, int rowLength, L2Slot.SlotType slotType)
    {
        _rowLength = rowLength;
        UpdateSlots(slotCount, slotType);
    }

    public void UpdateSlots(int slotCount, L2Slot.SlotType slotType)
    {
        ClearSlots();

        // Create empty slots
        _slots = new L2Slot[slotCount];

        CreateSlotElements(slotCount, slotType);

        PadSlots(slotCount);

        SelectDefaultSlot();
    }

    public virtual void CreateSlotElements(int slotCount, L2Slot.SlotType slotType)
    {
        for (int i = 0; i < slotCount; i++)
        {
            VisualElement slotElement = L2SlotManager.Instance.InventorySlotTemplate.Instantiate()[0];
            _slotContainerElement.Add(slotElement);
            L2Slot slot = null;

            if (slotType == L2Slot.SlotType.Inventory || slotType == L2Slot.SlotType.InventoryBis)
            {
                slot = new InventorySlot(i, slotElement, this, slotType);
            }
            else if (slotType == L2Slot.SlotType.Action)
            {
                slot = new ActionSlot(slotElement, i, slotType);
            }
            else if (slotType == L2Slot.SlotType.Product)
            {
                slot = new ProductSlot(i, slotElement, this, slotType);
            }

            _slots[i] = slot;
        }
    }


    public void AssignAction(int position, ActionType action)
    {
        ((ActionSlot)_slots[position]).AssignAction(action);
    }

    public virtual void AssignItemsToSlots(List<ItemInstance> items)
    {
        // Assign items to slots
        items?.ForEach(item =>
        {
            if (item.Slot < _slots.Length)
            {
                _slots[item.Slot].AssignItem(item);
            }
            else
            {
                Debug.LogWarning("Item slot index out of range");
            }
        });
    }

}