using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillbarWindow : L2PopupWindow
{
    private bool _locked = false;
    private bool _vertical = false;
    private bool _tooltipDisabled = false;
    public bool Locked { get { return _locked; } set { _locked = value; } }
    public bool Vertical { get { return _vertical; } set { _vertical = value; } }
    public bool TooltipDisabled { get { return _tooltipDisabled; } set { _tooltipDisabled = value; } }
    [SerializeField] private int _maxSkillbarCount = 5;
    [SerializeField] private int _expandedSkillbarCount;

    private List<Coroutine> _expandCoroutines;
    private List<Coroutine> _minimizeCoroutines;
    private VisualElement _skillbarContainer;
    private VisualTreeAsset _skillbarTemplate;

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
        _skillbarTemplate = LoadAsset("Data/UI/_Elements/Game/Skillbar/SkillBarHorizontal");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _skillbarContainer = GetElementById("SkillbarContainer");

        _skillbars = new List<AbstractSkillbar>();


        for (int i = 0; i < _maxSkillbarCount; i++)
        {
            AbstractSkillbar skillbar;
            if (i == 0)
            {
                skillbar = new SkillbarMain(_windowEle, i);
            }
            else
            {
                skillbar = new SkillbarMin(_windowEle, i);
            }

            StartCoroutine(skillbar.BuildWindow(_skillbarTemplate, _skillbarContainer));
            _skillbars.Add(skillbar);
        }
    }

    public void AddSkillbar()
    {

        if (_expandedSkillbarCount++ >= _maxSkillbarCount - 2)
        {
            ((SkillbarMain)_skillbars[0]).UpdateExpandInput(1);
        }

        ExpandSkillbar();
    }

    public void ResetSkillbar()
    {
        _expandedSkillbarCount = 0;

        ((SkillbarMain)_skillbars[0]).UpdateExpandInput(0);

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

        // Hide skillbars that are not supposed to be visible
        for (int i = _expandedSkillbarCount + 1; i < _maxSkillbarCount; i++)
        {
            _skillbars[i].HideBar();
        }
    }

    private void MinimizeSkillbar()
    {
        // Stop all skillbar expand animations
        _expandCoroutines.ForEach((c) => StopCoroutine(c));
        _expandCoroutines.Clear();

        // Minimize all skillbars
        for (int i = 1; i < _maxSkillbarCount; i++)
        {
            SkillbarMin skillbar = (SkillbarMin)_skillbars[i];
            _minimizeCoroutines.Add(StartCoroutine(skillbar.Minimize()));
        }
    }

}
