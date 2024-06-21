using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutReplacePanelMinimal : AbstractShortCutReplace
{
    private int sizeCell;
    private ShortCutChildrenModel[] arrayRowsPanels;
    private VisualElement[] arrayPanels;

    public ShortCutReplacePanelMinimal(int sizeCell , ShortCutChildrenModel[] arrayRowsPanels)
    {
        this.sizeCell = sizeCell;
        this.arrayRowsPanels = arrayRowsPanels;
    }

    public void SetArrayPanels(VisualElement[] arrayPanels)
    {
        this.arrayPanels = arrayPanels;
    }

    public void SetImageNext(int sizeCell, VisualElement indexCurrentPanel, int activePanel)
    {
        //VisualElement rootGroupBox = arrayPanels[indexCurrentPanel];
        if (activePanel == 0)
        {
            SetImage(indexCurrentPanel);
        }
        else
        {
            for (int cell = 0; cell <= sizeCell; cell++)
            {
                var row = indexCurrentPanel.Q(className: "row" + cell);
                var border = GetBorderRow(indexCurrentPanel, cell);
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
            SetRows(ShortCutPanel.Instance.GetShortCutChildrenModel(), border, row, cell) ;
        }
    }

    


}
