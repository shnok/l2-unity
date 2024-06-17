using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using static UnityEngine.Rendering.DebugUI.Table;


public class ShortCutHorizontalPosition : MonoBehaviour
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
        var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(null, "slide-hor");
        var indexImage = rootGroupBox.Q<VisualElement>(null, "ImageIndex");

        SetHorizontalSlider(buttonSliderHorizontal);

        if (indexImage != null) indexImage.transform.rotation = Quaternion.Euler(0, 0, 90);


        VisualElement buttonSlider =  shortCutPanel.GetSliderVisualElement();
        SetEmptyTextureHeadSlider(buttonSlider);


    }

    private void SetHorizontalSlider(VisualElement buttonSliderHorizontal)
    {
        if (buttonSliderHorizontal != null)
        {
            if (buttonSliderHorizontal.ClassListContains("slide-hor"))
            {
                buttonSliderHorizontal.RemoveFromClassList("slide-hor");
                buttonSliderHorizontal.AddToClassList("slide-hor-arrow");
            }
  
        }
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

        if (buttonSliderHorizontal != null)
        {
            buttonSliderHorizontal.RemoveFromClassList("slide-hor-arrow");
            buttonSliderHorizontal.AddToClassList("slide-hor");
        }

        var indexImage = rootGroupBox.Q<VisualElement>(null, "ImageIndex");
        if (indexImage != null) indexImage.transform.rotation = Quaternion.Euler(0, 0, 0);

        var buttonSlider = rootGroupBox.Q<VisualElement>(null, "button-slider-empty");

        if(buttonSlider != null)
        {
            buttonSlider.RemoveFromClassList("button-slider-empty");
            buttonSlider.AddToClassList("button-slider");
        }
       
    }

    private void RotateCellHorizontal()
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            row.AddToClassList("rotate_imgPlus90");

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
