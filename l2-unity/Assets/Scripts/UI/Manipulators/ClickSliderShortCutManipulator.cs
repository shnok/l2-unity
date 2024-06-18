using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;

public class ClickSliderShortCutManipulator : PointerManipulator
{
    private ShortCutPanelMinimal shortcutminimal;
    private int numberClick = 0;
    private DragManipulatorsChildren drag;
    private bool isVertical;
    public ClickSliderShortCutManipulator(ShortCutPanelMinimal shortcutminimal , DragManipulatorsChildren drag , bool isVertical)
    {
        this.shortcutminimal = shortcutminimal;
        this.drag = drag;
        //this.isVertical = isVertical;
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
        VisualElement element = (VisualElement)evt.currentTarget;
        if (!ShortCutPanel.Instance.IsVertical())
        {
            //vertical panel name
            //disable click 
            if (!element.name.Equals("head-slider"))
            {
                var visualElement = GetRootElement(evt, numberClick);
                Next(visualElement);
            }

        }
        else
        {
            if (element.name.Equals("head-slider"))
            {
                var visualElement = GetRootElement(evt, numberClick);
                Next(visualElement);
            }
        }
       
    }

    private void Next(VisualElement visualElement)
    {
        if (visualElement != null)
        {
            drag.SetActivePanel(numberClick);
            NextPanel(visualElement);
        }
        else
        {
            ShortCutPanel.Instance.ReplaceRightSliderToLeftSlider();
            ResetPositionPanel();
        }
    }

    private void NextPanel(VisualElement visualElement)
    {
        var rootVector2 = GetPositionWorld(visualElement, numberClick);


        if (numberClick <= shortcutminimal.Count())
        {
            if (ShortCutPanel.Instance.IsVertical())
            {
                float sdvig = 23;
                var newPosition = new Vector2(rootVector2.x - sdvig, rootVector2.y);
                shortcutminimal.NewPosition(newPosition, numberClick++, rootVector2 , ShortCutPanel.Instance.IsVertical());
            }
            else
            {
                float sdvig = 23;
                var newPosition = new Vector2(rootVector2.x, rootVector2.y - sdvig);
                shortcutminimal.NewPosition(newPosition, numberClick++, rootVector2 , ShortCutPanel.Instance.IsVertical());
            }
      
        }
    

    }

    public int GetShowPanels()
    {
        return numberClick;
    }

    private void ResetPositionPanel()
    {
        if (isVertical)
        {
            shortcutminimal.SetResetPosition();
            shortcutminimal.SetHidePanels();
            this.numberClick = 0;
        }
        else
        {
            shortcutminimal.SetResetPosition();
            shortcutminimal.SetHidePanels();
            this.numberClick = 0;
        }
      
    }

    private VisualElement GetRootElement(PointerDownEvent evt , int activeIndex)
    {
        if (activeIndex >= shortcutminimal.Count()) return null;
        if (activeIndex == 0)
        {
            return (VisualElement)evt.currentTarget;
        }

        if (activeIndex >= 1)
         {
            int minus1 = activeIndex - 1;
            int end = shortcutminimal.GetLastPosition(minus1);
             ReplaceSliderLeft(end, minus1);
            return shortcutminimal.GetLastElement(minus1);
        }
            
        
        return null;
    }

    private void ReplaceSliderLeft(int end, int minus1)
    {
        if (end == minus1)
        {
            ShortCutPanel.Instance.ReplaceLeftSliderToRightSlider();
        }

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
