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

    public bool chatOpened = false;
    public bool offlineChat = true;
    public bool autoscroll = true;

    private static ChatWindow instance;
    public static ChatWindow GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    void Start() {
        if(chatWindowTemplate == null) {
            chatWindowTemplate = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Data/UI/_Elements/ChatWindow.uxml");
        }
        if(chatWindowTemplate == null) {
            Debug.LogError("Could not load chat window template.");
        }
        if(tabTemplate == null) {
            tabTemplate = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Data/UI/_Elements/ChatTab.uxml");
        }
        if(tabTemplate == null) {
            Debug.LogError("Could not load chat tab template.");
        }
    }

    public void AddWindow(VisualElement root) {
        if(chatWindowTemplate == null) {
            return;
        }

        var chatWindowEle = chatWindowTemplate.Instantiate()[0];
        MouseOverDetectionManipulator mouseOverDetection = new MouseOverDetectionManipulator(chatWindowEle);
        chatWindowEle.AddManipulator(mouseOverDetection);

        var diagonalResizeHandle = chatWindowEle.Q<VisualElement>(null, "resize-diag");
        DiagonalResizeManipulator diagonalResizeManipulator = new DiagonalResizeManipulator(
            diagonalResizeHandle, chatWindowEle, chatWindowMinWidth, chatWindowMaxWidth, chatWindowMinHeight, chatWindowMaxHeight);
        diagonalResizeHandle.AddManipulator(diagonalResizeManipulator);

        chatInput = chatWindowEle.Q<TextField>("ChatInputField");
        chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
        chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
        L2GameUI.BlinkingCursor(chatInput);

        chatInputContainer = chatWindowEle.Q<VisualElement>("InnerBar");

        CreateTabs(chatWindowEle);

        root.Add(chatWindowEle);

        if(offlineChat) {
            ReceiveChatMessage(new MessageLoggedIn(PlayerEntity.GetInstance().Identity.Name));
        }      
    }

    private void CreateTabs(VisualElement chatWindowEle) {
        chatTabView = chatWindowEle.Q<TabView>("ChatTabView");
        
        foreach(var tab in tabs) {
            Tab t = (Tab) tabTemplate.CloneTree()[0][0];
            t.name = tab.tabName;
            tab.scrollView = t.Q<ScrollView>("ScrollView");
            tab.content = t.Q<Label>("Content");
            tab.content.text = "";
            tab.tab = t;
            chatTabView.Add(t);
        }
    }

    void Update() {
        if(InputManager.GetInstance().IsInputPressed(InputType.SendMessage)) {
            if(chatOpened) {
                CloseChat(true);
            } else {
                OpenChat();
            }
        }

        if(InputManager.GetInstance().IsInputPressed(InputType.Escape)) {
            if(chatOpened) {
                CloseChat(false);
            }
        }
    }

    public void OpenChat() {
        chatOpened = true;
        L2GameUI.GetInstance().BlurFocus();
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

    public void ClearTab(int tabIndex) {
        if(tabIndex <= tabs.Count - 1) {
            tabs[tabIndex].content.text = "";
        }
    }

 
    public void SendChatMessage(string text) {
        Message message = new ChatMessage(PlayerEntity.GetInstance().Identity.Name, text);

        if(offlineChat) {
            ReceiveChatMessage(message);
        } else {

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
                    //Scroll
                }
            }
        }
        
    }

    private void ConcatMessage(Label chatContent, string message) {

        if(chatContent.text.Length > 0) {
            chatContent.text += "\r\n";
        }

        chatContent.text += message;

        /*//TODO: Filter messages based on tabs
        Label label = tab1Text;
        Scroller scroller = tab1ScrollView.verticalScroller;

        if(label.text.Length > 0) {
            label.text += "\r\n";
        }

        label.text += message;

        if(autoscroll) {
            scroller.value = scroller.value = scroller.highValue > 0 ? scroller.highValue : 0;
        }*/
    }
}