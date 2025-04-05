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
    [SerializeField] private int _defaultSkillbarCount;
    [SerializeField] private int _expandedSkillbarCount;

    private List<Coroutine> _expandCoroutines;
    private List<Coroutine> _minimizeCoroutines;
    private Coroutine _slotToggleAnimationCoroutine;
    private Coroutine _cooldownAnimationCoroutine;
    private VisualElement _skillbarContainerHorizontal;
    private VisualElement _skillbarContainerVertical;
    private VisualTreeAsset _skillbarHorizontalTemplate;
    private VisualTreeAsset _skillbarVerticalTemplate;
    private List<AbstractSkillbar> _skillbars;
    private static SkillbarWindow _instance;
    private List<SkillbarSlot> _toggledSlots;
    private List<SkillbarSlot> _skillsOnCooldown;

    public bool Locked { get { return _locked; } set { _locked = value; } }
    public bool Vertical { get { return _vertical; } set { _vertical = value; } }
    public bool TooltipDisabled { get { return _tooltipDisabled; } set { _tooltipDisabled = value; } }
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
        _toggledSlots = new List<SkillbarSlot>();
        _skillsOnCooldown = new List<SkillbarSlot>();
    }

    private void OnDestroy()
    {
        _instance = null;
        if (_slotToggleAnimationCoroutine != null) StopCoroutine(_slotToggleAnimationCoroutine);
        if (_cooldownAnimationCoroutine != null) StopCoroutine(_cooldownAnimationCoroutine);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SkillbarWindow/SkillbarWindow");
        _skillbarHorizontalTemplate = LoadAsset("Data/UI/_Elements/Game/SkillbarWindow/SkillBarHorizontal");
        _skillbarVerticalTemplate = LoadAsset("Data/UI/_Elements/Game/SkillbarWindow/SkillBarVertical");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        // Center window
        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.right = StyleKeyword.Undefined;
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), 0));

        RegisterClickWindowEvent(_windowEle, null);
        _skillbarContainerHorizontal = GetElementById("SkillbarContainerHorizontal");
        _skillbarContainerVertical = GetElementById("SkillbarContainerVertical");
        ToggleRotate();

        _skillbars = new List<AbstractSkillbar>();

        for (int x = 0; x < 2; x++)
        {
            for (int i = 0; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                AbstractSkillbar skillbar;

                bool mainBar = i == 0;
                bool horizontalBar = x == 0;

                if (mainBar)
                {
                    skillbar = new SkillbarMain(_windowEle, i, i, horizontalBar);
                }
                else
                {
                    skillbar = new SkillbarMin(_windowEle, i, i, horizontalBar);
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

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < _defaultSkillbarCount; i++)
        {
            AddSkillbar();
        }

        _slotToggleAnimationCoroutine = StartCoroutine(PlayToggleAnimations());
        _cooldownAnimationCoroutine = StartCoroutine(PlayCooldownAnimations());

#if UNITY_EDITOR
        // DebugData();
#endif

        L2GameUI.Instance.WindowLoadComplete();
    }

    public void AddSkillbar()
    {
        for (int x = 0; x < 2; x++)
        {
            if (_expandedSkillbarCount >= PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT - 2)
            {
                ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(1);
            }
        }

        _expandedSkillbarCount++;

        ExpandSkillbar();
    }

    public void ResetSkillbar()
    {
        _expandedSkillbarCount = 0;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(0);
        }

        MinimizeSkillbar();
    }

    private void ExpandSkillbar()
    {
        // Stop all skillbar minimize animations
        _minimizeCoroutines.ForEach((c) => StopCoroutine(c));
        _minimizeCoroutines.Clear();

        for (int x = 0; x < 2; x++)
        {
            // Expand the next skillbar
            SkillbarMin skillbar = (SkillbarMin)_skillbars[_expandedSkillbarCount + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
            _expandCoroutines.Add(StartCoroutine(skillbar.Expand()));

            // Hide skillbars that are not supposed to be visible
            for (int i = _expandedSkillbarCount + 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                _skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].HideBar();
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
            for (int i = 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                SkillbarMin skillbar = (SkillbarMin)_skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
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

    public void OnPageChanged(int skillbarIndex, int page)
    {
        for (int x = 0; x < 2; x++)
        {
            _skillbars[skillbarIndex + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].ChangePage(page);
        }

        PlayerShortcuts.Instance.UpdatePageMapping(skillbarIndex, page);

        StartCoroutine(UpdateAllShortcuts(PlayerShortcuts.Instance.Shortcuts));
    }

    public void ToggleLockSkillBar()
    {
        Locked = !Locked;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleLockSkillBar();
        }
    }

    public void ToggleDisableTooltip()
    {
        TooltipDisabled = !TooltipDisabled;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleDisableTooltip();
        }
    }

    public IEnumerator UpdateAllShortcuts(List<Shortcut> shortcuts)
    {
        while (!PlayerInventory.Instance.Initialized)
        {
            yield return new WaitForSeconds(0.1f);
        }

        _toggledSlots.Clear();
        _skillsOnCooldown.Clear();

        _skillbars.ForEach((skillbar) => skillbar.ResetShortcuts());

        if (shortcuts == null)
        {
            yield return null;
        }

        shortcuts.ForEach((shortcut) =>
        {
            UpdateBarShortcut(shortcut);
        });
    }

    private void UpdateBarShortcut(Shortcut shortcut)
    {
        if (shortcut.Slot < 12)
        {
            _skillbars.ForEach((skillbar) =>
            {
                if (skillbar.Page == shortcut.Page)
                {
                    // Debug.Log($"Add shortcut at page {shortcut.Page} in slot {shortcut.Slot}.");
                    skillbar.UpdateShortcut(shortcut, shortcut.Slot);
                }
            });
        }
        else
        {
            Debug.LogError($"Slot value {shortcut.Slot} is too high.");
        }
    }

    public void AddShortcut(Shortcut shortcut)
    {
        UpdateBarShortcut(shortcut);
    }

    public void RemoveShortcut(int oldSlot)
    {
        int slot = oldSlot % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
        int page = oldSlot / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;

        _skillbars.ForEach((skillbar) =>
            {
                if (skillbar.Page == page)
                {
                    skillbar.DeleteShortcut(oldSlot);
                }
            });
    }

    private SkillbarSlot GetSlotAt(int page, int slot)
    {
        foreach (AbstractSkillbar skillbar in _skillbars)
        {
            if (skillbar.Page == page)
            {
                if (skillbar.BarSlots.Count <= slot)
                {
                    Debug.LogWarning($"Skillbar slot error: {slot}");
                    return null;
                }

                return skillbar.BarSlots[slot];
            }
        }

        return null;
    }

    public void AddToggledSlot(int page, int slot)
    {
        SkillbarSlot skillbarSlot = GetSlotAt(page, slot);
        if (skillbarSlot != null)
        {
            skillbarSlot.Toggled = true;
            AddToggledSlot(skillbarSlot);
        }
    }

    public void RemoveToggledSlot(int page, int slot)
    {
        SkillbarSlot skillbarSlot = GetSlotAt(page, slot);
        if (skillbarSlot != null)
        {
            skillbarSlot.Toggled = false;
            RemoveToggledSlot(skillbarSlot);
        }
    }

    public void AddToggledSlot(SkillbarSlot slot)
    {
        if (!_toggledSlots.Contains(slot))
        {
            _toggledSlots.Add(slot);
        }
    }

    public void RemoveToggledSlot(SkillbarSlot slot)
    {
        if (_toggledSlots.Contains(slot))
        {
            slot.SlotEffect.style.backgroundImage = new StyleBackground();
            _toggledSlots.Remove(slot);
        }
    }

    private IEnumerator PlayToggleAnimations()
    {
        Texture2D[] toggleImages = SkillbarImageTable.Instance.ToggleTextures;

        int toggleCount = 0;
        while (true)
        {
            _toggledSlots.ForEach((slot) =>
            {
                slot.SlotEffect.style.backgroundImage = new StyleBackground(toggleImages[toggleCount]);
            });

            yield return new WaitForSeconds(0.1f);

            if (toggleCount++ >= 12)
            {
                toggleCount = 0;
            }
        }
    }

    public void AddSkillOnCooldown(int page, int slot, SkillInfo skillInfo)
    {
        SkillbarSlot skillbarSlot = GetSlotAt(page, slot);
        if (skillbarSlot != null)
        {
            skillbarSlot.CooldownStartTime = skillInfo.CooldownStartTime;
            skillbarSlot.CooldownEndTime = skillInfo.CooldownEndTime;

            AddSkillOnCooldown(skillbarSlot);
        }
    }

    public void AddSkillOnCooldown(SkillbarSlot slot)
    {
        if (!_skillsOnCooldown.Contains(slot))
        {
            _skillsOnCooldown.Add(slot);
        }
    }

    public void RemoveOnCooldownSkill(SkillbarSlot slot)
    {
        if (_skillsOnCooldown.Contains(slot))
        {
            _skillsOnCooldown.Remove(slot);
        }
    }

    private IEnumerator PlayCooldownAnimations()
    {
        Texture2D[] cooltimeImages = SkillbarImageTable.Instance.CooltimeTextures;

        while (true)
        {
            float now = Time.time;
            for (int i = _skillsOnCooldown.Count - 1; i >= 0; i--)
            {
                if (now >= _skillsOnCooldown[i].CooldownEndTime)
                {
                    //Todo: play the skill cooldown end animation and sound
                    _skillsOnCooldown[i].SlotEffect.style.backgroundImage = new StyleBackground();
                    _skillsOnCooldown.RemoveAt(i);
                    AudioManager.Instance.PlayUISound("cooltime_end");
                    continue;
                }

                float elapsed = now - _skillsOnCooldown[i].CooldownStartTime;
                float duration = _skillsOnCooldown[i].CooldownEndTime - _skillsOnCooldown[i].CooldownStartTime;
                float ratio = elapsed / duration;
                int imageIndex = Mathf.Clamp((int)Mathf.Round(ratio * cooltimeImages.Length - 1), 0, cooltimeImages.Length - 1);

                _skillsOnCooldown[i].SlotEffect.style.backgroundImage = new StyleBackground(cooltimeImages[imageIndex]);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

#if UNITY_EDITOR
    private void DebugData()
    {
        StartCoroutine(DelayDebug());
    }

    private IEnumerator DelayDebug()
    {
        yield return new WaitForSeconds(1);

        List<Shortcut> shortcuts = new List<Shortcut>();
        for (int i = 0; i < PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR; i++)
        {
            Shortcut shortcut = new Shortcut(i, 0, Shortcut.TYPE_ACTION, (int)ActionType.Attack, -1);
            shortcuts.Add(shortcut);
        }
        for (int i = 0; i < PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR; i++)
        {
            Shortcut shortcut = new Shortcut(i, 1, Shortcut.TYPE_ACTION, (int)ActionType.Pickup, -1);
            shortcuts.Add(shortcut);
        }

        PlayerShortcuts.Instance.SetShortcutList(shortcuts);

        //UpdateShortcuts(shortcuts);
    }
#endif

}
