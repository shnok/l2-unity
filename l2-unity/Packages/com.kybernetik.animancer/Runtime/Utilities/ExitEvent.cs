// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;

namespace Animancer
{
    /// <summary>[Pro-Only]
    /// A callback to be triggered after an <see cref="AnimancerNode"/>
    /// either starts or finishes fading out to 0 <see cref="AnimancerNode.EffectiveWeight"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// The <see cref="AnimancerNode.EffectiveWeight"/> is only checked at the end of the animation update
    /// so if it's set multiple times in the same frame then the callback might not be triggered.
    /// <para></para>
    /// Most <see href="https://kybernetik.com.au/animancer/docs/manual/fsm">Finite State Machine</see>
    /// systems already have their own mechanism for notifying your code when a state is exited
    /// so this system is generally only useful when something like that is not already available.
    /// <para></para>
    /// <strong>Example:</strong> see the <see cref="ExitEvent(AnimancerNode, Action, bool)"/> constructor.
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/ExitEvent
    /// 
    public class ExitEvent : Updatable
    {
        /************************************************************************************************************************/

        private Action _Callback;

        /// <summary>The method to invoke when this event is triggered.</summary>
        public Action Callback
        {
            get => _Callback;
            set
            {
                _Callback = value;

                if (_Callback != null)
                    EnableIfActive();
                else
                    Disable();
            }
        }

        /************************************************************************************************************************/

        private AnimancerNode _Node;

        /// <summary>The target node which determines when to trigger this event.</summary>
        public AnimancerNode Node
        {
            get => _Node;
            set
            {
                _Node = value;

                if (_Node != null)
                    EnableIfActive();
                else
                    Disable();
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Should the <see cref="Callback"/> be invoked when the <see cref="Node"/> starts fading out?
        /// Otherwise, it will be invoked after the <see cref="AnimancerNode.EffectiveWeight"/> reaches 0.
        /// Default is <c>false</c>.
        /// </summary>
        public bool InvokeOnStartExiting { get; set; }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ExitEvent"/>.</summary>
        /// 
        /// <remarks>
        /// <strong>Example:</strong><code>
        /// private ExitEvent _OnStateExited;
        /// 
        /// void ExitEventExample(AnimancerComponent animancer, AnimationClip clip)
        /// {
        ///     var state = animancer.Play(clip);
        ///     
        ///     // One line initialization:
        ///     (_OnStateExited ??= new(state, OnStateExited)).Enable();
        ///     
        ///     // Or two lines:
        ///     _OnStateExited ??= new(state, OnStateExited);
        ///     _OnStateExited.Enable();
        /// }
        /// 
        /// private void OnStateExited()
        /// {
        ///     Debug.Log(_OnStateExited.State + " Exited");
        /// }
        /// </code></remarks>
        /// 
        public ExitEvent(
            AnimancerNode node,
            Action callback,
            bool invokeOnStartExiting = false)
        {
            _Node = node;
            _Callback = callback;
            InvokeOnStartExiting = invokeOnStartExiting;
        }

        /************************************************************************************************************************/

        /// <summary>Registers this event to start receiving updates.</summary>
        public void Enable()
        {
            if (_Callback != null)
                _Node?.Graph?.RequirePostUpdate(this);
        }

        /// <summary>
        /// Registers this event to start receiving updates if the
        /// <see cref="AnimancerNode.TargetWeight"/> is above 0 (i.e. it's not fading out).
        /// </summary>
        public void EnableIfActive()
        {
            if (_Callback != null &&
                _Node != null &&
                _Node.Graph != null &&
                _Node.TargetWeight > 0)
                _Node.Graph.RequirePostUpdate(this);
        }

        /************************************************************************************************************************/

        /// <summary>Cancels this event to stop receiving updates.</summary>
        public void Disable()
        {
            _Node?.Graph?.CancelPostUpdate(this);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Update()
        {
            if (!_Node.IsValid())
                return;

            if (InvokeOnStartExiting)
            {
                if (_Node.TargetWeight != 0)
                    return;
            }
            else
            {
                if (_Node.EffectiveWeight > 0)
                    return;
            }

            _Callback();
            Disable();
        }

        /************************************************************************************************************************/
    }
}

