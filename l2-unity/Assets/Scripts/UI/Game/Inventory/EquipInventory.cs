using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class EquipInventory
{
    
    private Dictionary<int, VisualElement> _equipListVE;

    private ButtonEquipInventory _buttonEquip;
    private VisualElement _selectCell;
    private VisualElement _lastSelectCell;
    private Dictionary<int, ModelItemDemo> _equipListMD;
    private InventoryWindow _inventory;

    public EquipInventory(InventoryWindow inventory) {
       // _equipListData = new Dictionary<int, ModelItemDemo>();
        _equipListVE = new Dictionary<int, VisualElement>();
        _equipListMD = new Dictionary<int, ModelItemDemo>();
        _buttonEquip = new ButtonEquipInventory(this);
        _inventory = inventory;
    }

    public void registerButtonEquip(VisualElement boxContent)
    {
        _equipListVE = CreateEquipButton(boxContent);
        RegisterAllButtons(_buttonEquip, _equipListVE);
    }

    private Dictionary<int, VisualElement> CreateEquipButton(VisualElement boxContent)
    {
        var slot_weapon = boxContent.Q<VisualElement>(className: "slot_weapon");
        var slot_head = boxContent.Q<VisualElement>(className: "slot_head");
        var shield_armor = boxContent.Q<VisualElement>(className: "shield_armor");
        var slot_chest = boxContent.Q<VisualElement>(className: "slot_chest");
        var slot_legs = boxContent.Q<VisualElement>(className: "slot_legs");
        var slot_gloves = boxContent.Q<VisualElement>(className: "slot_gloves");

        if (slot_weapon != null)
        {
            _equipListVE.Add(0, slot_weapon);
        }

        if (slot_head != null)
        {
            _equipListVE.Add(0x0040, slot_head);
        }

        if (shield_armor != null)
        {
            _equipListVE.Add(1, shield_armor);
        }

        if (slot_chest != null)
        {
            _equipListVE.Add(0x0400, slot_chest);
        }

        if (slot_legs != null)
        {
            _equipListVE.Add(0x0800, slot_legs);
        }

        if (slot_gloves != null)
        {
            _equipListVE.Add(0x0200, slot_gloves);
        }



       // _equipListVE.Add(0, slot_weapon);
         //   _equipListVE.Add(0x0040, slot_head);
         //   _equipListVE.Add(1, shield_armor);
         //   _equipListVE.Add(0x0400, slot_chest);
         //   _equipListVE.Add(0x0800, slot_legs);
         //   _equipListVE.Add(0x0200, slot_gloves);
        


        return _equipListVE;
    }

    private void RegisterAllButtons(ButtonEquipInventory _buttonEquip, Dictionary<int, VisualElement> _equipListVE)
    {
        foreach (KeyValuePair<int, VisualElement> entry in _equipListVE)
        {
            _buttonEquip.RegisterClickEquipCell(entry.Value);
        }
    }
    public void AddEquipList(int slot, ModelItemDemo item)
    {
        //_equipListData.Add(slot, item);
    }

    public void SelectRows(VisualElement selectCell)
    {
        if (selectCell != null)
        {
            VisualElement grow = selectCell.parent;
            UpdateBackGroundSelectElement(grow);
            if (_lastSelectCell == null)
            {
                UpdateLastPosition(selectCell);
            }
            else
            {

                if (selectCell.name.Equals(_lastSelectCell.name))
                {
                    //UpdateBackGroundLastElement(last_row, last_grow, _blackFrame);
                    Debug.Log("Ignore rows");
                }
                else
                {
                    UpdateBackGroundReset(_lastSelectCell);
                }

                UpdateLastPosition(selectCell);
            }
        }
    }

    public void Equip(ModelItemDemo equipModel, int typeEquip)
    {
        AddListMd(equipModel, typeEquip);
        AddImageToVe(_equipListVE[typeEquip], equipModel);
    }

    public void UnEquip(VisualElement ve)
    {
        
        int typeId = GetModelDemoByNameCell(ve.name);
        if (_equipListMD.ContainsKey(typeId))
        {
            ResetCellImage(ve);
            ModelItemDemo item = _equipListMD[typeId];
            _inventory.UnEquip(item);
        }
        
    }

    private void ResetCellImage(VisualElement ve)
    {
        if(ve != null)
        {
            ve.style.backgroundImage = StyleKeyword.None;
            ve.parent.style.backgroundImage = StyleKeyword.None;
        }

    }

    private int GetModelDemoByNameCell(string name)
    {
        foreach (KeyValuePair<int, VisualElement> entry in _equipListVE)
        {
            if(entry.Value.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return entry.Key;
            }
        }
        return -1;
    }

    private void AddListMd(ModelItemDemo equipModel, int typeEquip)
    {
        if (!_equipListMD.ContainsKey(typeEquip))
        {
            _equipListMD.Add(typeEquip, equipModel);
        }
        else
        {
            _equipListMD[typeEquip] = equipModel;
        }
    }

    private void AddImageToVe(VisualElement equip_img, ModelItemDemo equipModel)
    {
        Texture2D blackFrame = IconManager.Instance.LoadBlackBorderCell();
        equip_img.parent.style.backgroundImage = new StyleBackground(blackFrame);
        equip_img.style.backgroundImage = equipModel.Icon();
    }

    private void UpdateBackGroundSelectElement(VisualElement grow)
    {
        Texture2D blueFrame = IconManager.Instance.LoadColorBorderCell();
        SetBackground(blueFrame, grow);
    }

    private void SetBackground(Texture2D imgSource1, VisualElement element)
    {
        if (imgSource1 != null)
        {
            element.style.backgroundImage = new StyleBackground(imgSource1);
        }
    }

    public void EquipItemNoInventory(ModelItemDemo demo, int typeId, VisualElement boxContent)
    {
        
        if (_equipListVE.ContainsKey(typeId))
        {
            AddListMd(demo, typeId);
            var equip_img = _equipListVE[typeId];
            equip_img.parent.style.backgroundImage = new StyleBackground(IconManager.Instance.LoadBlackBorderCell());
            equip_img.style.backgroundImage = demo.Icon();
        }
    
    }


    private void UpdateBackGroundReset(VisualElement lastElementCell)
    {
        lastElementCell.parent.style.backgroundImage = null;
    }

    private void UpdateLastPosition(VisualElement _selectCell)
    {
        _lastSelectCell = _selectCell;
    }



}
