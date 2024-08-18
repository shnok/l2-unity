using UnityEngine;
using UnityEngine.UIElements;


public class ShortCutHorizontalPosition
{

    private ShortCutPanel shortCutPanel;
    private VisualElement rootGroupBox;
    private int sizeCell;

    public ShortCutHorizontalPosition(ShortCutPanel shortCutPanel, VisualElement rootGroupBox, int sizeCell)
    {
        this.shortCutPanel = shortCutPanel;
        this.rootGroupBox = rootGroupBox;
        this.sizeCell = sizeCell;
    }

    public void Start()
    {
        if (shortCutPanel.IsVertical())
        {
            ChangePositionHorizontalRootGroupBox();
            RotateHorizontalSlider();
            RotateCellHorizontal();
        }
        else
        {
            ChangePositionVerticalRootGroupBox();
            RotateVertiacalSlider();
            RotateCellVertical();
        }

    }

   

    private void ChangePositionHorizontalRootGroupBox()
    {
        rootGroupBox.AddToClassList("rotate_img90");
    }

    private void ChangePositionVerticalRootGroupBox()
    {
        rootGroupBox.RemoveFromClassList("rotate_img90");
    }




    public void RotateHorizontalSlider()
    {
        VisualElement buttonSliderlVerticalEnd = rootGroupBox.Q<VisualElement>(className: "button-slider-right");
        var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(null, "slide-hor");
        var indexImage = rootGroupBox.Q<VisualElement>(null, "ImageIndex");

        SetHorizontalSlider(buttonSliderHorizontal , buttonSliderlVerticalEnd);

        if (indexImage != null) indexImage.transform.rotation = Quaternion.Euler(0, 0, 90);


        VisualElement buttonSlider =  shortCutPanel.GetSliderVisualElement();
        SetEmptyTextureHeadSlider(buttonSlider);
        HideVerticalEndArrow(buttonSliderlVerticalEnd);

    }

    private void HideVerticalEndArrow(VisualElement buttonSliderlVerticalEnd)
    {
        if(buttonSliderlVerticalEnd != null)
        {
            buttonSliderlVerticalEnd.RemoveFromClassList("button-slider-right");
            //buttonSliderlVerticalEnd.AddToClassList("button-slider");
        }
    }
    //button-slider-left - horizontal finishing
    //slide-hor-arrow - horizontal normal 
    private void SetHorizontalSlider(VisualElement buttonSliderHorizontal , VisualElement buttonSliderlVerticalEnd)
    {
        if (buttonSliderHorizontal != null)
        {
            if(buttonSliderlVerticalEnd != null)
            {
                if (buttonSliderHorizontal.ClassListContains("slide-hor"))
                {
                    ShowEndHorizontalArrow(buttonSliderHorizontal);
                }
                   
            }
            else
            {
                if (buttonSliderHorizontal.ClassListContains("slide-hor"))
                {
                    ShowNormalHorizontalArrow(buttonSliderHorizontal);
                }
            }


        }

      
    }

    private void ShowNormalHorizontalArrow(VisualElement buttonSliderHorizontal)
    {
        buttonSliderHorizontal.RemoveFromClassList("slide-hor");
        buttonSliderHorizontal.AddToClassList("slide-hor-arrow");
    }

    private void ShowEndHorizontalArrow(VisualElement buttonSliderHorizontal)
    {
        buttonSliderHorizontal.RemoveFromClassList("slide-hor");
        buttonSliderHorizontal.AddToClassList("button-slider-left");
    }

    private void SetEmptyTextureHeadSlider(VisualElement buttonSlider)
    {
        if (buttonSlider != null)
        {

            buttonSlider.RemoveFromClassList("button-slider");
            buttonSlider.AddToClassList("button-slider-empty");
        }
    }

    public void RotateVertiacalSlider()
    {
        
        var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(null, "slide-hor-arrow");
        VisualElement buttonSliderHorizontal1 = null ;
        if (buttonSliderHorizontal != null)
        {
            if (buttonSliderHorizontal.ClassListContains("slide-hor-arrow"))
            {
                buttonSliderHorizontal.RemoveFromClassList("slide-hor-arrow");
                buttonSliderHorizontal.AddToClassList("slide-hor");
            }

        }
        else
        {
            buttonSliderHorizontal1 = rootGroupBox.Q<VisualElement>(null, "button-slider-left");
            if (buttonSliderHorizontal1 != null)
            {
                buttonSliderHorizontal1.RemoveFromClassList("button-slider-left");
                buttonSliderHorizontal1.AddToClassList("slide-hor");
            }
            
        }

        var indexImage = rootGroupBox.Q<VisualElement>(null, "ImageIndex");
        if (indexImage != null) indexImage.transform.rotation = Quaternion.Euler(0, 0, 0);

        var buttonSlider = rootGroupBox.Q<VisualElement>(null, "button-slider-empty");

        if(buttonSlider != null)
        {
            buttonSlider.RemoveFromClassList("button-slider-empty");

            if(buttonSliderHorizontal1 != null)
            {
                buttonSlider.AddToClassList("button-slider-right");
            }
            else
            {
                buttonSlider.AddToClassList("button-slider");
            }

        }
       
    }

    private void RotateCellHorizontal()
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            //var row = rootGroupBox.Q(className: "row" + cell);
            //row.AddToClassList("rotate_imgPlus90");
            var row = rootGroupBox.Q(className: "row" + cell);
            row.transform.rotation = Quaternion.Euler(0, 0, 90);

        }
    }

    

    private void RotateCellVertical()
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            row.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
