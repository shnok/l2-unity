using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements; 

[System.Serializable]
public class ChatTab
{
    [SerializeField] string _tabName = "Tab";
    [SerializeField] private List<MessageType> _filteredMessages;
    private bool _autoscroll = true;
    private ScrollView _scrollView;
    private Label _content;
    private Scroller _scroller;
    private VisualElement _chatWindowEle;
    private TabView _tabView;

    public string TabName { get { return _tabName; } }
    public List<MessageType> FilteredMessages { get { return _filteredMessages; } }
    public Label Content { get { return _content; } }

    public void Initialize(VisualElement chatWindowEle, Tab tab) {
        _chatWindowEle = chatWindowEle;
        _tabView = chatWindowEle.Q<TabView>("ChatTabView");
        _scrollView = tab.Q<ScrollView>("ScrollView");
        _scroller = _scrollView.verticalScroller;
        _content = tab.Q<Label>("Content");
        _content.text = "";

        tab.Q<VisualElement>("unity-tab__header").RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
            if(_tabView.activeTab != tab) {
                AudioManager.Instance.PlayUISound("window_open");
            }
        }, TrickleDown.TrickleDown);

        RegisterAutoScrollEvent();
        RegisterPlayerScrollEvent();
    }

    private void RegisterAutoScrollEvent() {
        _content.RegisterValueChangedCallback(evt => {
            if(_autoscroll) {
                ChatWindow.Instance.ScrollDown(_scroller);
            }
        });    
    }

    private void RegisterPlayerScrollEvent() {
        var highBtn = _scroller.Q<RepeatButton>("unity-high-button");
        var lowBtn = _scroller.Q<RepeatButton>("unity-low-button");
        var dragger = _scroller.Q<VisualElement>("unity-drag-container");

        highBtn.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        lowBtn.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        highBtn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
        lowBtn.RegisterCallback<MouseDownEvent>(evt => {
            AudioManager.Instance.PlayUISound("click_01");
        }, TrickleDown.TrickleDown);
        dragger.RegisterCallback<MouseUpEvent>(evt => {
            VerifyScrollValue();
        });
        dragger.RegisterCallback<WheelEvent>(evt => {
            VerifyScrollValue();
        });
        
        _chatWindowEle.RegisterCallback<GeometryChangedEvent>(evt => {
            if(_autoscroll) {
                ChatWindow.Instance.ScrollDown(_scroller);
            }
        });
    }

    private void VerifyScrollValue() {
        if(_scroller.highValue > 0 && _scroller.value == _scroller.highValue || _scroller.highValue == 0 && _scroller.value == 0) {
            _autoscroll = true;
        } else {
            _autoscroll = false;
        }
    }
}
