using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryTab : L2Tab
{
    private int _itemCount = 0;

    private VisualElement _contentContainer;
    public List<ItemType1> _filteredCategories;
    public bool MainTab { get; internal set; }
    private L2SlotContainer _slotContainer;

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(tabView, tabContainer, tabHeader);

        _contentContainer = tabContainer.Q<VisualElement>("Content");
        _slotContainer = new L2SlotContainer();
        _slotContainer.Initialize(_contentContainer, 9, 8);
    }

    public void UpdateItemList(List<ItemInstance> items)
    {
        _slotContainer.UpdateSlots(PlayerInventory.Instance.InventorySize, InventoryWindow.Instance.Expanded ? 12 : 9, 8);

        // _slotContainer.AssignItemsToSlots(items);

        // Assign items to slots
        items.ForEach(item =>
        {
            if (item.Location == ItemLocation.Inventory)
            {
                if (_filteredCategories == null || _filteredCategories.Count == 0)
                {
                    ((InventorySlot)_slotContainer.Slots[item.Slot]).AssignItem(item);
                    _itemCount++;
                }
                else if (_filteredCategories.Contains(item.Type1))
                {
                    ((InventorySlot)_slotContainer.Slots[_itemCount++]).AssignItem(item);
                }
            }
        });
    }

    public override void SelectSlot(int slotPosition)
    {
        _slotContainer.SelectSlot(slotPosition);
    }

    protected override void OnTabHeaderClicked()
    {
        base.OnTabHeaderClicked();
    }
}
