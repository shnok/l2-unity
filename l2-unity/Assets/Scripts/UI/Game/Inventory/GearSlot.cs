using UnityEngine.UIElements;

public class GearSlot : InventorySlot
{
    public GearSlot(int position, VisualElement slotElement, InventoryGearTab tab, SlotType slotType) : base(position, slotElement, tab, slotType)
    {
    }

    protected override void HandleRightClick()
    {
        UseItem();
    }
}
