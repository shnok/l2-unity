using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class CharCreationWindow : L2Window
{
    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator hairstyleManipulator;
    private ArrowInputManipulator hairColorManipulator;
    private ArrowInputManipulator faceManipulator;
    private ArrowInputManipulator genderManipulator;
    private ArrowInputManipulator classManipulator;
    private ArrowInputManipulator raceManipulator;
    private TextField userInputField;
    private VisualElement pawnRotateWindow;
    private Button pawnZoominButton;

    private static CharCreationWindow _instance;
    public static CharCreationWindow Instance { get { return _instance; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharCreationWindow/CharCreationWindow");
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Components/L2ArrowInput/L2ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        userInputField = (TextField)GetElementById("UserInputField").Q<TextField>("L2Input");
        userInputField.AddManipulator(new BlinkingCursorManipulator(userInputField));
        userInputField.RegisterValueChangedCallback(evt => ValidateNewName(evt));

        Button createButton = (Button)GetElementById("CreateButton").Q<Button>("L2Button");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreateButtonPressed());

        Button previousButton = (Button)GetElementById("PreviousButton").Q<Button>("L2Button");
        previousButton.AddManipulator(new ButtonClickSoundManipulator(previousButton));
        previousButton.RegisterCallback<ClickEvent>(evt => PreviousButtonPressed());

        VisualElement charDetailWindow = GetElementById("CharCreationDetailWindow");

        pawnRotateWindow = GetElementById("CharacterRotateWindow");
        HideRotatePawnWindow();

        Button pawnRotateLeftButton = pawnRotateWindow.Q<Button>("TurnLeftButton");
        Button pawnRotateRightButton = pawnRotateWindow.Q<Button>("TurnRightButton");
        pawnZoominButton = pawnRotateWindow.Q<Button>("ZoomButton");

        pawnRotateLeftButton.AddManipulator(new ButtonClickSoundManipulator(pawnRotateLeftButton));
        pawnRotateRightButton.AddManipulator(new ButtonClickSoundManipulator(pawnRotateRightButton));
        pawnZoominButton.AddManipulator(new ButtonClickSoundManipulator(pawnZoominButton));

        pawnRotateLeftButton.RegisterCallback<PointerDownEvent>((evt) =>
        {
            CharacterCreator.Instance.RotatePawn(true);
        }, TrickleDown.TrickleDown);

        pawnRotateRightButton.RegisterCallback<PointerDownEvent>((evt) =>
        {
            CharacterCreator.Instance.RotatePawn(false);
        }, TrickleDown.TrickleDown);

        pawnRotateLeftButton.RegisterCallback<PointerUpEvent>((evt) =>
        {
            CharacterCreator.Instance.StopRotatingPawn();
        });

        pawnRotateRightButton.RegisterCallback<PointerUpEvent>((evt) =>
        {
            CharacterCreator.Instance.StopRotatingPawn();
        });

        pawnZoominButton.RegisterCallback<ClickEvent>((evt) =>
        {
            ToggleZoomin(false);
        });

        VisualElement raceInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement classInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement genderInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairstyleInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairColorInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement faceInput = _arrowInputTemplate.Instantiate()[0];


        hairstyleManipulator = new ArrowInputManipulator(hairstyleInput, SysStringTable.Instance.GetSysString(167).Name, new string[] {
            SysStringTable.Instance.GetSysString(179).Name,
            SysStringTable.Instance.GetSysString(180).Name,
            SysStringTable.Instance.GetSysString(181).Name,
            SysStringTable.Instance.GetSysString(182).Name,
            SysStringTable.Instance.GetSysString(186).Name }, -1, (index, value) =>
        {
            if (CharacterCreator.Instance.PawnIndex == -1)
            {
                hairstyleManipulator.ClearInput();
                return;
            }

            CharacterCreator.Instance.ChangeCharacterHairStyle(index);
        });
        hairstyleInput.AddManipulator(hairstyleManipulator);

        hairColorManipulator = new ArrowInputManipulator(hairColorInput, SysStringTable.Instance.GetSysString(168).Name, new string[] {
            SysStringTable.Instance.GetSysString(179).Name,
            SysStringTable.Instance.GetSysString(180).Name,
            SysStringTable.Instance.GetSysString(181).Name,
            SysStringTable.Instance.GetSysString(182).Name }, -1, (index, value) =>
        {
            if (CharacterCreator.Instance.PawnIndex == -1)
            {
                hairColorManipulator.ClearInput();
                return;
            }

            CharacterCreator.Instance.ChangeCharacterHairColor(index);
        });
        hairColorInput.AddManipulator(hairColorManipulator);

        faceManipulator = new ArrowInputManipulator(faceInput, SysStringTable.Instance.GetSysString(169).Name, new string[] {
            SysStringTable.Instance.GetSysString(179).Name,
            SysStringTable.Instance.GetSysString(180).Name,
            SysStringTable.Instance.GetSysString(181).Name }, -1, (index, value) =>
        {
            if (CharacterCreator.Instance.PawnIndex == -1)
            {
                faceManipulator.ClearInput();
                return;
            }

            CharacterCreator.Instance.ChangeCharacterFace(index);
        });
        faceInput.AddManipulator(faceManipulator);

        genderManipulator = new ArrowInputManipulator(genderInput, SysStringTable.Instance.GetSysString(166).Name, new string[] {
            SysStringTable.Instance.GetSysString(177).Name,
            SysStringTable.Instance.GetSysString(178).Name }, -1, (index, value) =>
        {
            if (classManipulator.Value == "")
            {
                genderManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCameraManager.Instance.SelectGenderCamera(raceManipulator.Value, classManipulator.Index, index);
            if (cam != null)
            {
                LoginCameraManager.Instance.SwitchCamera(cam);
            }

            hairstyleManipulator.ResetInput();
            hairColorManipulator.ResetInput();
            faceManipulator.ResetInput();

            ShowRotatePawnWindow();
            CharacterCreator.Instance.ResetPawnSelection();
            CharacterCreator.Instance.SelectPawn(raceManipulator.Index, classManipulator.Index, index);
        });
        genderInput.AddManipulator(genderManipulator);

        classManipulator = new ArrowInputManipulator(classInput, SysStringTable.Instance.GetSysString(165).Name, new string[] {
            SysStringTable.Instance.GetSysString(175).Name,
            SysStringTable.Instance.GetSysString(176).Name }, -1, (index, value) =>
        {
            if (raceManipulator.Index == 4 && index == 1)
            {
                classManipulator.ResetInput();
                return;
            }

            if (raceManipulator.Value == "")
            {
                classManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCameraManager.Instance.SelectClassCamera(raceManipulator.Value, index);
            Debug.LogWarning(raceManipulator.Value);
            Debug.LogWarning(value);
            Debug.LogWarning(cam);
            if (cam != null)
            {
                LoginCameraManager.Instance.SwitchCamera(cam);
            }

            genderManipulator.ClearInput();
            hairstyleManipulator.ClearInput();
            hairColorManipulator.ClearInput();
            faceManipulator.ClearInput();

            HideRotatePawnWindow();
            CharacterCreator.Instance.ResetPawnSelection();
        });
        classInput.AddManipulator(classManipulator);

        raceManipulator = new ArrowInputManipulator(raceInput, SysStringTable.Instance.GetSysString(164).Name, new string[] {
            SysStringTable.Instance.GetSysString(170).Name,
            SysStringTable.Instance.GetSysString(171).Name,
            SysStringTable.Instance.GetSysString(172).Name,
            SysStringTable.Instance.GetSysString(173).Name,
            SysStringTable.Instance.GetSysString(174).Name }, -1, (index, value) =>
        {
            LoginCameraManager.Instance.SwitchCamera(value);
            classManipulator.ClearInput();
            genderManipulator.ClearInput();
            hairstyleManipulator.ClearInput();
            hairColorManipulator.ClearInput();
            faceManipulator.ClearInput();

            HideRotatePawnWindow();
            CharacterCreator.Instance.ResetPawnSelection();
        });
        raceInput.AddManipulator(raceManipulator);

        raceManipulator.ClearInput();

        charDetailWindow.Add(raceInput);
        charDetailWindow.Add(classInput);
        charDetailWindow.Add(genderInput);
        charDetailWindow.Add(hairstyleInput);
        charDetailWindow.Add(hairColorInput);
        charDetailWindow.Add(faceInput);

        L2LoginUI.Instance.WindowLoadComplete();
    }

    private void ValidateNewName(ChangeEvent<string> evt)
    {
        var regex = new Regex(@"^[a-zA-Z0-9]{0,12}$");
        if (!regex.IsMatch(evt.newValue))
        {
            userInputField.value = evt.previousValue;
            return;
        }
    }

    private void CreateButtonPressed()
    {
        if (raceManipulator.Index == 3)
        {
            L2ConfirmWindow.Instance.ShowWindow("Orcs are not available yet.", () => { }, null);
            return;
        }

        if (userInputField.value.Length == 0)
        {
            L2ConfirmWindow.Instance.ShowWindow(205, () => { }, null);
            return;
        }

        if (CharacterCreator.Instance.PawnIndex == -1)
        {
            return;
        }

        CharacterCreator.Instance.ValidateCharacterCreation(userInputField.value, classManipulator.Index == 1);
    }

    private void ClearInputs()
    {
        classManipulator.ClearInput();
        genderManipulator.ClearInput();
        hairstyleManipulator.ClearInput();
        hairColorManipulator.ClearInput();
        faceManipulator.ClearInput();
        raceManipulator.ClearInput();
        userInputField.value = "";
        CharacterCreator.Instance.ResetPawnSelection();
    }

    private void PreviousButtonPressed()
    {
        ClearInputs();

        GameManager.Instance.NotifyEvent(GameEvent.RETURN);
    }

    private void ShowRotatePawnWindow()
    {
        pawnRotateWindow.style.display = DisplayStyle.Flex;

        if (pawnZoominButton.ClassListContains("toggle"))
        {
            pawnZoominButton.RemoveFromClassList("toggle");
        }
    }

    private void HideRotatePawnWindow()
    {
        pawnRotateWindow.style.display = DisplayStyle.None;
    }

    private void ToggleZoomin(bool removeOnly)
    {
        if (pawnZoominButton.ClassListContains("toggle"))
        {
            LoginCameraManager.Instance.ZoomOut();

            pawnZoominButton.RemoveFromClassList("toggle");
        }
        else
        {

            LoginCameraManager.Instance.ZoomIn();

            pawnZoominButton.AddToClassList("toggle");
        }
    }

    public override void ShowWindow()
    {
        base.ShowWindow();

        ClearInputs();
    }
}