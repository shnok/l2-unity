// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System.Collections.Generic;

namespace Animancer.FSM
{
    /// <summary>An object with a <see cref="Priority"/>.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/fsm/utilities#state-selectors">
    /// State Selectors</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/IPrioritizable
    /// 
    public interface IPrioritizable : IState
    {
        float Priority { get; }
    }

    /************************************************************************************************************************/

    public partial class StateMachine<TState>
    {
        /// <summary>A prioritised list of potential states for a <see cref="StateMachine{TState}"/> to enter.</summary>
        /// 
        /// <remarks>
        /// <strong>Documentation:</strong>
        /// <see href="https://kybernetik.com.au/animancer/docs/manual/fsm#state-selectors">
        /// State Selectors</see>
        /// <para></para>
        /// <strong>Example:</strong><code>
        /// public StateMachine&lt;CharacterState&gt; stateMachine;
        /// public CharacterState run;
        /// public CharacterState idle;
        /// 
        /// private readonly StateMachine&lt;CharacterState&gt;.StateSelector
        ///     Selector = new();
        /// 
        /// private void Awake()
        /// {
        ///     Selector.Add(1, run);
        ///     Selector.Add(0, idle);
        /// }
        /// 
        /// public void RunOrIdle()
        /// {
        ///     stateMachine.TrySetState(Selector.Values);
        ///     // The "run" state has the highest priority so this will enter it if "run.CanEnterState" returns true.
        ///     // Otherwise if "idle.CanEnterState" returns true it will enter that state instead.
        ///     // If neither allows the transition, nothing happens and "stateMachine.TrySetState" returns false.
        /// }
        /// </code></remarks>
        /// 
        /// https://kybernetik.com.au/animancer/api/Animancer.FSM/StateSelector
        /// 
        public class StateSelector : SortedList<float, TState>
        {
            public StateSelector() : base(ReverseComparer<float>.Instance) { }

            /// <summary>Adds the `state` to this selector with its <see cref="IPrioritizable.Priority"/>.</summary>
            public void Add<TPrioritizable>(TPrioritizable state)
                where TPrioritizable : TState, IPrioritizable
                => Add(state.Priority, state);
        }
    }

    /************************************************************************************************************************/

    /// <summary>An <see cref="IComparer{T}"/> which reverses the default comparison.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/ReverseComparer_1
    public class ReverseComparer<T> : IComparer<T>
    {
        /// <summary>The singleton instance.</summary>
        public static readonly ReverseComparer<T> Instance = new();

        /// <summary>No need to let users create other instances.</summary>
        private ReverseComparer() { }

        /// <summary>Uses <see cref="Comparer{T}.Default"/> with the parameters swapped.</summary>
        public int Compare(T x, T y) => Comparer<T>.Default.Compare(y, x);
    }
}

