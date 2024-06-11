using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShortCutButton
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
                SetImageNumber(imageIndex, shortCutPanel.GetShowPanelIndex());
            }
            else
            {
                SetImageNumber(imageIndex, shortCutPanel.GetShowPanelIndex());
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
                SetImageNumber(imageIndex, shortCutPanel.GetShowPanelIndex());
                shortCutPanel.NextPanelToRootPanel();
            }
            else
            {
                int currentIndex = shortCutPanel.GetShowPanelIndex();
                SetImageNumber(imageIndex, currentIndex);
                shortCutPanel.NextPanelToRootPanel();
            }


        }, TrickleDown.TrickleDown);
    }

    public void SetImageNumber(VisualElement imageElement, int index)
    {
        if (index >= shortCutPanel.GetArrImgNextButton().Length)
        {
            SetImage(imageElement, GetImage(shortCutPanel.GetArrImgNextButton()[0]));
            shortCutPanel.NextPanelToRootPanel();
        }
        else
        {
            SetImage(imageElement, GetImage(shortCutPanel.GetArrImgNextButton()[index]));
            shortCutPanel.NextPanelToRootPanel();
        }
    }

    private void SetImage(VisualElement imageElement, Texture2D imgSource1)
    {
        if (imgSource1) imageElement.style.backgroundImage = new StyleBackground(imgSource1);
    }

    public Texture2D GetImage(string path)
    {
        return Resources.Load<Texture2D>(path);
    }

}
