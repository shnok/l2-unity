// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/MixerTransition2DDrawer
    [CustomPropertyDrawer(typeof(MixerTransition2D), true)]
    public class MixerTransition2DDrawer : MixerTransitionDrawer
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="MixerTransition2DDrawer"/> using a wider
        /// `thresholdWidth` than usual to accomodate both the X and Y values.
        /// </summary>
        public MixerTransition2DDrawer()
            : base(StandardThresholdWidth * 2 + 20)
        { }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void AddThresholdFunctionsToMenu(GenericMenu menu)
        {
            AddCalculateThresholdsFunction(menu, "From Velocity/XY", (state, threshold) =>
            {
                if (AnimancerUtilities.TryGetAverageVelocity(state, out var velocity))
                    return new(velocity.x, velocity.y);
                else
                    return new(float.NaN, float.NaN);
            });

            AddCalculateThresholdsFunction(menu, "From Velocity/XZ", (state, threshold) =>
            {
                if (AnimancerUtilities.TryGetAverageVelocity(state, out var velocity))
                    return new(velocity.x, velocity.z);
                else
                    return new(float.NaN, float.NaN);
            });

            AddCalculateThresholdsFunctionPerAxis(menu, "From Speed",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.magnitude
                : float.NaN);
            AddCalculateThresholdsFunctionPerAxis(menu, "From Velocity X",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.x
                : float.NaN);
            AddCalculateThresholdsFunctionPerAxis(menu, "From Velocity Y",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.y
                : float.NaN);
            AddCalculateThresholdsFunctionPerAxis(menu, "From Velocity Z",
                (state, threshold) => AnimancerUtilities.TryGetAverageVelocity(state, out var velocity)
                ? velocity.z
                : float.NaN);
            AddCalculateThresholdsFunctionPerAxis(menu, "From Angular Speed (Rad)",
                (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed)
                ? speed
                : float.NaN);
            AddCalculateThresholdsFunctionPerAxis(menu, "From Angular Speed (Deg)",
                (state, threshold) => AnimancerUtilities.TryGetAverageAngularSpeed(state, out var speed)
                ? speed * Mathf.Rad2Deg
                : float.NaN);

            AddPropertyModifierFunction(menu, "Initialize 4 Directions", Initialize4Directions);
            AddPropertyModifierFunction(menu, "Initialize 8 Directions", Initialize8Directions);
        }

        /************************************************************************************************************************/

        private void Initialize4Directions(SerializedProperty property)
        {
            var oldSpeedCount = CurrentSpeeds.arraySize;

            CurrentAnimations.arraySize = CurrentThresholds.arraySize = CurrentSpeeds.arraySize = 5;
            CurrentThresholds.GetArrayElementAtIndex(0).vector2Value = default;
            CurrentThresholds.GetArrayElementAtIndex(1).vector2Value = Vector2.up;
            CurrentThresholds.GetArrayElementAtIndex(2).vector2Value = Vector2.right;
            CurrentThresholds.GetArrayElementAtIndex(3).vector2Value = Vector2.down;
            CurrentThresholds.GetArrayElementAtIndex(4).vector2Value = Vector2.left;

            InitializeSpeeds(oldSpeedCount);

            var type = property.FindPropertyRelative(MixerTransition2D.TypeField);
            type.enumValueIndex = (int)MixerTransition2D.MixerType.Directional;
        }

        /************************************************************************************************************************/

        private void Initialize8Directions(SerializedProperty property)
        {
            var oldSpeedCount = CurrentSpeeds.arraySize;

            CurrentAnimations.arraySize = CurrentThresholds.arraySize = CurrentSpeeds.arraySize = 9;
            CurrentThresholds.GetArrayElementAtIndex(0).vector2Value = default;
            CurrentThresholds.GetArrayElementAtIndex(1).vector2Value = Vector2.up;
            CurrentThresholds.GetArrayElementAtIndex(2).vector2Value = new(1, 1);
            CurrentThresholds.GetArrayElementAtIndex(3).vector2Value = Vector2.right;
            CurrentThresholds.GetArrayElementAtIndex(4).vector2Value = new(1, -1);
            CurrentThresholds.GetArrayElementAtIndex(5).vector2Value = Vector2.down;
            CurrentThresholds.GetArrayElementAtIndex(6).vector2Value = new(-1, -1);
            CurrentThresholds.GetArrayElementAtIndex(7).vector2Value = Vector2.left;
            CurrentThresholds.GetArrayElementAtIndex(8).vector2Value = new(-1, 1);

            InitializeSpeeds(oldSpeedCount);

            var type = property.FindPropertyRelative(MixerTransition2D.TypeField);
            type.enumValueIndex = (int)MixerTransition2D.MixerType.Directional;
        }

        /************************************************************************************************************************/

        private void AddCalculateThresholdsFunction(
            GenericMenu menu,
            string label,
            Func<Object, Vector2, Vector2> calculateThreshold)
        {
            var functionState = CurrentAnimations == null || CurrentThresholds == null
                ? MenuFunctionState.Disabled
                : MenuFunctionState.Normal;

            AddPropertyModifierFunction(menu, label, functionState, property =>
            {
                GatherSubProperties(property);

                if (CurrentAnimations == null ||
                    CurrentThresholds == null)
                    return;

                var count = CurrentAnimations.arraySize;
                for (int i = 0; i < count; i++)
                {
                    var state = CurrentAnimations.GetArrayElementAtIndex(i).objectReferenceValue;
                    if (state == null)
                        continue;

                    var threshold = CurrentThresholds.GetArrayElementAtIndex(i);
                    var value = calculateThreshold(state, threshold.vector2Value);
                    if (!AnimancerEditorUtilities.IsNaN(value))
                        threshold.vector2Value = value;
                }
            });
        }

        /************************************************************************************************************************/

        private void AddCalculateThresholdsFunctionPerAxis(GenericMenu menu, string label,
            Func<Object, float, float> calculateThreshold)
        {
            AddCalculateThresholdsFunction(menu, "X/" + label, 0, calculateThreshold);
            AddCalculateThresholdsFunction(menu, "Y/" + label, 1, calculateThreshold);
        }

        private void AddCalculateThresholdsFunction(GenericMenu menu, string label, int axis,
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

                    var value = threshold.vector2Value;
                    var newValue = calculateThreshold(state, value[axis]);
                    if (!float.IsNaN(newValue))
                        value[axis] = newValue;
                    threshold.vector2Value = value;
                }
            });
        }

        /************************************************************************************************************************/
    }
}

#endif

