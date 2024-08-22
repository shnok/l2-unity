using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillbarWindow : L2PopupWindow
{
    private bool _locked = false;
    private bool _vertical = true;
    private bool _tooltipDisabled = false;
    public bool Locked { get { return _locked; } set { _locked = value; } }
    public bool Vertical { get { return _vertical; } set { _vertical = value; } }
    public bool TooltipDisabled { get { return _tooltipDisabled; } set { _tooltipDisabled = value; } }

    [SerializeField] private int _maxSkillbarCount = 5;
    [SerializeField] private int _expandedSkillbarCount;

    private List<Coroutine> _expandCoroutines;
    private List<Coroutine> _minimizeCoroutines;
    private VisualElement _skillbarContainerHorizontal;
    private VisualElement _skillbarContainerVertical;
    private VisualTreeAsset _skillbarHorizontalTemplate;
    private VisualTreeAsset _skillbarVerticalTemplate;

    private List<AbstractSkillbar> _skillbars;

    private static SkillbarWindow _instance;
    public static SkillbarWindow Instance { get { return _instance; } }

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

        _expandCoroutines = new List<Coroutine>();
        _minimizeCoroutines = new List<Coroutine>();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillbarWindow");
        _skillbarHorizontalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarHorizontal");
        _skillbarVerticalTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarVertical");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _skillbarContainerHorizontal = GetElementById("SkillbarContainerHorizontal");
        _skillbarContainerVertical = GetElementById("SkillbarContainerVertical");
        ToggleRotate();

        _skillbars = new List<AbstractSkillbar>();

        for (int x = 0; x < 2; x++)
        {
            for (int i = 0; i < _maxSkillbarCount; i++)
            {
                AbstractSkillbar skillbar;

                bool mainBar = i == 0;
                bool horizontalBar = x == 0;

                if (mainBar)
                {
                    skillbar = new SkillbarMain(_windowEle, i, horizontalBar);
                }
                else
                {
                    skillbar = new SkillbarMin(_windowEle, i, horizontalBar);
                }

                if (horizontalBar)
                {
                    StartCoroutine(skillbar.BuildWindow(_skillbarHorizontalTemplate, _skillbarContainerHorizontal));
                }
                else
                {
                    StartCoroutine(skillbar.BuildWindow(_skillbarVerticalTemplate, _skillbarContainerVertical));
                }

                _skillbars.Add(skillbar);
            }
        }

    }

    public void AddSkillbar()
    {

        if (_expandedSkillbarCount++ >= _maxSkillbarCount - 2)
        {
            ((SkillbarMain)_skillbars[0]).UpdateExpandInput(1);
            ((SkillbarMain)_skillbars[_maxSkillbarCount]).UpdateExpandInput(1);
        }

        ExpandSkillbar();
    }

    public void ResetSkillbar()
    {
        _expandedSkillbarCount = 0;

        ((SkillbarMain)_skillbars[0]).UpdateExpandInput(0);
        ((SkillbarMain)_skillbars[_maxSkillbarCount]).UpdateExpandInput(0);

        MinimizeSkillbar();
    }

    private void ExpandSkillbar()
    {
        // Stop all skillbar minimize animations
        _minimizeCoroutines.ForEach((c) => StopCoroutine(c));
        _minimizeCoroutines.Clear();

        // Expand the next skillbar
        SkillbarMin skillbar = (SkillbarMin)_skillbars[_expandedSkillbarCount];
        _expandCoroutines.Add(StartCoroutine(skillbar.Expand()));

        skillbar = (SkillbarMin)_skillbars[_expandedSkillbarCount + _maxSkillbarCount];
        _expandCoroutines.Add(StartCoroutine(skillbar.Expand()));

        // Hide skillbars that are not supposed to be visible
        for (int x = 0; x < 2; x++)
        {
            for (int i = _expandedSkillbarCount + 1; i < _maxSkillbarCount; i++)
            {
                _skillbars[i + _maxSkillbarCount * x].HideBar();
            }
        }
    }

    private void MinimizeSkillbar()
    {
        // Stop all skillbar expand animations
        _expandCoroutines.ForEach((c) => StopCoroutine(c));
        _expandCoroutines.Clear();

        // Minimize all skillbars
        for (int x = 0; x < 2; x++)
        {
            for (int i = 1; i < _maxSkillbarCount; i++)
            {
                SkillbarMin skillbar = (SkillbarMin)_skillbars[i + _maxSkillbarCount * x];
                _minimizeCoroutines.Add(StartCoroutine(skillbar.Minimize()));
            }
        }
    }

    public void ToggleRotate()
    {
        _vertical = !_vertical;

        if (_vertical)
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.None;
            _skillbarContainerVertical.style.display = DisplayStyle.Flex;
        }
        else
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.Flex;
            _skillbarContainerVertical.style.display = DisplayStyle.None;
        }
    }
}
