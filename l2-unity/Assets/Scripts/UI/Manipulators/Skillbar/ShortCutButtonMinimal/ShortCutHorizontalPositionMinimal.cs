using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutHorizontalPositionMinimal
{
    private int sizeCell;
    public VisualElement[] arrayPanels;
    private ShortCutPanel shortCutPanel;

    public ShortCutHorizontalPositionMinimal(VisualElement[] arrayPanels, ShortCutPanel shortcut ,int sizeCell)
    {
        this.arrayPanels = arrayPanels;
        this.sizeCell = sizeCell;
        this.shortCutPanel  = shortcut;
    }

    public void Start(VisualElement rootGroupBox , ClickSliderShortCutManipulator clickArrow)
    {
        if (shortCutPanel.IsVertical())
        {
            for(int i =0; i < arrayPanels.Length; i++)
            {
                ChangePositionHorizontalRootGroupBox(arrayPanels[i]);
                RotateCellHorizontal(arrayPanels[i] , 90);
                RotateImageIndex(arrayPanels[i] , 90);
            }
            shortCutPanel.SetPositionVerical(false);
            ClickButton(rootGroupBox , clickArrow);
        }
        else
        {
            for (int i = 0; i < arrayPanels.Length; i++)
            {
                ChangePositionVerticalRootGroupBox(arrayPanels[i]);
                RotateCellHorizontal(arrayPanels[i], 0);
                RotateImageIndex(arrayPanels[i], 0);
            }
            shortCutPanel.SetPositionVerical(true);
        }

    }

    private void ClickButton(VisualElement rootGroupBox , ClickSliderShortCutManipulator clickArrow)
    {
        //
        var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(null, "slide-hor-arrow");
        //if we haven't reached the end of the panels
        if (buttonSliderHorizontal != null)
        {
            ListenerNormalHorizontalArrow(buttonSliderHorizontal, clickArrow);
        }
        else
        {

            ListenerEndHorizontalArrow(rootGroupBox, clickArrow);
        }
    }

    private void ListenerEndHorizontalArrow(VisualElement rootGroupBox , ClickSliderShortCutManipulator clickArrow)
    {
        //the last panel is changed by the arrow and moved to a new horizontal panel
        var buttonSliderLeft = rootGroupBox.Q<VisualElement>(null, "button-slider-left");
        if (buttonSliderLeft != null)
        {
            buttonSliderLeft.AddManipulator(clickArrow);
        }
    }

    private void ListenerNormalHorizontalArrow(VisualElement buttonSliderHorizontal , ClickSliderShortCutManipulator clickArrow)
    {
        buttonSliderHorizontal.AddManipulator(clickArrow);
    }
    private void ChangePositionHorizontalRootGroupBox(VisualElement rootPanel)
    {
        if (rootPanel != null)
        {
            rootPanel.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
       
    }

    private void ChangePositionVerticalRootGroupBox(VisualElement rootPanel)
    {
        if (rootPanel != null)
        {
            rootPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
    }

    private void RotateCellHorizontal(VisualElement rootGroupBox, int angle)
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            row.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void RotateImageIndex(VisualElement rootGroupBox , int angle)
    {
        var indexImage = rootGroupBox.Q<VisualElement>(null, "ImageIndexMinimal");
        if (indexImage != null) indexImage.transform.rotation = Quaternion.Euler(0, 0, angle);
    }


}
