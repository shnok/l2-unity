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
    [SerializeField] private int _sellListId;
    [SerializeField] private int _buyListId;


    private Label _buyButtonLabel;

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

        VisualElement cancelButton = GetElementById("CancelButton").Q<Button>("L2Button");
        cancelButton.AddManipulator(new ButtonClickSoundManipulator(cancelButton));
        cancelButton.RegisterCallback<MouseUpEvent>((ev) => HideWindow(false), TrickleDown.TrickleDown);
        VisualElement buyButton = GetElementById("BuyButton").Q<Button>("L2Button");
        _buyButtonLabel = buyButton.Q<Label>("ButtonLabel");
        buyButton.AddManipulator(new ButtonClickSoundManipulator(buyButton));
        cancelButton.RegisterCallback<MouseUpEvent>((ev) => ConfirmPressed(), TrickleDown.TrickleDown);
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

        _tabs[0].UpdateProductList(null, 0);
        _tabs[1].UpdateProductList(null, 0);

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void CreateTabs()
    {
        VisualElement shopTabView = GetElementById("ShopTabView");

        _l2TabView = new L2TabView();
        _l2TabView.Initialize(shopTabView, _tabs, _tabTemplate, _tabHeaderTemplate, true);
    }


    public void RefreshProductList(int listId, int adena, Product[] products, ShopTab.ShopTabType type, bool openTab)
    {
        if (type == ShopTab.ShopTabType.SELL)
        {
            _sellListId = listId;
        }
        else
        {
            _buyListId = listId;
        }

        if (openTab && type == ShopTab.ShopTabType.SELL) // In theory should not happen as all the SELL options must be removed from NPCs html
        {
            _l2TabView.HideTab(0);
            _l2TabView.SwitchTab(_tabs[1]);
        }
        else
        {
            _l2TabView.ShowTab(0);
            _l2TabView.SwitchTab(_tabs[0]);
        }

        if (products == null || products.Length == 0)
        {
            Debug.Log("Shop product list is empty.");
            return;
        }

        _tabs[type == ShopTab.ShopTabType.BUY ? 0 : 1].UpdateProductList(products, adena);
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

    public void TabSwitched(ShopTab.ShopTabType type)
    {
        if (type == ShopTab.ShopTabType.BUY)
        {
            _buyButtonLabel.text = "Buy";
        }
        else
        {
            _buyButtonLabel.text = "Sell";
        }
    }

    private void ConfirmPressed()
    {

    }
}
