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
        _dragSlotData = new L2Slot(_windowEle);
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

        HandleDragRelease();
        ResetSlots();
    }

    private void HandleDragRelease()
    {
        switch (_draggedSlot.Type)
        {
            case L2Slot.SlotType.Gear:
                HandleGearDrag();
                break;
            case L2Slot.SlotType.Inventory:
            case L2Slot.SlotType.InventoryBis:
                HandleInventoryDrag();
                break;
            case L2Slot.SlotType.SkillBar:
                HandleSkillbarDrag();
                break;
            default:
                break;
        }
    }

    private void HandleGearDrag()
    {
        if (_hoverSlot.Type == L2Slot.SlotType.Inventory || _hoverSlot.Type == L2Slot.SlotType.InventoryBis)
        {
            Unequip();
        }
        else if (_hoverSlot.Type == L2Slot.SlotType.Trash)
        {
            DestroyItem();
        }
    }

    private void HandleInventoryDrag()
    {
        var inventorySlot = (InventorySlot)_draggedSlot;

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.Gear when IsItemGear(inventorySlot.ItemCategory):
                Equip();
                break;
            case L2Slot.SlotType.Inventory:
                ChangeItemOrder();
                break;
            case L2Slot.SlotType.Trash:
                DestroyItem();
                break;
            case L2Slot.SlotType.SkillBar:
                AddItemToSkillbar();
                break;
            default:
                if (!L2GameUI.Instance.MouseOverUI)
                {
                    DropItem();
                }
                break;
        }
    }

    private void HandleSkillbarDrag()
    {
        if (SkillbarWindow.Instance.Locked)
        {
            return;
        }

        if (_hoverSlot == null)
        {
            if (!L2GameUI.Instance.MouseOverUI)
            {
                RemoveSkillbarSlot();
            }
        }

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.SkillBar:
                MoveSkillbarSlot();
                break;
            default:
                if (!L2GameUI.Instance.MouseOverUI)
                {
                    RemoveSkillbarSlot();
                }
                break;
        }
    }

    private bool IsValidDrag() => _draggedSlot != null && (_hoverSlot != null || !L2GameUI.Instance.MouseOverUI);

    private bool IsSameSlot() => _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position;

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
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

    private void DestroyItem()
    {
        // Destroy item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        Debug.Log($"Destroy {slot.Id}.");
        PlayerInventory.Instance.DestroyItem(slot.ObjectId, 1);
    }

    private void ChangeItemOrder()
    {
        int fromSlot = _draggedSlot.Position;
        int toSlot = _hoverSlot.Position;

        Debug.Log($"Moving item from slot {fromSlot} to slot {toSlot}.");

        PlayerInventory.Instance.ChangeItemOrder(fromSlot, toSlot);
    }

    private void AddItemToSkillbar()
    {
        int itemId = ((InventorySlot)_draggedSlot).Id;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add item {itemId} to skillbar slot {slot}.");
    }

    private void MoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        int newSlot = _hoverSlot.Position;
        Debug.LogWarning($"Moving skillbar shortcut from slot {oldSlot} to slot {newSlot}.");
    }

    private void RemoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        Debug.LogWarning($"Renoving skillbar shortcut from slot {oldSlot}.");
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
