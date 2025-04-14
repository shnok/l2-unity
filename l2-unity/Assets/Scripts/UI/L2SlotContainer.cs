using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class L2SlotContainer : L2Scrollable
{
    private VisualElement _slotContainerElement;
    protected L2Slot[] _slots;
    public L2Slot[] Slots { get => _slots; }
    [SerializeField] private int _currentSelectedSlotId;
    [SerializeField] private int _rowLength;
    [SerializeField] private int _minimumContainerSize;

    public virtual void Initialize(VisualElement container, int rowLength, int minimumContainerSize)
    {
        base.Initialize(container, false);
        _container = container;
        _slotContainerElement = container.Q<VisualElement>("Content");
        _currentSelectedSlotId = -1;
        _rowLength = rowLength;
        _minimumContainerSize = minimumContainerSize;
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
        CreateSlots(slotCount, slotType);
    }

    public void CreateSlots(int slotCount, L2Slot.SlotType slotType)
    {
        ClearSlots();

        slotCount = slotCount > _minimumContainerSize ? slotCount : _minimumContainerSize;
        // Create empty slots
        _slots = new L2Slot[slotCount];

        CreateSlotElements(slotCount, slotType);

        PadSlots(slotCount);

        SelectDefaultSlot();
    }

    protected virtual void CreateSlotElements(int slotCount, L2Slot.SlotType slotType)
    {
        for (int i = 0; i < slotCount; i++)
        {
            VisualElement slotElement;
            L2Slot slot;

            switch (slotType)
            {
                case L2Slot.SlotType.Inventory:
                case L2Slot.SlotType.InventoryBis:
                    slotElement = L2SlotManager.Instance.InventorySlotTemplate.Instantiate()[0];
                    slot = new InventorySlot(i, slotElement, this, slotType);
                    break;
                case L2Slot.SlotType.Action:
                    slotElement = L2SlotManager.Instance.ActionSlotTemplate.Instantiate()[0];
                    slot = new ActionSlot(slotElement, i, slotType);
                    break;
                case L2Slot.SlotType.Product:
                    slotElement = L2SlotManager.Instance.ShopSlotTemplate.Instantiate()[0];
                    slot = new ProductSlot(i, slotElement, this, slotType);
                    break;
                case L2Slot.SlotType.Basket:
                    slotElement = L2SlotManager.Instance.ShopSlotTemplate.Instantiate()[0];
                    slot = new BasketSlot(i, slotElement, this, slotType);
                    break;
                case L2Slot.SlotType.Skill:
                    slotElement = L2SlotManager.Instance.SkillSlotTemplate.Instantiate()[0];
                    slot = new SkillSlot(i, slotElement, slotType);
                    break;
                default:
                    Debug.LogWarning("Invalid slot type assigned to slot container!");
                    continue;
            }

            _slotContainerElement.Add(slotElement);

            _slots[i] = slot;
        }
    }


    public void AssignAction(int position, ActionType action)
    {
        ((ActionSlot)_slots[position]).AssignAction(action);
    }

    public void AssignSkill(int position, SkillWindowInfo skillInfo)
    {
        ((SkillSlot)_slots[position]).AssignSkill(skillInfo);
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