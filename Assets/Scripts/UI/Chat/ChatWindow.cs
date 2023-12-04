using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatWindow : MonoBehaviour {
    [SerializeField] private VisualTreeAsset chatWindowTemplate;
    [SerializeField] private VisualTreeAsset tabTemplate;

    public float chatWindowMinWidth = 225.0f;
    public float chatWindowMaxWidth = 500.0f;
    public float chatWindowMinHeight = 175.0f;
    public float chatWindowMaxHeight = 600.0f;

    [SerializeField] public List<ChatTab> tabs;

    private TextField chatInput;
    private VisualElement chatInputContainer;
    private TabView chatTabView;
    private VisualElement chatWindowEle;

    public bool chatOpened = false;
    public bool autoscroll = true;

    [SerializeField] private int chatInputCharacterLimit = 64;

    private static ChatWindow instance;
    public static ChatWindow GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;

    }

    void Start() {
        LoadAssets();
    }

    private void LoadAssets() {
        if(chatWindowTemplate == null) {
            chatWindowTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ChatWindow");
        }
        if(chatWindowTemplate == null) {
            Debug.LogError("Could not load chat window template.");
        }
        if(tabTemplate == null) {
            tabTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ChatTab");
        }
        if(tabTemplate == null) {
            Debug.LogError("Could not load chat tab template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(chatWindowTemplate == null) {
            return;
        }
        StartCoroutine(BuildWindow(root));
    }

    IEnumerator BuildWindow(VisualElement root) {

        chatWindowEle = chatWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(chatWindowEle);
        chatWindowEle.AddManipulator(mouseOverDetection);

        var diagonalResizeHandle = chatWindowEle.Q<VisualElement>(null, "resize-diag");

        DiagonalResizeManipulator diagonalResizeManipulator = new DiagonalResizeManipulator(
            diagonalResizeHandle,
            chatWindowEle,
            chatWindowMinWidth,
            chatWindowMaxWidth,
            chatWindowMinHeight,
            chatWindowMaxHeight,
            14.5f,
            2f);

        diagonalResizeHandle.AddManipulator(diagonalResizeManipulator);

        chatInput = chatWindowEle.Q<TextField>("ChatInputField");
        chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
        chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
        chatInput.maxLength = chatInputCharacterLimit;

        var enlargeTextBtn = chatWindowEle.Q<Button>("EnlargeTextBtn");
        enlargeTextBtn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.GetInstance().PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
        var chatOptionsBtn = chatWindowEle.Q<Button>("ChatOptionsBtn");
        chatOptionsBtn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.GetInstance().PlayUISound("click_01");
        }, TrickleDown.TrickleDown);

        L2GameUI.BlinkingCursor(chatInput);

        chatInputContainer = chatWindowEle.Q<VisualElement>("InnerBar");

        CreateTabs();

        root.Add(chatWindowEle);

        yield return new WaitForEndOfFrame();
        diagonalResizeManipulator.SnapSize();
    }


    private void CreateTabs() {
        chatTabView = chatWindowEle.Q<TabView>("ChatTabView");
        
        foreach(var tab in tabs) {
            Tab t = (Tab) tabTemplate.CloneTree()[0][0];
            tab.Initialize(chatWindowEle, t);
            t.name = tab.tabName;
            t.label = tab.tabName;
          
            chatTabView.Add(t);
        }
    }

    void Update() {
        if(InputManager.GetInstance().IsInputPressed(InputType.SendMessage)) {
            if(chatOpened) {
                CloseChat(true);
            } else {
                StartCoroutine(OpenChat());
            }
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.Escape)) {
            if(chatOpened) {
                CloseChat(false);
            }
        }
    }

    IEnumerator OpenChat() {
        chatOpened = true;
        L2GameUI.GetInstance().BlurFocus();
        yield return new WaitForEndOfFrame();
        chatInput.Focus();
    }

    public void CloseChat(bool sendMessage) {
        chatOpened = false;

        L2GameUI.GetInstance().BlurFocus();

        if(sendMessage) {
            if(chatInput.text.Length > 0) {
                SendChatMessage(chatInput.text);
                chatInput.value = "";
            }
        }
    }

    private void OnChatInputFocus(FocusEvent evt) {
        if(!chatInputContainer.ClassListContains("highlighted")) {
            chatInputContainer.AddToClassList("highlighted");
        }

        if(!chatOpened) {
            chatOpened = true;
        }
    }

    private void OnChatInputBlur(BlurEvent evt) {
        if(chatInputContainer.ClassListContains("highlighted")) {
            chatInputContainer.RemoveFromClassList("highlighted");
        }

        if(chatOpened) {
            chatOpened = false;
        }
    }

    public void ClearChat() {
        for(int i = 0; i < tabs.Count; i++) {
            ClearTab(i);
        }
    }

    public void ClearTab(int tabIndex) {
        if(tabIndex <= tabs.Count - 1) {
            tabs[tabIndex].content.text = "";
        }
    }

 
    public void SendChatMessage(string text) {
        if(World.GetInstance().offlineMode) {
            Message message = new ChatMessage(PlayerEntity.GetInstance().Identity.Name, text);
            ReceiveChatMessage(message);
        } else {
            DefaultClient.GetInstance().SendChatMessage(text);
        }
    }

    public void ReceiveChatMessage(Message message) {
        if(message == null) {
            return;
        }

        for(int i = 0; i < tabs.Count; i++) {
            if(tabs[i].filteredMessages.Count > 0) {
                if(tabs[i].filteredMessages.Contains(message.messageType)) {
                    ConcatMessage(tabs[i].content, message.ToString());
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