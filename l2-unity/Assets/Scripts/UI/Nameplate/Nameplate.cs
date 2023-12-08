using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Nameplate {
    public int targetId;
    public string title;
    public string name;
    public float nameplateOffsetHeight;
    public Label nameplateEntityName;
    public Label nameplateEntityTitle;
    public Transform target;
    public bool visible;

    public VisualElement nameplateEle;
    public VisualElement leftBubbleEle;
    public VisualElement rightBubbleEle;

    public void SetStyle(string className) {
        if(leftBubbleEle != null) {
            SetClassName(leftBubbleEle, className);
        } else {
            leftBubbleEle = nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if(rightBubbleEle != null) {
            SetClassName(rightBubbleEle, className);
        } else {
            rightBubbleEle = nameplateEle.Q<VisualElement>("TargetBubbleRight");
        }
    }

    public void RemoveStyle(string className) {
        if(leftBubbleEle != null) {
            RemoveClassName(leftBubbleEle, className);
        } else {
            leftBubbleEle = nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if(rightBubbleEle != null) {
            RemoveClassName(rightBubbleEle, className);
        } else {
            rightBubbleEle = nameplateEle.Q<VisualElement>("TargetBubbleRight");
        }
    }

    private void SetClassName(VisualElement element, string className) {
        if(!element.ClassListContains(className)) {
            element.AddToClassList(className);
        }
    }

    private void RemoveClassName(VisualElement element, string className) {
        if(element.ClassListContains(className)) {
            element.RemoveFromClassList(className);
        }
    }
}
