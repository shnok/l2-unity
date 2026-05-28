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
    private VisualTreeAsset _minimizedTemplate;
    public VisualTreeAsset SkillSectionTemplate { get; private set; }
    private VisualElement _skillsTabView;

    [SerializeField] private SkillTab[] _tabs;
    [SerializeField] private List<SkillSlot> _slots;
    private Coroutine _slotAnimationCoroutine;
    private L2TabView _l2TabView;
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
        if (_slotAnimationCoroutine != null) StopCoroutine(_slotAnimationCoroutine);
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTabHeader");
        _minimizedTemplate = LoadAsset("Data/UI/_Elements/Game/SkillWindow/SkillTabSmall");
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

        _slots = new List<SkillSlot>();
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        CreateTabs();

        yield return new WaitForEndOfFrame();

        CenterWindow();

        L2GameUI.Instance.WindowLoadComplete();

        _slotAnimationCoroutine = StartCoroutine(PlayAnimations());
    }

    public override void ToggleHideWindow()
    {
        if (_isWindowHidden)
        {
            if (!PlayerSkill.Instance.Initialized)
            {
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
        _slots.Clear();

        if (skills == null)
        {
            skills = Array.Empty<List<SkillWindowInfo>>();
        }

        foreach (var t in _tabs)
        {
            t.UpdateSkills(skills);
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("window_open");
        L2GameUI.Instance.WindowOpened(this);
    }

    public override void HideWindow(bool silent)
    {
        if (_isWindowHidden)
        {
            return;
        }

        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("window_close");

        L2GameUI.Instance.WindowClosed(this);
    }

    private void CreateTabs()
    {
        _skillsTabView = GetElementById("SkillsTabView");

        _l2TabView = new L2TabView();
        _l2TabView.Initialize(_skillsTabView, _tabs, _tabTemplate, _tabHeaderTemplate, true);
    }

    private IEnumerator PlayAnimations()
    {
        while (true)
        {
            if (!_isWindowHidden)
            {
                float now = Time.time;

                foreach (SkillSlot slot in _slots)
                {
                    if (slot.SkillInfo.IsPassive)
                    {
                        continue;
                    }

                    slot.SlotAnimationManipulator.AnimateCoolTime(now);
                }
            }

            yield return new WaitForSeconds(1 / 30f); // 30 fps
        }
    }

    public void AddSlot(L2Slot l2Slot)
    {
        _slots.Add((SkillSlot)l2Slot);
    }
}