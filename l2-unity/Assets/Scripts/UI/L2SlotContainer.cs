using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2SlotContainer : L2Scrollable
{
    protected bool _autoscroll = true;

    private VisualElement _slotContainerElement;
    public VisualElement Container { get { return _container; } }
    private L2Slot[] _slots;
    public L2Slot[] Slots { get => _slots; }
    private int _currentSelectedSlotId;
    private int _colLength;
    private int _rowLength;

    public virtual void Initialize(VisualElement container, int colLenght, int rowLength)
    {
        base.Initialize(container, false);
        _container = container;
        _slotContainerElement = container.Q<VisualElement>("Content");
        _currentSelectedSlotId = -1;
        _colLength = colLenght;
        _rowLength = rowLength;
    }

    private void ClearSlots()
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

        _slotContainerElement.Clear();
    }

    private void PadSlots(int slotCount)
    {
        // Add disabled slot to fill up the window
        int padSlot = 0;
        if (slotCount < _colLength * _rowLength)
        {
            padSlot = _colLength * _rowLength - slotCount;
        }
        else if (slotCount % _rowLength != 0)
        {
            padSlot = _rowLength - slotCount % _rowLength;
        }

        for (int i = 0; i < padSlot; i++)
        {
            VisualElement slotElement = ShopWindow.Instance.ShopSlotTemplate.Instantiate()[0];
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

    public void UpdateSlots(int slotCount, int colLenght, int rowLength)
    {
        _colLength = colLenght;
        _rowLength = rowLength;
        UpdateSlots(slotCount, colLenght, rowLength);
    }

    public void UpdateSlots(int slotCount)
    {
        ClearSlots();

        // Create empty slots
        _slots = new L2Slot[slotCount];

        CreateSlotElements(slotCount);

        PadSlots(slotCount);

        SelectDefaultSlot();
    }

    public virtual void CreateSlotElements(int slotCount)
    {
        L2Slot.SlotType slotType = L2Slot.SlotType.Inventory;
        for (int i = 0; i < slotCount; i++)
        {
            VisualElement slotElement = ShopWindow.Instance.ShopSlotTemplate.Instantiate()[0];
            _slotContainerElement.Add(slotElement);

            L2Slot slot = new InventorySlot(i, slotElement, this, slotType);
            _slots[i] = slot;
        }
    }

    public virtual void AssignItemsToSlots(List<ItemInstance> items)
    {
        // Assign items to slots
        items?.ForEach(item =>
        {
            if (item.Slot < _slots.Length)
            {
                ((InventorySlot)_slots[item.Slot]).AssignItem(item);
            }
            else
            {
                Debug.LogWarning("Item slot index out of range");
            }
        });
    }
}