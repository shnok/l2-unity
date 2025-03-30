// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using Unity.Collections;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// Replaces the default <see cref="AnimancerLayerMixerList"/>
    /// with a <see cref="WeightedMaskLayerList"/>.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/WeightedMaskLayers
    [AddComponentMenu(Strings.MenuPrefix + "Weighted Mask Layers")]
    [AnimancerHelpUrl(typeof(WeightedMaskLayers))]
    [DefaultExecutionOrder(-10000)]// Awake before anything else initializes Animancer.
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class WeightedMaskLayers : WeightedMaskLayersInternal
    {
        /************************************************************************************************************************/

        [SerializeField] private AnimancerComponent _Animancer;

        /// <summary>[<see cref="SerializeField"/>] The component to apply the layers to.</summary>
        public AnimancerComponent Animancer
            => _Animancer;

        /************************************************************************************************************************/

        /// <summary>Finds the <see cref="Animancer"/> reference if it was missing.</summary>
        protected virtual void OnValidate()
        {
            gameObject.GetComponentInParentOrChildren(ref _Animancer);
        }

        /************************************************************************************************************************/

        /// <summary>Initializes the <see cref="Layers"/> and applies the default group weights.</summary>
        protected virtual void Awake()
        {
            if (Definition == null ||
                !Definition.IsValid)
                return;

            if (_Animancer == null)
                TryGetComponent(out _Animancer);

            Layers = WeightedMaskLayerList.Create(_Animancer.Animator);
            _Animancer.InitializeGraph(Layers.Graph);

            Indices = Definition.CalculateIndices(Layers);

            SetWeights(0);
        }

        /************************************************************************************************************************/
    }
}
