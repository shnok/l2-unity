// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;

namespace Animancer
{
    /// <summary>Extension methods for <see cref="FadeGroup"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/FadeGroupExtensions
    public static class FadeGroupExtensions
    {
        /************************************************************************************************************************/

        /// <summary>[Pro-Only]
        /// Assigns the `function` as the <see cref="FadeGroup.Easing"/> if the `fade` isn't <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <em>Animancer Lite ignores this feature in runtime builds.</em>
        /// <para></para>
        /// <strong>Example:</strong><code>
        /// void EasingExample(AnimancerComponent animancer, AnimationClip clip)
        /// {
        ///     // Start fading the animation normally.
        ///     AnimancerState state = animancer.Play(clip, 0.25f);
        ///     
        ///     // Then a custom Easing delegate to modify it.
        ///     state.FadeGroup.SetEasing(t => t * t);// Square the 0-1 value to start slow and end fast.
        ///     
        ///     // The Easing class has lots of standard mathematical curve functions.
        ///     state.FadeGroup.SetEasing(Easing.Sine.InOut);
        ///     
        ///     // Or you can use the Easing.Function enum.
        ///     state.FadeGroup.SetEasing(Easing.Function.SineInOut);
        /// }
        /// </code>
        /// </remarks>
        public static void SetEasing(this FadeGroup fade, Func<float, float> function)
        {
            if (fade != null)
                fade.Easing = function;
        }

        /************************************************************************************************************************/

        /// <summary>[Pro-Only]
        /// Assigns the <see cref="Easing.GetDelegate(Easing.Function)"/> as the
        /// <see cref="FadeGroup.Easing"/> if the `fade` isn't <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <em>Animancer Lite ignores this feature in runtime builds.</em>
        /// <para></para>
        /// <strong>Example:</strong><code>
        /// void EasingExample(AnimancerComponent animancer, AnimationClip clip)
        /// {
        ///     // Start fading the animation normally.
        ///     AnimancerState state = animancer.Play(clip, 0.25f);
        ///     
        ///     // Then a custom Easing delegate to modify it.
        ///     state.FadeGroup.SetEasing(t => t * t);// Square the 0-1 value to start slow and end fast.
        ///     
        ///     // The Easing class has lots of standard mathematical curve functions.
        ///     state.FadeGroup.SetEasing(Easing.Sine.InOut);
        ///     
        ///     // Or you can use the Easing.Function enum.
        ///     state.FadeGroup.SetEasing(Easing.Function.SineInOut);
        /// }
        /// </code>
        /// </remarks>
        public static void SetEasing(this FadeGroup fade, Easing.Function function)
        {
            if (fade != null)
                fade.Easing = function.GetDelegate();
        }

        /************************************************************************************************************************/
    }
}

