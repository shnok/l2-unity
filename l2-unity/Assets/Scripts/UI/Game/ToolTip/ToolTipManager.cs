using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolTipManager : L2PopupWindow
{

    private ShowToolTip _showToolTip;

    private static ToolTipManager _instance;
    private VisualElement _content;
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
        Debug.Log("");

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


}
