using UnityEngine.UIElements;

public class ShopSlot : InventorySlot
{
    public ShopSlot(int position, VisualElement slotElement, L2SlotContainer tab, SlotType slotType) : base(position, slotElement, tab, slotType)
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
}
