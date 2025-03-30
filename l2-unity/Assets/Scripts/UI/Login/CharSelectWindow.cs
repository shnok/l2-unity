using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class CharSelectWindow : L2Window
{
    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator _charNameManipulator;
    private List<CharSelectionInfoPackage> _characters;

    private Label _levelLabel;
    private Label _classLabel;
    private Label _hpLabel;
    private Label _mpLabel;
    private Label _expLabel;
    private Label _spLabel;
    private Label _karmaLabel;

    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _MPBar;
    private VisualElement _MPBarBG;
    private VisualElement _ExpBarBG;
    private VisualElement _ExpBar;

    private static CharSelectWindow _instance;
    public static CharSelectWindow Instance { get { return _instance; } }

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

    private void Update()
    {
        if (!_isWindowHidden)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance.PlayUISound("click_01");
                ReLoginPressed();
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                AudioManager.Instance.PlayUISound("click_01");
                StartGamePressed();
            }
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharSelectWindow/CharSelectWindow");
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Components/L2ArrowInput/L2ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button loginButton = (Button)GetElementById("StartButton").Q<Button>("L2Button");
        loginButton.AddManipulator(new ButtonClickSoundManipulator(loginButton));
        loginButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        Button deleteButton = (Button)GetElementById("DeleteButton").Q<Button>("L2Button");
        deleteButton.AddManipulator(new ButtonClickSoundManipulator(deleteButton));
        deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());

        Button createButton = (Button)GetElementById("CreateButton").Q<Button>("L2Button");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = (Button)GetElementById("ReloginButton").Q<Button>("L2Button");
        reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
        reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());

        VisualElement userNameInputContainer = GetElementById("UserSelectContainer");
        VisualElement userNameInput = _arrowInputTemplate.Instantiate()[0];
        _charNameManipulator = new ArrowInputManipulator(userNameInput, "Name", new string[] { }, -1, (index, value) =>
        {
            CharacterSelector.Instance.SelectCharacter(index);
        });

        _levelLabel = (Label)GetElementById("LevelLabel");
        _classLabel = (Label)GetElementById("ClassLabel");
        _spLabel = (Label)GetElementById("SPLabel");
        _karmaLabel = (Label)GetElementById("KarmaLabel");

        VisualElement hpBarContainer = GetElementById("HPBar");
        _HPBar = hpBarContainer.Q<VisualElement>("Bar");
        _HPBarBG = hpBarContainer.Q<VisualElement>("BarBg");
        _hpLabel = hpBarContainer.Q<Label>("Text");


        VisualElement mpBarContainer = GetElementById("MPBar");
        _MPBar = mpBarContainer.Q<VisualElement>("Bar");
        _MPBarBG = mpBarContainer.Q<VisualElement>("BarBg");
        _mpLabel = mpBarContainer.Q<Label>("Text");

        VisualElement expBarContainer = GetElementById("EXPBar");
        _ExpBar = expBarContainer.Q<VisualElement>("Bar");
        _ExpBarBG = expBarContainer.Q<VisualElement>("BarBg");
        _expLabel = expBarContainer.Q<Label>("Text");

        userNameInput.AddManipulator(_charNameManipulator);
        userNameInputContainer.Add(userNameInput);

        L2LoginUI.Instance.WindowLoadComplete();
    }

    public void SetCharacterList(List<CharSelectionInfoPackage> characters)
    {
        _characters = characters;
        string[] charArray = characters.Select(character => character.Name).ToArray();
        _charNameManipulator.UpdateValues(charArray);
    }

    public void SelectSlot(int slot)
    {
        if (slot == _charNameManipulator.Index)
        {
            return;
        }

        if (slot == -1)
        {
            _levelLabel.text = "0";
            _classLabel.text = "??";
            _hpLabel.text = "0/0";
            _mpLabel.text = "0/0";
            _expLabel.text = "0.00%";
            _spLabel.text = "0";
            _karmaLabel.text = "0";
            _charNameManipulator.ClearInput();
        }
        else if (slot < _characters.Count)
        {
            _levelLabel.text = _characters[slot].PlayerStats.Level.ToString();
            _classLabel.text = ((CharacterClass)(_characters[slot].ClassId)).ToString();
            _hpLabel.text = $"{_characters[slot].PlayerStatus.Hp}/{_characters[slot].PlayerStats.MaxHp}";
            _mpLabel.text = $"{_characters[slot].PlayerStatus.Mp}/{_characters[slot].PlayerStats.MaxMp}";
            _expLabel.text = (_characters[slot].ExpPercent * 100).ToString("0.00") + "%";
            _spLabel.text = _characters[slot].Sp.ToString();
            _karmaLabel.text = _characters[slot].Karma.ToString();

            StartCoroutine(UpdateBars(slot));

            _charNameManipulator.SelectIndex(slot);
        }
    }

    IEnumerator UpdateBars(int slot)
    {

        yield return new WaitForEndOfFrame();
        if (_HPBarBG != null && _HPBar != null)
        {
            float hpRatio = (float)_characters[slot].PlayerStatus.Hp / _characters[slot].PlayerStats.MaxHp;
            float bgWidth = _HPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * hpRatio;
            _HPBar.style.width = barWidth;
        }

        if (_MPBarBG != null && _MPBar != null)
        {
            float mpRatio = (float)_characters[slot].PlayerStatus.Mp / _characters[slot].PlayerStats.MaxMp;
            float bgWidth = _MPBarBG.resolvedStyle.width;
            float barWidth = bgWidth * mpRatio;
            _MPBar.style.width = barWidth;
        }

        if (_ExpBarBG != null && _ExpBar != null)
        {
            float bgWidth = _ExpBarBG.resolvedStyle.width;
            float barWidth = bgWidth * _characters[slot].ExpPercent;
            _ExpBar.style.width = barWidth;
        }
    }

    private void StartGamePressed()
    {
        L2ConfirmWindow.Instance.HideWindow(false);
        CharacterSelector.Instance.ConfirmSelection();
    }

    private void ReLoginPressed()
    {
        L2ConfirmWindow.Instance.HideWindow(false);
        GameClient.Instance.Disconnect();
    }

    private void CreatePressed()
    {
        L2ConfirmWindow.Instance.HideWindow(false);
        GameManager.Instance.ChangeState(GameState.CHAR_CREATION);
    }

    private void DeletePressed()
    {
        CharacterSelector.Instance.DeleteCharacter();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();

        if (GameManager.Instance.AutoLogin)
        {
            StartCoroutine(AutoLogin());
        }
    }

    private IEnumerator AutoLogin()
    {
        yield return new WaitForSeconds(1f);
        StartGamePressed();
    }
}