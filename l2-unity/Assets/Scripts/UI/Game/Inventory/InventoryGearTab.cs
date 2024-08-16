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

        _gearAnchors?.Clear();

        _gearAnchors = new Dictionary<ItemSlot, VisualElement>
        {
            { ItemSlot.head, _windowEle.Q<VisualElement>("Helmet") },
            { ItemSlot.gloves, _windowEle.Q<VisualElement>("Gloves") },
            { ItemSlot.chest, _windowEle.Q<VisualElement>("Torso") },
            { ItemSlot.feet, _windowEle.Q<VisualElement>("Boots") },
            { ItemSlot.legs, _windowEle.Q<VisualElement>("Legs") },
            { ItemSlot.rhand, _windowEle.Q<VisualElement>("Rhand") },
            { ItemSlot.lhand, _windowEle.Q<VisualElement>("Lhand") },
            { ItemSlot.neck, _windowEle.Q<VisualElement>("Neck") },
            { ItemSlot.rear, _windowEle.Q<VisualElement>("Rear") },
            { ItemSlot.lear, _windowEle.Q<VisualElement>("Lear") },
            { ItemSlot.rfinger, _windowEle.Q<VisualElement>("Rring") },
            { ItemSlot.lfinger, _windowEle.Q<VisualElement>("Lring") }
        };
    }

    public IEnumerator UpdateItemList(List<ItemInstance> items) {
        Debug.Log("Update gear slots");

        // Clean up slot callbacks and manipulators
        if(_gearSlots != null) {
            foreach (KeyValuePair<ItemSlot, GearSlot> kvp in _gearSlots) {
                if (kvp.Value != null) {
                    kvp.Value.UnregisterCallbacks();
                    kvp.Value.ClearSlot();
                }
            }
            _gearSlots.Clear();
        }
        _gearSlots = new Dictionary<ItemSlot, GearSlot>();

        // Clean up gear anchors from any child visual element
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
                Debug.Log("Equip item: " + item);
                if(item.BodyPart == ItemSlot.lrhand) {
                    _gearSlots[ItemSlot.lhand].AssignItem(item);
                    _gearSlots[ItemSlot.rhand].AssignItem(item);
                } else if(item.BodyPart == ItemSlot.fullarmor) {
                    _gearSlots[ItemSlot.chest].AssignItem(item);
                    _gearSlots[ItemSlot.legs].AssignItem(item);
                } else {
                    ItemSlot slot = (ItemSlot)item.Slot;
                    if(slot != ItemSlot.none) {
                        _gearSlots[(ItemSlot)item.Slot].AssignItem(item);
                    } else {
                        Debug.LogError("Can't equip item, assigned slot is " + slot);
                    }
                }
            }
        });

        yield return new WaitForEndOfFrame();
        
        if(_selectedSlot != -1) {
            SelectSlot(_selectedSlot);
        }
    }

    public override void SelectSlot(int slotPosition) {
        if (_selectedSlot != -1) {
            _gearSlots[(ItemSlot) _selectedSlot].UnSelect();
        }
        _gearSlots[(ItemSlot) slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }
}
