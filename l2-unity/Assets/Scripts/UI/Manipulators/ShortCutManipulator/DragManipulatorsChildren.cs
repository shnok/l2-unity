using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulatorsChildren : PointerManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;
    private Vector2 _childreStartPosition;
    private VisualElement target;
    private VisualElement[] children;

    public DragManipulatorsChildren(VisualElement target, VisualElement root)
    {
        this.target = target;
        this._root = root;
    }

    public void setChildren(VisualElement[] children)
    {
        if(children != null)
        {
            this.children = children;
        }
        
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
            _startPosition = _root.layout.position + target.layout.position;
            _childreStartPosition = children[0].layout.position + target.layout.position;

            target.CapturePointer(evt.pointerId);
            children[0].ReleasePointer(evt.pointerId);
            children[1].ReleasePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);

            float endX = _startPosition.x - diff.x;
            float endY = _startPosition.y - diff.y;
            _root.style.left = endX;
            _root.style.top = endY;
            float endX2 = _childreStartPosition.x - diff.x;
            float endY2 = _childreStartPosition.y - diff.y;
            children[0].style.left = endX2;
            children[0].style.top = endY2;

           // children[1].style.left = endX2;
           // children[1].style.top = endY;
            // changePositionChildren(endX, endY);
        }
        evt.StopPropagation();
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
            children[0].ReleasePointer(evt.pointerId);
            children[1].ReleasePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }

    private void changePositionChildren(float endX, float endY)
    {
        Vector2 vector2 = new Vector2(_root.worldBound.position.x - 30, _root.worldBound.position.y);
        Vector2 vector = new Vector2(_root.worldBound.position.x - 22, _root.worldBound.position.y);
         for(int i =0; i < children.Length; i++)
         {
            if (i == 0)
            {
                children[0].transform.position = vector2;
            }
            else
            {
                children[1].transform.position = vector2;
            }
         }

    }


}
