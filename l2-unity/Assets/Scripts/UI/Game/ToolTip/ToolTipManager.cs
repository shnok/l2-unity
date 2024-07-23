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

    private UnityEngine.UIElements.Label _mpLabel;
    private UnityEngine.UIElements.Label _rangeLabel;
    private UnityEngine.UIElements.Label _descriptedLabel;
    private UnityEngine.UIElements.Label _castlabel;
    private UnityEngine.UIElements.Label _typeSkill;
    private UnityEngine.UIElements.Label _nameSkill;
    private UnityEngine.UIElements.Label _lvlSkill;
    private VisualElement _icon;


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

        _content = GetElementByClass("content");

        _mpLabel = (UnityEngine.UIElements.Label)GetElementByClass("MpLabel");
        _rangeLabel = (UnityEngine.UIElements.Label)GetElementByClass("RangeLabel");
        _descriptedLabel = (UnityEngine.UIElements.Label)GetElementByClass("DescriptedLabel");
        _castlabel = (UnityEngine.UIElements.Label)GetElementByClass("Castlabel");
       _typeSkill = (UnityEngine.UIElements.Label)GetElementByClass("TypeSkill");
        _nameSkill = (UnityEngine.UIElements.Label)GetElementByClass("NameSkill");
        _lvlSkill = (UnityEngine.UIElements.Label)GetElementByClass("LvlSkill");
        _icon = GetElementByClass("Icon");

        yield return new WaitForEndOfFrame();
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
                        SetTestData(ve.name);
                        _showToolTip.Show(ve);
                    }
                   
                }, TrickleDown.TrickleDown);

                item.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        _showToolTip.Hide(ve);
                    }

                }, TrickleDown.TrickleDown);

            }
        });

    }

    public void NewPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        BringToFront();
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
            _mpLabel.text = "9";
            _rangeLabel.text = "40";
            _descriptedLabel.text = "Delivers a powerful blow that strikes the target with 30 Power added to P. Atk. Requires a sword or blunt weapon. Over-hit is possible.";
            _castlabel.text = "1.08 sec";
            _typeSkill.text = "Active Skill";
            _nameSkill.text = "Power Strike";
            _lvlSkill.text = "1 ";
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0003");
        }
        else if (name.Equals("imgbox1"))
        {
            _mpLabel.text = "19";
            _rangeLabel.text = "40";
            _descriptedLabel.text = "Supplements the user's P. Atk. with 36 Power to stun the enemy for 9 seconds. Requires a blunt weapon. Over-hit is possible.";
            _castlabel.text = "3 sec";
            _typeSkill.text = "Active Skill";
            _nameSkill.text = "Stun Attack";
            _lvlSkill.text = "1 ";
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0100");

        }
        else if (name.Equals("imgbox2"))
        {
            _mpLabel.text = "19";
            _rangeLabel.text = "40";
            _descriptedLabel.text = "Supplements the user's P. Atk. with 90 Power to inflict a powerful blow. Requires a sword or blunt weapon. Over-hit is possible.";
            _castlabel.text = "1.08 sec";
            _typeSkill.text = "Active Skill";
            _nameSkill.text = "Power Smash";
            _lvlSkill.text = "3 ";
            _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName("skill0255");

        }
    }


}
