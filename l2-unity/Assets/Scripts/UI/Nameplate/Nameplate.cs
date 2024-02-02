using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Nameplate {

    private VisualElement _nameplateEle;
    private VisualElement _leftBubbleEle;
    private VisualElement _rightBubbleEle;
    private Label _nameplateEntityName;
    private Label _nameplateEntityTitle;

    [SerializeField] private int _targetId;
    [SerializeField] private string _title;
    [SerializeField] private string _name;
    [SerializeField] private float _nameplateOffsetHeight;
    [SerializeField] private Transform _target;
    [SerializeField] private bool _visible;

    public VisualElement NameplateEle { get { return _nameplateEle; } set { _nameplateEle = value; } }
    public bool Visible { get { return _visible; } set { _visible = value; } }
    public Transform Target { get { return _target; } }
    public float NameplateOffsetHeight { get { return _nameplateOffsetHeight; } }

    public Nameplate(
        VisualElement visualElement, Label entityName, Label entityTitle, Transform target,
        string title, float nameplateHeight, string name, int targetId, bool visible) {
        _nameplateEle = visualElement;
        _nameplateEntityName = entityName;
        _nameplateEntityTitle = entityTitle;
        _target = target;
        _title = title;
        _nameplateOffsetHeight = nameplateHeight;
        _name = name;
        _targetId = targetId;
        _visible = visible;

        _nameplateEntityName.text = name;
        _nameplateEntityTitle.text = title;
    }

    public void SetStyle(string className) {
        if(_leftBubbleEle != null) {
            SetClassName(_leftBubbleEle, className);
        } else {
            _leftBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if(_rightBubbleEle != null) {
            SetClassName(_rightBubbleEle, className);
        } else {
            _rightBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleRight");
        }
    }

    public void RemoveStyle(string className) {
        if(_leftBubbleEle != null) {
            RemoveClassName(_leftBubbleEle, className);
        } else {
            _leftBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleLeft");
        }
        if(_rightBubbleEle != null) {
            RemoveClassName(_rightBubbleEle, className);
        } else {
            _rightBubbleEle = _nameplateEle.Q<VisualElement>("TargetBubbleRight");
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
