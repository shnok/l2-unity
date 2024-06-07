using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ClickSliderShortCutManipulator : PointerManipulator
{
    private VisualElement target_buttonSlider;
    private ShortCutPanelMinimal shortcutminimal;
    private int numberClick = 0;
    private DragManipulatorsChildren drag;
    public ClickSliderShortCutManipulator(ShortCutPanelMinimal shortcutminimal , DragManipulatorsChildren drag)
    {
        this.shortcutminimal = shortcutminimal;
        this.drag = drag;
    }
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        var visualElement = GetRootElement(evt, numberClick);

        if (visualElement != null)
        {
            drag.SetActivePanel(numberClick);
            NextPanel(visualElement);
        }
        else
        {
            ResetPositionPanel();
        }
    }

    private void NextPanel(VisualElement visualElement)
    {
        var rootVector2 = GetPositionWorld(visualElement, numberClick);

        //if (numberClick > shortcutminimal.Count()) numberClick = 0;

        if (numberClick <= shortcutminimal.Count())
        {
            float sdvig = 23;
            var newPosition = new Vector2(rootVector2.x - sdvig, rootVector2.y);
            shortcutminimal.NewPosition(newPosition, numberClick++, rootVector2);

        }
    

    }

    private void ResetPositionPanel()
    {
        
        shortcutminimal.SetResetPosition();
        shortcutminimal.SetHidePanels();
        this.numberClick = 0;
    }

    private VisualElement GetRootElement(PointerDownEvent evt , int activeIndex)
    {
        if (activeIndex >= shortcutminimal.Count()) return null;


        if (activeIndex == 0) {
            return (VisualElement) evt.currentTarget;
        } 

        if (activeIndex >= 1)
        {
            int minus1 = activeIndex - 1;
            return shortcutminimal.getLastElement(minus1);
        }

        return null;
        
    }

    private Vector2 GetPositionWorld(VisualElement rootElement , int index)
    {
        if(index == 0)
        {
            return  rootElement.parent.worldBound.position; 
            //var slider =   groupbox_slider.parent.worldBound.position;
           // return slider;
        }

        if(index > 0)
        {
            return rootElement.worldBound.position;
        }
        
        return new Vector2 (0, 0);
    }


}
