using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ShopTab : L2Tab
{
    public enum ShopTabType
    {
        BUY, SELL
    }

    private VisualElement _containerLeft;
    private VisualElement _containerRight;
    private Label _weightLabel;
    private Label _adenaCountLabel;
    private Label _priceLabel;
    private VisualElement _weightBarContainer;
    private VisualElement _weightBar;
    private VisualElement _weightBarBg;
    private int _rowLength = 6;
    // private int _minimumRows = 7;
    private int _listId = 0;

    [SerializeField] private ShopTabType _shopTabType;
    [SerializeField] private List<ShopSlotContainer> _slotContainers;

    public bool MainTab { get; internal set; }
    public ShopTabType TabType { get => _shopTabType; set { _shopTabType = value; } }

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(tabView, tabContainer, tabHeader);

        _containerLeft = tabContainer.Q<VisualElement>("ContainerInnerLeft");
        _containerRight = tabContainer.Q<VisualElement>("ContainerInnerRight");

        if (_shopTabType == ShopTabType.BUY)
        {
            _containerLeft.Q<Label>("TabLabel").text = "Shop List";
            _containerRight.Q<Label>("TabLabel").text = "Purchase List";
        }
        else
        {
            _containerLeft.Q<Label>("TabLabel").text = "Inventory";
            _containerRight.Q<Label>("TabLabel").text = "Items on Sale";
        }

        _adenaCountLabel = tabContainer.Q<Label>("AdenaCount");
        _priceLabel = tabContainer.Q<Label>("Price");
        _weightBarContainer = tabContainer.Q<VisualElement>("WeightBar");
        _weightLabel = _weightBarContainer.Q<Label>("Text");
        _weightBar = _weightBarContainer.Q<VisualElement>("Bar");
        _weightBarBg = _weightBarContainer.Q<VisualElement>("BarBg");

        _slotContainers = new List<ShopSlotContainer>
        {
            new ShopSlotContainer(),
            new ShopSlotContainer()
        };

        _slotContainers[0].Initialize(_containerLeft, _rowLength, _shopTabType, L2Slot.SlotType.Product, 42, _slotContainers[1], () =>
        {

        });

        _slotContainers[1].Initialize(_containerRight, _rowLength, _shopTabType, L2Slot.SlotType.Basket, 42, _slotContainers[0], () =>
        {
            if (_shopTabType == ShopTabType.BUY)
            {
                // Add weight
                RefreshWeight(_slotContainers[1].ContentWeight);
            }
            else
            {
                // Remove weight
                RefreshWeight(-_slotContainers[1].ContentWeight);
            }

            RefreshPrice(_slotContainers[1].ContentPrice);
        });
    }

    public void UpdateProductList(Product[] products, int adenas, int listId)
    {
        _listId = listId;

        _slotContainers[0].RefreshProducts(products);
        _slotContainers[1].RefreshProducts(null);

        RefreshAdenas(adenas);
        RefreshWeight(0);
    }

    private void RefreshAdenas(int adenas)
    {
        _adenaCountLabel.text = $"{adenas:n0}";
    }

    private void RefreshPrice(int price)
    {
        _priceLabel.text = $"{price:n0}";
    }

    public void RefreshWeight(int difference)
    {
        if (PlayerEntity.Instance == null)
        {
            return;
        }

        int weight = ((PlayerStats)PlayerEntity.Instance.Stats).CurrWeight + difference;
        int maxWeight = ((PlayerStats)PlayerEntity.Instance.Stats).MaxWeight;

        if (_weightBarBg != null && _weightBar != null)
        {
            float bgWidth = 128; //TODO fix resolvedStyle width = 0
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

    protected override void OnTabHeaderClicked()
    {
        base.OnTabHeaderClicked();

        ShopWindow.Instance.TabSwitched(_shopTabType);
    }

    public void Submit()
    {
        if (_shopTabType == ShopTabType.BUY)
        {
            GameClient.Instance.ClientPacketHandler.SendRequestBuyItem(_listId, _slotContainers[1].Products);
        }
        else
        {
            GameClient.Instance.ClientPacketHandler.SendRequestSellItem(_listId, _slotContainers[1].Products);
        }
    }
}
