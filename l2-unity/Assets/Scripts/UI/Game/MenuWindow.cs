using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class MenuWindow : L2Window {
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

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/MenuWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var charBtn = _windowEle.Q<Button>("CharacterButton");
        charBtn.AddManipulator(new ButtonClickSoundManipulator(charBtn));
        charBtn.RegisterCallback<ClickEvent>((evt) => CharacterInfoWindow.Instance.ToggleHideWindow());

        var inventoryBtn = _windowEle.Q<Button>("InventoryButton");
        inventoryBtn.AddManipulator(new ButtonClickSoundManipulator(inventoryBtn));
        inventoryBtn.RegisterCallback<ClickEvent>((evt) => InventoryWindow.Instance.ToggleHideWindow());

        var actionBtn = _windowEle.Q<Button>("ActionButton");
        actionBtn.AddManipulator(new ButtonClickSoundManipulator(actionBtn));
        actionBtn.RegisterCallback<ClickEvent>((evt) => ActionWindow.Instance.ToggleHideWindow());

        var skillBtn = _windowEle.Q<Button>("SkillButton");
        skillBtn.AddManipulator(new ButtonClickSoundManipulator(skillBtn));
        skillBtn.RegisterCallback<ClickEvent>((evt) => SkillLearn.Instance.ToggleHideWindow());

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
}
