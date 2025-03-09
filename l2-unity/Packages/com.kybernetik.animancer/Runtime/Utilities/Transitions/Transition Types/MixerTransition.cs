// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using System;
using UnityEngine;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerTransition_2
    [Serializable]
#if ! UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public abstract class MixerTransition<TMixer, TParameter> : ManualMixerTransition<TMixer>,
        ICopyable<MixerTransition<TMixer, TParameter>>
        where TMixer : MixerState<TParameter>
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The parameter values at which each of the states are used and blended")]
        private TParameter[] _Thresholds;

        /// <summary>[<see cref="SerializeField"/>]
        /// The parameter values at which each of the states are used and blended.
        /// </summary>
        public ref TParameter[] Thresholds => ref _Thresholds;

        /// <summary>The name of the serialized backing field of <see cref="Thresholds"/>.</summary>
        public const string ThresholdsField = nameof(_Thresholds);

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The initial parameter value to give the mixer when first created")]
        private TParameter _DefaultParameter;

        /// <summary>[<see cref="SerializeField"/>]
        /// The initial parameter value to give the mixer when first created.
        /// </summary>
        public ref TParameter DefaultParameter => ref _DefaultParameter;

        /// <summary>The name of the serialized backing field of <see cref="DefaultParameter"/>.</summary>
        public const string DefaultParameterField = nameof(_DefaultParameter);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void InitializeState()
        {
            base.InitializeState();

            State.SetThresholds(_Thresholds);
            State.Parameter = _DefaultParameter;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public sealed override void CopyFrom(ManualMixerTransition<TMixer> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(MixerTransition<TMixer, TParameter> copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _DefaultParameter = default;
                _Thresholds = default;
                return;
            }

            _DefaultParameter = copyFrom._DefaultParameter;
            AnimancerUtilities.CopyExactArray(copyFrom._Thresholds, ref _Thresholds);
        }

        /************************************************************************************************************************/
    }
}

