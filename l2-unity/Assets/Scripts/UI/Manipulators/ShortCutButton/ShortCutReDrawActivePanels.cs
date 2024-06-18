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
                NewPosition(rootDiffPosition.x, rootDiffPosition.y,  arrShowPanels);
            }
            
        }
        else
        {

        }
    }

    private void NewPosition(float x_root, float y_root, VisualElement[] arrayPanels)
    {
        for (int i = 0; i < arrayPanels.Length; i++)
        {
            if(arrayPanels != null)
            {
                //44 ширина short cut
                //25 отступ от верхней границы экрана
                if (i == 0)
                {
                    arrayPanels[i].transform.position = new Vector2(x_root - 51, y_root);
                }
                else
                {
                    Vector2 position = new Vector2(arrayPanels[i - 1].worldBound.position.x - 45, arrayPanels[i - 1].worldBound.position.y);
                    arrayPanels[i].transform.position = new Vector2(position.x, position.y);
                }
               
            }

        }

    }
}
