using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutReplacePanel: AbstractShortCutReplace
{
    private int sizeCell;
    private ShortCutChildrenModel arrayRowsPanels;
    private VisualElement shortCutPanelElements;
    public ShortCutReplacePanel(int sizeCell , ShortCutChildrenModel arrayRowsPanels)
    {
        this.sizeCell = sizeCell;
        this.arrayRowsPanels = arrayRowsPanels;
    }

    public void SetRootPanel(VisualElement shortCutPanelElements)
    {
        this.shortCutPanelElements = shortCutPanelElements;
    }
    public void SetImageNext(int sizeCell , VisualElement rootGroupBox, int activePanel)
    {
        if(activePanel == 0)
        {
            SetImage(shortCutPanelElements);
        }
        else
        {
            for (int cell = 0; cell <= sizeCell; cell++)
            {
                var row = rootGroupBox.Q(className: "row" + cell);
                var border = GetBorderRow(rootGroupBox, cell);
                SetImageNext(border, row, cell, activePanel);
            }

        }
     
    }


    public void SetImage(VisualElement rootGroupBox)
    {
        for (int cell = 0; cell <= sizeCell; cell++)
        {
            var row = rootGroupBox.Q(className: "row" + cell);
            var border = GetBorderRow(rootGroupBox, cell);
            SetRows(ShortCutPanel.Instance.GetShortCutChildrenModel() , border, row, cell);
        }
    }

  


}
