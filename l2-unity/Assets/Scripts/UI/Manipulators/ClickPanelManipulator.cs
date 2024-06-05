using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI.Manipulators
{
    public class ClickPanelManipulation : PointerManipulator
    {
        private IconOverlay iconOverlay;
        private VisualElement target;
        private Vector2 target_vector;
        private Vector2 _startMousePosition;
        private Vector2 _startPosition;

        public ClickPanelManipulation(VisualElement target, IconOverlay iconOverlay) {
            this.iconOverlay = iconOverlay;
            this.target_vector = target.worldTransform.GetPosition();
            this.target = target;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            //Debug.Log("");
        }

        private void PointerDownHandler(PointerDownEvent evt)
        {
            var ve = (VisualElement)evt.currentTarget;
            iconOverlay.newPosition(new Vector2(ve.worldBound.position.x, ve.worldBound.position.y));
            iconOverlay.AddAnim(SkillTimeArray.GetArrayImage() , 0.0001f);
           
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            //Debug.Log("");
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {

          
            //Debug.Log("");
        }


    }
}