using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionWindow : L2PopupWindow
{
    public VisualElement minimal_panel;
    private VisualElement _boxContent;
    private VisualElement _background;
    private VisualElement _boxHeader;
    private VisualElement _rootWindow;
    private ButtonActive _button;

    private static ActionWindow _instance;
    public static ActionWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _button = new ButtonActive(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Action/ActionWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        

        yield return new WaitForEndOfFrame();

         _rootWindow = GetElementByClass("root-windows");
         _boxHeader = GetElementByClass("drag-area");
         _boxContent = GetElementByClass("action_content");
         var testToolTipRow0_2 = GetElementByClass("image0_2");
         var testToolTipRow4_4 = GetElementByClass("image4_4");
         var testToolTipRow6_0 = GetElementByClass("image6_0");
         var testToolTipRow4_0 = GetElementByClass("image4_0");
         var testToolTipRow8_0 = GetElementByClass("image8_0");
         var testToolTipRow2_0 = GetElementByClass("image2_0");



        _background = GetElementByClass("background_over");
        _button.RegisterButtonCloseWindow(_rootWindow, "btn-close-frame");
        _button.RegisterClickWindow(_boxContent, _boxHeader);

        DragManipulator drag = new DragManipulator(_boxHeader, _windowEle);
        _boxHeader.AddManipulator(drag);
        List<VisualElement> list = new List<VisualElement>
        {
            testToolTipRow0_2 , testToolTipRow4_4 , testToolTipRow6_0, testToolTipRow4_0 ,testToolTipRow8_0,testToolTipRow2_0
        };
        ToolTipManager.Instance.RegisterCallbackActions(list);
        HideWindow();
    }

    public bool isWindowContain(Vector2 vector2)
    {
        return _windowEle.worldBound.Contains(vector2);
    }

    public float getYposition()
    {
        return _windowEle.worldBound.y;
    }
    public float getHeight()
    {
        return _windowEle.worldBound.height;
    }
}
