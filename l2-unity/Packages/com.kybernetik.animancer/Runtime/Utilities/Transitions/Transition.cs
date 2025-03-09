// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using Animancer.Units;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <summary>
    /// A serializable <see cref="ITransition"/> which can create a particular type of
    /// <see cref="AnimancerState"/> when passed into <see cref="AnimancerLayer.Play(ITransition)"/>.
    /// </summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">
    /// Transitions</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/Transition_1
    [Serializable]
    public abstract class Transition<TState> :
        IPolymorphic,
        ITransition<TState>,
        ITransitionDetailed,
        ICopyable<Transition<TState>>,
        ICloneable<Transition<TState>>
        where TState : AnimancerState
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.FadeDuration)]
        [AnimationTime(AnimationTimeAttribute.Units.Seconds, Rule = Validate.Value.IsNotNegative)]
        [DefaultFadeValue]
        private float _FadeDuration = AnimancerGraph.DefaultFadeDuration;

        /// <inheritdoc/>
        /// <remarks>[<see cref="SerializeField"/>]</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting the value to a negative number.</exception>
        public float FadeDuration
        {
            get => _FadeDuration;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        $"{nameof(FadeDuration)} must not be negative");

                _FadeDuration = value;
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.OptionalSpeed)]
        [AnimationSpeed(DisabledText = Strings.Tooltips.SpeedDisabled)]
        [DefaultValue(1f, -1f)]
        private float _Speed = 1;

        /// <summary>[<see cref="SerializeField"/>]
        /// Determines how fast the animation plays (1x = normal speed, 2x = double speed).
        /// </summary>
        /// <remarks>
        /// This sets the <see cref="AnimancerNodeBase.Speed"/> when this transition is played.
        /// </remarks>
        public float Speed
        {
            get => _Speed;
            set => _Speed = value;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        /// <remarks>Returns <c>false</c> unless overridden.</remarks>
        public virtual bool IsLooping => false;

        /// <inheritdoc/>
        public virtual float NormalizedStartTime
        {
            get => float.NaN;
            set { }
        }

        /// <inheritdoc/>
        public abstract float MaximumDuration { get; }

        /************************************************************************************************************************/

        [SerializeField, Tooltip(Strings.ProOnlyTag + "Events which will be triggered as the animation plays")]
        private AnimancerEvent.Sequence.Serializable _Events;

        /// <inheritdoc/>
        /// <remarks>This property returns the <see cref="AnimancerEvent.Sequence.Serializable.Events"/>.</remarks>
        public virtual AnimancerEvent.Sequence Events
        {
            get => (_Events ??= new()).Events;
            set => (_Events ??= new()).Events = value;
        }

        /// <inheritdoc/>
        public ref AnimancerEvent.Sequence.Serializable SerializedEvents
            => ref _Events;

        /************************************************************************************************************************/

        /// <summary>
        /// The state that was created by this object. Specifically, this is the state that was most recently
        /// passed into <see cref="Apply"/> (usually by <see cref="AnimancerGraph.Play(ITransition)"/>).
        /// <para></para>
        /// You can use <see cref="AnimancerStateDictionary.GetOrCreate(ITransition)"/> or
        /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/> to get or create the state for a
        /// specific object.
        /// <para></para>
        /// <see cref="State"/> is simply a shorthand for casting this to <typeparamref name="TState"/>.
        /// </summary>
        public AnimancerState BaseState { get; private set; }

        /************************************************************************************************************************/

        private TState _State;

        /// <summary>
        /// The state that was created by this object. Specifically, this is the state that was most recently
        /// passed into <see cref="Apply"/> (usually by <see cref="AnimancerGraph.Play(ITransition)"/>).
        /// </summary>
        /// 
        /// <remarks>
        /// You can use <see cref="AnimancerStateDictionary.GetOrCreate(ITransition)"/> or
        /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/>
        /// to get or create the state for a specific object.
        /// <para></para>
        /// This property is shorthand for casting the <see cref="BaseState"/> to <typeparamref name="TState"/>.
        /// </remarks>
        /// 
        /// <exception cref="InvalidCastException">
        /// The <see cref="BaseState"/> is not actually a <typeparamref name="TState"/>.
        /// This should only happen if a different type of state was created by something else
        /// and registered using the <see cref="Key"/>,
        /// causing this <see cref="AnimancerGraph.Play(ITransition)"/> to pass that state into
        /// <see cref="Apply"/> instead of calling <see cref="CreateState"/>
        /// to make the correct type of state.
        /// </exception>
        public TState State
        {
            get => _State ??= (TState)BaseState;
            protected set => BaseState = _State = value;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        /// <remarks>Returns <c>true</c> unless overridden.</remarks>
        public virtual bool IsValid
            => true;

        /// <summary>The <see cref="AnimancerState.Key"/> which the created state will be registered with.</summary>
        /// <remarks>Returns <c>this</c> unless overridden.</remarks>
        public virtual object Key
            => this;

        /// <inheritdoc/>
        /// <remarks>Returns <see cref="FadeMode.FixedSpeed"/> unless overridden.</remarks>
        public virtual FadeMode FadeMode
            => FadeMode.FixedSpeed;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public abstract TState CreateState();

        /// <inheritdoc/>
        AnimancerState ITransition.CreateState()
            => CreateAndInitializeState();

        /// <summary>Calls <see cref="CreateState"/> and assigns the <see cref="Events"/> to the state.</summary>
        public TState CreateAndInitializeState()
        {
            var state = CreateState();

            AnimancerState.SetExpectFade(state, _FadeDuration);

            State = state;

            state.SharedEvents = _Events;

            return state;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public virtual void Apply(AnimancerState state)
        {
#if UNITY_ASSERTIONS
            if (state.MainObject != MainObject)
            {
                OptionalWarning.MainObjectMismatch.Log(
                    $"A state.{nameof(MainObject)} doesn't match the transition.{nameof(MainObject)} being applied to it." +
                    $" transition.{nameof(ReconcileMainObject)} must be called for every state created by the transition" +
                    $" after its {nameof(MainObject)} is changed." +
                    $" This includes {nameof(ClipTransition)}.{nameof(ClipTransition.Clip)}," +
                    $" {nameof(ControllerTransition)}.{nameof(ControllerTransition.Controller)}, and" +
                    $" {nameof(PlayableAssetTransition)}.{nameof(PlayableAssetTransition.Asset)}" +
                    $"\n• State: {state}" +
                    $"\n• State.{nameof(MainObject)}: {state.MainObject}" +
                    $"\n• Transition.{nameof(MainObject)}: {MainObject}" +
                    $"\n• Component: {state.Graph?.Component}",
                    state.Graph?.Component);
            }
#endif

            if (_State != state)
            {
                _State = null;
                BaseState = state;
            }

            if (!float.IsNaN(_Speed))
                state.Speed = _Speed;
        }

        /************************************************************************************************************************/

        /// <summary>Applies the `normalizedStartTime` to the `state`.</summary>
        public static void ApplyNormalizedStartTime(AnimancerState state, float normalizedStartTime)
        {
            if (!float.IsNaN(normalizedStartTime))
                state.NormalizedTime = normalizedStartTime;
            else if (state.Weight == 0)
                state.NormalizedTime = AnimancerEvent.Sequence.GetDefaultNormalizedStartTime(state.Speed);
        }

        /************************************************************************************************************************/

        /// <summary>The <see cref="AnimancerState.MainObject"/> that the created state will have.</summary>
        public virtual Object MainObject { get; }

        /// <summary>The display name of this transition.</summary>
        public virtual string Name
        {
            get
            {
                var mainObject = MainObject;
                return mainObject != null ? mainObject.name : null;
            }
        }

        /// <summary>Returns the <see cref="Name"/> and type of this transition.</summary>
        public override string ToString()
        {
            var type = GetType().FullName;

            var name = Name;
            return name is null
                ? type :
                $"{name} ({type})";
        }

        /************************************************************************************************************************/

        /// <summary>
        /// If a state exists with its <see cref="AnimancerState.MainObject"/> not matching the
        /// <see cref="MainObject"/>, this method returns a new state for the correct object.
        /// </summary>
        /// <remarks>
        /// This method only applies to the state registered with the <see cref="Key"/> so
        /// if this transition is played on multiple different characters or used to create
        /// multiple states for the same character, this method must be called for each state.
        /// </remarks>
        public AnimancerState ReconcileMainObject(AnimancerGraph animancer)
            => animancer.States.TryGet(this, out var state)
            ? ReconcileMainObject(state)
            : null;

        /************************************************************************************************************************/

        /// <summary>
        /// If the <see cref="AnimancerState.MainObject"/> doesn't match the <see cref="MainObject"/>,
        /// this method returns a new state for the correct object.
        /// </summary>
        /// <remarks>
        /// If this transition is played on multiple different characters or used to create
        /// multiple states for the same character, this method must be called for each state.
        /// </remarks>
        public AnimancerState ReconcileMainObject(AnimancerState state)
        {
            var oldMainObject = state.MainObject;
            var newMainObject = MainObject;
            if (oldMainObject == newMainObject)
                return state;

#if UNITY_ASSERTIONS
            if (oldMainObject == null)
                Debug.LogError(
                    $"{state} had no {nameof(state.MainObject)} to change from.",
                    state.Graph?.Component as Object);
            if (oldMainObject == null)
                Debug.LogError(
                    $"{this} has no {nameof(MainObject)} to change to.",
                    state.Graph?.Component as Object);
#endif

            // Change the old state's key to its object so we can get it back later.
            state.Key = oldMainObject;

            // If there was already a state for the new object, give it the correct key.
            if (state.Graph.States.TryGet(newMainObject, out var existingState))
            {
                existingState.Key = Key;
                state = existingState;
            }
            else// Otherwise, create a state for the new object.
            {
                var layer = state.Layer;
                state = CreateState();
                state.Key = Key;
                state.SetParent(layer);
            }

            _State = null;
            BaseState = state;
            return state;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public abstract Transition<TState> Clone(CloneContext context);

        /// <inheritdoc/>
        public virtual void CopyFrom(Transition<TState> copyFrom, CloneContext context)
        {
            if (copyFrom == null)
            {
                _FadeDuration = AnimancerGraph.DefaultFadeDuration;
                _Events = default;
                _Speed = 1;
                return;
            }

            _FadeDuration = copyFrom._FadeDuration;
            _Speed = copyFrom._Speed;
            _Events = copyFrom._Events.Clone();
        }

        /************************************************************************************************************************/
    }
}

