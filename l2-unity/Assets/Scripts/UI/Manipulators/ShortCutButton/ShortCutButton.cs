using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutButton : AbstracetShortCutButton
{
    private ShortCutPanel shortCutPanel;
    public ShortCutButton(ShortCutPanel shortCutPanel) {
        this.shortCutPanel = shortCutPanel;
    }

    public void RegisterButtonCallBackNext(VisualElement imageIndex, string buttonId)
    {
        var btn = shortCutPanel.GetShortCutPanelElements().Q<Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {

          //  ++showPanelIndex;
            int showPanelIndex = shortCutPanel.GetShowPanelIndex();
            shortCutPanel.SetPanelIndex(++showPanelIndex);

            if (shortCutPanel.GetShowPanelIndex() >= shortCutPanel.GetArrImgNextButton().Length)
            {
                shortCutPanel.SetPanelIndex(0);
                SetImageNumber(shortCutPanel , imageIndex, shortCutPanel.GetShowPanelIndex());
            }
            else
            {
                SetImageNumber(shortCutPanel , imageIndex, shortCutPanel.GetShowPanelIndex());
            }



        }, TrickleDown.TrickleDown);
    }

    public void RegisterButtonCallBackPreview(VisualElement imageIndex, string buttonId)
    {
        var btn = shortCutPanel.GetShortCutPanelElements().Q<UnityEngine.UIElements.Button>(buttonId);
        if (btn == null)
        {
            Debug.LogError(buttonId + " can't be found.");
            return;
        }

        btn.RegisterCallback<MouseDownEvent>(evt => {
            int showPanelIndex = shortCutPanel.GetShowPanelIndex();
            shortCutPanel.SetPanelIndex(--showPanelIndex);

            if (showPanelIndex < 0)
            {
                shortCutPanel.SetPanelIndex(shortCutPanel.GetArrImgNextButton().Length - 1);
                SetImageNumber(shortCutPanel , imageIndex, shortCutPanel.GetShowPanelIndex());
                shortCutPanel.NextPanelToRootPanel(null,0);
            }
            else
            {
                int currentIndex = shortCutPanel.GetShowPanelIndex();
                SetImageNumber(shortCutPanel , imageIndex, currentIndex);
                shortCutPanel.NextPanelToRootPanel(null, 0);
            }


        }, TrickleDown.TrickleDown);
    }





}
