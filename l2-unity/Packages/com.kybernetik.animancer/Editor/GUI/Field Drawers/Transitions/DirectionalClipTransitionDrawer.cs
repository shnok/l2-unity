// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/DirectionalClipTransitionDrawer
    [CustomPropertyDrawer(typeof(DirectionalClipTransition), true)]
    public class DirectionalClipTransitionDrawer : TransitionDrawer
    {
        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="DirectionalClipTransitionDrawer"/>.</summary>
        public DirectionalClipTransitionDrawer()
            : base(DirectionalClipTransition.AnimationSetField)
        { }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoChildPropertyGUI(
            ref Rect area,
            SerializedProperty rootProperty,
            SerializedProperty property,
            GUIContent label)
        {
            var width = area.width;

            var path = property.propertyPath;
            if (path.EndsWith($".{ClipTransition.ClipFieldName}"))
            {
                if (property.objectReferenceValue != null)
                {
                    var removeArea = AnimancerGUI.StealFromRight(
                        ref area, AnimancerGUI.LineHeight, AnimancerGUI.StandardSpacing);

                    var removeContent = AnimancerIcons.ClearIcon(
                        $"A {nameof(DirectionalClipTransition)}" +
                        $" will get its Clip from the Animation Set at runtime" +
                        $" so the Clip might as well be null until then.");

                    if (GUI.Button(removeArea, removeContent, AnimancerGUI.NoPaddingButtonStyle))
                        property.objectReferenceValue = null;
                }

                if (Context.Transition is DirectionalClipTransition directionalClipTransition &&
                    directionalClipTransition.AnimationSet != null)
                {
                    var dropdownArea = AnimancerGUI.StealFromRight(
                        ref area, area.height, AnimancerGUI.StandardSpacing);

                    if (GUI.Button(dropdownArea, GUIContent.none, EditorStyles.popup))
                        PickAnimation(property, directionalClipTransition);
                }
            }

            base.DoChildPropertyGUI(ref area, rootProperty, property, label);

            area.width = width;
        }

        /************************************************************************************************************************/

        /// <summary>Shows a context menu to choose an <see cref="AnimationClip"/> from the `source`.</summary>
        private void PickAnimation(SerializedProperty property, object source)
        {
            var menu = new GenericMenu();

            using (SetPool<AnimationClip>.Instance.Acquire(out var clips))
            {
                clips.GatherFromSource(source);
                if (clips.Count == 0)
                    return;

                property = property.Copy();

                foreach (var clip in clips)
                {
                    menu.AddPropertyModifierFunction(property, clip.name, true, modify =>
                    {
                        modify.objectReferenceValue = clip;
                    });
                }
            }

            menu.ShowAsContext();
        }

        /************************************************************************************************************************/
    }
}

#endif

