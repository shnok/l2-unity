using UnityEngine.UIElements;

[System.Serializable]
public class ChatTab : L2Tab
{
    private Label _content;
    public Label Content { get { return _content; } }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        base.Initialize(chatWindowEle, tabContainer, tabHeader);
        _content = tabContainer.Q<Label>("Content");
        _content.text = "";
        _scrollStepSize = 12f;
    }

    protected override void OnGeometryChanged() {
        if (_autoscroll) {
            ChatWindow.Instance.ScrollDown(_scroller);
        }
    }

    protected override void OnSwitchTab() {
        if (ChatWindow.Instance.SwitchTab(this)) {
            AudioManager.Instance.PlayUISound("window_open");
        }
    }
    
    public void AddMessage(string message) {
        ConcatMessage(message.ToString());
    }

    private void ConcatMessage(string message) {
        if(_content.text.Length > 0) {
            _content.text += "\r\n";
        }
        _content.text += message;

        if(_autoscroll) {
            ChatWindow.Instance.ScrollDown(_scroller);
        }
    }
}
