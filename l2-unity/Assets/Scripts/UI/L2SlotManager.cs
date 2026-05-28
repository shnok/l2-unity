using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class L2SlotManager : L2PopupWindow
{
    [SerializeField] private L2Slot _draggedSlot;
    [SerializeField] private L2Slot _hoverSlot;
    private L2Slot _dragSlotData;
    private VisualTreeAsset _actionSlotTemplate;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _shopSlotTemplate;
    private VisualTreeAsset _skillSlotTemplate;
    private VisualTreeAsset _skillBarSlotTemplate;
    public VisualTreeAsset ActionSlotTemplate { get { return _actionSlotTemplate; } }
    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    public VisualTreeAsset ShopSlotTemplate { get { return _shopSlotTemplate; } }
    public VisualTreeAsset SkillSlotTemplate { get { return _skillSlotTemplate; } }
    public VisualTreeAsset SkillBarSlotTemplate { get { return _skillBarSlotTemplate; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/DraggedSlot");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/InventorySlot");
        _actionSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/ActionSlot");
        _shopSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/InventorySlot");
        _skillSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/SkillSlot");
        _skillBarSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/SkillbarSlot");
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

        StyleBackground background = slot.SlotBg.style.backgroundImage;
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
        HideWindow(false);

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
            case L2Slot.SlotType.Action:
                HandleActionDrag();
                break;
            case L2Slot.SlotType.Skill:
                HandleSkillDrag();
                break;
            case L2Slot.SlotType.Product:
                HandleProductDrag();
                break;
            case L2Slot.SlotType.Basket:
                HandleBasketDrag();
                break;
            default:
                break;
        }
    }

    #region DragReleaseHandlers
    private void HandleActionDrag()
    {
        if (_hoverSlot == null)
        {
            return;
        }

        if (_hoverSlot.Type == L2Slot.SlotType.SkillBar)
        {
            AddActionToSkillbar();
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

        if (_hoverSlot == null)
        {
            if (!L2GameUI.Instance.MouseOverUI)
            {
                DropItem();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.Gear when IsItemGear(inventorySlot.Type1):
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

            return;
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

    private void HandleSkillDrag()
    {
        if (SkillbarWindow.Instance.Locked)
        {
            return;
        }

        if (_hoverSlot == null)
        {
            return;
        }

        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.SkillBar:
                AddSkillToSkillbar();
                break;
            default:
                break;
        }
    }


    private void HandleProductDrag()
    {
        ProductSlot productSlot = (ProductSlot)_draggedSlot;

        if (_hoverSlot == null)
        {
            return;
        }


        switch (_hoverSlot.Type)
        {
            case L2Slot.SlotType.Basket:
                // ((ShopSlotContainer)((BasketSlot)_hoverSlot).SlotContainer).AddToBasket(productSlot.Product, 1);
                productSlot.SwapBasket();
                break;
            default:
                break;
        }
    }

    private void HandleBasketDrag()
    {
        BasketSlot productSlot = (BasketSlot)_draggedSlot;

        if (_hoverSlot == null || _hoverSlot.Type != L2Slot.SlotType.Basket)
        {
            // ((ShopSlotContainer)productSlot.SlotContainer).RemoveFromBasket(productSlot.Product, productSlot.Position);
            productSlot.SwapBasket();
        }
    }

    #endregion

    #region SlotVerification
    private bool IsValidDrag() => _draggedSlot != null && (_hoverSlot != null || !L2GameUI.Instance.MouseOverUI);

    private bool IsSameSlot() => _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position && _draggedSlot.Type == _hoverSlot.Type;

    private bool IsItemGear(ItemType1 category)
    {
        return category == ItemType1.TYPE1_SHIELD_ARMOR
            || category == ItemType1.TYPE1_WEAPON_RING_EARRING_NECKLACE;
    }

    #endregion

    #region DragActions

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
        Debug.LogWarning("TODO: Item drops. For now dropping an item destroys it.");

        // Drop item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        SMParam[] smParams = new SMParam[1];
        smParams[0] = new SMParam(SMParam.SMParamType.TYPE_ITEM_NAME, slot.Id);

        if (slot.Count <= 1)
        {
            SystemMessage systemMessage = new SystemMessage(smParams, SystemMessageTable.Instance.SystemMessages[400]);
            L2ConfirmWindow.Instance.ShowWindow(systemMessage, () =>
            {
                PlayerInventory.Instance.DestroyItem(slot.ObjectId, 1);
            }, () => { });
        }
        else
        {
            //Discard amount systemMessageId => 71
            SystemMessage systemMessage = new SystemMessage(smParams, SystemMessageTable.Instance.SystemMessages[71]);
            L2InputAmountWindow.Instance.ShowWindow(systemMessage, slot.Count, (amount) =>
            {
                PlayerInventory.Instance.DestroyItem(slot.ObjectId, amount);
            }, () => { });
        }

        Debug.Log($"Drop {slot.Id}.");
    }

    private void DestroyItem()
    {
        // Destroy item logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        SMParam[] smParams = new SMParam[1];
        smParams[0] = new SMParam(SMParam.SMParamType.TYPE_ITEM_NAME, slot.Id);
        if (slot.Count <= 1)
        {
            SystemMessage systemMessage = new SystemMessage(smParams, SystemMessageTable.Instance.SystemMessages[74]);
            L2ConfirmWindow.Instance.ShowWindow(systemMessage, () =>
            {
                PlayerInventory.Instance.DestroyItem(slot.ObjectId, 1);
            }, () => { });
        }
        else
        {
            SystemMessage systemMessage = new SystemMessage(smParams, SystemMessageTable.Instance.SystemMessages[73]);
            L2InputAmountWindow.Instance.ShowWindow(systemMessage, slot.Count, (amount) =>
            {
                PlayerInventory.Instance.DestroyItem(slot.ObjectId, amount);
            }, () => { });
        }

        Debug.Log($"Destroy {slot.Id}.");
    }

    private void AddActionToSkillbar()
    {
        int actionId = ((ActionSlot)_draggedSlot).ActionId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add action ID {actionId} into skillbar slot {slot}.");

        PlayerShortcuts.Instance.AddShortcut(slot, actionId, Shortcut.TYPE_ACTION);
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
        int itemId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add item {itemId} to skillbar slot {slot}.");

        PlayerShortcuts.Instance.AddShortcut(slot, itemId, Shortcut.TYPE_ITEM);
    }

    private void MoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        int newSlot = _hoverSlot.Position;
        Debug.LogWarning($"Moving skillbar shortcut from slot {oldSlot} to slot {newSlot}.");
        PlayerShortcuts.Instance.MoveShortcut(oldSlot, newSlot);
    }

    private void RemoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        Debug.LogWarning($"Removing skillbar shortcut from slot {oldSlot}.");
        PlayerShortcuts.Instance.DeleteShortcut(oldSlot);
    }

    private void AddSkillToSkillbar()
    {
        int skillId = ((SkillSlot)_draggedSlot).Skill.SkillId;
        int slot = _hoverSlot.Position;
        Debug.LogWarning($"Add skill {skillId} to skillbar slot {slot}.");

        PlayerShortcuts.Instance.AddShortcut(slot, skillId, Shortcut.TYPE_SKILL);
    }

    private void RemoveSkillSlot()
    {
        int oldSlot = _draggedSlot.Position;
        Debug.LogWarning($"Removing skillbar shortcut from slot {oldSlot}.");
        PlayerShortcuts.Instance.DeleteShortcut(oldSlot);
    }

    #endregion

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
    }

    public override void ShowWindow()
    {
        _windowEle.style.opacity = 1;
    }

    public override void HideWindow(bool silent)
    {
        _windowEle.style.opacity = 0;
    }
}
