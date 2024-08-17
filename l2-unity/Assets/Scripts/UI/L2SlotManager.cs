using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class L2SlotManager : L2PopupWindow
{
    [SerializeField] private L2Slot _draggedSlot;
    [SerializeField] private L2Slot _hoverSlot;
    private L2Slot _dragSlotData;

    private static L2SlotManager _instance;
    public static L2SlotManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/Slot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
        _dragSlotData = new L2Slot(_windowEle, 0, 0, "", "", "", L2Slot.SlotType.Other);
        _windowEle.usageHints = UsageHints.None;
        _windowEle.pickingMode = PickingMode.Ignore;
        _windowEle.style.position = Position.Absolute;
    }

    public void SetHoverSlot(L2Slot slot)
    {
        _hoverSlot = slot;
    }

    public void SetDraggedSlot(L2Slot slot)
    {
        _draggedSlot = slot;

        _dragSlotData.Icon = slot.Icon;
        _dragSlotData.Id = slot.Id;

        StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(slot.Id));
        _dragSlotData.SlotBg.style.backgroundImage = background;
    }

    internal void DragSlot(float right, float bottom)
    {
        ShowWindow();
        _windowEle.style.right = right;
        _windowEle.style.bottom = bottom;
    }

    public void ReleaseDrag()
    {
        HideWindow();

        if (!IsValidDrag() || IsSameSlot())
        {
            ResetSlots();
            return;
        }

        bool isDraggedGear = _draggedSlot.Type == L2Slot.SlotType.Gear;
        bool isDraggedInventory = _draggedSlot.Type == L2Slot.SlotType.Inventory;
        bool isHoverInventory = _hoverSlot.Type == L2Slot.SlotType.Inventory;
        bool isHoverGear = _hoverSlot.Type == L2Slot.SlotType.Gear;

        if (_hoverSlot == null)
        {
            DropItem();
            ResetSlots();
            return;
        }

        if (isDraggedGear && isHoverInventory && !isHoverGear)
        {
            Unequip();
        }
        else if (isDraggedInventory)
        {
            HandleInventorySlotDrag((InventorySlot)_draggedSlot, isHoverGear, isHoverInventory);
        }
        else if (!isHoverInventory && !!isHoverGear)
        {
            //skillbar?
        }

        ResetSlots();
    }

    private bool IsValidDrag()
    {
        return _draggedSlot != null && (_hoverSlot != null || !L2GameUI.Instance.MouseOverUI); // Either dragging in the void or on another slot?
    }

    private bool IsSameSlot()
    {
        return _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position; // Is it the same slot ?
    }

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
    }

    private void HandleInventorySlotDrag(InventorySlot inventorySlot, bool isHoverGear, bool isHoverInventory)
    {
        if (isHoverGear && IsItemGear(inventorySlot.ItemCategory))
        {
            Equip();
        }
        else if (isHoverInventory && !isHoverGear)
        {
            ChangeItemOrder();
        }
        else
        {
            //skillbar?
        }
    }

    private bool IsItemGear(ItemCategory category)
    {
        return category == ItemCategory.Weapon
            || category == ItemCategory.ShieldArmor
            || category == ItemCategory.Jewel;
    }

    private void Unequip()
    {
        // Unequip logic
        GearSlot slot = (GearSlot)_draggedSlot;
        Debug.Log($"Unequip {slot.Id}.");
        slot.UseItem();
    }

    private void Equip()
    {
        // Equip logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        Debug.Log($"Equip {slot.Id}.");
        slot.UseItem();
    }

    private void DropItem()
    {
        // Drop item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        Debug.Log($"Drop {slot.Id}.");
    }

    private void ChangeItemOrder()
    {
        int fromSlot = _draggedSlot.Position;
        int toSlot = _hoverSlot.Position;

        Debug.Log($"Moving item from slot {fromSlot} to slot {toSlot}.");

        PlayerInventory.Instance.ChangeItemOrder(fromSlot, toSlot);
    }

    public override void ShowWindow()
    {
        _windowEle.style.opacity = 1;
    }

    public override void HideWindow()
    {
        _windowEle.style.opacity = 0;
    }
}
