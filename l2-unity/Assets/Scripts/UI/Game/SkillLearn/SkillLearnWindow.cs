using System.Collections;
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
    public SkillWindowInfo[] Skills;
    public PacketSkillType SkillType;

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
        _skillDetailAsset = LoadAsset("Data/UI/_Elements/Game/SkillLearnWindow/SkillLearnDetail");
    }

    public void InitSkillsList(SkillWindowInfo[] skills)
    {
        Skills = new SkillWindowInfo[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            Skills[i] = skills[i];
        }
        
        skillListView = _windowEle.Q<ListView>("SkillList");
        skillListView.makeItem = () => _skillItemAsset.CloneTree();
        skillListView.bindItem = BindSkill;
        skillListView.itemsSource = Skills;
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
        skillLabel.text = Skills[index].Name;
        levelLabel.text = Skills[index].Level.ToString();
        spCostLabel.text = Skills[index].SpCost.ToString();
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
        
        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
    }
    
    private void OnItemSelected(object item)
    {
        SkillWindowInfo skill = (SkillWindowInfo)item;
        GameClient.Instance.ClientPacketHandler.SendRequestAcquireSkillInfo(skill.SkillId, skill.Level, SkillType);
        _skillDetail.style.display = DisplayStyle.Flex;
    }

    public void ShowSkillDetail(SkillRequirement[] requirements)
    {
        // show requirements
        
    }
}
