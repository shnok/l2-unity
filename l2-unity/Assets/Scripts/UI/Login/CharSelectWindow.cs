using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharSelectWindow : L2Window {
    private VisualTreeAsset _arrowInputTemplate;
    private ArrowInputManipulator _charNameManipulator;
    private static CharSelectWindow _instance;
    public static CharSelectWindow Instance { get { return _instance; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/CharSelectWindow"); 
        _arrowInputTemplate = LoadAsset("Data/UI/_Elements/Template/ArrowInput");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button loginButton = (Button) GetElementById("StartButton");
        loginButton.AddManipulator(new ButtonClickSoundManipulator(loginButton));
        loginButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        Button deleteButton = (Button)GetElementById("DeleteButton");
        deleteButton.AddManipulator(new ButtonClickSoundManipulator(deleteButton));
        deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());

        Button createButton = (Button)GetElementById("CreateButton");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = (Button)GetElementById("ReloginButton");
        reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
        reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());

        VisualElement userNameInputContainer = GetElementById("UserSelectContainer");
        VisualElement userNameInput = _arrowInputTemplate.Instantiate()[0];
        _charNameManipulator = new ArrowInputManipulator(userNameInput, "Name", new string[] { "Type A", "Type B", "Type C", "Type D", "Type E" }, -1, (index, value) => {
            
        });
        userNameInput.AddManipulator(_charNameManipulator);
        userNameInputContainer.Add(userNameInput);

    }

    public void SetUserList() {

    }

    public void SelectSlot(int slot) {

    }

    private void StartGamePressed() {
        CharacterSelector.Instance.ConfirmSelection();
    }

    private void ReLoginPressed() {
        GameManager.Instance.OnRelogin();
        GameClient.Instance.Disconnect();
    }

    private void CreatePressed() {
        GameManager.Instance.OnCreateUser();
    }

    private void DeletePressed() {

    }
}