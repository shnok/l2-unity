using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HighlightedInputFieldManipulator : PointerManipulator {
    private IVisualElementScheduledItem _scheduler;
    private int _animationFrameCount = 20;
    private int _currentFrame = 0;
    private VisualElement _backgroundElement;

    public HighlightedInputFieldManipulator(VisualElement target, VisualElement backgroundElement, int frames) {
        this.target = target;
        _animationFrameCount = frames;
        _backgroundElement = backgroundElement;
    }

    protected override void RegisterCallbacksOnTarget() {
        _scheduler = target.schedule.Execute(() => NextBackgroundFrame()).Every(70);
        _scheduler.Pause();

        target.RegisterCallback<BlurEvent>(OnBlurEvent);
        target.RegisterCallback<FocusEvent>(OnFocusEvent);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<BlurEvent>(OnBlurEvent);
        target.UnregisterCallback<FocusEvent>(OnFocusEvent);
    }

    private void OnFocusEvent(FocusEvent evt) {
        _currentFrame = 0;

        _scheduler.Resume();
    }

    private void OnBlurEvent(BlurEvent evt) {
        if (_backgroundElement.ClassListContains("bg-highlight-" + _currentFrame)) {
            _backgroundElement.RemoveFromClassList("bg-highlight-" + _currentFrame);
        }
        
        _scheduler.Pause();
    }

    private void NextBackgroundFrame() {
        int lastFrame = _currentFrame;
        if (_currentFrame == _animationFrameCount - 1) {
            _currentFrame = 0;
        } else {
            _currentFrame++;
        }

        if (_backgroundElement.ClassListContains("bg-highlight-" + lastFrame)) {
            _backgroundElement.RemoveFromClassList("bg-highlight-" + lastFrame);
        }

        _backgroundElement.AddToClassList("bg-highlight-" + _currentFrame);
    }
}