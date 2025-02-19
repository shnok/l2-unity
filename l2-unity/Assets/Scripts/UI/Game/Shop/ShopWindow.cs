using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopWindow : L2PopupWindow
{
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _smallTabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _shopSlotTemplate;
    private VisualElement _shopTabView;
    private ShopTab _activeTab;

    [SerializeField] private List<ShopTab> _tabs;

    [SerializeField] private int _usedSlots;
    [SerializeField] private int _slotCount;
    [SerializeField] private int _adenaCount;

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ShopWindow/ShopWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/ShopWindow/ShopTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/ShopWindow/ShopTabHeader");
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

        UpdateItemList();

#if UNITY_EDITOR
        // DebugData();
#endif

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void CreateTabs()
    {
        _shopTabView = GetElementById("ShopTabView");

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

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement, _tabs[i].TabType);
        }

        _activeTab = null;

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
            _activeTab?.TabContainer?.AddToClassList("unselected-tab");
            _activeTab?.TabHeader?.RemoveFromClassList("active");

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            //ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;

            return true;
        }

        return false;
    }

    public void UpdateItemList()
    {
        // Tabs
        _tabs.ForEach((tab) =>
        {
            tab.UpdateItemList();
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
            HideWindow(false);
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("window_close");

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
