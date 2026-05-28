using UnityEngine.UIElements;

public class GearSlot : InventorySlot
{
    public GearSlot(int position, VisualElement slotElement, L2SlotContainer tab, SlotType slotType) : base(position, slotElement, tab, slotType)
    {
    }

    protected override void HandleRightClick()
    {
        UseItem();
    }

    protected override void HandleLeftClick()
    {
        if (!Empty)
            InventoryWindow.Instance.GearTab.SelectSlot(_position);
    }
}
