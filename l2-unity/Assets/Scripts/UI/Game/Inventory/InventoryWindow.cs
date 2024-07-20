using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryWindow : L2PopupWindow {
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _inventorySlotTemplate;
    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }

    [SerializeField] private InventoryGearTab _gearTab;

    private VisualElement _inventoryTabView;
    [SerializeField] private List<InventoryTab> _tabs;
    private InventoryTab _activeTab;

    public List<ItemInstance> _playerItems;

    private static InventoryWindow _instance;
    public static InventoryWindow Instance {
        get { return _instance; }
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        /* TO REMOVE FOR TESTING ONLY */
        ItemTable.Instance.Initialize();
        ItemNameTable.Instance.Initialize();
        ItemStatDataTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        WeapongrpTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();
        IconManager.Instance.Initialize();
        IconManager.Instance.CacheIcons();

        /* TO REMOVE FOR TESTING ONLY */
        _playerItems = new List<ItemInstance> ();


        _playerItems.Add(new ItemInstance(0, 28, ItemLocation.Inventory, 1, 1, ItemCategory.ShieldArmor, false, ItemSlot.legs, 0));
        _playerItems.Add(new ItemInstance(0, 48, ItemLocation.Inventory, 2, 1, ItemCategory.ShieldArmor, false, ItemSlot.gloves, 0));
        _playerItems.Add(new ItemInstance(0, 45, ItemLocation.Inventory, 3, 1, ItemCategory.ShieldArmor, false, ItemSlot.head, 0));
        _playerItems.Add(new ItemInstance(0, 35, ItemLocation.Inventory, 4, 1, ItemCategory.ShieldArmor, false, ItemSlot.feet, 0));
        _playerItems.Add(new ItemInstance(0, 21, ItemLocation.Inventory, 5, 1, ItemCategory.ShieldArmor, false, ItemSlot.chest, 0));
        _playerItems.Add(new ItemInstance(0, 2, ItemLocation.Inventory, 6, 1, ItemCategory.Weapon, false, ItemSlot.rhand, 0));
        _playerItems.Add(new ItemInstance(0, 20, ItemLocation.Inventory, 7, 1, ItemCategory.ShieldArmor, false, ItemSlot.lhand, 0));

        _playerItems.Add(new ItemInstance(0, 57, ItemLocation.Inventory, 14, 69420, ItemCategory.Adena, false, ItemSlot.none, 0));
        _playerItems.Add(new ItemInstance(0, 1835, ItemLocation.Inventory, 15, 69420, ItemCategory.Item, false, ItemSlot.none, 0));
        _playerItems.Add(new ItemInstance(0, 3947, ItemLocation.Inventory, 16, 69420, ItemCategory.Item, false, ItemSlot.none, 0));
        _playerItems.Add(new ItemInstance(0, 2509, ItemLocation.Inventory, 17, 69420, ItemCategory.Item, false, ItemSlot.none, 0));
        _playerItems.Add(new ItemInstance(0, 736, ItemLocation.Inventory, 18, 69420, ItemCategory.Item, false, ItemSlot.none, 0));

        _playerItems.Add(new ItemInstance(0, 118, ItemLocation.Inventory, 45, 1, ItemCategory.ShieldArmor, false, ItemSlot.neck, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Inventory, 46, 1, ItemCategory.ShieldArmor, false, ItemSlot.lear, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Inventory, 47, 1, ItemCategory.ShieldArmor, false, ItemSlot.rear, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Inventory, 48, 1, ItemCategory.ShieldArmor, false, ItemSlot.rfinger, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Inventory, 49, 1, ItemCategory.ShieldArmor, false, ItemSlot.lfinger, 0));



        // gear
        _playerItems.Add(new ItemInstance(0, 28,  ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.legs, 0));
        _playerItems.Add(new ItemInstance(0, 48, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.gloves, 0));
        _playerItems.Add(new ItemInstance(0, 45,  ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.head, 0));
        _playerItems.Add(new ItemInstance(0, 35,  ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.feet, 0));
        _playerItems.Add(new ItemInstance(0, 21,  ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.chest, 0));
        _playerItems.Add(new ItemInstance(0, 4,  ItemLocation.Equipped, 0, 1, ItemCategory.Weapon, true,  ItemSlot.rhand, 0));
        _playerItems.Add(new ItemInstance(0, 20,  ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true,  ItemSlot.lhand, 0));

        _playerItems.Add(new ItemInstance(0, 118, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true, ItemSlot.neck, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true, ItemSlot.lear, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true, ItemSlot.rear, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true, ItemSlot.rfinger, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Equipped, 0, 1, ItemCategory.ShieldArmor, true, ItemSlot.lfinger, 0));
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTabHeader");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/Slot");
    }

    protected override void InitWindow(VisualElement root) {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        CreateTabs();

        yield return new WaitForEndOfFrame();

        /* TO REMOVE FOR TESTING ONLY */
        UpdateItemList(_playerItems);
    }


    private void CreateTabs() {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null) {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null) {
            Debug.LogError("tab-content-container");
        }

        for (int i = _tabs.Count -1; i >= 0; i--) {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            // tabElement.name = _tabs[i].TabName;
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement);
        }

        if (_tabs.Count > 0) {
            SwitchTab(_tabs[0]);
        }

        _gearTab = new InventoryGearTab();
        _gearTab.Initialize(_windowEle, null, null);
    }

    public bool SwitchTab(InventoryTab switchTo) {
        if (_activeTab != switchTo) {
            if (_activeTab != null) {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            //ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;
            return true;
        }

        return false;
    }

    public void UpdateItemList(List<ItemInstance> items) {
        _playerItems = items;

        _gearTab.UpdateItemList(items);

        _tabs.ForEach((tab) => {
            tab.UpdateItemList(items);
        });
    }
}
