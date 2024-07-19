using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryTab : L2Tab {
    private L2Slot[] _inventorySlots;

    private VisualElement _contentContainer;

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);

        _contentContainer = tabContainer.Q<VisualElement>("Content");
        
        for(int i = 0; i < 40; i++) {
            VisualElement slot = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            _contentContainer.Add(slot);
        }
    }

    protected override void OnGeometryChanged() {
    }

    protected override void OnSwitchTab() {
    }

    protected override void RegisterAutoScrollEvent() {
    }
}
