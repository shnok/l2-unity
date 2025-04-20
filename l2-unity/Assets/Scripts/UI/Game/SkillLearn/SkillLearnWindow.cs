using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class SkillLearnWindow : L2PopupWindow
{
    private VisualElement _skillDetail;
    private VisualElement _spGroup;

    private Label _skillDetailName;
    private VisualElement _skillDetailIcon;
    private Label _detailLevelValue;
    private Label _detailDescription;
    private Label _detailSpValue;
    private Label _userSpDetailValue;
    private Label _userSpValue;
    private Label _detailType;
    private VisualElement _detailRangeStats;
    private VisualElement _detailMpStats;
    private Label _detailRangeValue;
    private Label _detailMpCostValue;
    private Label _spGroupValue;

    private L2ScrollableList<SkillWindowInfo> _skillList;
    private VisualTreeAsset _skillItemAsset;
    private VisualTreeAsset _skillDetailAsset;
    private VisualTreeAsset _skillRequirementAsset;
    private SkillWindowInfo[] _skills;
    private SkillWindowInfo _selectedSkill;
    public PacketSkillType SkillType;
    [SerializeField] private int playerSp;

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
        _skills = new SkillWindowInfo[skills.Length];
        for (int i = 0; i < skills.Length; i++)
        {
            _skills[i] = skills[i];
        }

        if (_skillList == null)
        {
            // preadd skill detail 
            VisualElement content = _windowEle.Q<VisualElement>(className: "skill-learn-detail-content");
            _skillDetail = _skillDetailAsset.Instantiate()[0];
            _skillDetail.style.display = DisplayStyle.None;
            content.Add(_skillDetail);

            _spGroup = _windowEle.Q<VisualElement>("UserSPGroup");
            _userSpValue = _windowEle.Q<Label>("UserSPValue");

            TemplateContainer learn = _skillDetail.Q<TemplateContainer>("LearnButton");
            ButtonClickSoundManipulator buttonLearnSoundManipulator = new ButtonClickSoundManipulator(learn);
            learn.AddManipulator(buttonLearnSoundManipulator);
            learn.RegisterCallback<MouseDownEvent>(_ =>
            {
                LearnSkill();
                _selectedSkill = null;
                _skillList?.RemoveSelectedFromList();
                ToggleShowSkillDetail();
            }, TrickleDown.TrickleDown);

            TemplateContainer cancel = _skillDetail.Q<TemplateContainer>("CancelButton");
            ButtonClickSoundManipulator buttonCancelSoundManipulator = new ButtonClickSoundManipulator(cancel);
            cancel.AddManipulator(buttonCancelSoundManipulator);
            cancel.RegisterCallback<MouseDownEvent>(_ =>
            {
                _skillList?.UnSelect();
                _selectedSkill = null;
                ToggleShowSkillDetail();
            }, TrickleDown.TrickleDown);

            _skillDetailName = _skillDetail.Q<Label>("SkillDetailName");
            _skillDetailIcon = _skillDetail.Q<VisualElement>("SkillDetailcon").Q("SlotBg");
            _detailLevelValue = _skillDetail.Q<Label>("DetailLevelValue");
            _detailDescription = _skillDetail.Q<Label>("DetailDescription");
            _detailSpValue = _skillDetail.Q<Label>("DetailSPValue");
            _detailType = _skillDetail.Q<Label>("DetailType");
            _detailMpCostValue = _skillDetail.Q<Label>("DetailMPCostValue");
            _detailMpStats = _skillDetail.Q<VisualElement>("DetailMpStats");
            _detailRangeStats = _skillDetail.Q<VisualElement>("DetailRangeStats");
            _detailRangeValue = _skillDetail.Q<Label>("DetailRangeValue");
            _userSpDetailValue = _skillDetail.Q<Label>("UserSPValue");
        }

        _skillList = new L2ScrollableList<SkillWindowInfo>();
        _skillList.Initialize(_windowEle.Q<VisualElement>("ListView"), _skills, BindSkill, alternatingRowColor: true);
        _skillList.RemoveItem = RemoveSkill;
        playerSp = ((PlayerStats)PlayerEntity.Instance.Stats).Sp;
        _userSpValue.text = playerSp.ToString();
        _userSpDetailValue.text = playerSp.ToString();

        _selectedSkill = null;
        _spGroup.style.display = DisplayStyle.Flex;
        _skillDetail.style.display = DisplayStyle.None;
        _skillList.Show();

        ShowWindow();
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

        CenterWindow();

        L2GameUI.Instance.WindowLoadComplete();
    }

    private void BindSkill(int index, out VisualElement item)
    {
        item = _skillItemAsset.Instantiate()[0];
        item?.RegisterCallback<PointerDownEvent>(_ => OnItemSelected(index));

        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(_skills[index].Icon));
        VisualElement icon = item.Q<VisualElement>("SlotBg");
        icon.style.backgroundImage = background;
        Label skillLabel = item.Q<Label>("SkillName");
        Label levelLabel = item.Q<Label>("LevelValue");
        Label spCostLabel = item.Q<Label>("SPCostValue");

        skillLabel.text = _skills[index].Name;
        levelLabel.text = _skills[index].Level.ToString();
        spCostLabel.text = _skills[index].SpCost.ToString();
    }

    private void RemoveSkill(VisualElement item, int index)
    {
        item.UnregisterCallback<PointerDownEvent>(_ => OnItemSelected(index));
    }

    private void OnItemSelected(int index)
    {
        AudioManager.Instance.PlayUISound("click_01");
        _selectedSkill = _skills[index];
        _skillList.SelectItem(index);
        GameClient.Instance.ClientPacketHandler.SendRequestAcquireSkillInfo(_selectedSkill.SkillId, _selectedSkill.Level, SkillType);

        _skillDetailName.text = _selectedSkill.Name;
        _skillDetailIcon.style.backgroundImage = IconTable.Instance.LoadTextureByName(_selectedSkill.Icon);
        _detailLevelValue.text = _selectedSkill.Level.ToString();
        _detailDescription.text = _selectedSkill.ComposeDescription();
        _detailSpValue.text = _selectedSkill.SpCost.ToString();
        _userSpDetailValue.text = playerSp.ToString();
        _detailType.text = _selectedSkill.GetSkillType();

        // if (_selectedSkill.Type is global::SkillType.Passive or global::SkillType.EquipmentPassive or
        //     global::SkillType.WeightLimit or global::SkillType.CraftAndItems)
        // {
        //     _detailRangeStats.style.display = DisplayStyle.None;
        //     _detailMpStats.style.display = DisplayStyle.None;
        // }
        // else
        // {
        //     _detailRangeValue.text = _selectedSkill.Range.ToString();
        //     _detailRangeStats.style.display = DisplayStyle.Flex;
        //     _detailMpCostValue.text = _selectedSkill.MpCost.ToString();
        //     _detailMpStats.style.display = DisplayStyle.Flex;
        // }

        if (_selectedSkill.MpCost == 0)
        {
            _detailMpStats.style.display = DisplayStyle.None;
        }
        else
        {
            _detailMpCostValue.text = _selectedSkill.MpCost.ToString();
            _detailMpStats.style.display = DisplayStyle.Flex;
        }

        if (_selectedSkill.Range == 0 || _selectedSkill.Range == -1)
        {
            _detailRangeStats.style.display = DisplayStyle.None;

        }
        else
        {
            _detailRangeValue.text = _selectedSkill.Range.ToString();
            _detailRangeStats.style.display = DisplayStyle.Flex;
        }

        ToggleShowSkillDetail();
    }

    public void ShowSkillDetail(SkillRequirement[] requirements)
    {
        _selectedSkill.SkillRequirement = requirements;
        VisualElement conditions = _skillDetail.Q<VisualElement>("SkillLearnConditions");
        for (var i = 0; i < _selectedSkill.SkillRequirement.Length; i++)
        {
            SkillRequirement skillReq = _selectedSkill.SkillRequirement[i];
            VisualElement skillRequirementVisual = _skillRequirementAsset.Instantiate()[0];

            skillRequirementVisual.Q<Label>("SkillRequirementIcon").style.backgroundImage = new StyleBackground(
                IconTable.Instance.GetIcon(skillReq.ItemId));
            skillRequirementVisual.Q<Label>("SkillRequirementName").text =
                $"{ItemTable.Instance.EtcItems[skillReq.ItemId]} x{skillReq.Count}";
            conditions.Add(skillRequirementVisual);
        }
    }

    private void ToggleShowSkillDetail()
    {
        AudioManager.Instance.PlayUISound("window_open");

        if (_selectedSkill != null)
        {
            _spGroup.style.display = DisplayStyle.None;
            _skillDetail.style.display = DisplayStyle.Flex;
        }
        else
        {
            _spGroup.style.display = DisplayStyle.Flex;
            _skillDetail.style.display = DisplayStyle.None;
        }

        _skillList.ToggleShowHide();
    }

    private void LearnSkill()
    {
        GameClient.Instance.ClientPacketHandler.SendRequestAcquireSkill(_selectedSkill.SkillId, _selectedSkill.Level, SkillType);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("window_close");

        L2GameUI.Instance.WindowClosed(this);
    }
}
