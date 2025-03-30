// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;

namespace Animancer
{
    /// <summary>
    /// Fades the child weights of a <see cref="MixerState{TParameter}"/> to a new parameter value
    /// instead of fading the actual parameter.
    /// </summary>
    /// 
    /// <remarks>
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/parameters#smoothing">
    /// Smoothing</see>
    /// <para></para>
    /// <strong>Example:</strong>
    /// Imagine a Linear Mixer with thresholds 0, 1, 2 and child states A, B, C.
    /// If you fade its Parameter from 0 to 1 the states would go from A to B to C.
    /// But if you use this system instead, the states would go directly from A to C.
    /// <h2>Usage</h2>
    /// <code>
    /// [SerializeField] private AnimancerComponent _Animancer;
    /// [SerializeField] private LinearMixerTransition _Mixer;
    /// 
    /// public void FadeMixerTo(float parameter, float fadeDuration)
    /// {
    ///     _Mixer.State.FadeChildWeights(parameter, fadeDuration);
    /// }
    /// </code></remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerChildFade
    /// 
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public static class MixerChildFade
    {
        /************************************************************************************************************************/

        private static readonly List<float>
            ChildWeights = new();

        /************************************************************************************************************************/

        /// <summary>
        /// Fades the child weights of a <see cref="MixerState{TParameter}"/> to a new parameter value instead of fading
        /// the actual parameter.
        /// </summary>
        /// <remarks>See <see cref="MixerChildFade"/> for a usage example.</remarks>
        public static void FadeChildWeights<TParameter>(
            this MixerState<TParameter> mixer,
            TParameter parameter,
            float fadeDuration)
        {
            ChildWeights.Clear();

            var childCount = mixer.ChildCount;
            for (int i = 0; i < childCount; i++)
                ChildWeights.Add(mixer.GetChild(i).Weight);

            mixer.Parameter = parameter;
            if (!mixer.RecalculateWeights())
                return;

            var mixerPlayable = mixer.Playable;

            for (int i = 0; i < childCount; i++)
            {
                var child = mixer.GetChild(i);
                mixerPlayable.SetChildWeight(child, ChildWeights[i]);
                child.StartFade(Math.Max(child.TargetWeight, float.Epsilon), fadeDuration);
            }
        }

        /************************************************************************************************************************/
    }
}

