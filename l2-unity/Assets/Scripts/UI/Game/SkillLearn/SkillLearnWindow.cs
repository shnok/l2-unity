using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SkillLearn : L2Window
{
    public VisualElement minimal_panel;
    private VisualElement boxContent;
    private VisualElement background;
    private VisualElement boxHeader;
    private VisualElement rootWindow;
    private ButtonSkillLearn _button;
    private bool isHide;
    private VisualElement[] _menuItems;
    private VisualElement[] _rootTabs;
    private int[] _arrDfSelect;
    private string[] fillBackgroundDf = { "Data/UI/Window/Skills/QuestWndPlusBtn_v2", "Data/UI/Window/Skills/Button_DF_Skills_Down_v3" };

    private static SkillLearn _instance;
    public static SkillLearn Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _button = new ButtonSkillLearn(this);
            _menuItems = new VisualElement[3];
            _rootTabs = new VisualElement[3];
            _arrDfSelect = new int[3] { 0,0,0};
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillLearnWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        rootWindow = GetElementByClass("root-windows");

        _rootTabs[0] = GetElementByClass("tab_active");
        _rootTabs[1] = GetElementByClass("tab_passive");

        boxHeader = GetElementByClass("drag-area");
        //var test = GetElementByClass("df-button-active-skills");

        boxContent = GetElementByClass("skill_content");

        CreateTab(boxContent, _menuItems);

         background = GetElementByClass("background_over");

        _button.RegisterButtonCloseWindow(rootWindow, "btn-close-frame");
        _button.RegisterClickWindow(boxContent, boxHeader);

        _button.RegisterClickAction(_menuItems[0]);
        _button.RegisterClickPassive(_menuItems[1]);
        _button.RegisterClickLearn(_menuItems[2]);
        _button.RegisterClickButtonPhysical(_rootTabs[0]);
       // _button.RegisterClickButtonPhysical(_rootTabs[1], "df-line-active-skills");

        DragManipulator drag = new DragManipulator(boxHeader, _windowEle);
        boxHeader.AddManipulator(drag);
        ChangeMenuSelect(0);
        //HideElements(true);
    }


    private VisualElement[] CreateTab(VisualElement boxContent, VisualElement[] _menuItems)
    {
        _menuItems[0] = boxContent.Q<VisualElement>(className: "activeTab");
        _menuItems[1] = boxContent.Q<VisualElement>(className: "passiveTab");
        _menuItems[2] = boxContent.Q<VisualElement>(className: "learnSkillTab");
        return _menuItems;
    }

    public void ToggleHideWindow()
    {
        HideElements(!isHide);
    }

    public void HideElements(bool is_hide)
    {
        if (is_hide)
        {
            isHide = is_hide;
            HideWindow();
            if (_mouseOverDetection != null) _mouseOverDetection.Disable();
            SendToBack();
        }
        else
        {
            isHide = is_hide;

            ShowWindow();
            BringToFront();
            if (_mouseOverDetection != null) _mouseOverDetection.Enable();

        }
    }

    public void ChangeMenuSelect(int indexMenu)
    {
        for (int i = 0; i < _menuItems.Length; i++)
        {
            var item = _menuItems[i];

            if (i == indexMenu)
            {
                string id = "line" + i;
                var line = item.Q<VisualElement>(className: "line" + i);
                var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i);
                var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label" + i);
                VisualElement content = _rootTabs[i];
                if(content != null) HideElement(false, _rootTabs[i]);
                HideElement(true, line);
                bigBtn(btn, label1);
            }
            else
            {
                string id = "line" + i;
                var line = item.Q<VisualElement>(className: "line" + i);
                var btn = item.Q<UnityEngine.UIElements.Button>(className: "btn" + i);
                var label1 = item.Q<UnityEngine.UIElements.Label>(className: "label" + i);
                VisualElement content = _rootTabs[i];
                if (content != null) HideElement(true, _rootTabs[i]);
                HideElement(false, line);
                normBtn(btn, label1);
            }
        }
    }
    private void HideElement(bool is_hide, VisualElement line)
    {
        if (is_hide)
        {
            line.style.display = DisplayStyle.None;
        }
        else
        {
            line.style.display = DisplayStyle.Flex;
        }

    }

    private void bigBtn(UnityEngine.UIElements.Button btn, UnityEngine.UIElements.Label label1)
    {
        btn.style.height = 21;
        btn.style.top = -2;
        label1.style.top = 0;
    }

    private void normBtn(UnityEngine.UIElements.Button btn, UnityEngine.UIElements.Label label1)
    {
        btn.style.height = 18;
        btn.style.top = 0;
        label1.style.top = 0;
    }

    public void clickDfPhysical(UnityEngine.UIElements.Button btn)
    {
        if (_arrDfSelect[0] == 0)
        {
            Texture2D iconDfNoraml = LoadTextureDF(fillBackgroundDf[0]);
            var children = btn.Children();
            var e = children.FirstOrDefault();
            e.style.display = DisplayStyle.None;
            setBackgroundDf(btn , iconDfNoraml);
            _arrDfSelect[0] = 1;
        }
        else
        {
            var children = btn.Children();
            var e = children.FirstOrDefault();
            e.style.display = DisplayStyle.Flex;
            Texture2D iconDfNoraml = LoadTextureDF(fillBackgroundDf[1]);
            setBackgroundDf(btn, iconDfNoraml);
            _arrDfSelect[0] = 0;
        }
    }

    private void setBackgroundDf(UnityEngine.UIElements.Button btn , Texture2D iconDfNoraml)
    {
        btn.style.backgroundImage = new StyleBackground(iconDfNoraml);
    }

    private Texture2D LoadTextureDF(string path)
    {
        return Resources.Load<Texture2D>(path);
    }



}
