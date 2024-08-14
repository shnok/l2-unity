using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GearSlot : InventorySlot
{
    public GearSlot(int position, VisualElement slotElement, InventoryGearTab tab) : base(position, slotElement, tab) {
    }
    
    protected override void HandleRightClick() {
        GameClient.Instance.ClientPacketHandler.UnEquipItem(_position);
    }
}
