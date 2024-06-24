using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatWindow : MonoBehaviour {
    private VisualTreeAsset _chatWindowTemplate;
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private TextField _chatInput;
    private VisualElement _chatInputContainer;
    private VisualElement _chatTabView;
    private VisualElement _chatWindowEle;
    private ChatTab _activeTab;

    [SerializeField] private float _chatWindowMinWidth = 225.0f;
    [SerializeField] private float _chatWindowMaxWidth = 500.0f;
    [SerializeField] private float _chatWindowMinHeight = 175.0f;
    [SerializeField] private float _chatWindowMaxHeight = 600.0f;
    [SerializeField] public List<ChatTab> _tabs;
    [SerializeField] private bool _chatOpened = false;
    [SerializeField] private int _chatInputCharacterLimit = 64;

    public bool ChatOpened { get { return _chatOpened; } }

    private static ChatWindow _instance;
    public static ChatWindow Instance { get { return _instance; } }

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
        if(_chatWindowTemplate == null) {
            _chatWindowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ChatWindow");
        }
        if(_chatWindowTemplate == null) {
            Debug.LogError("Could not load chat window template.");
        }
        if(_tabTemplate == null) {
            _tabTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ChatTab");
        }
        if(_tabTemplate == null) {
            Debug.LogError("Could not load chat tab template.");
        }
        if (_tabHeaderTemplate == null) {
            _tabHeaderTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ChatTabHeader");
        }
        if (_tabHeaderTemplate == null) {
            Debug.LogError("Could not load chat tab header template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(_chatWindowTemplate == null) {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {

        _chatWindowEle = _chatWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(_chatWindowEle);
        _chatWindowEle.AddManipulator(mouseOverDetection);

        var diagonalResizeHandle = _chatWindowEle.Q<VisualElement>(null, "resize-diag");

        DiagonalResizeManipulator diagonalResizeManipulator = new DiagonalResizeManipulator(
            diagonalResizeHandle,
            _chatWindowEle,
            _chatWindowMinWidth,
            _chatWindowMaxWidth,
            _chatWindowMinHeight,
            _chatWindowMaxHeight,
            14.5f,
            2f);

        diagonalResizeHandle.AddManipulator(diagonalResizeManipulator);

        _chatInput = _chatWindowEle.Q<TextField>("ChatInputField");
        _chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
        _chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
        _chatInput.maxLength = _chatInputCharacterLimit;

        var enlargeTextBtn = _chatWindowEle.Q<Button>("EnlargeTextBtn");
        enlargeTextBtn.AddManipulator(new ButtonClickSoundManipulator(_chatInput));

        var chatOptionsBtn = _chatWindowEle.Q<Button>("ChatOptionsBtn");
        chatOptionsBtn.AddManipulator(new ButtonClickSoundManipulator(_chatInput));

        _chatInput.AddManipulator(new BlinkingCursorManipulator(_chatInput));

        _chatInputContainer = _chatWindowEle.Q<VisualElement>("InnerBar");

        CreateTabs();

        root.Add(_chatWindowEle);

        yield return new WaitForEndOfFrame();
        diagonalResizeManipulator.SnapSize();
    }


    private void CreateTabs() {
        _chatTabView = _chatWindowEle.Q<VisualElement>("ChatTabView");

        VisualElement tabHeaderContainer = _chatTabView.Q<VisualElement>("tab-header-container");
        if(tabHeaderContainer == null) {
            Debug.LogError("tab-header-container is null");
        }
        VisualElement tabContainer = _chatTabView.Q<VisualElement>("tab-content-container");

        if (tabContainer == null) {
            Debug.LogError("tab-content-container");
        }

        for (int i = 0; i < _tabs.Count; i++) {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            // tabElement.name = _tabs[i].TabName;
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderContainer.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_chatWindowEle, tabElement, tabHeaderElement);
        }

        if (_tabs.Count > 0) {
            SwitchTab(_tabs[0]);
        }
    }

    public bool SwitchTab(ChatTab switchTo) {
        if (_activeTab != switchTo) {
            if(_activeTab != null) {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            ScrollDown(switchTo.Scroller);

            _activeTab = switchTo;
            return true;
        }

        return false;
    }

    void Update() {
        if(InputManager.Instance.IsInputPressed(InputType.SendMessage)) {
            if(_chatOpened) {
                CloseChat(true);
            } else {
                StartCoroutine(OpenChat());
            }
        }

        if(InputManager.Instance.IsInputPressed(InputType.Escape)) {
            if(_chatOpened) {
                CloseChat(false);
            }
        }
    }

    IEnumerator OpenChat() {
        _chatOpened = true;
        L2GameUI.Instance.BlurFocus();
        yield return new WaitForEndOfFrame();
        _chatInput.Focus();
    }

    public void CloseChat(bool sendMessage) {
        _chatOpened = false;

        L2GameUI.Instance.BlurFocus();

        if(sendMessage) {
            if(_chatInput.text.Length > 0) {
                SendChatMessage(_chatInput.text);
                _chatInput.value = "";
            }
        }
    }

    private void OnChatInputFocus(FocusEvent evt) {
        if(!_chatInputContainer.ClassListContains("highlighted")) {
            _chatInputContainer.AddToClassList("highlighted");
        }

        if(!_chatOpened) {
            _chatOpened = true;
        }
    }

    private void OnChatInputBlur(BlurEvent evt) {
        if(_chatInputContainer.ClassListContains("highlighted")) {
            _chatInputContainer.RemoveFromClassList("highlighted");
        }

        if(_chatOpened) {
            _chatOpened = false;
        }
    }

    public void ClearChat() {
        for(int i = 0; i < _tabs.Count; i++) {
            ClearTab(i);
        }
    }

    public void ClearTab(int tabIndex) {
        if(tabIndex <= _tabs.Count - 1) {
            _tabs[tabIndex].Content.text = "";
        }
    }

 
    public void SendChatMessage(string text) {
        if(World.Instance.OfflineMode) {
            Message message = new ChatMessage(PlayerEntity.Instance.Identity.Name, text);
            ReceiveChatMessage(message);
        } else {
            GameClientPacketHandler.Instance.SendMessage(text);
        }
    }

    public void ReceiveChatMessage(Message message) {
        if(message == null) {
            return;
        }

        for(int i = 0; i < _tabs.Count; i++) {
            if(_tabs[i].FilteredMessages.Count > 0) {
                if(_tabs[i].FilteredMessages.Contains(message.MessageType)) {
                    ConcatMessage(_tabs[i].Content, message.ToString());
                }
            }
        }
        
    }

    private void ConcatMessage(Label chatContent, string message) {
        if(chatContent.text.Length > 0) {
            chatContent.text += "\r\n";
        }
        chatContent.text += message;
    }

    internal void ScrollDown(Scroller scroller) {
        StartCoroutine(ScrollDownWithDelay(scroller));
    }

    IEnumerator ScrollDownWithDelay(Scroller scroller) {
        yield return new WaitForEndOfFrame();
        scroller.value = scroller.highValue > 0 ? scroller.highValue : 0;
    }
}