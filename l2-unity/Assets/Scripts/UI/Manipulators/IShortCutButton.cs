using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IShortCutButton
{

    string[] GetArrImgNextButton();
    void NextPanelToRootPanel(VisualElement rootGroupBox, int showPanelIndex);
}
