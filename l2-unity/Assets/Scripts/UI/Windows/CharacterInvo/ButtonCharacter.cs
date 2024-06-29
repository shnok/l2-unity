using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonCharacter
{
    private CharacterInfo character;

    public ButtonCharacter(CharacterInfo character)
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

        btn.RegisterCallback<MouseDownEvent>(evt => {
            Debug.Log("Click event");
            character.HideElements(true, rootWindows);

        }, TrickleDown.TrickleDown);
    }
}
