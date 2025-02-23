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
    private L2TabView _l2TabView;

    [SerializeField] private ShopTab[] _tabs;

    [SerializeField] private int _usedSlots;
    [SerializeField] private int _slotCount;
    [SerializeField] private int _adenaCount;

    private VisualElement _weightBarContainer;
    private VisualElement _weightBar;
    private VisualElement _weightBarBg;

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

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void CreateTabs()
    {
        VisualElement shopTabView = GetElementById("ShopTabView");

        _l2TabView = new L2TabView();
        _l2TabView.Initialize(shopTabView, _tabs, _tabTemplate, _tabHeaderTemplate, true);
    }


    public void UpdateItemList()
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].UpdateItemList();
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
}
