using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonClickSoundManipulator : PointerManipulator {

    public ButtonClickSoundManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<ClickEvent>(MouseDownHandler);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<ClickEvent>(MouseDownHandler);
    }

    private void MouseDownHandler(ClickEvent evt) {
        AudioManager.Instance.PlayUISound("click_01");
    }
}