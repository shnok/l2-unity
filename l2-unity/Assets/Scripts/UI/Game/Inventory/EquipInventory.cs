using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class EquipInventory
{
    private Dictionary<int, ModelItemDemo> _equipListData;
    private Dictionary<int, VisualElement> _equipListVE;
    private string[] fillBackground = { "Data/UI/ShortCut/demo_skills/fill_black", "Data/UI/Window/Inventory/bg_windows/blue_tab_v5" };
    private ButtonEquipInventory _buttonEquip;
    public EquipInventory() {
        _equipListData = new Dictionary<int, ModelItemDemo>();
        _equipListVE = new Dictionary<int, VisualElement>();
        _buttonEquip = new ButtonEquipInventory();
    }

    public void registerButtonEquip(VisualElement boxContent)
    {
        _equipListVE = CreateEquipButton(boxContent);
        RegisterAllButtons(_buttonEquip, _equipListVE);
    }
    
    private Dictionary<int, VisualElement> CreateEquipButton(VisualElement boxContent)
    {
        var slot_weapon = boxContent.Q<GroupBox>(className: "slot_weapon");
        var slot_head = boxContent.Q<VisualElement>(className: "slot_head");
        var shield_armor = boxContent.Q<VisualElement>(className: "shield_armor");
        var slot_chest = boxContent.Q<VisualElement>(className: "slot_chest");
        var slot_legs = boxContent.Q<VisualElement>(className: "slot_legs");
        var slot_gloves = boxContent.Q<VisualElement>(className: "slot_gloves");

        if(slot_weapon != null & slot_head != null & shield_armor != null & slot_chest != null & slot_legs != null & slot_gloves != null)
        {
            _equipListVE.Add(0, slot_weapon);

            _equipListVE.Add(0x0040, slot_head);
            _equipListVE.Add(1, shield_armor);
            _equipListVE.Add(0x0400, slot_chest);
            _equipListVE.Add(0x0800, slot_legs);
            _equipListVE.Add(0x0200, slot_gloves);
        }


        return _equipListVE;
    }

    private void RegisterAllButtons(ButtonEquipInventory _buttonEquip , Dictionary<int, VisualElement> _equipListVE)
    {
        foreach (KeyValuePair<int, VisualElement> entry in _equipListVE)
        {
            _buttonEquip.RegisterClickEquipCell(entry.Value);
        }
    }
    public void AddEquipList(int slot , ModelItemDemo item)
    {
        _equipListData.Add(slot , item);
    }

    public void EquipItemNoInventory(ModelItemDemo demo, string typeName , VisualElement boxContent)
    {
        VisualElement equip_img = boxContent.Q<VisualElement>(className: typeName);
        Texture2D blackFrame = Resources.Load<Texture2D>(fillBackground[0]);
        equip_img.parent.style.backgroundImage = new StyleBackground(blackFrame);
        equip_img.style.backgroundImage = demo.Icon();
    }

}
