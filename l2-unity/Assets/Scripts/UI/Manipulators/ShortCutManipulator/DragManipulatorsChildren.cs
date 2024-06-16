using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulatorsChildren : PointerManipulator
{
    private VisualElement rootShortCutPanel;

    private Vector2 _startMousePosition;
    private Vector2 _startPosition;
    private Vector2[] _childreStartPosition;
    private int activePanelIndex = 0;
    private VisualElement target;
    private VisualElement[] children;

    public DragManipulatorsChildren(VisualElement target, VisualElement rootShortCutPanel)
    {
        this.target = target;
        this.rootShortCutPanel = rootShortCutPanel;
    }

    public void setChildren(VisualElement[] children)
    {
        if(children != null)
        {
            this.children = children;
            this._childreStartPosition = new Vector2[children.Length];
        }
        
    }

    public void SetActivePanel(int index)
    {
        activePanelIndex = index;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
    }


    public void PointerDownHandler(PointerDownEvent evt)
    {
        if (evt.button == 0)
        {
            _startMousePosition = evt.position;
            _startPosition = rootShortCutPanel.layout.position + target.layout.position;
            SetChildrenStartPosition();
            target.CapturePointer(evt.pointerId);
            SetPointerChildren(evt.pointerId);
        }
        evt.StopPropagation();
    }

    private void SetChildrenStartPosition()
    {
        for(int i=0; i < _childreStartPosition.Length; i++)
        {
            _childreStartPosition[i] = children[i].layout.position + target.layout.position;
        }
    }
    private void SetPointerChildren(int pointerId)
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].ReleasePointer(pointerId);
        }
    }

    public Vector2 getPositionRoot()
    {
        return _startPosition;
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);

            float endX = _startPosition.x - diff.x;
            float endY = _startPosition.y - diff.y;

            rootShortCutPanel.style.left = endX;
            rootShortCutPanel.style.top = endY;

            AddDiffChildren(diff);
        }
        evt.StopPropagation();
    }

    private void AddDiffChildren(Vector2 diff)
    {
        for(int i = 0; i < children.Length; i++)
        {
            float endX2 = _childreStartPosition[i].x - diff.x;
            float endY2 = _childreStartPosition[i].y - diff.y;
            children[i].style.left = endX2;
            children[i].style.top = endY2;
        }
    }



    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
            SetPointerChildren(evt.pointerId);
        }
        evt.StopPropagation();
    }

  





}
