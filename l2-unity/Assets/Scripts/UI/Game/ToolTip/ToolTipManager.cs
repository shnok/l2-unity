using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipManager : L2PopupWindow
{

    private ShowToolTip _showToolTip;

    private static ToolTipManager _instance;
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

    public static ToolTipManager Instance { get { return _instance; } }



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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTip");
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

    public void RegisterCallback(List<VisualElement> list)
    {

        list.ForEach(item =>
        {
            if (item != null)
            {
                item.RegisterCallback<MouseOverEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if(ve != null)
                    {
                        _windowEle.style.display = DisplayStyle.Flex;
                        SetTestData(ve.name);
                        _showToolTip.Show(ve);
                       
                    }
                }, TrickleDown.TrickleDown);

                item.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                            _heightContent = _windowEle.worldBound.height;
                            _showToolTip.Hide(ve);
                            _windowEle.style.display = DisplayStyle.None;
                    }
                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        });

    }

    public void NewPosition(Vector2 newPoint , float sdfig)
    {

        var testPoint = checkBound(newPoint, _heightContent);
        if (!SkillLearn.Instance.isWindowContain(testPoint))
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

    private void SetTestData(string name)
    {
        if (name.Equals("imgbox7"))
        {
           // _mpLabel.text = "9";
            //_rangeLabel.text = "40";
            //_descriptedLabel.text = "Delivers a powerful blow that strikes the target with 30 Power added to P. Atk. Requires a sword or blunt weapon. Over-hit is possible.";
            //_castlabel.text = "1.08 sec";
            //_typeSkill.text = "Active Skill";
            //_nameSkill.text = "Power Strike";
            //_lvlSkill.text = "1 ";
            SetDataTooTip("", "9", "40", "Delivers a powerful blow that strikes the target with 30 Power added to P. Atk. Requires a sword or blunt weapon. Over-hit is possible.", "1.08 sec", "Active Skill", "Power Strike", "1");
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0003");
            HideEmptyData("", "9", "40", "Delivers a powerful blow that strikes the target with 30 Power added to P. Atk. Requires a sword or blunt weapon. Over-hit is possible.", "1.08 sec", "Active Skill", "Power Strike", "1");
            ShowHidenData("", "9", "40", "Delivers a powerful blow that strikes the target with 30 Power added to P. Atk. Requires a sword or blunt weapon. Over-hit is possible.", "1.08 sec", "Active Skill", "Power Strike", "1");
        }
        else if (name.Equals("imgbox1"))
        {
            //test data no hp label
            SetDataTooTip("", "19", "40", "Supplements the user's P. Atk. with 36 Power to stun the enemy for 9 seconds. Requires a blunt weapon. Over-hit is possible.", "3 sec", "Active Skill", "Stun Attack", "1");
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0100");
            HideEmptyData("", "19", "40", "Supplements the user's P. Atk. with 36 Power to stun the enemy for 9 seconds. Requires a blunt weapon. Over-hit is possible.", "3 sec", "Active Skill", "Stun Attack", "1");
            ShowHidenData("", "19", "40", "Supplements the user's P. Atk. with 36 Power to stun the enemy for 9 seconds. Requires a blunt weapon. Over-hit is possible.", "3 sec", "Active Skill", "Stun Attack", "1");
        }
        else if (name.Equals("imgbox2"))
        {
            //test data no radius data
            SetDataTooTip("200", "72", "40", "Transforms target into the 100 souls.", "2.5 sec", "Active Skill", "Final Flying Form", "3");
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0840");
            HideEmptyData("200", "72", "40", "Transforms target into the 100 souls.", "2.5 sec", "Active Skill", "Final Flying Form", "3");
            ShowHidenData("200", "72", "40", "Transforms target into the 100 souls.", "2.5 sec", "Active Skill", "Final Flying Form", "3");
        }
        else if (name.Equals("imgbox91"))
        {
            //test data no radius data
            SetDataTooTip("", "", "", "Allows the user to manufacture level 10 items.", "2.5 sec", "Passive Skill", "Create Item", "10");
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0172");
            HideEmptyData("", "", "", "Allows the user to manufacture level 10 items.", "2.5 sec", "Passive Skill", "Create Item", "10");
            ShowHidenData("", "", "", "Allows the user to manufacture level 10 items.", "2.5 sec", "Passive Skill", "Create Item", "10");
        }
    }

    private void SetDataTooTip(string hp , string mp , string radius , string descripted , string cast , string type , string name , string lvl)
    {
        if (!string.IsNullOrEmpty(hp))
        {
            _hpLabel.text = hp;
        }

        if (!string.IsNullOrEmpty(mp))
        {
            _mpLabel.text = mp;
        }

        if (!string.IsNullOrEmpty(radius))
        {
            _rangeLabel.text = radius;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedLabel.text = descripted;
        }

        if (!string.IsNullOrEmpty(cast))
        {
            _castlabel.text = cast;
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
