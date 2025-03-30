// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>A [<see cref="SerializableAttribute"/>] wrapper around an <see cref="TransitionAssetBase"/>.</summary>
    /// <remarks>
    /// This allows Transition Assets to be referenced inside [<see cref="SerializeReference"/>]
    /// fields which can't directly reference <see cref="UnityEngine.Object"/>s.
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/assets">
    /// Transition Assets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/TransitionAssetReference
    [Serializable]
    public class TransitionAssetReference :
        IAnimationClipSource,
        ICopyable<TransitionAssetReference>,
        IPolymorphic,
        ITransitionDetailed,
        IWrapper
    {
        /************************************************************************************************************************/

        [SerializeField]
        private TransitionAssetBase _Asset;

        /// <summary>[<see cref="SerializeField"/>] The wrapped Transition Asset.</summary>
        public ref TransitionAssetBase Asset
            => ref _Asset;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="TransitionAssetReference"/>.</summary>
        public TransitionAssetReference() { }

        /// <summary>Creates a new <see cref="TransitionAssetReference"/>.</summary>
        public TransitionAssetReference(TransitionAssetBase asset)
        {
            _Asset = asset;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        object IWrapper.WrappedObject
            => _Asset;

        /************************************************************************************************************************/

        /// <summary>Can this transition create a valid <see cref="AnimancerState"/>?</summary>
        public virtual bool IsValid
            => _Asset.IsValid();

        /// <inheritdoc/>
        public virtual float FadeDuration
            => _Asset != null
            ? _Asset.FadeDuration
            : 0;

        /// <inheritdoc/>
        public virtual object Key
            => _Asset != null
            ? _Asset.Key
            : null;

        /// <inheritdoc/>
        public virtual FadeMode FadeMode
            => _Asset != null
            ? _Asset.FadeMode
            : default;

        /// <inheritdoc/>
        public bool IsLooping
            => _Asset != null
            && _Asset.IsLooping;

        /// <inheritdoc/>
        public float NormalizedStartTime
        {
            get => _Asset != null
                ? _Asset.NormalizedStartTime
                : float.NaN;
            set => _Asset.NormalizedStartTime = value;// No null check. Don't silently ignore commands.
        }

        /// <inheritdoc/>
        public float MaximumDuration
            => _Asset != null
            ? _Asset.MaximumDuration
            : 0;

        /// <inheritdoc/>
        public float Speed
        {
            get => _Asset != null
                ? _Asset.Speed
                : 1;
            set => _Asset.Speed = value;// No null check. Don't silently ignore commands.
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        [Obsolete(TransitionAssetBase.ObsoleteEventsMessage)]
        public AnimancerEvent.Sequence Events
            => _Asset != null
            ? _Asset.Events
            : null;

        /// <inheritdoc/>
        [Obsolete(TransitionAssetBase.ObsoleteEventsMessage)]
        public ref AnimancerEvent.Sequence.Serializable SerializedEvents
            => ref _Asset.SerializedEvents;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public virtual AnimancerState CreateState()
            => _Asset.CreateState();

        /// <inheritdoc/>
        public virtual void Apply(AnimancerState state)
            => _Asset.Apply(state);

        /************************************************************************************************************************/

        /// <summary>[<see cref="IAnimationClipSource"/>]
        /// Calls <see cref="AnimancerUtilities.GatherFromSource(ICollection{AnimationClip}, object)"/>.
        /// </summary>
        public virtual void GetAnimationClips(List<AnimationClip> clips)
            => clips.GatherFromSource(_Asset);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public void CopyFrom(TransitionAssetReference copyFrom, CloneContext context)
        {
            if (copyFrom == null)
            {
                _Asset = default;
                return;
            }

            _Asset = copyFrom._Asset;
        }

        /************************************************************************************************************************/

        /// <summary>Describes the <see cref="Asset"/>.</summary>
        public override string ToString()
            => $"{nameof(TransitionAssetReference)}({_Asset})";

        /************************************************************************************************************************/
    }
}

