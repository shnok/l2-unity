using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class ToolTipAction : L2PopupWindow, IToolTips
{
    private ShowToolTip _showToolTip;

    private static ToolTipAction _instance;
    private VisualElement _content;

    //Block
    private Label _nameText;
    private Label _descriptedText;

    private VisualElement _icon;
    private float _lastHeightContent = 0;
    private float _heightContent = 0;
    private float _widtchContent = 0;
    private VisualElement _selectShow;

    public static ToolTipAction Instance { get { return _instance; } }



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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAction");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("content");


        _nameText = (Label)GetElementByClass("NameSkill");
        _descriptedText = (Label)GetElementByClass("DescriptedLabel");
        _icon = GetElementByClass("Icon");


        _heightContent = _windowEle.worldBound.height;
        _widtchContent = _windowEle.worldBound.width;
        _windowEle.style.display = DisplayStyle.None;
        _descriptedText.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
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
                    if (ve != null)
                    {
                        StartCoroutine(StartNewPosition(ve));
                    }
                }, TrickleDown.TrickleDown);

                item.RegisterCallback<MouseOutEvent>(evt =>
                {
                    VisualElement ve = (VisualElement)evt.currentTarget;
                    if (ve != null)
                    {
                        StartCoroutine(EndNewPosition(ve));
                    }

                    evt.StopPropagation();

                }, TrickleDown.TrickleDown);

            }
        });

    }

    private IEnumerator EndNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.None;
        _showToolTip.Hide(ve);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator StartNewPosition(VisualElement ve)
    {
        _windowEle.style.display = DisplayStyle.Flex;
        AddData(ActionNameTable.Instance.GetAciton(ParseId(ve.name)));
        _selectShow = ve;
        //esle position layout != 0 <--- (EndNewPosition add new Vector2(0,0))
        //This means the layout did not return to the base, most likely this is an error. Restarting the transition to a new position
        if (_windowEle.worldBound.height != 0)
        {
            _showToolTip.Show(_selectShow);
            yield return new WaitForEndOfFrame();
        }

    }

    //experimental code 
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (evt.newRect.height == 0)
            return;

        if (_windowEle != null)
        {
            _heightContent = _windowEle.worldBound.height;
            _widtchContent = _windowEle.worldBound.width;
            _showToolTip.Show(_selectShow);
        }

    }
    //experimental code 
    public void NewPosition(Vector2 newPoint, float sdfig)
    {
        
        var highest = highestPoint(newPoint, _heightContent);
        var lowest = lowestPoint(newPoint, _heightContent);
        bool insideRoot = L2GameUI.Instance.IsWindowContain(highest);
        if (!ActionWindow.Instance.IsWindowContain(lowest) | IsTop(newPoint))
        {
            if (!insideRoot)
            {
                //shift down to 0 and to the right to the icon border
                //2px border 
                float sdfig1 = sdfig + 2;
               // float new_x = newPoint.x + _widtchContent;
                float new_x2 = newPoint.x + sdfig1;
                Vector2 reversePoint = new Vector2(new_x2, 0);

                _content.transform.position = reversePoint;
            }
            else
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

    private Vector2 highestPoint(Vector2 newPoint, float element)
    {
        //Added 28px. If there are problems with the upper tooltips, you need to remove them
        var element1 = element + 28;
        return new Vector2(newPoint.x, newPoint.y - element1);
    }

    //PosY/2 - center point left vertical border
    // PosY > newPoint = TOP
    //PosY < newPoint = Footer
    private bool IsTop(Vector2 newPoint)
    {
        float yPos = ActionWindow.Instance.getYposition();
        float heightAction = ActionWindow.Instance.getHeight() / 2;
        var end = yPos + heightAction;
        return end >= newPoint.y;
    }
    private Vector2 lowestPoint(Vector2 newPoint, float element)
    {
        return new Vector2(newPoint.x, newPoint.y + element);
    }

    private int ParseId(string image_id)
    {
        string str_id = image_id.Replace("image", "");
        int numVal = Int32.Parse(str_id);
        return numVal;
    }

    private void AddData(ActionData data)
    {
        if (data != null)
        {
            SetTestData(data.Name, data.Descripion, data.Icon);
        }
    }
    private void SetTestData(string name , string des , string icon)
    {
        SetDataTooTip(name, des);
       _icon.style.backgroundImage = IconManager.Instance.LoadTextureByName(icon);  
    }

    private void SetDataTooTip(string name, string descripted)
    {
        if (!string.IsNullOrEmpty(name))
        {
            _nameText.text = name;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            _descriptedText.text = descripted;
        }
    }

    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }
}
