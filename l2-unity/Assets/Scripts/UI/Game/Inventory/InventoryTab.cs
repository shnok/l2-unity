using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryTab : L2Tab {
    private L2Slot[] _inventorySlots;

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);
    }

    protected override void OnGeometryChanged() {
    }

    protected override void OnSwitchTab() {
    }

    protected override void RegisterAutoScrollEvent() {
    }
}
