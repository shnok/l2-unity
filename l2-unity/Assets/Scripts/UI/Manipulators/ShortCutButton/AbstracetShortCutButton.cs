using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstracetShortCutButton {

    public void SetImageNumber(IShortCutButton shortCut , VisualElement imageElement, int index)
    {
        if (index >= shortCut.GetArrImgNextButton().Length)
        {
            SetImage(imageElement, GetImage(shortCut.GetArrImgNextButton()[0]));
            shortCut.NextPanelToRootPanel(null,0);
        }
        else
        {
            SetImage(imageElement, GetImage(shortCut.GetArrImgNextButton()[index]));
            shortCut.NextPanelToRootPanel(null,0);
        }
    }

    public void SetImage(VisualElement imageElement, Texture2D imgSource1)
    {
        if (imgSource1) imageElement.style.backgroundImage = new StyleBackground(imgSource1);
    }


    public Texture2D GetImage(string path)
    {
        return Resources.Load<Texture2D>(path);
    }

}
