using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatWindow : L2Window
{
    public static int MAXIMUM_INPUT_HISTORY = 25;
    public static int MAXIMUM_MESSAGE_COUNT = 100;

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private TextField _chatInput;
    private VisualElement _chatInputContainer;
    private L2TabView _l2TabView;
    private List<string> _history;

    private int _historyIndex = 0;

    [SerializeField] private float _chatWindowMinWidth = 225.0f;
    [SerializeField] private float _chatWindowMaxWidth = 500.0f;
    [SerializeField] private float _chatWindowMinHeight = 175.0f;
    [SerializeField] private float _chatWindowMaxHeight = 600.0f;
    [SerializeField] public ChatTab[] _tabs;
    [SerializeField] private bool _chatOpened = false;
    [SerializeField] private int _chatInputCharacterLimit = 100;

    public bool ChatOpened { get { return _chatOpened; } }

    private static ChatWindow _instance;
    public static ChatWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _history = new List<string>();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ChatWindow/ChatWindow");
        _tabTemplate = LoadAsset("Data/UI/_Elements/Game/ChatWindow/ChatTab");
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/ChatWindow/ChatTabHeader");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var diagonalResizeHandle = GetElementByClass("resize-diag");

        DiagonalResizeManipulator diagonalResizeManipulator = new DiagonalResizeManipulator(
            diagonalResizeHandle,
            _windowEle,
            _chatWindowMinWidth,
            _chatWindowMaxWidth,
            _chatWindowMinHeight,
            _chatWindowMaxHeight,
            14.5f,
            2f);

        diagonalResizeHandle.AddManipulator(diagonalResizeManipulator);

        _chatInput = (TextField)GetElementById("ChatInputField");
        _chatInput.RegisterCallback<FocusEvent>(OnChatInputFocus);
        _chatInput.RegisterCallback<BlurEvent>(OnChatInputBlur);
        _chatInput.maxLength = _chatInputCharacterLimit;

        var enlargeTextBtn = (Button)GetElementById("EnlargeTextBtn");
        enlargeTextBtn.AddManipulator(new ButtonClickSoundManipulator(enlargeTextBtn));

        var chatOptionsBtn = (Button)GetElementById("ChatOptionsBtn");
        chatOptionsBtn.AddManipulator(new ButtonClickSoundManipulator(chatOptionsBtn));

        _chatInput.AddManipulator(new BlinkingCursorManipulator(_chatInput));

        _chatInputContainer = GetElementById("InnerBar");

        CreateTabs();

        yield return new WaitForEndOfFrame();
        diagonalResizeManipulator.SnapSize();

        L2GameUI.Instance.WindowLoadComplete();
    }


    private void CreateTabs()
    {
        VisualElement chatTabView = GetElementById("ChatTabView");

        _l2TabView = new L2TabView();
        _l2TabView.Initialize(chatTabView, _tabs, _tabTemplate, _tabHeaderTemplate, false);
    }

    void Update()
    {
        if (InputManager.Instance.Validate)
        {
            if (_chatOpened)
            {
                CloseChat(true);
            }
            else
            {
                _historyIndex = _history.Count;
                StartCoroutine(OpenChat());
            }
        }

        if (InputManager.Instance.ArrowDown)
        {
            if (_chatOpened && _history.Count > 0)
            {
                _historyIndex = Mathf.Min(_historyIndex + 1, _history.Count);

                _chatInput.value = _historyIndex < _history.Count ? _history[_historyIndex] : "";
                _chatInput.cursorIndex = _chatInput.value.Length;
            }
        }

        if (InputManager.Instance.ArrowUp)
        {
            if (_chatOpened && _history.Count > 0)
            {
                _historyIndex = Mathf.Max(_historyIndex - 1, 0);

                _chatInput.value = _history[_historyIndex];
                _chatInput.cursorIndex = _chatInput.value.Length;
            }
        }
    }

    IEnumerator OpenChat()
    {
        _chatOpened = true;
        L2GameUI.Instance.BlurFocus();
        yield return new WaitForEndOfFrame();
        _chatInput.Focus();
    }

    public void CloseChat(bool sendMessage)
    {
        _chatOpened = false;

        L2GameUI.Instance.BlurFocus();

        if (sendMessage)
        {
            if (_chatInput.text.Length > 0)
            {
                SendChatMessage(_chatInput.text);

                _history.Add(_chatInput.text);

                // Limit history to 25 messages
                if (_history.Count > MAXIMUM_INPUT_HISTORY)
                {
                    _history.RemoveAt(0);  // Remove oldest entry
                }

                _historyIndex = _history.Count; // Set to end of history

                _chatInput.value = "";
            }
        }
    }

    private void OnChatInputFocus(FocusEvent evt)
    {
        if (!_chatInputContainer.ClassListContains("highlighted"))
        {
            _chatInputContainer.AddToClassList("highlighted");
        }

        if (!_chatOpened)
        {
            _chatOpened = true;
        }
    }

    private void OnChatInputBlur(BlurEvent evt)
    {
        if (_chatInputContainer.ClassListContains("highlighted"))
        {
            _chatInputContainer.RemoveFromClassList("highlighted");
        }

        if (_chatOpened)
        {
            _chatOpened = false;
        }
    }

    public void ClearChat()
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            ClearTab(i);
        }
    }

    public void ClearTab(int tabIndex)
    {
        if (tabIndex <= _tabs.Length - 1)
        {
            _tabs[tabIndex].Content.text = "";
        }
    }


    public void SendChatMessage(string text)
    {
        if (World.Instance.OfflineMode)
        {
            ChatMessage message = new ChatMessage(PlayerEntity.Instance.Identity.Name, text);
            ReceiveChatMessage(message);
        }
        else
        {
            if (text.StartsWith("//"))
            {
                GameClient.Instance.ClientPacketHandler.SendGMCommand(text.Replace("//", ""));
            }
            else if (text.Length > 0)
            {
                L2MessageType messageType = L2MessageType.ALL;
                string target = null;

                switch (text[0])
                {
                    case '+':
                        messageType = L2MessageType.TRADE;
                        break;
                    case '!':
                        messageType = L2MessageType.SHOUT;
                        break;
                    case '#':
                        messageType = L2MessageType.PARTY;
                        break;
                    case '@':
                        messageType = L2MessageType.CLAN;
                        break;
                    case '$':
                        messageType = L2MessageType.ALLIANCE;
                        break;
                    case '%':
                        messageType = L2MessageType.HERO_VOICE;
                        break;
                    case '"':
                        string[] s = text[1..].Split(" ");
                        target = (s.Length > 0) ? s[0] : "";
                        messageType = L2MessageType.TELL;
                        break;
                    default:
                        break;
                }

                if (messageType != L2MessageType.ALL)
                {
                    text = text[1..];
                }

                GameClient.Instance.ClientPacketHandler.SendMessage(text, messageType, target);
            }
        }
    }

    public void ReceiveChatMessage(ChatMessage message)
    {
        if (message == null)
        {
            return;
        }

        for (int i = 0; i < _tabs.Length; i++)
        {
            if (_tabs[i].FilteredMessages.Count > 0)
            {
                if (_tabs[i].FilteredMessages.Contains(message.MessageType))
                {
                    _tabs[i].AddMessage(message.ToString());
                }
            }
        }
    }

    public void ReceiveSystemMessage(SystemMessage message)
    {
        if (message == null)
        {
            return;
        }

        for (int i = 0; i < _tabs.Length; i++)
        {
            if (_tabs[i].FilteredMessages.Contains(L2MessageType.SYSTEM_MESSAGE))
            {
                _tabs[i].AddMessage(message.ToString());
            }
        }
    }

    public void ScrollDown(Scroller scroller)
    {
        StartCoroutine(ScrollDownWithDelay(scroller));
    }

    IEnumerator ScrollDownWithDelay(Scroller scroller)
    {
        yield return new WaitForEndOfFrame();
        scroller.value = scroller.highValue > 0 ? scroller.highValue : 0;
    }
}