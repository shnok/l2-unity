using System;
using UnityEngine.UIElements;

public class ProductSlot : InventorySlot
{
    public Product Product { get; private set; }
    public ProductSlot(int position, VisualElement slotElement, L2SlotContainer tab, SlotType slotType) : base(position, slotElement, tab, slotType)
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

    protected override void AddTooltip(ItemInstance item)
    {
        base.AddTooltip(item);
    }

    public void AssignProduct(Product product)
    {
        Product = product;
    }

    protected override void HandleLeftDoubleClick()
    {
        SwapBasket();
    }

    public virtual void SwapBasket()
    {
        ((ShopSlotContainer)_currentSlotContainer).AdjacentContainer.AddToBasket(Product, 1);
    }
}
