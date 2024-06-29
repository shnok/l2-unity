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

        RegisterButtonCharacterCallBack("CharacterButton");
        RegisterButtonInventoryCallBack("InventoryButton");
        RegisterButtonCallBack("ActionButton");
        RegisterButtonCallBack("SkillButton");
        RegisterButtonCallBack("QuestButton");
        RegisterButtonCallBack("ClanButton");
        RegisterButtonCallBack("MapButton");
        RegisterButtonCallBack("SystemMenuButton");

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
           // HideInventory();
        }, TrickleDown.TrickleDown);
    }

    private void RegisterButtonInventoryCallBack(string buttonId)
    {
        var btn = _windowEle.Q<Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
            HideInventory();
        }, TrickleDown.TrickleDown);
    }

    private void RegisterButtonCharacterCallBack(string buttonId)
    {
        var btn = _windowEle.Q<Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
            HideCharacter();
        }, TrickleDown.TrickleDown);
    }

    private void HideInventory()
    {
        if (CharacterInventory.Instance.isHideWindow())
        {
            CharacterInventory.Instance.HideElements(false);
        }
        else
        {
            CharacterInventory.Instance.HideElements(true);
        }
    }

    private void HideCharacter()
    {
        bool hide = CharacterInfo.Instance.isHideWindow();
        if (hide)
        {
            CharacterInfo.Instance.HideElements(false);
        }
        else
        {
            CharacterInfo.Instance.HideElements(true);
        }
    }
}
