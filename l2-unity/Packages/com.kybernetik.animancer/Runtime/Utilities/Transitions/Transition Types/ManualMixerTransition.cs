// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ManualMixerTransition
    [Serializable]
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class ManualMixerTransition : ManualMixerTransition<ManualMixerState>,
        ICopyable<ManualMixerTransition>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ManualMixerState CreateState()
        {
            State = new();
            InitializeState();
            return State;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<ManualMixerState> Clone(CloneContext context)
            => new ManualMixerTransition();

        /// <inheritdoc/>
        public sealed override void CopyFrom(ManualMixerTransition<ManualMixerState> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ManualMixerTransition copyFrom, CloneContext context)
            => base.CopyFrom(copyFrom, context);

        /************************************************************************************************************************/
    }
}

