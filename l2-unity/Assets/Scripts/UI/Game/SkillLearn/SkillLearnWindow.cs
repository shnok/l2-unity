using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.Rendering.DebugUI;

public class SkillLearn : L2PopupWindow
{

    public VisualElement minimal_panel;
    private VisualElement _boxContent;
    private VisualElement _background;
    private VisualElement _boxHeader;
    private VisualElement _rootWindow;
    private ButtonSkillLearn _button;
    private bool isHide;
    private VisualElement[] _menuItems;
    private VisualElement[] _rootTabs;
    private int[] _arrDfActiveSelect;
    private int[] _arrDfPassiveSelect;

    private VisualElement _activeTab_physicalContent;
    private VisualElement _activeTab_magicContent;
    private VisualElement _activeTab_enhancingContent;
    private VisualElement _activeTab_debilitatingContent;
    private VisualElement _activeTab_clanContent;
    private string _border_gold = "itemwindow_df_frame_Clear";

    private VisualElement _passiveTab_abilityContent;
    private VisualElement _passiveTab_subjectContent;

    //Debilitating
    private ActiveSkillsHide _supportActiveSkills;
    private PassiveSkillsHide _supportPassiveSkills;


    //All Active Skills
    private Dictionary<int, Skillgrp> _activeSkills;
    //All Passive Skills
    private Dictionary<int, Skillgrp> _passiveSkills;

    private Dictionary<int, VisualElement> _physicalSkillsRow;
    private Dictionary<int, VisualElement> _magicSkillsRow;
    private Dictionary<int, VisualElement> _enchancingSkillsRow;
    private Dictionary<int, VisualElement> _debilitatingSkillsRow;
    private Dictionary<int, VisualElement> _clanSkillsRow;

    private Dictionary<int, VisualElement> _abillitySkillsRow;
    private Dictionary<int, VisualElement> _subjectSkillsRow;
    private int _sizeActiveCells = 42;
    private int _sizePassiveCells = 14;

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
            _arrDfActiveSelect = new int[5] { 0,0,0,0,0};
            _arrDfPassiveSelect = new int[2] { 0, 0 };
            _supportActiveSkills = new ActiveSkillsHide(this);
            _supportPassiveSkills = new PassiveSkillsHide(this);
            //42 cells active panels
            _physicalSkillsRow = new Dictionary<int, VisualElement>(7);
            _magicSkillsRow = new Dictionary<int, VisualElement>(7);
            _enchancingSkillsRow = new Dictionary<int, VisualElement>(7);
            _debilitatingSkillsRow = new Dictionary<int, VisualElement>(7);
            _clanSkillsRow = new Dictionary<int, VisualElement>(14);

            _activeSkills = new Dictionary<int, Skillgrp>(_sizeActiveCells);
            _passiveSkills = new Dictionary<int, Skillgrp>(_sizeActiveCells);
            //14 cells passive panels > temporarily
            _abillitySkillsRow = new Dictionary<int, VisualElement>(_sizePassiveCells);
            _subjectSkillsRow = new Dictionary<int, VisualElement>(_sizePassiveCells);
        }
        else
        {
            Destroy(this);
        }
    }

    public bool IsWindowContain(Vector2 vector2)
    {
        return  _windowEle.worldBound.Contains(vector2);
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

        _rootWindow = GetElementByClass("root-windows");

        _rootTabs[0] = GetElementByClass("tab_active");
        _rootTabs[1] = GetElementByClass("tab_passive");
        _rootTabs[2] = GetElementByClass("tab_learn");

        _activeTab_physicalContent = GetElementByClass("row-physical-content");
        _activeTab_magicContent = GetElementByClass("row-magic-content");
        _activeTab_enhancingContent = GetElementByClass("row-enhancing-content");
        _activeTab_debilitatingContent = GetElementByClass("row-debilitating-content");
        _activeTab_clanContent = GetElementByClass("row-clan-content");

        _passiveTab_abilityContent = GetElementByClass("row-ability-content");
        _passiveTab_subjectContent = GetElementByClass("row-subject-content");

        //ActiveSkills 
        InitializedCells(_physicalSkillsRow , 0 , 6);
        InitializedCells(_magicSkillsRow , 7 , 13);
        InitializedCells(_enchancingSkillsRow , 14 , 20);
        InitializedCells(_debilitatingSkillsRow , 21 , 27);
        InitializedCells(_clanSkillsRow, 28 , 41);

        //Passive Skills
        InitializedCellsPassive(_abillitySkillsRow, 0 , 6);
        InitializedCellsPassive(_subjectSkillsRow, 7 , 13);

        //physic  test data
        AddSkillToCell(3,  3, 0);
        AddSkillToCell(190, 1, 1);
        AddSkillToCell(410, 1, 2);

        //magic  test data
        AddSkillToCell(1177, 1, 8);
        AddSkillToCell(1147, 1, 7);

        //passive ability
        AddSkillToCell(172, 3, 0);

        // var testToolTipRow0  = GetElementByClass("imgbox7");
        //var testToolTipRow1 = GetElementByClass("imgbox1");
        // var testToolTipRow2 = GetElementByClass("imgbox2");
        // var testToolTipRow3 = GetElementByClass("imgbox3");
        // var testToolTipRow16 = GetElementByClass("imgbox20");
        // var testToolTipRow15 = GetElementByClass("imgbox19");
        // var dropTest99 = GetElementByClass("imgbox99");
        // var dropTest100 = GetElementByClass("imgbox100");
        // var testToolTipRow91_passive = GetElementByClass("imgbox91");




        ToolTipManager.Instance.RegisterCallbackActiveSkills(_physicalSkillsRow , this);
        ToolTipManager.Instance.RegisterCallbackActiveSkills(_magicSkillsRow, this);
        ToolTipManager.Instance.RegisterCallbackActiveSkills(_enchancingSkillsRow, this);
        ToolTipManager.Instance.RegisterCallbackActiveSkills(_debilitatingSkillsRow,  this);
        ToolTipManager.Instance.RegisterCallbackActiveSkills(_clanSkillsRow,  this);

        ToolTipManager.Instance.RegisterCallbackPassiveSkills(_abillitySkillsRow, this);
        ToolTipManager.Instance.RegisterCallbackPassiveSkills(_subjectSkillsRow, this);

        //ToolTipManager.Instance.RegisterCallbackSkills(_abillitySkillsRow, 1 , this);
        //ToolTipManager.Instance.RegisterCallbackSkills(_subjectSkillsRow , 1 ,  this);

        UnityEngine.UIElements.Button closeButton = (UnityEngine.UIElements.Button)GetElementById("CloseButton");

        _boxHeader = GetElementByClass("drag-area");
        _boxContent = GetElementByClass("skill_content");
        CreateTab(_boxContent, _menuItems);
         _background = GetElementByClass("background_over");

       
        _button.RegisterButtonCloseWindow(_rootWindow, "btn-close-frame");
        _button.RegisterClickCloseButton(closeButton);
        _button.RegisterClickWindow(_boxContent, _boxHeader);


        _button.RegisterClickAction(_menuItems[0]);
        _button.RegisterClickPassive(_menuItems[1]);
        _button.RegisterClickLearn(_menuItems[2]);
        _button.RegisterClickButtonPhysical(_rootTabs[0]);
        _button.RegisterClickButtonMagic(_rootTabs[0]);
        _button.RegisterClickButtonEnhancing(_rootTabs[0]);
        _button.RegisterClickButtonDebilitating(_rootTabs[0]);
        _button.RegisterClickButtonClan(_rootTabs[0]);

        _button.RegisterClickButtonAbility(_rootTabs[1]);
        _button.RegisterClickButtonSubject(_rootTabs[1]);

       // List<VisualElement> list_Drop = new List<VisualElement>
        //{
        //    dropTest99 , dropTest100 , testToolTipRow0
        //};

        //DragAndDropManager.getInstance().RegisterList(list_Drop);

        DragManipulator drag = new DragManipulator(_boxHeader, _windowEle);
        _boxHeader.AddManipulator(drag);
        ChangeMenuSelect(0);

        _mouseOverDetection = new MouseOverDetectionManipulator(_rootWindow);
        _rootWindow.AddManipulator(_mouseOverDetection);
        HideWindow();

    }

    public Skillgrp GetSkillIdByCellId(int active , int cellId)
    {
        if(active == 1)
        {
            if(_activeSkills.ContainsKey(cellId)) return _activeSkills[cellId];
        }
        else if(active == 2)
        {
            if (_passiveSkills.ContainsKey(cellId)) return _passiveSkills[cellId];
        }
        return null;
    }

    private void AddSkillToCell(int skillId , int skillLevel , int cell)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, skillLevel);
        if(skillgrp != null)
        {
            if (skillgrp.IsMagic == 0)
            {
                //1 - active
                //2 - passive
                if(skillgrp.OperateType == 1 | skillgrp.OperateType == 0)
                {
                    AddSkillToVisualElement(skillgrp, _physicalSkillsRow, cell);
                    AddSkillToCellDataActivePanel(cell, skillgrp);
                }else if(skillgrp.OperateType == 2)
                {
                    AddSkillToVisualElement(skillgrp, _abillitySkillsRow, cell);
                    AddSkillToCellDataPassivePanel(cell, skillgrp);
                }
               
            }
            else
            {
                if (skillgrp.OperateType == 1 | skillgrp.OperateType == 0)
                {
                    AddSkillToVisualElement(skillgrp, _magicSkillsRow, cell);
                    AddSkillToCellDataActivePanel(cell, skillgrp);
                }
                else if (skillgrp.OperateType == 2)
                {
                    AddSkillToVisualElement(skillgrp, _abillitySkillsRow, cell);
                    AddSkillToCellDataPassivePanel(cell, skillgrp);
                }
                  
            }
        }
      
    }

    private void AddSkillToVisualElement(Skillgrp skillgrp , Dictionary<int, VisualElement> dict , int cell )
    {
        if (dict.ContainsKey(cell)){
            VisualElement element = dict[cell];
            VisualElement parent = element.parent;
            parent.style.backgroundImage = IconManager.Instance.LoadTextureByName(_border_gold);
            element.style.backgroundImage = IconManager.Instance.LoadTextureByName(skillgrp.Icon);
        }
      
    }
    //cell - layout
    //_activeSkills - all cell and all SkillGrp
    private void AddSkillToCellDataActivePanel(int cell , Skillgrp skillgrp)
    {
        if (!_activeSkills.ContainsKey(cell))
        {
            _activeSkills.Add(cell, skillgrp);
        }
        else
        {
            _activeSkills[cell] = skillgrp;
        }
    }

    //cell - layout
    //_activeSkills - all cell and all SkillGrp
    private void AddSkillToCellDataPassivePanel(int cell, Skillgrp skillgrp)
    {
        if (!_passiveSkills.ContainsKey(cell))
        {
            _passiveSkills.Add(cell, skillgrp);
        }
        else
        {
            _passiveSkills[cell] = skillgrp;
        }
    }

    private VisualElement[] CreateTab(VisualElement boxContent, VisualElement[] _menuItems)
    {
        _menuItems[0] = boxContent.Q<VisualElement>(className: "activeTab");
        _menuItems[1] = boxContent.Q<VisualElement>(className: "passiveTab");
        _menuItems[2] = boxContent.Q<VisualElement>(className: "learnSkillTab");
        return _menuItems;
    }

    private void InitializedCells(Dictionary<int, VisualElement> dict ,int begin ,int end)
    {
        for(int i= begin; i <= end; i++)
        {
            var element = GetElementByClass("imgbox"+i);
            if(element != null) dict.Add(i, element);
        }
    }

    private void InitializedCellsPassive(Dictionary<int, VisualElement> dict, int begin, int end)
    {
        for (int i = begin; i <= end; i++)
        {
            var element = GetElementByClass("pasbox" + i);
            if (element != null) dict.Add(i, element);
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
        _supportActiveSkills.clickDfPhysical( btn,  _activeTab_physicalContent, _arrDfActiveSelect);
    }

    public void clickDfMagic(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfMagic(btn, _activeTab_magicContent, _arrDfActiveSelect);
    }

    public void clickDfEnhancing(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfEnhancing(btn, _activeTab_enhancingContent, _arrDfActiveSelect);
    }

    public void clickDfDebilitating(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfDebilitating(btn, _activeTab_debilitatingContent, _arrDfActiveSelect);
    }

    public void clickDfClan(UnityEngine.UIElements.Button btn)
    {
        _supportActiveSkills.clickDfClan(btn, _activeTab_clanContent, _arrDfActiveSelect);
    }

    public void clickDfAbility(UnityEngine.UIElements.Button btn)
    {
        _supportPassiveSkills.clickDfAbiliti(btn, _passiveTab_abilityContent, _arrDfPassiveSelect);
    }

    public void clickDfSubject(UnityEngine.UIElements.Button btn)
    {
        _supportPassiveSkills.clickDfSubject(btn, _passiveTab_subjectContent, _arrDfPassiveSelect);
    }





}
