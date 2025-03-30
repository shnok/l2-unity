// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using static Animancer.Editor.AnimancerGUI;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/PlayableAssetTransitionDrawer
    [CustomPropertyDrawer(typeof(PlayableAssetTransition), true)]
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class PlayableAssetTransitionDrawer : TransitionDrawer
    {
        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="PlayableAssetTransitionDrawer"/>.</summary>
        public PlayableAssetTransitionDrawer()
            : base(PlayableAssetTransition.AssetField)
        { }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _CurrentAsset = null;

            var height = base.GetPropertyHeight(property, label);

            if (property.isExpanded)
            {
                var bindings = property.FindPropertyRelative(PlayableAssetTransition.BindingsField);
                if (bindings != null)
                {
                    bindings.isExpanded = true;
                    height -= StandardSpacing + LineHeight;
                }
            }

            return height;
        }

        /************************************************************************************************************************/

        private PlayableAsset _CurrentAsset;

        /// <inheritdoc/>
        protected override void DoMainPropertyGUI(
            Rect area,
            out Rect labelArea,
            SerializedProperty rootProperty,
            SerializedProperty mainProperty)
        {
            _CurrentAsset = mainProperty.objectReferenceValue as PlayableAsset;
            base.DoMainPropertyGUI(area, out labelArea, rootProperty, mainProperty);
        }

        /// <inheritdoc/>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(area, property, label);
            _CurrentAsset = null;
        }

        /// <inheritdoc/>
        protected override void DoChildPropertyGUI(
            ref Rect area,
            SerializedProperty rootProperty,
            SerializedProperty property,
            GUIContent label)
        {
            var path = property.propertyPath;
            if (path.EndsWith($".{PlayableAssetTransition.BindingsField}"))
            {
                DoBindingsGUI(ref area, property, label);
                return;
            }

            base.DoChildPropertyGUI(ref area, rootProperty, property, label);
        }

        /************************************************************************************************************************/

        private void DoBindingsGUI(
            ref Rect area,
            SerializedProperty property,
            GUIContent label)
        {
            var outputCount = GetOutputCount(out var outputEnumerator, out var firstBindingIsAnimation);

            // Bindings.
            property.Next(true);
            // Array.
            property.Next(true);
            // Array Size.
            DoBindingsCountGUI(area, property, label, outputCount, firstBindingIsAnimation, out var bindingCount);

            EditorGUI.indentLevel++;

            for (int i = 0; i < bindingCount; i++)
            {
                NextVerticalArea(ref area);

                if (!property.Next(false))
                {
                    EditorGUI.LabelField(area, "Binding Count Mismatch");
                    break;
                }
                // First Array Item.

                if (outputEnumerator != null && outputEnumerator.MoveNext())
                {
                    DoBindingGUI(area, property, label, outputEnumerator, i);
                }
                else
                {
                    var color = GUI.color;
                    GUI.color = WarningFieldColor;

                    EditorGUI.PropertyField(area, property, false);

                    GUI.color = color;
                }
            }

            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        private int GetOutputCount(
            out IEnumerator<PlayableBinding> outputEnumerator,
            out bool firstBindingIsAnimation)
        {
            var outputCount = 0;

            firstBindingIsAnimation = false;
            if (_CurrentAsset != null)
            {
                var outputs = _CurrentAsset.outputs;
                _CurrentAsset = null;
                outputEnumerator = outputs.GetEnumerator();

                while (outputEnumerator.MoveNext())
                {
                    PlayableAssetState.GetBindingDetails(
                        outputEnumerator.Current, out var _, out var _, out var isMarkers);
                    if (isMarkers)
                        continue;

                    if (outputCount == 0 && outputEnumerator.Current.outputTargetType == typeof(Animator))
                        firstBindingIsAnimation = true;

                    outputCount++;
                }

                outputEnumerator = outputs.GetEnumerator();
            }
            else outputEnumerator = null;

            return outputCount;
        }

        /************************************************************************************************************************/

        private void DoBindingsCountGUI(
            Rect area,
            SerializedProperty property,
            GUIContent label,
            int outputCount,
            bool firstBindingIsAnimation,
            out int bindingCount)
        {
            var color = GUI.color;

            var sizeArea = area;
            bindingCount = property.intValue;

            // Button to fix the number of bindings in the array.
            if (bindingCount != outputCount && !(bindingCount == 0 && outputCount == 1 && firstBindingIsAnimation))
            {
                GUI.color = WarningFieldColor;

                var labelText = label.text;
                var style = MiniButtonStyle;

                var countLabel = outputCount.ToStringCached();
                var fixSizeWidth = style.CalculateWidth(countLabel);
                var fixSizeArea = StealFromRight(
                    ref sizeArea, fixSizeWidth, StandardSpacing);
                if (GUI.Button(fixSizeArea, countLabel, style))
                    property.intValue = bindingCount = outputCount;

                label.text = labelText;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(sizeArea, property, label, false);

            if (EditorGUI.EndChangeCheck())
                bindingCount = property.intValue;

            GUI.color = color;
        }

        /************************************************************************************************************************/

        private void DoBindingGUI(
            Rect area,
            SerializedProperty property,
            GUIContent label,
            IEnumerator<PlayableBinding> outputEnumerator,
            int trackIndex)
        {
            CheckIfSkip:
            PlayableAssetState.GetBindingDetails(
                outputEnumerator.Current,
                out var name,
                out var bindingType,
                out var isMarkers);

            if (isMarkers)
            {
                outputEnumerator.MoveNext();
                goto CheckIfSkip;
            }

            label.text = name;

            var targetObject = property.serializedObject.targetObject;
            var allowSceneObjects =
                targetObject != null &&
                !EditorUtility.IsPersistent(targetObject);

            label = EditorGUI.BeginProperty(area, label, property);
            var fieldArea = area;
            var obj = property.objectReferenceValue;
            var objExists = obj != null;

            if (objExists)
                DoRemoveButtonIfNecessary(ref fieldArea, property, trackIndex, ref bindingType, ref obj);

            if (bindingType != null || objExists)
            {
                property.objectReferenceValue =
                    EditorGUI.ObjectField(fieldArea, label, obj, bindingType, allowSceneObjects);
            }
            else
            {
                EditorGUI.LabelField(fieldArea, label);
            }

            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private static void DoRemoveButtonIfNecessary(
            ref Rect area,
            SerializedProperty property,
            int trackIndex,
            ref Type bindingType,
            ref Object obj)
        {
            if (trackIndex == 0 && bindingType == typeof(Animator))
            {
                DoRemoveButton(ref area, property, ref obj,
                    "This Animation Track is the first Track" +
                    " so it will automatically control the Animancer output" +
                    " and likely doesn't need a binding.");
            }
            else if (bindingType == null)
            {
                DoRemoveButton(ref area, property, ref obj,
                    "This Track doesn't need a binding.");
                bindingType = typeof(Object);
            }
            else if (!bindingType.IsAssignableFrom(obj.GetType()))
            {
                DoRemoveButton(ref area, property, ref obj,
                    "This binding has the wrong type for this Track.");
            }
        }

        /************************************************************************************************************************/

        private static void DoRemoveButton(
            ref Rect area,
            SerializedProperty property,
            ref Object obj,
            string tooltip)
        {
            GUI.color = WarningFieldColor;

            var removeArea = StealFromRight(
                ref area,
                area.height,
                StandardSpacing);

            if (GUI.Button(
                removeArea,
                AnimancerIcons.ClearIcon(tooltip),
                NoPaddingButtonStyle))
                property.objectReferenceValue = obj = null;
        }

        /************************************************************************************************************************/
    }
}

#endif

