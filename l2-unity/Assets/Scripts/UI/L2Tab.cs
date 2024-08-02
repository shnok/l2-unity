using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class L2Tab {
    [SerializeField] string _tabName = "Tab";
    protected bool _autoscroll = true;
    private ScrollView _scrollView;
    protected Scroller _scroller;
    private VisualElement _tabContainer;
    private VisualElement _tabHeader;
    protected VisualElement _windowEle;
    public string TabName { get { return _tabName; } }
    public VisualElement TabContainer { get { return _tabContainer; } }
    public VisualElement TabHeader { get { return _tabHeader; } }
    public Scroller Scroller { get { return _scroller; } }

    public virtual void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader) {
        _windowEle = chatWindowEle;

        if (tabContainer != null) {
            _tabContainer = tabContainer;
            _tabHeader = tabHeader;
            _scrollView = tabContainer.Q<ScrollView>("ScrollView");
            _scroller = _scrollView.verticalScroller;

            tabHeader.AddManipulator(new ButtonClickSoundManipulator(tabHeader));

            tabHeader.RegisterCallback<MouseDownEvent>(evt => {
                OnSwitchTab();
            }, TrickleDown.TrickleDown);

            RegisterAutoScrollEvent();
            RegisterPlayerScrollEvent();
        }
    }

    protected virtual void OnSwitchTab() { }

    protected virtual void RegisterAutoScrollEvent() { }

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

        _windowEle.RegisterCallback<GeometryChangedEvent>(evt => {
            OnGeometryChanged();
        });
    }

    protected virtual void OnGeometryChanged() { }

    private void VerifyScrollValue() {
        if (_scroller.highValue > 0 && _scroller.value == _scroller.highValue || _scroller.highValue == 0 && _scroller.value == 0) {
            _autoscroll = true;
        } else {
            _autoscroll = false;
        }
    }

    public virtual void SelectSlot(int slot) { }
}