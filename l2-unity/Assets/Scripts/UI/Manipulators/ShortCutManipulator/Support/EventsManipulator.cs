using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class EventsManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;
    private VisualElement target;

    public EventsManipulator(VisualElement _root  , VisualElement target)
    {
        this._root = _root;
        this.target = target;
    }

    public void setTarget(VisualElement target)
    {
        this.target = target;
    }

    
}
