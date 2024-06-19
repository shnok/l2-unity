using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutReDrawActivePanels
{

    public ShortCutReDrawActivePanels(ShortCutPanel shortcutpanel , ShortCutPanelMinimal shortcutminimal , ClickSliderShortCutManipulator clickArrow)
    {
        PreparationData(shortcutpanel, shortcutminimal , clickArrow);
    }

    private void PreparationData(ShortCutPanel shortcutpanel , ShortCutPanelMinimal shortcutminimal , ClickSliderShortCutManipulator clickArrow)
    {
        if (shortcutpanel.IsVertical())
        {
            Vector2 rootPosition = shortcutpanel.GetPositionRoot();
            int activePanels = clickArrow.GetShowPanels();


            if (activePanels > 0)
            {
                VisualElement[] arrShowPanels = shortcutminimal.GetActiveAllPanels(activePanels);
                Vector2 rootDiffPosition = new Vector2(rootPosition.x + 235, rootPosition.y - 226);
                NewPositionVercital(rootDiffPosition.x, rootDiffPosition.y,  arrShowPanels);
            }
            
        }
        else
        {
            Vector2 rootPosition = shortcutpanel.GetPositionRoot();
            int activePanels = clickArrow.GetShowPanels();

            if (activePanels > 0)
            {
                VisualElement[] arrShowPanels = shortcutminimal.GetActiveAllPanels(activePanels);
                Vector2 rootDiffPosition = new Vector2(rootPosition.x - 185, rootPosition.y + 190);
                NewPositionHorizontal(rootDiffPosition.x, rootDiffPosition.y, arrShowPanels);
            }

        }
    }

    private void NewPositionVercital(float x_root, float y_root, VisualElement[] arrayPanels)
    {
        for (int i = 0; i < arrayPanels.Length; i++)
        {
            var minimalPanel = arrayPanels[i];
            if (minimalPanel != null)
            {
               

                //44 only shortcutpanel
                //46 shortcupt + broder

                if (i == 0)
                {
                    minimalPanel.style.left = 0;
                    minimalPanel.style.top = 0;
                    minimalPanel.transform.position = new Vector2(x_root - 51, y_root);
                }
                else
                {
                    minimalPanel.style.left = 0;
                    minimalPanel.style.top = 0;
                    // Vector2 position = new Vector2(arrayPanels[i - 1].worldBound.position.x - 45, arrayPanels[i - 1].worldBound.position.y);
                    int sdvig = i ;
                    int diff = 46 * sdvig;
                    Vector2 position = new Vector2(x_root - diff, y_root);
                    minimalPanel.transform.position = new Vector2(position.x, position.y);
                }

            }

        }

    }

    private void NewPositionHorizontal(float x_root, float y_root, VisualElement[] arrayPanels)
    {
        try
        {
            for (int i = 0; i < arrayPanels.Length; i++)
            {
                var minimalPanel = arrayPanels[i];
                if (minimalPanel != null)
                {
                    //44 ширина short cut
                    //25 отступ от верхней границы экрана
                    if (i == 0)
                    {
                        minimalPanel.style.left = 0;
                        minimalPanel.style.top = 0;

                        minimalPanel.transform.position = new Vector2(x_root - 51, y_root);
                    }
                    else
                    {

                        minimalPanel.style.left = 0;
                        minimalPanel.style.top = 0;
                     
                        int sdvig = i ;
                        int diff = 46 * sdvig;

                        minimalPanel.transform.position = new Vector2(x_root - 51, y_root - diff);
                    }

                }

            }
        }
        catch (NullReferenceException er)
        {
            Debug.Log(er.ToString());
        }
        

    }
}
