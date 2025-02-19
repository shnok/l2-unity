using System;
using System.Collections.Generic;
using System.Linq;
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


    [SerializeField] private ShopTabType _shopTabType;
    [SerializeField] private List<ShopTabSmall> _tabs;

    public bool MainTab { get; internal set; }
    public ShopTabType TabType { get => _shopTabType; set { _shopTabType = value; } }

    public void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader, ShopTabType shopTabType)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader, true);

        _containerLeft = tabContainer.Q<VisualElement>("ContainerInnerLeft");
        _containerRight = tabContainer.Q<VisualElement>("ContainerInnerRight");
        _shopTabType = shopTabType;

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

        _tabs[0].Initialize(chatWindowEle, _containerLeft, null);
        _tabs[1].Initialize(chatWindowEle, _containerRight, null);
    }

    public void UpdateItemList()
    {
        if (_shopTabType == ShopTabType.SELL)
        {
            _tabs[0].UpdateItemList(PlayerInventory.Instance.Items);
            _tabs[1].UpdateItemList(null);
        }
        else
        {
            _tabs[0].UpdateItemList(null);
            _tabs[1].UpdateItemList(null);
        }

        RefreshAdenas();
        RefreshWeight();

    }

    public override void SelectSlot(int slotPosition)
    {

    }

    private void RefreshAdenas()
    {
        if (PlayerInventory.Instance.Items.Count > 0)
        {
            ItemInstance adenaItem = PlayerInventory.Instance.Items.FirstOrDefault(o => o.Type2 == ItemType2.TYPE2_MONEY);

            if (adenaItem != null)
            {
                _adenaCountLabel.text = $"{adenaItem.Count:n0}";
            }
        }
    }

    public void RefreshWeight()
    {
        if (PlayerEntity.Instance == null)
        {
            Debug.LogWarning("Player does not exist, weight refresh canceled.");
            return;
        }

        int weight = ((PlayerStats)PlayerEntity.Instance.Stats).CurrWeight;
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

    protected override void OnSwitchTab()
    {
        if (ShopWindow.Instance.SwitchTab(this))
        {
            AudioManager.Instance.PlayUISound("window_open");
            UpdateItemList();
        }
    }
}
