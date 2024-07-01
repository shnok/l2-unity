using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonCharacter
{
    private CharacterInfoWindow character;

    public ButtonCharacter(CharacterInfoWindow character)
    {
        this.character = character;
    }


    public void RegisterButtonCloseWindow(VisualElement rootWindows, string buttonId)
    {
        var btn = rootWindows.Q<Button>(className: buttonId);

        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseUpEvent>(evt => {
            Debug.Log("Click event");
            character.HideElements(true, rootWindows);

        }, TrickleDown.TrickleDown);
    }

    public void RegisterClickWindow(VisualElement rootWindows, VisualElement contentId , VisualElement headerId)
    {

        if (contentId == null | headerId == null)
        {
            Debug.LogError(contentId + " can't be found.");
            return;
        }

        contentId.RegisterCallback<MouseDownEvent>(evt => {
            character.BringFront();

        }, TrickleDown.TrickleDown);

        headerId.RegisterCallback<MouseDownEvent>(evt => {
            character.BringFront();
        }, TrickleDown.TrickleDown);
    }
}
