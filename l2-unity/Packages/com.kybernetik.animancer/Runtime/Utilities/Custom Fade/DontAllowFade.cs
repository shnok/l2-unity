// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if ! UNITY_EDITOR
#pragma warning disable CS0612 // Type or member is obsolete (for Layers in Animancer Lite).
#pragma warning disable CS0618 // Type or member is obsolete (for Layers in Animancer Lite).
#endif

using UnityEngine;

namespace Animancer
{
    /// <summary>An <see cref="IUpdatable"/> that cancels any fades and logs warnings when they occur.</summary>
    /// 
    /// <remarks>
    /// This is useful for <see cref="Sprite"/> based characters since fading does nothing for them.
    /// <para></para>
    /// You can also set the <see cref="AnimancerGraph.DefaultFadeDuration"/> to 0 so that you don't need to set it
    /// manually on all your transitions.
    /// <para></para>
    /// <strong>Example:</strong>
    /// <code>
    /// [SerializeField] private AnimancerComponent _Animancer;
    /// 
    /// protected virtual void Awake()
    /// {
    ///     // To only apply it only in the Unity Editor and Development Builds:
    ///     DontAllowFade.Assert(_Animancer);
    ///     
    ///     // Or to apply it at all times:
    ///     _Animancer.Graph.RequireUpdate(new DontAllowFade());
    /// }
    /// </code></remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/DontAllowFade
    /// 
    public class DontAllowFade : Updatable
    {
        /************************************************************************************************************************/

        /// <summary>[Assert-Conditional] Applies a <see cref="DontAllowFade"/> to `animancer`.</summary>
        [System.Diagnostics.Conditional(Strings.Assertions)]
        public static void Assert(AnimancerGraph animancer)
        {
#if UNITY_EDITOR
            var warnings = OptionalWarning.ProOnly.DisableTemporarily();
            animancer.RequirePreUpdate(new DontAllowFade());
            warnings.Enable();
#endif
        }

        /************************************************************************************************************************/

        /// <summary>If the `node` is fading, this methods logs a warning (Assert-Only) and cancels the fade.</summary>
        private static void Validate(AnimancerNode node)
        {
            if (node != null && node.FadeSpeed != 0)
            {
#if UNITY_ASSERTIONS
                Debug.LogWarning($"The following {node.GetType().Name} is fading even though " +
                    $"{nameof(DontAllowFade)} is active: {node.GetDescription()}",
                    node.Graph.Component as Object);
#endif

                node.Weight = node.TargetWeight;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="Validate"/> on all layers and their <see cref="AnimancerLayer.CurrentState"/>.</summary>
        public override void Update()
        {
            var layers = AnimancerGraph.Current.Layers;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];
                Validate(layer);
                Validate(layer.CurrentState);
            }
        }

        /************************************************************************************************************************/
    }
}

