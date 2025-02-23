using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ChatTab : L2Tab
{
    [SerializeField] private List<L2MessageType> _filteredMessages;
    public List<L2MessageType> FilteredMessages { get { return _filteredMessages; } }
    private int _messageCount = 0;
    private L2Scrollable _l2scrollable;

    private Label _content;
    public Label Content { get { return _content; } }

    public override void Initialize(L2TabView tabView, VisualElement tabContainer, VisualElement tabHeader)
    {
        base.Initialize(tabView, tabContainer, tabHeader);
        _content = tabContainer.Q<Label>("Content");
        _content.text = "";

        _l2scrollable = new L2Scrollable();
        _l2scrollable.Initialize(tabContainer, true);
        _l2scrollable.ScrollStepSize = 12f;
    }

    public void AddMessage(string message)
    {
        ConcatMessage(message.ToString());
    }

    private void ConcatMessage(string message)
    {
        if (_content.text.Length > 0)
        {
            _content.text += "\r\n";
        }
        _content.text += message;

        if (_messageCount++ >= ChatWindow.MAXIMUM_MESSAGE_COUNT)
        {
            int firstLineBreak = _content.text.IndexOf("\r\n");
            if (firstLineBreak >= 0)
            {
                _content.text = _content.text[(firstLineBreak + 2)..];  // +2 to skip the delimiter itself
            }

            _messageCount = ChatWindow.MAXIMUM_MESSAGE_COUNT;
        }

        if (_l2scrollable.AutoScroll)
        {
            ChatWindow.Instance.ScrollDown(_l2scrollable.Scroller);
        }
    }

    protected override void OnTabHeaderClicked()
    {
        base.OnTabHeaderClicked();
    }

    //TODO: Disable autoscroll when manually scrolled, force scroll when window is resized
}
