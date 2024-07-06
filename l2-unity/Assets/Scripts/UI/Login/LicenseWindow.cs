using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LicenseWindow : L2Window
{
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

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/LicenseWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button agreeButton = (Button)GetElementById("AgreeButton");
        agreeButton.AddManipulator(new ButtonClickSoundManipulator(agreeButton));
        agreeButton.RegisterCallback<ClickEvent>(evt => AgreeButtonPressed());

        Button disagreeButton = (Button)GetElementById("DisagreeButton");
        disagreeButton.AddManipulator(new ButtonClickSoundManipulator(disagreeButton));
        disagreeButton.RegisterCallback<ClickEvent>(evt => DisagreeButtonPressed());

        var highBtn = (RepeatButton)GetElementById("unity-high-button");
        var lowBtn = (RepeatButton)GetElementById("unity-low-button");
        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));
    }

    private void AgreeButtonPressed() {
        LoginClient.Instance.ClientPacketHandler.SendRequestServerList();
    }

    private void DisagreeButtonPressed() {
        LoginClient.Instance.Disconnect();
    }
}
