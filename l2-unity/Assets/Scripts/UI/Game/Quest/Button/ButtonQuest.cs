using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonQuest 
{
    private QuestWindow _quest;
    public ButtonQuest(QuestWindow _quest)
    {
        this._quest = _quest;
    }


    public void RegisterButtonCloseWindow(VisualElement rootWindows, string buttonId)
    {
        var btn = rootWindows.Q<Button>(className: buttonId);
        if (btn == null) { Debug.LogError(buttonId + " can't be found."); return; }
        btn.RegisterCallback<MouseUpEvent>(evt => _quest.HideWindow(), TrickleDown.TrickleDown);
    }

    public void RegisterClickWindow(VisualElement contentId, VisualElement headerId)
    {

        if (contentId == null | headerId == null)
        {
            Debug.LogError(contentId + " can't be found.");
            return;
        }

        contentId.RegisterCallback<MouseDownEvent>(evt => {
            Debug.Log("Evennt cliiikkk 1");
            _quest.BringToFront();

        }, TrickleDown.TrickleDown);

        headerId.RegisterCallback<MouseDownEvent>(evt => {
            Debug.Log("Evennt cliiikkk 2");
            _quest.BringToFront();
        }, TrickleDown.TrickleDown);
    }
}
