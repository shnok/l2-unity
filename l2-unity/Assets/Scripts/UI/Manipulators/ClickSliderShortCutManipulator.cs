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
    public ClickSliderShortCutManipulator(VisualElement target_buttonSlider, ShortCutPanelMinimal shortcutminimal)
    {
        this.target_buttonSlider = target_buttonSlider;
        this.shortcutminimal = shortcutminimal;
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
        var visualelement = GetRootElement(evt, numberClick);

        if(visualelement != null)
        {
            var rootVector2 = GetPositionWorld(visualelement, numberClick);

            if (numberClick > shortcutminimal.Count()) numberClick = 0;

            if (numberClick <= shortcutminimal.Count())
            {
                int clickCount = numberClick + 1;
                float sdvig = 23;

                var newPosition = new Vector2(rootVector2.x - sdvig, rootVector2.y);

                shortcutminimal.newPosition(newPosition, numberClick++);
                //transform.position = Vector3.MoveTowards(transform.position, _endPos, Time.deltaTime * _velocity);
                //iconOverlay.AddAnim(SkillTimeArray.GetArrayImage(), 0.0001f);
            }
        }
      

    }

    private VisualElement GetRootElement(PointerDownEvent evt , int activeIndex)
    {
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