using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class SkillLearnWindow : L2PopupWindow
{

    private VisualElement _boxContent;
    private VisualElement _boxHeader;
    private VisualElement _rootWindow;
    private VisualElement _skillDetail;

    private VisualTreeAsset _skillItemAsset;
    private VisualTreeAsset _skillDetailAsset;
    private ListView skillListView;
    private List<SkillData> skills = new List<SkillData>
    {
        new SkillData("Mortal Blow", 1, 1000),
        new SkillData("Power Strike", 2, 1500),
        new SkillData("Fireball", 3, 2000),
        new SkillData("Backstab", 1, 1000),
        new SkillData("Something", 2, 1500),
        new SkillData("Recharge", 3, 2000),
        new SkillData("Resurrection", 1, 1000),
        new SkillData("Dryad Root", 2, 1500),
        new SkillData("Wind Strike", 3, 2000)

    };

    private static SkillLearnWindow _instance;
    public static SkillLearnWindow Instance
    {
        get { return _instance; }
    }
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearnWindow/SkillLearnWindow");
        _skillItemAsset = LoadAsset("Data/UI/_Elements/Game/SkillLearnWindow/SkillLearnItem");
        _skillDetailAsset = LoadAsset("Data/UI/_Elements/Game/SkillLearnWindow/SkillDetailWindow");
    }

    public void InitSkillsList()
    {
        skillListView = _windowEle.Q<ListView>("SkillList");
        skillListView.makeItem = () => _skillItemAsset.CloneTree();
        skillListView.bindItem = BindSkill;
        skillListView.itemsSource = skills;
        skillListView.selectionChanged += OnItemSelected;
        var sc = skillListView.Q<ScrollView>(className: "unity-scroll-view");
        sc.AddToClassList("l2-scroll-view");
        sc.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
    }
    
    private void BindSkill(VisualElement item, int index)
    {
        Label skillLabel = item.Q<Label>("SkillName");
        Label levelLabel = item.Q<Label>("LevelValue");
        Label spCostLabel = item.Q<Label>("SPCostValue");
        skillLabel.text = skills[index].Name;
        levelLabel.text = skills[index].Level.ToString();
        spCostLabel.text = skills[index].SpCost.ToString();
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle, this);
        dragArea.AddManipulator(drag);
        
        _skillDetail = _windowEle.Q<VisualElement>("SkillDescriptionDetail");
        _skillDetail.style.display = DisplayStyle.None;

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        
        yield return new WaitForEndOfFrame();

        InitSkillsList();

        L2GameUI.Instance.WindowLoadComplete();
    }
    
    private void OnItemSelected(object selectedItem)
    {
        _skillDetail.style.display = DisplayStyle.Flex;
    }
}

public class SkillData
{
    public string Name;
    public int Level;
    public int SpCost;

    public SkillData(string name, int level, int spCost)
    {
        Name = name;
        Level = level;
        SpCost = spCost;
    }
}
