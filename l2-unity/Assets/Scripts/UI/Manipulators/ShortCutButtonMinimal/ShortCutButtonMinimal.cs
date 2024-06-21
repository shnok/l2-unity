using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutButtonMinimal :  AbstracetShortCutButton
{
    private VisualElement panel_minimal;
    private int showPanelIndex = 0;
    private ShortCutPanelMinimal shortcutminimal;
    public ShortCutButtonMinimal(VisualElement panel_minimal, ShortCutPanelMinimal shortcutminimal, int showPanelIndex)
    {
        this.panel_minimal = panel_minimal;
        this.shortcutminimal = shortcutminimal;
        this.showPanelIndex = showPanelIndex;
    }
    public void RegisterButtonCallBackNext(VisualElement imageIndex, string buttonId)
    {
        var btn = panel_minimal.Q<Button>(buttonId);

        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            ++showPanelIndex;


         if (showPanelIndex >= shortcutminimal.GetArrImgNextButton().Length)
         {
              showPanelIndex = 0;
              SetImageNumber(imageIndex, showPanelIndex);
              shortcutminimal.NextPanelToRootPanel(panel_minimal , showPanelIndex);
         }
         else
         {
              SetImageNumber(imageIndex, showPanelIndex);
              shortcutminimal.NextPanelToRootPanel(panel_minimal, showPanelIndex);
         }

            Debug.Log("Click " + showPanelIndex);
        }, TrickleDown.TrickleDown);
    }

    public void RegisterButtonCallBackPreview(VisualElement imageIndex, string buttonId)
    {
        var btn = panel_minimal.Q<Button>(buttonId);

        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            --showPanelIndex;

            if (showPanelIndex < 0)
            {
                showPanelIndex = shortcutminimal.GetArrImgNextButton().Length - 1;
                SetImageNumber(imageIndex, showPanelIndex);
                shortcutminimal.NextPanelToRootPanel(panel_minimal , showPanelIndex);
            }
            else
            {
                SetImageNumber(imageIndex, showPanelIndex);
                shortcutminimal.NextPanelToRootPanel(panel_minimal, showPanelIndex);
            }
            Debug.Log("Click " + showPanelIndex);
        }, TrickleDown.TrickleDown);
    }


    public void SetImageNumber(VisualElement imageElement, int index)
    {
        if (index >= shortcutminimal.GetArrImgNextButton().Length)
        {
            SetImage(imageElement, GetImage(shortcutminimal.GetArrImgNextButton()[0]));
        }
        else
        {
            SetImage(imageElement, GetImage(shortcutminimal.GetArrImgNextButton()[index]));
        }
    }

   
}
