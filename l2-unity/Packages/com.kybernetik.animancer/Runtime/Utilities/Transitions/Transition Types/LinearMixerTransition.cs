// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/LinearMixerTransition
    [Serializable]
#if ! UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class LinearMixerTransition : MixerTransition<LinearMixerState, float>,
        ICopyable<LinearMixerTransition>
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("Should setting the Parameter above the highest threshold" +
            " increase the Speed of the mixer proportionally?")]
        private bool _ExtrapolateSpeed = true;

        /// <summary>[<see cref="SerializeField"/>]
        /// Should setting the <see cref="MixerState{TParameter}.Parameter"/> above the highest threshold
        /// increase the <see cref="AnimancerNode.Speed"/> of the mixer proportionally?
        /// </summary>
        public ref bool ExtrapolateSpeed => ref _ExtrapolateSpeed;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.MixerParameterBinding)]
        private StringAsset _ParameterName;

        /// <summary>[<see cref="SerializeField"/>] The <see cref="LinearMixerState.ParameterName"/>.</summary>
        public ref StringAsset ParameterName => ref _ParameterName;

        /************************************************************************************************************************/

        /// <summary>
        /// Are all <see cref="ManualMixerTransition{TMixer}.Animations"/> assigned and
        /// <see cref="MixerTransition{TMixer, TParameter}.Thresholds"/> unique and sorted in ascending order?
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (!base.IsValid)
                    return false;

                var previous = float.NegativeInfinity;

                var thresholds = Thresholds;
                for (int i = 0; i < thresholds.Length; i++)
                {
                    var threshold = thresholds[i];
                    if (threshold < previous)
                        return false;
                    else
                        previous = threshold;
                }

                return true;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override LinearMixerState CreateState()
        {
            State = new()
            {
                ParameterName = ParameterName,
            };
            InitializeState();
            return State;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            State.ExtrapolateSpeed = _ExtrapolateSpeed;
        }

        /************************************************************************************************************************/

        /// <summary>Sorts all states so that their thresholds go from lowest to highest.</summary>
        /// <remarks>This method uses Bubble Sort which is inefficient for large numbers of states.</remarks>
        public void SortByThresholds()
        {
            var thresholdCount = Thresholds.Length;
            if (thresholdCount <= 1)
                return;

            var speedCount = Speeds.Length;
            var syncCount = SynchronizeChildren.Length;

            var previousThreshold = Thresholds[0];

            for (int i = 1; i < thresholdCount; i++)
            {
                var threshold = Thresholds[i];
                if (threshold >= previousThreshold)
                {
                    previousThreshold = threshold;
                    continue;
                }

                Thresholds.Swap(i, i - 1);
                Animations.Swap(i, i - 1);

                if (i < speedCount)
                    Speeds.Swap(i, i - 1);

                if (i == syncCount && !SynchronizeChildren[i - 1])
                {
                    var sync = SynchronizeChildren;
                    Array.Resize(ref sync, ++syncCount);
                    sync[i - 1] = true;
                    sync[i] = false;
                    SynchronizeChildren = sync;
                }
                else if (i < syncCount)
                {
                    SynchronizeChildren.Swap(i, i - 1);
                }

                if (i == 1)
                {
                    i = 0;
                    previousThreshold = float.NegativeInfinity;
                }
                else
                {
                    i -= 2;
                    previousThreshold = Thresholds[i];
                }
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<LinearMixerState> Clone(CloneContext context)
            => new LinearMixerTransition();

        /// <inheritdoc/>
        public sealed override void CopyFrom(MixerTransition<LinearMixerState, float> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(LinearMixerTransition copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _ExtrapolateSpeed = true;
                return;
            }

            _ExtrapolateSpeed = copyFrom._ExtrapolateSpeed;
            _ParameterName = copyFrom._ParameterName;
        }

        /************************************************************************************************************************/
    }
}

