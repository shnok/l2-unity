using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonInventory
{

    private CharacterInventory inventory;

    public ButtonInventory(CharacterInventory inventory)
    {
        this.inventory = inventory;
    }


    public void RegisterButtonCloseWindow(VisualElement rootWindows, string buttonId)
    {
        var btn = rootWindows.Q<Button>(className:buttonId);

        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {

            inventory.HideElements(true, rootWindows);

        }, TrickleDown.TrickleDown);
    }


}
