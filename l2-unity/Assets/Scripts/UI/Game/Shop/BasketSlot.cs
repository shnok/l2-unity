using UnityEngine.UIElements;

public class BasketSlot : ProductSlot
{
    public BasketSlot(int position, VisualElement slotElement, L2SlotContainer tab, SlotType slotType) : base(position, slotElement, tab, slotType)
    {
    }

    protected override void HandleLeftClick()
    {
        if (_currentSlotContainer != null)
        {
            _currentSlotContainer.SelectSlot(_position);
        }
    }

    protected override void HandleRightClick()
    {
    }

    protected override void HandleMiddleClick()
    {
    }

    public override void SwapBasket()
    {
        //Discard amount systemMessageId => 71
        SMParam[] smParams = new SMParam[1];
        smParams[0] = new SMParam(SMParam.SMParamType.TYPE_ITEM_NAME, Product.ItemId);
        SystemMessage systemMessage = new SystemMessage(smParams, SystemMessageTable.Instance.SystemMessages[72]);

        if (Product.Count == 1)
        {
            ((ShopSlotContainer)_currentSlotContainer).AddToBasket(Product, -1);
        }
        else
        {
            L2InputAmountWindow.Instance.ShowWindow(systemMessage, Product.Count, (amount) =>
            {
                ((ShopSlotContainer)_currentSlotContainer).AddToBasket(Product, -amount);
            }, () => { });
        }

    }
}
