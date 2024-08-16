using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CharCreationWindow : L2Window {
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

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharCreationWindow");
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        userInputField = (TextField) GetElementById("UserInputField");
        userInputField.AddManipulator(new BlinkingCursorManipulator(userInputField));

        Button createButton = (Button) GetElementById("CreateButton");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreateButtonPressed());

        Button previousButton = (Button)GetElementById("PreviousButton");
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

        pawnRotateLeftButton.RegisterCallback<PointerDownEvent>((evt) => {
            CharacterCreator.Instance.RotatePawn(true);
        }, TrickleDown.TrickleDown);

        pawnRotateRightButton.RegisterCallback<PointerDownEvent>((evt) => {
            CharacterCreator.Instance.RotatePawn(false);
        }, TrickleDown.TrickleDown);

        pawnRotateLeftButton.RegisterCallback<PointerUpEvent>((evt) => {
            CharacterCreator.Instance.StopRotatingPawn();
        });

        pawnRotateRightButton.RegisterCallback<PointerUpEvent>((evt) => {
            CharacterCreator.Instance.StopRotatingPawn();
        });

        pawnZoominButton.RegisterCallback<ClickEvent>((evt) => {
            ToggleZoomin(false);
        });

        VisualElement raceInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement classInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement genderInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairstyleInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement hairColorInput = _arrowInputTemplate.Instantiate()[0];
        VisualElement faceInput = _arrowInputTemplate.Instantiate()[0];


        hairstyleManipulator = new ArrowInputManipulator(hairstyleInput, "Hairstyle", new string[] { "Type A", "Type B", "Type C", "Type D", "Type E" }, -1, (index, value) => {
            if (CharacterCreator.Instance.PawnIndex == -1) {
                hairstyleManipulator.ClearInput();
                return;
            }
        });
        hairstyleInput.AddManipulator(hairstyleManipulator);

        hairColorManipulator = new ArrowInputManipulator(hairColorInput, "Hair Color", new string[] { "Type A", "Type B", "Type C", "Type D" }, -1, (index, value) => {
            if (CharacterCreator.Instance.PawnIndex == -1) {
                hairColorManipulator.ClearInput();
                return;
            }
        });
        hairColorInput.AddManipulator(hairColorManipulator);

        faceManipulator = new ArrowInputManipulator(faceInput, "Face", new string[] { "Type A", "Type B", "Type C" }, -1, (index, value) => {
            if(CharacterCreator.Instance.PawnIndex == -1) {
                faceManipulator.ClearInput();
                return;
            }
        });
        faceInput.AddManipulator(faceManipulator);

        genderManipulator = new ArrowInputManipulator(genderInput, "Gender", new string[] { "Male", "Female" }, -1, (index, value) => {
            if (classManipulator.Value == "") {
                genderManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCameraManager.Instance.SelectGenderCamera(raceManipulator.Value, classManipulator.Value, value);
            if (cam != null) {
                LoginCameraManager.Instance.SwitchCamera(cam);
            }

            hairstyleManipulator.ResetInput();
            hairColorManipulator.ResetInput();
            faceManipulator.ResetInput();

            ShowRotatePawnWindow();
            CharacterCreator.Instance.ResetPawnSelection();
            CharacterCreator.Instance.SelectPawn(raceManipulator.Value, classManipulator.Value, value);
        });
        genderInput.AddManipulator(genderManipulator);

        classManipulator = new ArrowInputManipulator(classInput, "Class", new string[] { "Fighter", "Mystic" }, -1, (index, value) => {
            if(raceManipulator.Value == "Dwarf" && value == "Mystic") {
                classManipulator.ResetInput();
                return;
            }

            if (raceManipulator.Value == "") {
                classManipulator.ClearInput();
                return;
            }

            Camera cam = LoginCameraManager.Instance.SelectClassCamera(raceManipulator.Value, value);
            if (cam != null) {
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

        raceManipulator = new ArrowInputManipulator(raceInput, "Race", new string[] { "Human", "Elf", "Dark Elf", "Orc", "Dwarf" }, -1, (index, value) => {
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
    }


    private void CreateButtonPressed() {
    }

    private void PreviousButtonPressed() {
        classManipulator.ClearInput();
        genderManipulator.ClearInput();
        hairstyleManipulator.ClearInput();
        hairColorManipulator.ClearInput();
        faceManipulator.ClearInput();
        raceManipulator.ClearInput();
        userInputField.value = "";

        GameManager.Instance.OnAuthAllowed();
    }

    private void ShowRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.Flex;

        if (pawnZoominButton.ClassListContains("toggle")) {
            pawnZoominButton.RemoveFromClassList("toggle");
        }
    }

    private void HideRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.None;
    }

    private void ToggleZoomin(bool removeOnly) {
        if(pawnZoominButton.ClassListContains("toggle")) {
            LoginCameraManager.Instance.ZoomOut();

            pawnZoominButton.RemoveFromClassList("toggle");
        } else {

            LoginCameraManager.Instance.ZoomIn();

            pawnZoominButton.AddToClassList("toggle");
        }
    }
}