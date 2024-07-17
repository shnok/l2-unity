using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
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
    
    private VisualElement _activeTab_physicalContent;
    private VisualElement _activeTab_magicContent;
    private VisualElement _activeTab_enhancingContent;
    private VisualElement _activeTab_debilitatingContent;
    private VisualElement _activeTab_clanContent;
    //Debilitating
    private ActiveSkillsHide _supportActiveSkills;

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
            _arrDfSelect = new int[5] { 0,0,0,0,0};
            _supportActiveSkills = new ActiveSkillsHide(this);
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

        _activeTab_physicalContent = GetElementByClass("row-physical-content");
        _activeTab_magicContent = GetElementByClass("row-magic-content");
        _activeTab_enhancingContent = GetElementByClass("row-enhancing-content");
        _activeTab_debilitatingContent = GetElementByClass("row-debilitating-content");
        _activeTab_clanContent = GetElementByClass("row-clan-content");


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
        _button.RegisterClickButtonMagic(_rootTabs[0]);
        _button.RegisterClickButtonEnhancing(_rootTabs[0]);
        _button.RegisterClickButtonDebilitating(_rootTabs[0]);
        _button.RegisterClickButtonClan(_rootTabs[0]);


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
    public void HideElement(bool is_hide, VisualElement line)
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
        _supportActiveSkills.clickDfPhysical( btn,  _activeTab_physicalContent,  _arrDfSelect);
    }

    public void clickDfMagic(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfMagic(btn, _activeTab_magicContent, _arrDfSelect);
    }

    public void clickDfEnhancing(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfEnhancing(btn, _activeTab_enhancingContent, _arrDfSelect);
    }

    public void clickDfDebilitating(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfDebilitating(btn, _activeTab_debilitatingContent, _arrDfSelect);
    }

    public void clickDfClan(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfClan(btn, _activeTab_clanContent, _arrDfSelect);
    }





}
