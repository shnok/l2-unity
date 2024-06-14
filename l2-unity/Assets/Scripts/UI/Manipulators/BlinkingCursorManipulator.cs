using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlinkingCursorManipulator : PointerManipulator {
    private IVisualElementScheduledItem _scheduler;

    public BlinkingCursorManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        _scheduler = target.schedule.Execute(() => {
            if (target.ClassListContains("transparent-cursor"))
                target.RemoveFromClassList("transparent-cursor");
            else
                target.AddToClassList("transparent-cursor");
        }).Every(500);
    }

    protected override void UnregisterCallbacksFromTarget() {
        _scheduler.Pause();
    }
}
