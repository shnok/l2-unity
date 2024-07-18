using System.Collections.Generic;
using UnityEngine;
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

    protected override void RegisterAutoScrollEvent() {
        _content.RegisterValueChangedCallback(evt => {
            if(_autoscroll) {
                ChatWindow.Instance.ScrollDown(_scroller);
            }
        });    
    }
}
