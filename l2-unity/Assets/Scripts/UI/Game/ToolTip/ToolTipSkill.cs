using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipSkill : L2PopupWindow, IToolTips
{

    private ShowToolTip _showToolTip;

    private static ToolTipSkill _instance;
    private VisualElement _content;

    private UnityEngine.UIElements.Label _hpLabel;
    private UnityEngine.UIElements.Label _mpLabel;
    private UnityEngine.UIElements.Label _rangeLabel;
    private UnityEngine.UIElements.Label _descriptedLabel;
    private UnityEngine.UIElements.Label _castlabel;
    private UnityEngine.UIElements.Label _typeSkill;
    private UnityEngine.UIElements.Label _nameSkill;
    private UnityEngine.UIElements.Label _lvlSkill;


    //Block
    private VisualElement _hpBlockText;
    private VisualElement _mpBlockText;
    private VisualElement _radiusBlockText;
    private VisualElement _castBlockText;
    private VisualElement _descriptedText;

    private VisualElement _icon;
    private float _heightContent = 0;
    private float _widtchContent = 0;

    public static ToolTipSkill Instance { get { return _instance; } }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _showToolTip = new ShowToolTip(this);
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSkill");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("content");

        //only elements
        _hpLabel = (UnityEngine.UIElements.Label)GetElementByClass("HpLabel");
        _mpLabel = (UnityEngine.UIElements.Label)GetElementByClass("MpLabel");
        _rangeLabel = (UnityEngine.UIElements.Label)GetElementByClass("RangeLabel");
        _descriptedLabel = (UnityEngine.UIElements.Label)GetElementByClass("DescriptedLabel");
        _castlabel = (UnityEngine.UIElements.Label)GetElementByClass("Castlabel");
        _typeSkill = (UnityEngine.UIElements.Label)GetElementByClass("TypeSkill");
        _nameSkill = (UnityEngine.UIElements.Label)GetElementByClass("NameSkill");
        _lvlSkill = (UnityEngine.UIElements.Label)GetElementByClass("LvlSkill");

        //all GroupBox
        _hpBlockText = GetElementByClass("HpText");
        _mpBlockText = GetElementByClass("MpText");
        _radiusBlockText = GetElementByClass("RadiusText");
        _castBlockText = GetElementByClass("TimeText");
        _descriptedText = GetElementByClass("DescriptedText");


        _icon = GetElementByClass("Icon");


        _heightContent = _windowEle.worldBound.height;
        _widtchContent = _windowEle.worldBound.width;
        _windowEle.style.display = DisplayStyle.None;
    }

    public void RegisterCallbackActive(Dictionary<int, VisualElement> dict, SkillLearn skillWindow)
    {

        foreach (var item in dict)
        {
            VisualElement element = item.Value;
            if (element != null)
            {
                element.RegisterCallback<MouseOverEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        if (DragAndDropManager.getInstance().IsDrag())
                        {
                            ResetElement(ve);
                        }
                        else
                        {
                            int cellId = ParceCellId(ve.name);
                            Skillgrp skillGrp = skillWindow.GetSkillIdByCellId(1 , cellId);

                            if (skillGrp != null)
                            {
                                SkillNameData nameData = SkillgrpTable.Instance.GetSkillName(skillGrp.Id, skillGrp.Level);
                                if(nameData != null)
                                {
                                    SetData(skillGrp, nameData);
                                    CalcNewPosition(ve);
                                }
                                
                            }
                            
                            
                        }
                    }
                }, TrickleDown.TrickleDown);

                element.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        ResetElement(ve);
                    }
                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        }
    }


    public void RegisterCallbackPassive(Dictionary<int, VisualElement> dict, SkillLearn skillWindow)
    {

        foreach (var item in dict)
        {
            VisualElement element = item.Value;
            if (element != null)
            {
                element.RegisterCallback<MouseOverEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        if (DragAndDropManager.getInstance().IsDrag())
                        {
                            ResetElement(ve);
                        }
                        else
                        {
                            int cellId = ParcePassiveCellId(ve.name);
                            Skillgrp skillGrp = skillWindow.GetSkillIdByCellId(2, cellId);

                            if (skillGrp != null)
                            {
                                SkillNameData nameData = SkillgrpTable.Instance.GetSkillName(skillGrp.Id, skillGrp.Level);
                                if (nameData != null)
                                {
                                    SetData(skillGrp, nameData);
                                    CalcNewPosition(ve);
                                }

                            }


                        }
                    }
                }, TrickleDown.TrickleDown);

                element.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        ResetElement(ve);
                    }
                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        }
    }

    private int ParceCellId(string name)
    {
        string strId = name.Replace("imgbox", "");
        return Int32.Parse(strId);
    }

    private int ParcePassiveCellId(string name)
    {
        string strId = name.Replace("pasbox", "");
        return Int32.Parse(strId);
    }
    
    private void CalcNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.Flex;
        
        _showToolTip.Show(ve);
    }

    private void ResetElement(VisualElement ve)
    {
        _heightContent = _windowEle.worldBound.height;
        _showToolTip.Hide(ve);
        _windowEle.style.display = DisplayStyle.None;
    }

    public void NewPosition(Vector2 newPoint , float sdfig)
    {

        var testPoint = checkBound(newPoint, _heightContent);
        if (!SkillLearn.Instance.IsWindowContain(testPoint))
        {
            float width = _heightContent;
            float newddfig = width;
            //2px border 
            float sdfig1 = sdfig + 2;
            float new_y = newPoint.y - newddfig;
            float new_y2 = new_y - sdfig1;
            Vector2 reversePoint = new Vector2(newPoint.x, new_y2);
            _content.transform.position = reversePoint;
        }
        else
        {
            //2px border
            float new_y = newPoint.y + 2;
            Vector2 reversePoint = new Vector2(newPoint.x, new_y);
            _content.transform.position = reversePoint;
        }
        BringToFront();
    }

    private Vector2 checkBound(Vector2 newPoint , float element)
    {
        return  new Vector2(newPoint.x, newPoint.y + element);
    }

    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }

    private void SetData(Skillgrp skillgrp, SkillNameData skillNameData)
    {
        string hp = skillgrp.HpConsume.ToString();
        string mp = skillgrp.MpConsume.ToString();
        string radius = skillgrp._cast_range.ToString();
        string desc = skillNameData.Desc;
        string time = skillgrp.ReuseDelay.ToString();
        string name = skillNameData.Name;
        string level = skillgrp.Level.ToString();
        string nameActiveSkill = GetActive(skillgrp.OperateType);

        if (desc.Equals("")) desc = SkillNameTable.Instance.GetName(skillgrp.Id, 1).Desc;


        SetDataTooTip(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill , name, level);
        _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(skillgrp.Icon);
        HideEmptyData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
        ShowHidenData(DataEmpty(hp), DataEmpty(mp), DataEmpty(radius), desc, DataEmpty(time), nameActiveSkill, name, level);
    }

    private string GetActive(int operate_type)
    {
        if(operate_type == 1)
        {
            return "Active Skill";
        }
        else if (operate_type == 2)
        {
            return "Passive Skill";
        }
        return "";
    }

    private string DataEmpty(string data)
    {
        if(data.Equals("0") | data.Equals("-1"))
        {
            return "";
        }
        return data;
    }
    private void SetDataTooTip(string hp , string mp , string radius , string descripted , string cast , string type , string name , string lvl)
    {
        if (!string.IsNullOrEmpty(hp))
        {
            if (!hp.Equals("0"))
            {
                _hpLabel.text = hp;
            }
            
        }

        if (!string.IsNullOrEmpty(mp))
        {
            if (!mp.Equals("0"))
            {
                _mpLabel.text = mp;
            }
                
        }

        if (!string.IsNullOrEmpty(radius))
        {
            if (!mp.Equals("-1"))
            {
                _rangeLabel.text = radius;
            }
               
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedLabel.text = descripted;
        }

        if (!string.IsNullOrEmpty(cast))
        {
            _castlabel.text = cast + " sec";
        }

        if (!string.IsNullOrEmpty(type))
        {
            _typeSkill.text = type;
        }

        if (!string.IsNullOrEmpty(name))
        {
            _nameSkill.text = name;
        }

        if (!string.IsNullOrEmpty(lvl))
        {
            _lvlSkill.text = lvl;
        }

    }

    private void HideEmptyData(string hp, string mp, string radius, string descripted, string cast, string type, string name, string lvl)
    {
        if (string.IsNullOrEmpty(hp))
        {
            _hpBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(mp))
        {
            _mpBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(radius))
        {
            _radiusBlockText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(descripted))
        {
            _descriptedText.style.display = DisplayStyle.None;
        }

        if (string.IsNullOrEmpty(cast))
        {
            _castBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(type))
        {
           // _typeBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(name))
        {
           // _hpBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(lvl))
        {
           // _hpBlockText.style.display = DisplayStyle.None;
        }
    }

    private void ShowHidenData(string hp, string mp, string radius, string descripted, string cast, string type, string name, string lvl)
    {
        if (!string.IsNullOrEmpty(hp))
        {
            _hpBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(mp))
        {
            _mpBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(radius))
        {
            _radiusBlockText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedText.style.display = DisplayStyle.Flex;
        }

        if (!string.IsNullOrEmpty(cast))
        {
            _castBlockText.style.display = DisplayStyle.Flex;
        }
        //always given
        if (string.IsNullOrEmpty(type))
        {
            // _typeBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(name))
        {
            // _hpBlockText.style.display = DisplayStyle.None;
        }
        //always given
        if (string.IsNullOrEmpty(lvl))
        {
            // _hpBlockText.style.display = DisplayStyle.None;
        }
    }

}
