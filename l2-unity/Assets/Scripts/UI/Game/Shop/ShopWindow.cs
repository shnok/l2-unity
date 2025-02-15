using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopWindow : L2PopupWindow
{
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _shopSlotTemplate;
    private VisualElement _shopTabView;
    private ShopTab _activeTab;

    [SerializeField] private List<ShopTab> _tabs;

    public List<ItemInstance> _playerItems;

    [SerializeField] private int _usedSlots;
    [SerializeField] private int _slotCount;
    [SerializeField] private int _adenaCount;

    private Label _weightLabel;
    private Label _adenaCountLabel;
    private Label _priceLabel;

    private VisualElement _weightBarContainer;
    private VisualElement _weightBar;
    private VisualElement _weightBarBg;

    public VisualTreeAsset ShopSlotTemplate { get { return _shopSlotTemplate; } }
    public int UsedSlots { get { return _usedSlots; } }

    private static ShopWindow _instance;
    public static ShopWindow Instance
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/InventoryWindow/InventoryWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/InventoryWindow/InventoryTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/InventoryWindow/InventoryTabHeader");
        _shopSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/InventorySlot");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Store";

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        _adenaCountLabel = GetLabelById("AdenaCount");

        _weightBarContainer = GetElementById("WeightBar");
        _weightLabel = _weightBarContainer.Q<Label>("Text");
        _weightBar = _weightBarContainer.Q<VisualElement>("Bar");
        _weightBarBg = _weightBarContainer.Q<VisualElement>("BarBg");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        CreateTabs();

        yield return new WaitForEndOfFrame();

        // UpdateItemList(_playerItems);

#if UNITY_EDITOR
        // DebugData();
#endif

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void CreateTabs()
    {
        _shopTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _shopTabView.Q<VisualElement>("tab-header-container");
        if (tabHeaderContainer == null)
        {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _shopTabView.Q<VisualElement>("tab-content-container");

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
    }

    public bool SwitchTab(ShopTab switchTo)
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
        if (items == null)
        {
            items = new List<ItemInstance>();
        }

        _playerItems = items;

        RefreshSlotsAndAdenas();
        RefreshWeight();

        // Tabs
        _tabs.ForEach((tab) =>
        {
            tab.UpdateItemList(items);
        });
    }

    private void RefreshSlotsAndAdenas()
    {
        _adenaCount = 0;
        _usedSlots = 0;

        if (_playerItems.Count > 0)
        {
            _usedSlots = _playerItems.Where(o => o.Location == ItemLocation.Inventory).Count();

            ItemInstance adenaItem = _playerItems.FirstOrDefault(o => o.Type2 == ItemType2.TYPE2_MONEY);

            if (adenaItem != null)
            {
                _adenaCount = adenaItem.Count;
            }
        }

        // Slot count
        _slotCount = PlayerInventory.Instance.InventorySize;
        //Adena
        _adenaCountLabel.text = $"{_adenaCount:n0}";
    }

    public void RefreshWeight()
    {
        if (PlayerEntity.Instance == null)
        {
            return;
        }

        int weight = ((PlayerStats)PlayerEntity.Instance.Stats).CurrWeight;
        int maxWeight = ((PlayerStats)PlayerEntity.Instance.Stats).MaxWeight;

        if (_weightBarBg != null && _weightBar != null)
        {
            float bgWidth = 132; //TODO fix resolvedStyle width = 0
            float weightRatio = Math.Min(1, (float)weight / maxWeight);

            for (int i = 1; i <= 5; i++)
            {
                _weightBarContainer.RemoveFromClassList("weight-" + i);
            }

            int weightLevel = (int)Mathf.Floor(weightRatio / 0.25f) + 1;
            _weightBarContainer.AddToClassList("weight-" + weightLevel);

            float barWidth = bgWidth * weightRatio;
            _weightBar.style.width = barWidth;
        }

        if (weight > 0)
        {
            _weightLabel.text = $"{((float)weight / maxWeight * 100f).ToString("0.00")}%";
        }
        else
        {
            _weightLabel.text = $"00.00%";
        }
    }

    public override void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            GameClient.Instance.ClientPacketHandler.SendRequestOpenInventory();
        }
        else
        {
            HideWindow(false);
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("inventory_open_01");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
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

    }
#endif
}
