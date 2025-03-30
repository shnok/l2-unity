// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/LinearMixerTransitionDrawer
    [CustomPropertyDrawer(typeof(LinearMixerTransition), true)]
    public class LinearMixerTransitionDrawer : MixerTransitionDrawer
    {
        /************************************************************************************************************************/

        private static GUIContent _SortingErrorContent;
        private static GUIStyle _SortingErrorStyle;

        /// <inheritdoc/>
        protected override void DoThresholdGUI(Rect area, int index)
        {
            var color = GUI.color;

            var iconArea = default(Rect);

            if (index > 0)
            {
                var previousThreshold = CurrentThresholds.GetArrayElementAtIndex(index - 1);
                var currentThreshold = CurrentThresholds.GetArrayElementAtIndex(index);
                if (previousThreshold.floatValue >= currentThreshold.floatValue)
                {
                    iconArea = AnimancerGUI.StealFromRight(
                        ref area,
                        area.height,
                        AnimancerGUI.StandardSpacing);

                    GUI.color = AnimancerGUI.ErrorFieldColor;
                }
            }

            base.DoThresholdGUI(area, index);

            if (iconArea != default)
            {
                _SortingErrorContent ??= new(AnimancerIcons.Error)
                {
                    tooltip =
                        "Linear Mixer Thresholds must always be unique" +
                        " and sorted in ascending order (click to sort)"
                };

                _SortingErrorStyle ??= new(GUI.skin.label)
                {
                    padding = new(),
                };

                if (GUI.Button(iconArea, _SortingErrorContent, _SortingErrorStyle))
                {
                    AnimancerGUI.Deselect();
                    Serialization.RecordUndo(Context.Property);
                    ((LinearMixerTransition)Context.Transition).SortByThresholds();
                }
            }

            GUI.color = color;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void AddThresholdFunctionsToMenu(GenericMenu menu)
        {
            const string EvenlySpaced = "Evenly Spaced";

            var count = CurrentThresholds.arraySize;
            if (count <= 1)
            {
                menu.AddDisabledItem(new(EvenlySpaced));
            }
            else
            {
                var first = CurrentThresholds.GetArrayElementAtIndex(0).floatValue;
                var last = CurrentThresholds.GetArrayElementAtIndex(count - 1).floatValue;

                if (last == first)
                    last++;

                AddPropertyModifierFunction(menu, $"{EvenlySpaced} ({first} to {last})", _ =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        CurrentThresholds.GetArrayElementAtIndex(i).floatValue =
                            Mathf.Lerp(first, last, i / (float)(count - 1));
                    }
                });
            }

            AddCalculateThresholdsFunction(menu, "From Speed",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.magnitude
                : float.NaN);
            AddCalculateThresholdsFunction(menu, "From Velocity X",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.x
                : float.NaN);
            AddCalculateThresholdsFunction(menu, "From Velocity Y",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.y
                : float.NaN);
            AddCalculateThresholdsFunction(menu, "From Velocity Z",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.z
                : float.NaN);
            AddCalculateThresholdsFunction(menu, "From Angular Speed (Rad)",
                (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed)
                ? speed
                : float.NaN);
            AddCalculateThresholdsFunction(menu, "From Angular Speed (Deg)",
                (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed)
                ? speed * Mathf.Rad2Deg
                : float.NaN);
        }

        /************************************************************************************************************************/

        private void AddCalculateThresholdsFunction(
            GenericMenu menu,
            string label,
            Func<Object, float, float> calculateThreshold)
        {
            AddPropertyModifierFunction(menu, label, (property) =>
            {
                var count = CurrentAnimations.arraySize;
                for (int i = 0; i < count; i++)
                {
                    var state = CurrentAnimations.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (state == null)
                        continue;

                    var threshold = CurrentThresholds.GetArrayElementAtIndex(i);
                    var value = calculateThreshold(state, threshold.floatValue);
                    if (!float.IsNaN(value))
                        threshold.floatValue = value;
                }
            });
        }

        /************************************************************************************************************************/
    }
}

#endif

