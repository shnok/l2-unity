// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A system for synchronizing the <see cref="AnimancerState.NormalizedTime"/>
    /// of animations within the same "group".
    /// </summary>
    /// 
    /// <remarks>
    /// <list type="number">
    /// <item>Store a <see cref="TimeSynchronizer{T}"/> in a field.</item>
    /// <item>Call any of the <see cref="StoreTime(AnimancerState)"/> methods before playing a new animation.</item>
    /// <item>Then call any of the <see cref="SyncTime(AnimancerState, T, float)"/> methods after playing the animation.</item>
    /// </list>
    /// <strong>Sample:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/samples/sprites/character#synchronization">
    /// Character Controller -> Synchronization</see>
    /// <code>
    /// // 1. Define your group type.
    /// // You could use strings or ints or whatever you want, but enums are often best.
    /// public enum AnimationGroup
    /// {
    ///     None,
    ///     Movement,
    /// }
    /// 
    /// [SerializeField] private AnimancerComponent _Animancer;
    /// 
    /// // 2. Store a TimeSynchronizer in a field.
    /// private readonly TimeSynchronizer&lt;AnimationGroup&gt;
    ///     TimeSynchronizer = new();
    /// 
    /// public AnimancerState Play(AnimationClip clip, AnimationGroup group)
    /// {
    ///     // 3. Call one of the StoreTime methods before playing a new animation.
    ///     TimeSynchronizer.StoreTime(_Animancer);
    ///     
    ///     // 4. Play an animation.
    ///     var state = _Animancer.Play(clip);
    ///     
    ///     // 5. Call one of the SyncTime methods after playing the animation.
    ///     // If the `group` was the same as the value from last time you called it,
    ///     // then the state's NormalizedTime will be set to the stored value.
    ///     TimeSynchronizer.SyncTime(state, group);
    ///     
    ///     return state;
    /// }
    /// </code></remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/TimeSynchronizer_1
    /// 
    public class TimeSynchronizer<T>
    {
        /************************************************************************************************************************/

        /// <summary>The group that the current animation is in.</summary>
        public T CurrentGroup { get; set; }

        /// <summary>Should synchronization be applied when the <see cref="CurrentGroup"/> is at its default value?</summary>
        /// <remarks>This is false by default so that the <c>default</c> group represents "ungrouped".</remarks>
        public bool SynchronizeDefaultGroup { get; set; }

        /// <summary>The state which the <see cref="NormalizedTime"/> came from (to avoid syncing with itself).</summary>
        public AnimancerState State { get; set; }

        /// <summary>The stored <see cref="AnimancerState.NormalizedTimeD"/>.</summary>
        public double NormalizedTime { get; set; } = double.NaN;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="TimeSynchronizer{T}"/>.</summary>
        public TimeSynchronizer()
        { }

        /// <summary>Creates a new <see cref="TimeSynchronizer{T}"/>.</summary>
        public TimeSynchronizer(T group, bool synchronizeDefaultGroup = false)
        {
            CurrentGroup = group;
            SynchronizeDefaultGroup = synchronizeDefaultGroup;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Stores the <see cref="AnimancerState.NormalizedTimeD"/> of the <see cref="AnimancerLayer.CurrentState"/>.
        /// </summary>
        public void StoreTime(AnimancerLayer layer)
            => StoreTime(layer.CurrentState);

        /// <summary>Stores the <see cref="AnimancerState.NormalizedTimeD"/> of the `state`.</summary>
        public void StoreTime(AnimancerState state)
            => StoreTime(state, state != null ? state.NormalizedTimeD : double.NaN);

        /************************************************************************************************************************/

        /// <summary>Sets the <see cref="State"/> and <see cref="NormalizedTime"/>.</summary>
        public void StoreTime(AnimancerState state, double normalizedTime)
        {
            State = state;
            NormalizedTime = normalizedTime;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Applies the <see cref="NormalizedTime"/> to the <see cref="AnimancerLayer.CurrentState"/>
        /// if the `group` matches the <see cref="CurrentGroup"/>.
        /// </summary>
        public bool SyncTime(AnimancerLayer layer, T group)
            => SyncTime(layer.CurrentState, group, Time.deltaTime);

        /// <summary>
        /// Applies the <see cref="NormalizedTime"/> to the <see cref="AnimancerLayer.CurrentState"/>
        /// if the `group` matches the <see cref="CurrentGroup"/>.
        /// </summary>
        public bool SyncTime(AnimancerLayer layer, T group, float deltaTime)
            => SyncTime(layer.CurrentState, group, deltaTime);

        /// <summary>
        /// Applies the <see cref="NormalizedTime"/> to the `state`
        /// if the `group` matches the <see cref="CurrentGroup"/>.
        /// </summary>
        public bool SyncTime(AnimancerState state, T group)
            => SyncTime(state, group, Time.deltaTime);

        /// <summary>
        /// Applies the <see cref="NormalizedTime"/> to the `state`
        ///  and returns true if the `group` matches the <see cref="CurrentGroup"/>.
        /// </summary>
        /// <remarks>
        /// If the `state` is the same one the time was stored from, this method does nothing and returns false.
        /// </remarks>
        public bool SyncTime(AnimancerState state, T group, float deltaTime)
        {
            if (state == null ||
                state == State ||
                double.IsNaN(NormalizedTime) ||
                !EqualityComparer<T>.Default.Equals(CurrentGroup, group) ||
                (!SynchronizeDefaultGroup && EqualityComparer<T>.Default.Equals(default, group)))
            {
                CurrentGroup = group;
                return false;
            }

            // Setting the Time forces it to stay at that value after the next animation update.
            // But we actually want it to keep playing, so we need to add deltaTime manually.
            state.MoveTime(NormalizedTime * state.Length + deltaTime * state.EffectiveSpeed, false);

            return true;
        }

        /************************************************************************************************************************/
    }
}

