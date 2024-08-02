using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryGearTab : L2Tab
{
    private Dictionary<ItemSlot, GearSlot> _gearSlots;
    private Dictionary<ItemSlot, VisualElement> _gearAnchors;
    [SerializeField] private int _selectedSlot = -1;

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);

        _selectedSlot = -1;

        if (_gearAnchors != null) {
            _gearAnchors.Clear();
        }

        _gearAnchors = new Dictionary<ItemSlot, VisualElement>();

        _gearAnchors.Add(ItemSlot.head, _windowEle.Q<VisualElement>("Helmet"));

        _gearAnchors.Add(ItemSlot.gloves, _windowEle.Q<VisualElement>("Gloves"));
        _gearAnchors.Add(ItemSlot.chest, _windowEle.Q<VisualElement>("Torso"));
        _gearAnchors.Add(ItemSlot.feet, _windowEle.Q<VisualElement>("Boots"));

        _gearAnchors.Add(ItemSlot.legs, _windowEle.Q<VisualElement>("Legs"));

        _gearAnchors.Add(ItemSlot.rhand, _windowEle.Q<VisualElement>("Rhand"));
        _gearAnchors.Add(ItemSlot.lhand, _windowEle.Q<VisualElement>("Lhand"));

        _gearAnchors.Add(ItemSlot.neck, _windowEle.Q<VisualElement>("Neck"));
        _gearAnchors.Add(ItemSlot.rear, _windowEle.Q<VisualElement>("Rear"));
        _gearAnchors.Add(ItemSlot.lear, _windowEle.Q<VisualElement>("Lear"));

        _gearAnchors.Add(ItemSlot.rfinger, _windowEle.Q<VisualElement>("Rring"));
        _gearAnchors.Add(ItemSlot.lfinger, _windowEle.Q<VisualElement>("Lring"));
    }

    public void UpdateItemList(List<ItemInstance> items) {
        _gearSlots = new Dictionary<ItemSlot, GearSlot>();
        foreach (KeyValuePair<ItemSlot, VisualElement> kvp in _gearAnchors) {
            if (kvp.Value == null) {
                Debug.LogWarning($"Inventory gear slot {kvp.Key} is null.");
                continue;
            }

            // Clear gear slots
            kvp.Value.Clear();

            // Create gear slots
            VisualElement slotElement = InventoryWindow.Instance.InventorySlotTemplate.Instantiate()[0];
            kvp.Value.Add(slotElement);

            GearSlot slot = new GearSlot((int)kvp.Key, kvp.Value, this);
            _gearSlots.Add(kvp.Key, slot);
        }

        items.ForEach(item => {
            if (item.Equipped) {
                _gearSlots[(ItemSlot)item.BodyPart].AssignItem(item);
            }
        });
    }

    public override void SelectSlot(int slotPosition) {
        if (_selectedSlot != slotPosition) {
            if (_selectedSlot != -1) {
                _gearSlots[(ItemSlot) _selectedSlot].UnSelect();
            }
            _gearSlots[(ItemSlot) slotPosition].SetSelected();
            _selectedSlot = slotPosition;
        }
    }
}
