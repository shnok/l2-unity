using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharSelectWindow : MonoBehaviour {
    private VisualTreeAsset _windowTemplate;
    private VisualElement _windowEle;

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

    void Start() {
        LoadAssets();
    }

    private void LoadAssets() {
        if (_windowTemplate == null) {
            _windowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Login/CharSelectWindow");
        }
        if (_windowTemplate == null) {
            Debug.LogError("Could not load char select window template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if (_windowTemplate == null) {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {
        _windowEle = _windowTemplate.Instantiate()[0];

        Button loginButton = GetButtonById("StartButton");
        loginButton.AddManipulator(new ButtonClickSoundManipulator(loginButton));
        loginButton.RegisterCallback<ClickEvent>(evt => StartGamePressed());

        Button deleteButton = GetButtonById("DeleteButton");
        deleteButton.AddManipulator(new ButtonClickSoundManipulator(deleteButton));
        deleteButton.RegisterCallback<ClickEvent>(evt => DeletePressed());

        Button createButton = GetButtonById("CreateButton");
        createButton.AddManipulator(new ButtonClickSoundManipulator(createButton));
        createButton.RegisterCallback<ClickEvent>(evt => CreatePressed());

        Button reloginButton = GetButtonById("ReloginButton");
        reloginButton.AddManipulator(new ButtonClickSoundManipulator(reloginButton));
        reloginButton.RegisterCallback<ClickEvent>(evt => ReLoginPressed());

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

    private Button GetButtonById(string id) {
        var btn = _windowEle.Q<Button>(id);
        if (btn == null) {
            Debug.LogError(id + " can't be found.");
            return null;
        }

        return btn;
    }

    private void StartGamePressed() {
        GameManager.Instance.OnCharacterSelect();
    }

    private void ReLoginPressed() {
        GameManager.Instance.OnRelogin();
        DefaultClient.Instance.Disconnect();
    }

    private void CreatePressed() {

    }

    private void DeletePressed() {

    }

    public void HideWindow() {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowWindow() {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}