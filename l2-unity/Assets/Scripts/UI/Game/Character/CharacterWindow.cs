using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfoWindow : L2Window
{
    public VisualElement minimal_panel;
    private VisualElement content;
    private ButtonCharacter _buttonCharacter;

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
    public static CharacterInfoWindow Instance {
        get { return _instance; }
    }
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            _buttonCharacter = new ButtonCharacter(this);
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/CharacterInfoWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        content = GetElementByClass("content");
        _buttonCharacter.RegisterButtonCloseWindow("btn-close-frame");
        _buttonCharacter.RegisterClickWindow(_windowEle, content, dragArea);

        // player
        _nameLabel = GetLabelById("CharacterNameLabel");
        _levelLabel = GetLabelById("LvlLabelName");
        _classLabel = GetLabelById("ClassLabelName");

        //bars
        _hpLabel = GetLabelById("HpLabel");
        _mpLabel = GetLabelById("MpLabel");
        _spLabel = GetLabelById("SpLabel");
        _expLabel = GetLabelById("ExpLabel");
        _weightLabel = GetLabelById("WeightLabel");
        _cpLabel = GetLabelById("CpLabel");
        _hpBar = GetElementById("HpGauge");
        _hpBarBg = GetElementById("HpBg");
        _mpBar = GetElementById("MpGauge");
        _mpBarBg = GetElementById("MpBg");
        _cpBar = GetElementById("CpGauge");
        _cpBarBg = GetElementById("CpBg");
        _weightBar = GetElementById("WeightGauge");
        _weightBarBg = GetElementById("WeightBg");

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
    }
}
