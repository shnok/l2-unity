using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUI : MonoBehaviour
{
    public static void BlinkingCursor(VisualElement tf) {
        tf.schedule.Execute(() => {
            if (tf.ClassListContains("transparent-cursor"))
                tf.RemoveFromClassList("transparent-cursor");
            else
                tf.AddToClassList("transparent-cursor");
        }).Every(500);
    }

    public static void RegisterButtonClickSoundCallback(Button button) {
        Debug.Log("Registering button click callback for " + button);
        button.RegisterCallback<MouseDownEvent>(evt => AudioManager.Instance.PlayUISound("click_01"), TrickleDown.TrickleDown);
    }

}
