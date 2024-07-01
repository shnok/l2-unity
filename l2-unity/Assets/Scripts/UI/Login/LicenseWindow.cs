using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LicenseWindow : MonoBehaviour
{
    private VisualTreeAsset _windowTemplate;
    private VisualElement _windowEle;
    private VisualElement _logo;
    private TextField _userInput;
    private TextField _passwordInput;

    private static LicenseWindow _instance;
    public static LicenseWindow Instance { get { return _instance; } }

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
            _windowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Login/LicenseWindow");
        }
        if (_windowTemplate == null) {
            Debug.LogError("Could not load license window template.");
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

        Button agreeButton = _windowEle.Q<Button>("AgreeButton");
        agreeButton.AddManipulator(new ButtonClickSoundManipulator(agreeButton));
        agreeButton.RegisterCallback<ClickEvent>(evt => AgreeButtonPressed());

        Button disagreeButton = _windowEle.Q<Button>("DisagreeButton");
        disagreeButton.AddManipulator(new ButtonClickSoundManipulator(disagreeButton));
        disagreeButton.RegisterCallback<ClickEvent>(evt => DisagreeButtonPressed());

        var highBtn = _windowEle.Q<RepeatButton>("unity-high-button");
        var lowBtn = _windowEle.Q<RepeatButton>("unity-low-button");
        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

    private void AgreeButtonPressed() {
        LoginClient.Instance.ClientPacketHandler.SendRequestServerList();
    }

    private void DisagreeButtonPressed() {
        LoginClient.Instance.Disconnect();
    }

    public void HideWindow() {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowWindow() {
        _windowEle.style.display = DisplayStyle.Flex;
    }
}
