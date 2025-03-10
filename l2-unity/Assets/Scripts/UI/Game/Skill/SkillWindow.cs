using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillWindow : L2PopupWindow
{
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _minimizedTemplate;
    public VisualTreeAsset SkillSectionTemplate { get; private set; }
    public VisualTreeAsset SkillSlotTemplate { get; private set; }
    private VisualElement _skillsTabView;
    private VisualElement _content;
    private SkillTab _activeTab;
    private bool isActive;
    
    private List<SkillWindowInfo>[] _skillsList;
    [SerializeField] private List<SkillTab> _tabs;
    private static SkillWindow _instance;
    public static SkillWindow Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTabHeader");
        _minimizedTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTabSmall");
        SkillSlotTemplate = LoadAsset("Data/UI/_Elements/Components/L2Slot/SkillSlot");
        SkillSectionTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillSection");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        
        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        CreateTabs();
        
        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
    }
    
    public override void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            if (!PlayerSkill.Instance.Initialized) 
            {
                GameClient.Instance.ClientPacketHandler.SendRequestSkill();
                SetSkills(PlayerSkill.Instance.GetSkillsForWindow());
            }
            ShowWindow();
        }
        else
        {
            HideWindow(false);
        }
    }

    public void SetSkills(List<SkillWindowInfo>[] skills)
    {
        if (skills == null)
        {
            skills = Array.Empty<List<SkillWindowInfo>>();
        }
        
        _skillsList = skills;
        foreach (var t in _tabs)
        {
            t.UpdateSkills(skills);
        }
    }
    
    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("click_01");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("click_02");

        L2GameUI.Instance.WindowClosed(this);
    }
    
    private void CreateTabs()
    {
        _skillsTabView = GetElementById("SkillsTabView");

        VisualElement tabHeaderContainer = _skillsTabView.Q<VisualElement>("tab-header-container");
        VisualElement tabContainer = _skillsTabView.Q<VisualElement>("tab-content-container");

        for (int i = _tabs.Count - 1; i >= 0; i--)
        {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;
            
            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement, _tabs[i].TabType);
        }

        _activeTab = null;

        if (_tabs.Any())
        {
            SwitchTab(_tabs[0]);
            isActive = true;
        }
    }

    public bool SwitchTab(SkillTab switchTo)
    {
        if (_activeTab != switchTo)
        {
            _activeTab?.TabContainer?.AddToClassList("unselected-tab");
            _activeTab?.TabHeader?.RemoveFromClassList("active");

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");

            _activeTab = switchTo;
            isActive = !isActive;
            return true;
        }
        return false;
    }
}