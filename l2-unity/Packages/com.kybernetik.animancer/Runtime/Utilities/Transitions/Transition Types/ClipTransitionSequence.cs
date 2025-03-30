// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if ! UNITY_EDITOR
#pragma warning disable CS0618 // Type or member is obsolete (for Animancer Events in Animancer Lite).
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <inheritdoc/>
    /// <summary>A group of <see cref="ClipTransition"/>s which play one after the other.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/ClipTransitionSequence
    /// 
    [Serializable]
    public class ClipTransitionSequence : ClipTransition,
        ICopyable<ClipTransitionSequence>
    {
        /************************************************************************************************************************/

        [DrawAfterEvents]
        [SerializeField]
        [Tooltip("The other transitions to play in order after the first one.")]
        private ClipTransition[] _Others = Array.Empty<ClipTransition>();

        /// <summary>[<see cref="SerializeField"/>] The transitions to play in order after the first one.</summary>
        /// <remarks>
        /// If you modify any individual items of this array at runtime without actually setting this property, you
        /// must call <see cref="InitializeEndEventChain"/> afterwards.
        /// </remarks>
        public ClipTransition[] Others
        {
            get => _Others;
            set
            {
                _Others = value;
                InitializeEndEventChain();
            }
        }

        /// <summary>The last of the <see cref="Others"/> (or <c>this</c> if there are none).</summary>
        public ClipTransition LastTransition
            => _Others.Length > 0
            ? _Others[^1]
            : this;

        /************************************************************************************************************************/

        private Action _OnEnd;

        /// <summary>Initializes the End Events of each of the <see cref="Others"/> to play the next one.</summary>
        public void InitializeEndEventChain()
        {
            if (_Others == null ||
                _Others.Length <= 0)
                return;

            _OnEnd ??= () => AnimancerEvent.Current.State.Layer.Play(_Others[0]);
            InitializeFirstEndEvent();

            // Assign each of the other end events, but this first one will be set by Apply.

            var previous = _Others[0];
            for (int i = 1; i < _Others.Length; i++)
            {
                var next = _Others[i];
                previous.Events.OnEnd += () => AnimancerEvent.Current.State.Layer.Play(next);
                previous = next;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If an end event is assigned other than the one to play the next transition,
        /// this method replaces it and move it to be the end event of the last transition instead.
        /// </summary>
        private void InitializeFirstEndEvent()
        {
            var onEnd = Events.OnEnd;
            if (onEnd == _OnEnd ||
                _Others == null ||
                _Others.Length <= 0)
                return;

            Events.OnEnd = _OnEnd;
            onEnd -= _OnEnd;
            _Others[^1].Events.OnEnd += onEnd;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ClipState CreateState()
        {
            if (_OnEnd == null)
                InitializeEndEventChain();
            else if (_Others.Length > 0)
                InitializeFirstEndEvent();

            return base.CreateState();
        }

        /************************************************************************************************************************/

        /// <summary>Is everything in this sequence valid?</summary>
        public override bool IsValid
        {
            get
            {
                if (!base.IsValid)
                    return false;

                for (int i = 0; i < _Others.Length; i++)
                    if (!_Others[i].IsValid)
                        return false;

                return true;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Is the last animation in this sequence looping?</summary>
        public override bool IsLooping
            => _Others.Length > 0
            ? LastTransition.IsLooping
            : base.IsLooping;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float Length
        {
            get
            {
                var length = base.Length;
                for (int i = 0; i < _Others.Length; i++)
                    length += _Others[i].Length;
                return length;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float MaximumDuration
        {
            get
            {
                var value = base.MaximumDuration;
                for (int i = 0; i < _Others.Length; i++)
                    value += _Others[i].MaximumDuration;
                return value;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float AverageAngularSpeed
        {
            get
            {
                var speed = base.AverageAngularSpeed;
                if (_Others.Length == 0)
                    return speed;

                var duration = base.MaximumDuration;
                speed *= duration;

                for (int i = 0; i < _Others.Length; i++)
                {
                    var other = _Others[i];
                    var otherSpeed = other.AverageAngularSpeed;
                    var otherDuration = other.MaximumDuration;
                    speed += otherSpeed * otherDuration;
                    duration += otherDuration;
                }

                return speed / duration;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector3 AverageVelocity
        {
            get
            {
                var velocity = base.AverageVelocity;

                if (_Others.Length == 0)
                    return velocity;

                var duration = base.MaximumDuration;
                velocity *= duration;

                for (int i = 0; i < _Others.Length; i++)
                {
                    var other = _Others[i];
                    var otherVelocity = other.AverageVelocity;
                    var otherDuration = other.MaximumDuration;
                    velocity += otherVelocity * otherDuration;
                    duration += otherDuration;
                }

                return velocity / duration;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Adds the <see cref="ClipTransition.Clip"/> of everything in this sequence to the collection.</summary>
        public override void GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            base.GatherAnimationClips(clips);
            for (int i = 0; i < _Others.Length; i++)
                _Others[i].GatherAnimationClips(clips);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<ClipState> Clone(CloneContext context)
            => new ClipTransitionSequence();

        /// <inheritdoc/>
        public sealed override void CopyFrom(ClipTransition copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ClipTransitionSequence copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _Others = Array.Empty<ClipTransition>();
                return;
            }

            AnimancerUtilities.CopyExactArray(copyFrom._Others, ref _Others);
        }

        /************************************************************************************************************************/

        /// <summary>Tries to calculate the <see cref="AnimancerState.Time"/> relative to the start of this sequence.</summary>
        /// <returns>
        /// True if this sequence contains a transition matching the `state`.
        /// Otherwise false, indicating that it isn't associated with this sequence.
        /// </returns>
        public bool TryGetCumulativeTime(AnimancerState state, out float time)
        {
            time = state.Time;

            var clip = state.Clip;
            var onEnd = state.SharedEvents?.OnEnd;
            if (Clip == clip && _OnEnd == onEnd)
                return true;

            time += Clip.length;

            for (int i = 0; i < _Others.Length; i++)
            {
                var other = _Others[i];
                if (other.Clip == clip && other.Events.OnEnd == onEnd)
                    return true;

                time += other.Length;
            }

            return false;
        }

        /************************************************************************************************************************/
        #region Events
        /************************************************************************************************************************/

        /// <summary>The <see cref="AnimancerEvent.Sequence.EndEvent"/> of the last transition in this sequence.</summary>
        public AnimancerEvent EndEvent
        {
            get => LastTransition.Events.EndEvent;
            set => LastTransition.Events.EndEvent = value;
        }

        /// <summary>The <see cref="AnimancerEvent.Sequence.OnEnd"/> of the last transition in this sequence.</summary>
        public Action OnEnd
        {
            get => LastTransition.Events.OnEnd;
            set => LastTransition.Events.OnEnd = value;
        }

        /************************************************************************************************************************/

        /// <summary>Adds an event at the specified time relative to the entire sequence.</summary>
        public void AddEvent(float time, bool normalized, Action callback)
        {
            // Convert time to Seconds.
            if (normalized)
                time *= Length;

            if (TryAddEvent(this, base.Length, ref time, callback))
                return;

            for (int i = 0; i < _Others.Length - 1; i++)
            {
                var other = _Others[i];
                if (TryAddEvent(other, other.Length, ref time, callback))
                    return;
            }

            AddEvent(LastTransition, time, callback);
        }

        /// <summary>
        /// Tries to add the `callback` as an event to the `transition` if the `time` is within the `length` and
        /// returns true if successful. Otherwise subtracts the `length` from the `time` and returns false so it can be
        /// tried in the next transition in the sequence.
        /// </summary>
        private static bool TryAddEvent(ClipTransition transition, float length, ref float time, Action callback)
        {
            if (time > length)
            {
                time -= length;
                return false;
            }

            AddEvent(transition, time, callback);
            return true;
        }

        /// <summary>
        /// Adds the `callback` as an event to the `transition` at the specified `time` (in seconds, starting from the
        /// <see cref="ClipTransition.NormalizedStartTime"/>).
        /// </summary>
        private static void AddEvent(ClipTransition transition, float time, Action callback)
        {
            var start = transition.NormalizedStartTime;
            if (float.IsNaN(start))
                start = AnimancerEvent.Sequence.GetDefaultNormalizedStartTime(start);

            time /= transition.Clip.length * (1 - start);
            time += start;

            transition.Events.Add(time, callback);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

