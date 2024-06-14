using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginWindow : MonoBehaviour
{
    private VisualTreeAsset _windowTemplate;
    private VisualElement _windowEle;
    private TextField _userInput;
    private TextField _passwordInput;

    private static LoginWindow _instance;
    public static LoginWindow Instance { get { return _instance; } }

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
            _windowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/LoginWindow");
        }
        if (_windowTemplate == null) {
            Debug.LogError("Could not load login window template.");
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

        Button loginButton = GetButtonById("LoginButton");
        loginButton.AddManipulator(new ButtonClickSoundManipulator(loginButton));
        loginButton.RegisterCallback<ClickEvent>(evt => LoginButtonPressed());

        Button exitButton = GetButtonById("ExitButton");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>(evt => ExitButtonPressed());

        VisualElement userInputBg = _windowEle.Q<VisualElement>("UserInputBg");
        _userInput = _windowEle.Q<TextField>("UserInputField");
        _userInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _userInput));
        _userInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _userInput));
        _userInput.maxLength = 16;

        _userInput.AddManipulator(new HighlightedInputFieldManipulator(_userInput, userInputBg, 20));
        _userInput.AddManipulator(new BlinkingCursorManipulator(_userInput));

        VisualElement passwordInputBg = _windowEle.Q<VisualElement>("PasswordInputBg");
        _passwordInput = _windowEle.Q<TextField>("PasswordInputField");
        _passwordInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _passwordInput));
        _passwordInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _passwordInput));
        _passwordInput.maxLength = 16;

        _passwordInput.AddManipulator(new HighlightedInputFieldManipulator(_passwordInput, passwordInputBg, 20));
        _passwordInput.AddManipulator(new BlinkingCursorManipulator(_passwordInput));

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

    private void OnInputFocus(FocusEvent evt, VisualElement input) {
        if (!input.ClassListContains("highlighted")) {
            input.AddToClassList("highlighted");
        }
    }

    private void OnInputBlur(BlurEvent evt, VisualElement input) {
        if (input.ClassListContains("highlighted")) {
            input.RemoveFromClassList("highlighted");
        }
    }

    private Button GetButtonById(string id) {
        var btn = _windowEle.Q<Button>(id);
        if (btn == null) {
            Debug.LogError(id + " can't be found.");
            return null;
        }

        return btn;
    }

    private void LoginButtonPressed() {
        DefaultClient.Instance.Connect(StringUtils.GenerateRandomString());
    }

    private void ExitButtonPressed() {
        Application.Quit();
    }
}