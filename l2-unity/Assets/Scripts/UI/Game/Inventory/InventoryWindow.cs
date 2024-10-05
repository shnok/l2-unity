using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryWindow : L2PopupWindow
{
    public static int PLAYER_INVENTORY_SIZE = 80;

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _minimizedTemplate;
    private VisualElement _inventoryTabView;
    private VisualElement _minimizedInventoryBtn;
    private InventoryTab _activeTab;

    private MouseOverDetectionManipulator _minimizedInventoryMouseOverManipulator;
    private DragManipulator _minimizedInventoryDragManipulator;
    [SerializeField] private bool _expanded = false;
    private VisualElement _expandButton;

    [SerializeField] private InventoryGearTab _gearTab;
    [SerializeField] private List<InventoryTab> _tabs;

    public List<ItemInstance> _playerItems;

    [SerializeField] private int _usedSlots;
    [SerializeField] private int _slotCount;
    // [SerializeField] private int _currentWeight;
    // [SerializeField] private int _maximumWeight;
    [SerializeField] private int _adenaCount;

    private Label _inventoryCountLabel;
    private Label _weightLabel;
    private Label _adenaCountLabel;

    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    public bool Expanded { get { return _expanded; } }
    public int UsedSlots { get { return _usedSlots; } }
    public int SlotCount { get { return _slotCount; } }

    private static InventoryWindow _instance;
    public static InventoryWindow Instance
    {
        get { return _instance; }
    }

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

        // ItemTable.Instance.Initialize();
        // ItemNameTable.Instance.Initialize();
        // ItemStatDataTable.Instance.Initialize();
        // ArmorgrpTable.Instance.Initialize();
        // EtcItemgrpTable.Instance.Initialize();
        // WeapongrpTable.Instance.Initialize();
        // ItemTable.Instance.CacheItems();
        // NpcgrpTable.Instance.Initialize();
        // NpcNameTable.Instance.Initialize();
        // ActionNameTable.Instance.Initialize();
        // SysStringTable.Instance.Initialize();
        // SkillNameTable.Instance.Initialize();
        // SkillgrpTable.Instance.Initialize();
        // SystemMessageTable.Instance.Initialize();
        // IconTable.Instance.Initialize();
        // KeyImageTable.Instance.Initialize();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTabHeader");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/InventorySlot");
        _minimizedTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryMin");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        _expanded = false;

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Inventory";

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        _expandButton = (Button)GetElementByClass("expand-btn");
        _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
        _expandButton.RegisterCallback<MouseDownEvent>(OnExpandButtonPressed, TrickleDown.TrickleDown);

        Button adenaDistribution = (Button)GetElementById("AdenaDistribBtn");
        adenaDistribution.AddManipulator(new ButtonClickSoundManipulator(adenaDistribution));
        adenaDistribution.AddManipulator(new TooltipManipulator(adenaDistribution, "Adena distribution"));

        Button compoundBtn = (Button)GetElementById("CompoundBtn");
        compoundBtn.AddManipulator(new ButtonClickSoundManipulator(compoundBtn));
        compoundBtn.AddManipulator(new TooltipManipulator(adenaDistribution, "Compound"));

        Button trashBtn = (Button)GetElementById("TrashBtn");
        trashBtn.AddManipulator(new ButtonClickSoundManipulator(trashBtn));
        trashBtn.AddManipulator(new TooltipManipulator(adenaDistribution, "Trash"));
        L2Slot trashSlot = new L2Slot(trashBtn, 0, L2Slot.SlotType.Trash);

        _inventoryCountLabel = GetLabelById("InventoryCount");
        _adenaCountLabel = GetLabelById("AdenaCount");
        _weightLabel = GetLabelById("CurrentWeight");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        CreateTabs();

        CreateMinimizedWindow();

        yield return new WaitForEndOfFrame();

        UpdateItemList(_playerItems);

#if UNITY_EDITOR
        // DebugData();
#endif
    }

    private void OnExpandButtonPressed(MouseDownEvent evt)
    {
        if (!_expanded)
        {
            if (!_windowEle.ClassListContains("expanded"))
            {
                _windowEle.AddToClassList("expanded");
            }
            if (!_expandButton.ClassListContains("expanded"))
            {
                _expandButton.AddToClassList("expanded");
            }
            _expanded = true;

            UpdateItemList(_playerItems);
        }
        else
        {
            if (_windowEle.ClassListContains("expanded"))
            {
                _windowEle.RemoveFromClassList("expanded");
            }
            if (_expandButton.ClassListContains("expanded"))
            {
                _expandButton.RemoveFromClassList("expanded");
            }
            _expanded = false;

            UpdateItemList(_playerItems);
        }

        evt.PreventDefault();
    }

    private void CreateMinimizedWindow()
    {

        // Header button
        Button minimizeWindowButton = (Button)GetElementByClass("minimize-btn");
        minimizeWindowButton.AddManipulator(new ButtonClickSoundManipulator(minimizeWindowButton));
        minimizeWindowButton.RegisterCallback<MouseUpEvent>(OnMinimizeInventoryClick, TrickleDown.TrickleDown);

        // Minized inventory button
        _minimizedInventoryBtn = _minimizedTemplate.Instantiate()[0];
        _minimizedInventoryMouseOverManipulator = new MouseOverDetectionManipulator(_minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryMouseOverManipulator);
        _minimizedInventoryMouseOverManipulator.Disable();

        _minimizedInventoryBtn.style.left = new StyleLength(Screen.width / 2);
        _minimizedInventoryBtn.style.top = new StyleLength(Screen.height / 2);

        _minimizedInventoryBtn.RegisterCallback<ClickEvent>(OnMinimizedInventoryClick, TrickleDown.TrickleDown);
        _minimizedInventoryDragManipulator = new DragManipulator(_minimizedInventoryBtn, _minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryDragManipulator);

        _root.Add(_minimizedInventoryBtn);
    }

    private void OnMinimizeInventoryClick(MouseUpEvent evt)
    {
        _minimizedInventoryBtn.style.display = DisplayStyle.Flex;
        _minimizedInventoryMouseOverManipulator.Enable();
        HideWindow();
    }

    private void OnMinimizedInventoryClick(ClickEvent evt)
    {
        if (!_minimizedInventoryDragManipulator.dragged)
        {
            AudioManager.Instance.PlayUISound("click_01");
            _minimizedInventoryBtn.style.display = DisplayStyle.None;
            _minimizedInventoryMouseOverManipulator.Disable();
            ShowWindow();
        }
    }

    private void CreateTabs()
    {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null)
        {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null)
        {
            Debug.LogError("tab-content-container");
        }

        for (int i = _tabs.Count - 1; i >= 0; i--)
        {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement);
        }

        if (_tabs.Count > 0)
        {
            SwitchTab(_tabs[0]);
        }

        _tabs[0].MainTab = true;

        _gearTab = new InventoryGearTab();
        _gearTab.Initialize(_windowEle, null, null);
    }

    public bool SwitchTab(InventoryTab switchTo)
    {
        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
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

    public void UpdateItemList(List<ItemInstance> items)
    {
        _adenaCount = 0;
        _usedSlots = 0;

        if (items == null)
        {
            items = new List<ItemInstance>();
        }
        else if (items.Count > 0)
        {
            _usedSlots = items.Where(o => o.Location == ItemLocation.Inventory).Count();

            ItemInstance adenaItem = items.First(o => o.Category == ItemCategory.Adena);
            if (adenaItem != null)
            {
                _adenaCount = adenaItem.Count;
            }
        }

        _playerItems = items;

        // Slot count
        _slotCount = PLAYER_INVENTORY_SIZE;
        _inventoryCountLabel.text = $"({_usedSlots}/{_slotCount})";

        //Adena
        _adenaCountLabel.text = _adenaCount.ToString();

        // _currentWeight = 0;
        // _maximumWeight = 10000;
        _weightLabel.text = "00.00%";

        // Tabs
        _gearTab.UpdateItemList(items);

        _tabs.ForEach((tab) =>
        {
            tab.UpdateItemList(items);
        });
    }

    public override void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            GameClient.Instance.ClientPacketHandler.SendRequestOpenInventory();
        }
        else
        {
            HideWindow();
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("inventory_open_01");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        AudioManager.Instance.PlayUISound("inventory_close_01");
        L2GameUI.Instance.WindowClosed(this);
    }

    public void SelectSlot(int slot)
    {
        _tabs[0].SelectSlot(slot);
    }

#if UNITY_EDITOR
    public void DebugData()
    {
        /* TO REMOVE FOR TESTING ONLY */

        List<ItemInstance> _playerItems = new List<ItemInstance>();

        _playerItems.Add(new ItemInstance(0, 28, ItemLocation.Inventory, 1, 1, ItemCategory.ShieldArmor, false, ItemSlot.legs, 0, 0));
        _playerItems.Add(new ItemInstance(0, 48, ItemLocation.Inventory, 2, 1, ItemCategory.ShieldArmor, false, ItemSlot.gloves, 0, 0));
        _playerItems.Add(new ItemInstance(0, 45, ItemLocation.Inventory, 3, 1, ItemCategory.ShieldArmor, false, ItemSlot.head, 0, 0));
        _playerItems.Add(new ItemInstance(0, 35, ItemLocation.Inventory, 4, 1, ItemCategory.ShieldArmor, false, ItemSlot.feet, 0, 0));
        _playerItems.Add(new ItemInstance(0, 21, ItemLocation.Inventory, 5, 1, ItemCategory.ShieldArmor, false, ItemSlot.chest, 0, 0));
        _playerItems.Add(new ItemInstance(0, 2, ItemLocation.Inventory, 6, 1, ItemCategory.Weapon, false, ItemSlot.rhand, 0, 0));
        _playerItems.Add(new ItemInstance(0, 20, ItemLocation.Inventory, 7, 1, ItemCategory.ShieldArmor, false, ItemSlot.lhand, 0, 0));

        _playerItems.Add(new ItemInstance(0, 57, ItemLocation.Inventory, 14, 69420, ItemCategory.Adena, false, ItemSlot.none, 0, 0));
        _playerItems.Add(new ItemInstance(0, 1835, ItemLocation.Inventory, 15, 69420, ItemCategory.Item, false, ItemSlot.none, 0, 0));
        _playerItems.Add(new ItemInstance(0, 3947, ItemLocation.Inventory, 16, 69420, ItemCategory.Item, false, ItemSlot.none, 0, 0));
        _playerItems.Add(new ItemInstance(0, 2509, ItemLocation.Inventory, 17, 69420, ItemCategory.Item, false, ItemSlot.none, 0, 0));
        _playerItems.Add(new ItemInstance(0, 736, ItemLocation.Inventory, 18, 69420, ItemCategory.Item, false, ItemSlot.none, 0, 0));

        _playerItems.Add(new ItemInstance(0, 118, ItemLocation.Inventory, 45, 1, ItemCategory.ShieldArmor, false, ItemSlot.neck, 0, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Inventory, 46, 1, ItemCategory.ShieldArmor, false, ItemSlot.lear, 0, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Inventory, 47, 1, ItemCategory.ShieldArmor, false, ItemSlot.rear, 0, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Inventory, 48, 1, ItemCategory.ShieldArmor, false, ItemSlot.rfinger, 0, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Inventory, 49, 1, ItemCategory.ShieldArmor, false, ItemSlot.lfinger, 0, 0));

        // gear
        _playerItems.Add(new ItemInstance(0, 28, ItemLocation.Equipped, (int)ItemSlot.legs, 1, ItemCategory.ShieldArmor, true, ItemSlot.legs, 0, 0));
        _playerItems.Add(new ItemInstance(0, 48, ItemLocation.Equipped, (int)ItemSlot.gloves, 1, ItemCategory.ShieldArmor, true, ItemSlot.gloves, 0, 0));
        _playerItems.Add(new ItemInstance(0, 45, ItemLocation.Equipped, (int)ItemSlot.head, 1, ItemCategory.ShieldArmor, true, ItemSlot.head, 0, 0));
        _playerItems.Add(new ItemInstance(0, 35, ItemLocation.Equipped, (int)ItemSlot.feet, 1, ItemCategory.ShieldArmor, true, ItemSlot.feet, 0, 0));
        _playerItems.Add(new ItemInstance(0, 21, ItemLocation.Equipped, (int)ItemSlot.chest, 1, ItemCategory.ShieldArmor, true, ItemSlot.chest, 0, 0));
        _playerItems.Add(new ItemInstance(0, 4, ItemLocation.Equipped, (int)ItemSlot.rhand, 1, ItemCategory.Weapon, true, ItemSlot.rhand, 0, 0));
        _playerItems.Add(new ItemInstance(0, 20, ItemLocation.Equipped, (int)ItemSlot.lhand, 1, ItemCategory.ShieldArmor, true, ItemSlot.lhand, 0, 0));

        _playerItems.Add(new ItemInstance(0, 118, ItemLocation.Equipped, (int)ItemSlot.neck, 1, ItemCategory.ShieldArmor, true, ItemSlot.neck, 0, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Equipped, (int)ItemSlot.lear, 1, ItemCategory.ShieldArmor, true, ItemSlot.lear, 0, 0));
        _playerItems.Add(new ItemInstance(0, 112, ItemLocation.Equipped, (int)ItemSlot.rear, 1, ItemCategory.ShieldArmor, true, ItemSlot.rear, 0, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Equipped, (int)ItemSlot.rfinger, 1, ItemCategory.ShieldArmor, true, ItemSlot.rfinger, 0, 0));
        _playerItems.Add(new ItemInstance(0, 116, ItemLocation.Equipped, (int)ItemSlot.lfinger, 1, ItemCategory.ShieldArmor, true, ItemSlot.lfinger, 0, 0));

        PlayerInventory.Instance.SetInventory(_playerItems.ToArray(), true);
    }
#endif
}
