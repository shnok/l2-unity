using UnityEngine;
using UnityEngine.UIElements;

public class L2Scrollable
{
    protected ScrollView _scrollView;
    protected Scroller _scroller;
    protected VisualElement _container;
    public Scroller Scroller { get { return _scroller; } }
    protected float _scrollStepSize = 22f;
    public float ScrollStepSize { get { return _scrollStepSize; } set { _scrollStepSize = value; } }
    private bool _autoscroll;
    public bool AutoScroll { get { return _autoscroll; } set { _autoscroll = value; } }

    public virtual void Initialize(VisualElement container, bool autoscroll)
    {
        _container = container;
        _autoscroll = autoscroll;

        InitScroller();
    }

    private void InitScroller()
    {
        _scrollView = _container?.Q<ScrollView>("ScrollView");
        _scroller = _scrollView?.verticalScroller;

        RegisterAutoScrollEvent();
        RegisterPlayerScrollEvent();
    }

    protected void AdjustScrollValue(int direction)
    {
        if (_scrollView == null || _scroller == null) return;

        float contentHeight = _scrollView.contentContainer.worldBound.height;
        float viewportHeight = _scrollView.worldBound.height;

        if (contentHeight <= viewportHeight) return; // No need to scroll if content fits in viewport

        // float scrollRange = contentHeight - viewportHeight;
        // float stepSize = _scrollStepSize / scrollRange;
        // float newValue = (direction > 0 ? 1 : -1) * (_scroller.value + stepSize) * _scroller.highValue;

        float newValue = (direction > 0 ? 1 : -1) * _scrollStepSize + _scroller.value;

        _scroller.value = Mathf.Clamp(newValue, 0, _scroller.highValue);
    }

    protected virtual void RegisterAutoScrollEvent()
    {
        if (_scroller == null)
        {
            return;
        }
    }

    private void RegisterPlayerScrollEvent()
    {
        if (_scroller == null)
        {
            return;
        }

        var highBtn = _scroller.Q<RepeatButton>("unity-high-button");
        var lowBtn = _scroller.Q<RepeatButton>("unity-low-button");
        var dragger = _scroller.Q<VisualElement>("unity-drag-container");

        highBtn.RegisterCallback<MouseUpEvent>(evt =>
        {
            AdjustScrollValue(1);
            VerifyScrollValue();
        });
        lowBtn.RegisterCallback<MouseUpEvent>(evt =>
        {
            AdjustScrollValue(-1);
            VerifyScrollValue();
        });

        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));

        dragger.RegisterCallback<MouseUpEvent>(evt =>
        {
            VerifyScrollValue();
        });

        dragger.RegisterCallback<WheelEvent>(evt =>
        {
            VerifyScrollValue();
        });

        _scrollView.RegisterCallback<WheelEvent>(evt =>
        {
            int direction = evt.delta.y > 0 ? 1 : -1;
            AdjustScrollValue(direction);
            VerifyScrollValue();
            evt.StopPropagation();
        });
    }

    private void VerifyScrollValue()
    {
        if (_scroller.highValue > 0 && _scroller.value == _scroller.highValue || _scroller.highValue == 0 && _scroller.value == 0)
        {
            _autoscroll = true;
        }
        else
        {
            _autoscroll = false;
        }
    }
}