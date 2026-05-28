using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfoWindow : L2PopupWindow
{
    // player
    private Label _nameLabel;
    private Label _levelLabel;
    private Label _classLabel;

    //bars
    private Label _hpLabel;
    private Label _mpLabel;
    private Label _spLabel;
    private Label _expLabel;
    private Label _weightLabel;
    private Label _cpLabel;
    private VisualElement _hpBar;
    private VisualElement _hpBarBg;
    private VisualElement _mpBar;
    private VisualElement _mpBarBg;
    private VisualElement _cpBar;
    private VisualElement _cpBarBg;
    private VisualElement _weightBar;
    private VisualElement _weightBarBg;
    private VisualElement _expBar;
    private VisualElement _expBarBg;
    private VisualElement _weightBarContainer;

    //combat
    private Label _patkLabel;
    private Label _pdefLabel;
    private Label _paccLabel;
    private Label _pevaLabel;
    private Label _pcritLabel;
    private Label _patkspdLabel;
    private Label _speedLabel;
    private Label _matkLabel;
    private Label _mdefLabel;
    private Label _maccuracyLabel;
    private Label _mevasionLabel;
    private Label _mcritLabel;
    private Label _castspeedLabel;

    //stats
    private Label _strLabel;
    private Label _intLabel;
    private Label _dexLabel;
    private Label _witLabel;
    private Label _conLabel;
    private Label _menLabel;

    //social
    private Label _repLabel;
    private Label _pvpLabel;
    private Label _recLabel;
    private Label _raidLabel;


    private static CharacterInfoWindow _instance;
    public static CharacterInfoWindow Instance
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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/CharacterInfoWindow/CharacterInfoWindow");
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

        _windowEle.style.left = new Length(50, LengthUnit.Percent);
        _windowEle.style.top = new Length(50, LengthUnit.Percent);
        _windowEle.style.translate = new StyleTranslate(new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent)));

        Label _windowName = (Label)GetElementById("windows-name-label");
        _windowName.text = "Character Status";

        // player
        _nameLabel = GetLabelById("CharacterNameLabel");
        _levelLabel = GetLabelById("LvlLabelName");
        _classLabel = GetLabelById("ClassLabelName");

        //bars
        VisualElement HPBarContainer = GetElementById("HPBar");
        VisualElement MPBarContainer = GetElementById("MPBar");
        VisualElement CPBarContainer = GetElementById("CPBar");
        VisualElement EXPBarContainer = GetElementById("EXPBar");
        _weightBarContainer = GetElementById("WeightBar");

        _hpLabel = HPBarContainer.Q<Label>("Text");
        _mpLabel = MPBarContainer.Q<Label>("Text");
        _spLabel = GetLabelById("SpLabel");
        _expLabel = EXPBarContainer.Q<Label>("Text");
        _weightLabel = _weightBarContainer.Q<Label>("Text");
        _cpLabel = CPBarContainer.Q<Label>("Text");
        _hpBar = HPBarContainer.Q<VisualElement>("Bar");
        _hpBarBg = HPBarContainer.Q<VisualElement>("BarBg");
        _mpBar = MPBarContainer.Q<VisualElement>("Bar");
        _mpBarBg = MPBarContainer.Q<VisualElement>("BarBg");
        _cpBar = CPBarContainer.Q<VisualElement>("Bar");
        _cpBarBg = CPBarContainer.Q<VisualElement>("BarBg");
        _weightBar = _weightBarContainer.Q<VisualElement>("Bar");
        _weightBarBg = _weightBarContainer.Q<VisualElement>("BarBg");
        _expBar = EXPBarContainer.Q<VisualElement>("Bar");
        _expBarBg = EXPBarContainer.Q<VisualElement>("BarBg");

        //combat
        _patkLabel = GetLabelById("PAtkLabel");
        _pdefLabel = GetLabelById("PDefLabel");
        _paccLabel = GetLabelById("PAccuracyLabel");
        _pevaLabel = GetLabelById("PEvasionLabel");
        _pcritLabel = GetLabelById("PCriticalLabel");
        _patkspdLabel = GetLabelById("PAtkSpdLabel");
        _speedLabel = GetLabelById("PSpeedLabel");
        _matkLabel = GetLabelById("MAtkLabel");
        _mdefLabel = GetLabelById("MDefLabel");
        _maccuracyLabel = GetLabelById("MAccuracyLabel");
        _mevasionLabel = GetLabelById("MEvasionLabel");
        _mcritLabel = GetLabelById("MCriticalLabel");
        _castspeedLabel = GetLabelById("MCastingLabel");

        //stats
        _strLabel = GetLabelById("StrLabel");
        _intLabel = GetLabelById("IntLabel");
        _dexLabel = GetLabelById("DEXLabel");
        _witLabel = GetLabelById("WITLabel");
        _conLabel = GetLabelById("CONLabel");
        _menLabel = GetLabelById("MENLabel");

        //social
        _repLabel = GetLabelById("RepLabel");
        _pvpLabel = GetLabelById("PvpLabel");
        _recLabel = GetLabelById("RecLabel");
        _raidLabel = GetLabelById("RaidLabel");

        L2GameUI.Instance.WindowLoadComplete();
    }

    public void UpdateValues()
    {
        PlayerEntity player = PlayerEntity.Instance;

        if (player == null)
        {
            Debug.LogWarning("Player entity is null, can't update character window");
            return;
        }

        StartCoroutine(UpdateWithDelay(player));
    }

    private IEnumerator UpdateWithDelay(PlayerEntity player)
    {
        yield return new WaitForSeconds(0.2f);

        UpdatePlayer(player.Identity, (PlayerStats)player.Stats);

        UpdateStats((PlayerStats)player.Stats);

        UpdateCombatValues(player.Running, (PlayerStats)player.Stats);

        UpdateBars((PlayerStatus)player.Status, (PlayerStats)player.Stats);

        UpdateSocial((PlayerStats)player.Stats);
    }

    private void UpdatePlayer(NetworkIdentity identity, PlayerStats stats)
    {
        _nameLabel.text = identity.Name;
        _classLabel.text = ((CharacterClass)(identity.PlayerClass)).ToString();
        _levelLabel.text = stats.Level.ToString();

    }
    private void UpdateStats(PlayerStats stats)
    {
        _strLabel.text = stats.Str.ToString();
        _intLabel.text = stats.Int.ToString();
        _dexLabel.text = stats.Dex.ToString();
        _witLabel.text = stats.Wit.ToString();
        _conLabel.text = stats.Con.ToString();
        _menLabel.text = stats.Men.ToString();
    }

    private void UpdateCombatValues(bool running, PlayerStats stats)
    {
        _patkLabel.text = stats.PAtk.ToString();
        _pdefLabel.text = stats.PDef.ToString();
        _paccLabel.text = stats.PAccuracy.ToString();
        _pevaLabel.text = stats.PEvasion.ToString();
        _pcritLabel.text = stats.PCritical.ToString();
        _patkspdLabel.text = stats.PAtkSpd.ToString();
        _speedLabel.text = running ? stats.RunSpeed.ToString() : stats.WalkSpeed.ToString();

        _matkLabel.text = stats.MAtk.ToString();
        _mdefLabel.text = stats.MDef.ToString();
        _maccuracyLabel.text = stats.MAccuracy.ToString();
        _mevasionLabel.text = stats.MEvasion.ToString();
        _mcritLabel.text = stats.MCritical.ToString();
        _castspeedLabel.text = stats.MAtkSpd.ToString();
    }

    private void UpdateSocial(PlayerStats stats)
    {
        _repLabel.text = stats.Karma.ToString();
        _pvpLabel.text = $"{stats.PvpKills} / {stats.PkKills}";
        _recLabel.text = "0 / 0";
        _raidLabel.text = "0";
    }

    private void UpdateBars(PlayerStatus status, PlayerStats stats)
    {
        _hpLabel.text = $"{status.Hp}/{stats.MaxHp}";
        _mpLabel.text = $"{status.Mp}/{stats.MaxMp}";
        _cpLabel.text = $"{status.Cp}/{stats.MaxCp}";
        _spLabel.text = stats.Sp.ToString();

        if (stats.ExpPercent > 0)
        {
            _expLabel.text = $"{(stats.ExpPercent * 100f).ToString("0.00")}%";
        }
        else
        {
            _expLabel.text = $"00.00%";
        }

        if (stats.CurrWeight > 0)
        {
            _weightLabel.text = $"{((float)stats.CurrWeight / stats.MaxWeight * 100f).ToString("0.00")}%";
        }
        else
        {
            _weightLabel.text = $"00.00%";
        }

        if (_hpBarBg != null && _hpBar != null)
        {
            float hpRatio = Math.Min(1, (float)status.Hp / stats.MaxHp);
            float bgWidth = _hpBarBg.resolvedStyle.width;
            float barWidth = bgWidth * hpRatio;
            if (stats.MaxHp == 0)
            {
                barWidth = 0;
            }
            _hpBar.style.width = barWidth;
        }

        if (_mpBarBg != null && _mpBar != null)
        {
            float mpRatio = Math.Min(1, (float)status.Mp / stats.MaxMp);
            float bgWidth = _mpBarBg.resolvedStyle.width;
            float barWidth = bgWidth * mpRatio;
            if (stats.MaxMp == 0)
            {
                barWidth = 0;
            }
            _mpBar.style.width = barWidth;
        }

        if (_cpBarBg != null && _cpBar != null)
        {
            float bgWidth = _cpBarBg.resolvedStyle.width;
            float cpRatio = Math.Min(1, (float)status.Cp / stats.MaxCp);
            float barWidth = bgWidth * cpRatio;
            if (stats.MaxCp == 0)
            {
                barWidth = 0;
            }
            _cpBar.style.width = barWidth;
        }

        if (_expBarBg != null && _expBar != null)
        {
            float bgWidth = _expBarBg.resolvedStyle.width;
            float expRatio = stats.ExpPercent;
            float barWidth = bgWidth * expRatio;
            if (expRatio == 0)
            {
                barWidth = 0;
            }
            _expBar.style.width = barWidth;
        }

        if (_weightBarBg != null && _weightBar != null)
        {
            float bgWidth = _weightBarBg.resolvedStyle.width;
            float weightRatio = Math.Min(1, (float)stats.CurrWeight / stats.MaxWeight);

            for (int i = 1; i <= 5; i++)
            {
                _weightBarContainer.RemoveFromClassList("weight-" + i);
            }

            _weightBarContainer.AddToClassList("weight-" + ((int)Mathf.Floor(weightRatio / 0.25f) + 1));

            float barWidth = bgWidth * weightRatio;
            if (stats.MaxWeight == 0)
            {
                barWidth = 0;
            }
            _weightBar.style.width = barWidth;
        }

    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        AudioManager.Instance.PlayUISound("charstat_open_01");
        L2GameUI.Instance.WindowOpened(this);
        UpdateValues();
    }

    public override void HideWindow(bool silent)
    {
        base.HideWindow(silent);

        if (!silent)
            AudioManager.Instance.PlayUISound("charstat_close_01");

        L2GameUI.Instance.WindowClosed(this);
    }
}
