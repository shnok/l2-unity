using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventoryGearTab : L2Tab
{
    private Dictionary<Paperdoll, GearSlot> _gearSlots;
    private Dictionary<Paperdoll, VisualElement> _gearAnchors;
    [SerializeField] private int _selectedSlot = -1;

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(null, tabContainer, tabHeader);

        _selectedSlot = -1;

        _gearAnchors?.Clear();

        _gearAnchors = new Dictionary<Paperdoll, VisualElement>
        {
            { Paperdoll.HEAD, _tabContainer.Q<VisualElement>("Helmet") },
            { Paperdoll.GLOVES, _tabContainer.Q<VisualElement>("Gloves") },
            { Paperdoll.CHEST, _tabContainer.Q<VisualElement>("Torso") },
            { Paperdoll.FEET, _tabContainer.Q<VisualElement>("Boots") },
            { Paperdoll.LEGS, _tabContainer.Q<VisualElement>("Legs") },
            { Paperdoll.RHAND, _tabContainer.Q<VisualElement>("Rhand") },
            { Paperdoll.LHAND, _tabContainer.Q<VisualElement>("Lhand") },
            { Paperdoll.NECK, _tabContainer.Q<VisualElement>("Neck") },
            { Paperdoll.REAR, _tabContainer.Q<VisualElement>("Rear") },
            { Paperdoll.LEAR, _tabContainer.Q<VisualElement>("Lear") },
            { Paperdoll.RFINGER, _tabContainer.Q<VisualElement>("Rring") },
            { Paperdoll.LFINGER, _tabContainer.Q<VisualElement>("Lring") }
        };
    }

    public void UpdateItemList(List<ItemInstance> items)
    {
        //Debug.Log("Update gear slots");

        // Clean up slot callbacks and manipulators
        if (_gearSlots != null)
        {
            foreach (KeyValuePair<Paperdoll, GearSlot> kvp in _gearSlots)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.UnregisterClickableCallback();
                    kvp.Value.ClearManipulators();
                }
            }
            _gearSlots.Clear();
        }

        _gearSlots = new Dictionary<Paperdoll, GearSlot>();
        // Clean up gear anchors from any child visual element
        foreach (KeyValuePair<Paperdoll, VisualElement> kvp in _gearAnchors)
        {
            if (kvp.Value == null)
            {
                Debug.LogWarning($"Inventory gear slot {kvp.Key} is null.");
                continue;
            }

            // Clear gear slots
            kvp.Value.Clear();

            // Create gear slots
            VisualElement slotElement = L2SlotManager.Instance.InventorySlotTemplate.Instantiate()[0];
            kvp.Value.Add(slotElement);

            GearSlot slot = new GearSlot((int)kvp.Key, slotElement, null, L2Slot.SlotType.Gear);
            _gearSlots.Add(kvp.Key, slot);
        }

        items.ForEach(item =>
        {
            if (item.Equipped)
            {
                if (item.BodyPart == ItemSlot.SLOT_LR_HAND && item.Type1 == ItemType1.TYPE1_ITEM_QUESTITEM_ADENA)
                {
                    _gearSlots[(Paperdoll)item.Slot].AssignItem(item); // Arrows
                }
                else
                if (item.Type2 == ItemType2.TYPE2_WEAPON && (((Weapongrp)item.ItemData.Itemgrp).WeaponType == WeaponType.bigblunt
                || ((Weapongrp)item.ItemData.Itemgrp).WeaponType == WeaponType.bigword
                || ((Weapongrp)item.ItemData.Itemgrp).WeaponType == WeaponType.dual
                || ((Weapongrp)item.ItemData.Itemgrp).WeaponType == WeaponType.pole
                || ((Weapongrp)item.ItemData.Itemgrp).WeaponType == WeaponType.fist))
                {
                    _gearSlots[Paperdoll.RHAND].AssignItem(item);
                    _gearSlots[Paperdoll.LHAND].AssignItem(item);
                }
                else
                if (item.Slot == (int)Paperdoll.CHEST && item.BodyPart == ItemSlot.SLOT_FULL_ARMOR)
                {
                    _gearSlots[Paperdoll.CHEST].AssignItem(item);
                    _gearSlots[Paperdoll.LEGS].AssignItem(item);
                }
                else
                {
                    Paperdoll slot = (Paperdoll)item.Slot;
                    if (slot != Paperdoll.NULL)
                    {
                        _gearSlots[slot].AssignItem(item);
                    }
                    else
                    {
                        Debug.LogError("Can't equip item, assigned slot is " + slot);
                    }
                }
            }
        });

        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }

    public override void SelectSlot(int slotPosition)
    {
        if (_selectedSlot != -1)
        {
            _gearSlots[(Paperdoll)_selectedSlot].UnSelect();
        }
        _gearSlots[(Paperdoll)slotPosition].SetSelected();
        _selectedSlot = slotPosition;
    }
}
