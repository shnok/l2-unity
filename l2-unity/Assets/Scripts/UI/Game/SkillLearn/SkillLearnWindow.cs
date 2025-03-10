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
    private VisualTreeAsset _skillRequirementAsset;
    private ListView skillListView;
    public SkillWindowInfo[] Skills;
    public SkillWindowInfo SelectedSkill;
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
        _skillRequirementAsset = LoadAsset("Data/UI/_Elements/Game/SkillLearnWindow/SkillLearnRequirement");
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
        SelectedSkill = (SkillWindowInfo)item;
        GameClient.Instance.ClientPacketHandler.SendRequestAcquireSkillInfo(SelectedSkill.SkillId, SelectedSkill.Level, SkillType);
    }

    public void ShowSkillDetail(SkillRequirement[] requirements)
    {
        SelectedSkill.SkillRequirement = requirements;
        _windowEle.style.display = DisplayStyle.None;

        _skillDetail = _skillDetailAsset.Instantiate()[0];
        _skillDetail.Q<VisualElement>("SkillDetailIcon").style.backgroundImage = IconTable.Instance.LoadTextureByName(SelectedSkill.Icon);
        _skillDetail.Q<Label>("DetailMPCostValue").text = SelectedSkill.MpCost.ToString();
        _skillDetail.Q<Label>("DetailRangeValue").text = SelectedSkill.Range.ToString();
        _skillDetail.Q<Label>("DetailDescription").text = SelectedSkill.Desc;
        _skillDetail.Q<Label>("DetailSPValue").text = SelectedSkill.SpCost.ToString();
        
        Button learn = _skillDetail.Q<Button>("ButtonLearn");
        ButtonClickSoundManipulator buttonLearnSoundManipulator = new ButtonClickSoundManipulator(learn);
        learn.AddManipulator(buttonLearnSoundManipulator);
        learn.RegisterCallback<MouseDownEvent>(evt =>
        {
            LearnSkill();
            _skillDetail.style.display = DisplayStyle.None;
        }, TrickleDown.TrickleDown);
        
        Button cancel = _skillDetail.Q<Button>("CancelButton");
        ButtonClickSoundManipulator buttonCancelSoundManipulator = new ButtonClickSoundManipulator(cancel);
        cancel.AddManipulator(buttonCancelSoundManipulator);
        learn.RegisterCallback<MouseDownEvent>(evt =>
        {
            _skillDetail.style.display = DisplayStyle.None;
        }, TrickleDown.TrickleDown);

        for (var i = 0; i < SelectedSkill.SkillRequirement.Length; i++)
        {
            SkillRequirement skillReq = SelectedSkill.SkillRequirement[i];
            VisualElement skillRequirementVisual = _skillRequirementAsset.Instantiate()[0];
            
            skillRequirementVisual.Q<Label>("SkillRequirementIcon").style.backgroundImage = new StyleBackground(
                IconTable.Instance.GetIcon(skillReq.ItemId));
            skillRequirementVisual.Q<Label>("SkillRequirementName").text = 
                $"{ItemTable.Instance.EtcItems[skillReq.ItemId]} x{skillReq.Count}";
        }
        _windowEle.style.display = DisplayStyle.Flex;
    }

    private void LearnSkill()
    {
        GameClient.Instance.ClientPacketHandler.SendRequestAcquireSkill(SelectedSkill.SkillId, SelectedSkill.Level, SkillType);
        PlayerSkill.Instance.AcquireSkill(SelectedSkill.SkillId, SelectedSkill.SpCost);
    }
}
