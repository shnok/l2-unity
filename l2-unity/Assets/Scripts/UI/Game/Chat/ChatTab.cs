using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements; 

[System.Serializable]
public class ChatTab
{
    [SerializeField] string _tabName = "Tab";
    //[SerializeField] private List<MessageType> _filteredMessages;
    private bool _autoscroll = true;
    private ScrollView _scrollView;
    private Label _content;
    private Scroller _scroller;
    private VisualElement _tabContainer;
    private VisualElement _tabHeader;
    private VisualElement _chatWindowEle;
    public string TabName { get { return _tabName; } }
    //public List<MessageType> FilteredMessages { get { return _filteredMessages; } }
    public Label Content { get { return _content; } }
    public VisualElement TabContainer { get { return _tabContainer; } }
    public VisualElement TabHeader { get { return _tabHeader; } }
    public Scroller Scroller { get { return _scroller; } }

    public void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        _chatWindowEle = chatWindowEle;
        _tabContainer = tabContainer;
        _tabHeader = tabHeader;
        _scrollView = tabContainer.Q<ScrollView>("ScrollView");
        _scroller = _scrollView.verticalScroller;
        _content = tabContainer.Q<Label>("Content");
        _content.text = "";

        tabHeader.AddManipulator(new ButtonClickSoundManipulator(tabHeader));

        tabHeader.RegisterCallback<MouseDownEvent>(evt => {
            if(ChatWindow.Instance.SwitchTab(this)) {
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
        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));
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
