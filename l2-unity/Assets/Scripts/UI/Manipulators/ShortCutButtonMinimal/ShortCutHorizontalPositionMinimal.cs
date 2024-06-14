using System.Collections;
using System.Collections.Generic;
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

    public void Start(VisualElement rootGroupBox)
    {
        if (shortCutPanel.IsVertical())
        {
            for(int i =0; i < arrayPanels.Length; i++)
            {
                ChangePositionHorizontalRootGroupBox(arrayPanels[i]);
                RotateCellHorizontal(arrayPanels[i]);
            }
            shortCutPanel.SetPositionVerical(false);
            test(rootGroupBox);
        }
        else
        {
            for (int i = 0; i < arrayPanels.Length; i++)
            {
                ChangePositionVerticalRootGroupBox(arrayPanels[i]);
            }
            shortCutPanel.SetPositionVerical(true);
        }

    }

    private void test(VisualElement rootGroupBox)
    {
        var buttonSliderHorizontal = rootGroupBox.Q<VisualElement>(null, "slide-hor-arrow");
        ClickSliderShortCutManipulator slider_horizontal = new ClickSliderShortCutManipulator(ShortCutPanelMinimal.Instance, shortCutPanel.GetDrag(), shortCutPanel.IsVertical());
        buttonSliderHorizontal.AddManipulator(slider_horizontal);
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

    private void RotateCellHorizontal(VisualElement rootGroupBox)
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            row.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}
