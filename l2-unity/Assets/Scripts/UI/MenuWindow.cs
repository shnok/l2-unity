using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class MenuWindow : MonoBehaviour {

    private VisualTreeAsset _windowTemplate;
    private VisualElement _windowEle;

    private static MenuWindow _instance;
    public static MenuWindow Instance { get { return _instance; } }

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
            _windowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/MenuWindow");
        }
        if (_windowTemplate == null) {
            Debug.LogError("Could not load menu window template.");
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
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(_windowEle);
        _windowEle.AddManipulator(mouseOverDetection);

        var charBtn = _windowEle.Q<Button>("CharacterButton");
        charBtn.AddManipulator(new ButtonClickSoundManipulator(charBtn));

        var inventoryBtn = _windowEle.Q<Button>("InventoryButton");
        inventoryBtn.AddManipulator(new ButtonClickSoundManipulator(inventoryBtn));

        var actionBtn = _windowEle.Q<Button>("ActionButton");
        actionBtn.AddManipulator(new ButtonClickSoundManipulator(actionBtn));

        var skillBtn = _windowEle.Q<Button>("SkillButton");
        skillBtn.AddManipulator(new ButtonClickSoundManipulator(skillBtn));

        var questBtn = _windowEle.Q<Button>("QuestButton");
        questBtn.AddManipulator(new ButtonClickSoundManipulator(questBtn));

        var clanBtn = _windowEle.Q<Button>("ClanButton");
        clanBtn.AddManipulator(new ButtonClickSoundManipulator(clanBtn));

        var mapBtn = _windowEle.Q<Button>("MapButton");
        mapBtn.AddManipulator(new ButtonClickSoundManipulator(mapBtn));

        var sysBtn = _windowEle.Q<Button>("SystemMenuButton");
        sysBtn.AddManipulator(new ButtonClickSoundManipulator(sysBtn));
        sysBtn.RegisterCallback<ClickEvent>((evt) => GameClient.Instance.Disconnect());

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        var dragAreaEle = _windowEle.Q<VisualElement>(null, "drag-area");
        DragManipulator drag = new DragManipulator(dragAreaEle, _windowEle);
        dragAreaEle.AddManipulator(drag);
    }

    private void RegisterButtonCallBack(string buttonId) {
        var btn = _windowEle.Q<Button>(buttonId);
        if (btn == null) {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
            //TODO: open window
        }, TrickleDown.TrickleDown);
    }
}
