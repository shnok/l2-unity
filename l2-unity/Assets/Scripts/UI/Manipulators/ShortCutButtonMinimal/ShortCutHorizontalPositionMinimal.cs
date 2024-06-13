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

    public void Start()
    {
        if (shortCutPanel.IsVertical())
        {
            for(int i =0; i < arrayPanels.Length; i++)
            {
                ChangePositionHorizontalRootGroupBox(arrayPanels[i]);
            }
            shortCutPanel.SetPositionVerical(false);
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

    private void ChangePositionHorizontalRootGroupBox(VisualElement rootPanel)
    {
        if (rootPanel != null)
        {
            rootPanel.transform.rotation = Quaternion.Euler(0, 0, -90);
            rootPanel.transform.position = new Vector2(-23, 55);
        }
       
    }

    private void ChangePositionVerticalRootGroupBox(VisualElement rootPanel)
    {
        if (rootPanel != null)
        {
            rootPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
    }
}
