// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if ! UNITY_EDITOR
#pragma warning disable CS0618 // Type or member is obsolete (for Animancer Events in Animancer Lite).
#endif

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ClipTransition
    [Serializable]
    public class ClipTransition : Transition<ClipState>,
        IMotion,
        IAnimationClipCollection,
        ICopyable<ClipTransition>
    {
        /************************************************************************************************************************/

        /// <summary>The name of the serialized backing field of <see cref="Clip"/>.</summary>
        public const string ClipFieldName = nameof(_Clip);

        [SerializeField, Tooltip("The animation to play")]
        private AnimationClip _Clip;

        /// <summary>[<see cref="SerializeField"/>] The animation to play.</summary>
        /// <remarks>
        /// If you set this property and this transition has been played on multiple characters,
        /// you will need to call <see cref="Transition{T}.ReconcileMainObject(AnimancerGraph)"/>
        /// for each of them to create new states for the newly assigned object.
        /// </remarks>
        public AnimationClip Clip
        {
            get => _Clip;
            set
            {
                Validate.AssertAnimationClip(value, false, $"set {nameof(ClipTransition)}.{nameof(Clip)}");
                _Clip = value;

                if (BaseState != null)
                    ReconcileMainObject(BaseState);
            }
        }

        /// <inheritdoc/>
        public override Object MainObject => _Clip;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.NormalizedStartTime)]
        [DefaultValue(float.NaN, 0f)]
        [AnimationTime(
            AnimationTimeAttribute.Units.Normalized,
            DisabledText = Strings.Tooltips.StartTimeDisabled)]
        private float _NormalizedStartTime = float.NaN;

        /// <inheritdoc/>
        public override float NormalizedStartTime
        {
            get => _NormalizedStartTime;
            set => _NormalizedStartTime = value;
        }

        /// <summary>
        /// If this transition will set the <see cref="AnimancerState.Time"/>, then it needs to use
        /// <see cref="FadeMode.FromStart"/>.
        /// </summary>
        public override FadeMode FadeMode
            => float.IsNaN(_NormalizedStartTime)
            ? FadeMode.FixedSpeed
            : FadeMode.FromStart;

        /************************************************************************************************************************/

        /// <summary>
        /// The length of the <see cref="Clip"/> (in seconds), accounting for the <see cref="NormalizedStartTime"/> and
        /// <see cref="AnimancerEvent.Sequence.NormalizedEndTime"/> (but not <see cref="Speed"/>).
        /// </summary>
        public virtual float Length
        {
            get
            {
                if (!IsValid)
                    return 0;

                var normalizedEndTime = Events.NormalizedEndTime;
                normalizedEndTime = !float.IsNaN(normalizedEndTime)
                    ? normalizedEndTime
                    : AnimancerEvent.Sequence.GetDefaultNormalizedEndTime(Speed);

                var normalizedStartTime = !float.IsNaN(_NormalizedStartTime)
                    ? _NormalizedStartTime
                    : AnimancerEvent.Sequence.GetDefaultNormalizedStartTime(Speed);

                return _Clip.length * (normalizedEndTime - normalizedStartTime);
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool IsValid => _Clip != null && !_Clip.legacy;

        /// <summary>[<see cref="ITransitionDetailed"/>] Is the <see cref="Clip"/> looping?</summary>
        public override bool IsLooping => _Clip != null && _Clip.isLooping;

        /// <inheritdoc/>
        public override float MaximumDuration => _Clip != null ? _Clip.length : 0;

        /// <inheritdoc/>
        public virtual float AverageAngularSpeed => _Clip != null ? _Clip.averageAngularSpeed : default;

        /// <inheritdoc/>
        public virtual Vector3 AverageVelocity => _Clip != null ? _Clip.averageSpeed : default;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ClipState CreateState()
        {
#if UNITY_ASSERTIONS
            if (_Clip == null)
                throw new ArgumentException(
                    $"Unable to create {nameof(ClipState)} because the" +
                    $" {nameof(ClipTransition)}.{nameof(Clip)} is null.");
#endif

            return State = new(_Clip);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            ApplyNormalizedStartTime(state, _NormalizedStartTime);
            base.Apply(state);
        }

        /************************************************************************************************************************/

        /// <summary>[<see cref="IAnimationClipCollection"/>] Adds the <see cref="Clip"/> to the collection.</summary>
        public virtual void GatherAnimationClips(ICollection<AnimationClip> clips)
            => clips.Gather(_Clip);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<ClipState> Clone(CloneContext context)
            => new ClipTransition();

        /// <inheritdoc/>
        public sealed override void CopyFrom(Transition<ClipState> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ClipTransition copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _Clip = default;
                _NormalizedStartTime = float.NaN;
                return;
            }

            _Clip = copyFrom._Clip;
            _NormalizedStartTime = copyFrom._NormalizedStartTime;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns a new <see cref="ClipTransition"/>
        /// if the `target` is an <see cref="AnimationClip"/>.
        /// </summary>
        [TryCreateTransition]
        public static ITransitionDetailed TryCreateTransition(Object target)
            => target is not AnimationClip clip
            ? null
            : new ClipTransition()
            {
                Clip = clip,
            };

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] Validates that the `command` is targeting an asset.</summary>
        [UnityEditor.MenuItem("CONTEXT/" + nameof(AnimationClip) + "/Create Transition Asset", validate = true)]
        private static bool ValidateCreateTransitionAsset(UnityEditor.MenuCommand command)
            => TryCreateTransitionAttribute.CanCreateAndSave(command.context);

        /// <summary>[Editor-Only] Tries to create an asset containing an appropriate transition for the `command`.</summary>
        [UnityEditor.MenuItem("CONTEXT/" + nameof(AnimationClip) + "/Create Transition Asset")]
        private static void CreateTransitionAsset(UnityEditor.MenuCommand command)
            => TryCreateTransitionAttribute.TryCreateTransitionAsset(command.context, true);
#endif

        /************************************************************************************************************************/
    }
}

