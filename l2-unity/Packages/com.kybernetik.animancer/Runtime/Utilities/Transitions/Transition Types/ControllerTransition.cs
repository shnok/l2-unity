// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerTransition_1
    [Serializable]
#if ! UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public abstract class ControllerTransition<TState> : Transition<TState>,
        IAnimationClipCollection,
        ICopyable<ControllerTransition<TState>>
        where TState : ControllerState
    {
        /************************************************************************************************************************/

        [SerializeField]
        private RuntimeAnimatorController _Controller;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="ControllerState.Controller"/> that will be used for the created state.
        /// </summary>
        /// <remarks>
        /// If you set this property and this transition has been played on multiple characters,
        /// you will need to call <see cref="Transition{T}.ReconcileMainObject(AnimancerGraph)"/>
        /// for each of them to create new states for the newly assigned object.
        /// </remarks>
        public RuntimeAnimatorController Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;

                if (BaseState != null)
                    ReconcileMainObject(BaseState);
            }
        }

        /// <inheritdoc/>
        public override Object MainObject => _Controller;

#if UNITY_EDITOR
        /// <summary>[Editor-Only] The name of the serialized backing field of <see cref="Controller"/>.</summary>
        public const string ControllerFieldName = nameof(_Controller);
#endif

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("Configure parameters in the Animator Controller to follow the values of Animancer parameters")]
        private ControllerState.SerializableParameterBindings _ParameterBindings;

        /// <summary>[<see cref="SerializeField"/>]
        /// Parameter names which make parameters in the <see cref="RuntimeAnimatorController"/>
        /// follow the values of parameters in the <see cref="AnimancerGraph.Parameters"/>.
        /// </summary>
        public ref ControllerState.SerializableParameterBindings ParameterBindings
            => ref _ParameterBindings;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("Determines what each layer does when " +
            nameof(ControllerState) + "." + nameof(ControllerState.Stop) + " is called." +
            "\n• If empty, all layers will reset to their default state." +
            "\n• If this array is smaller than the layer count," +
            " any additional layers will use the last value in this array.")]
        private ControllerState.ActionOnStop[] _ActionsOnStop;

        /// <summary>[<see cref="SerializeField"/>]
        /// Determines what each layer does when <see cref="ControllerState.Stop"/> is called.
        /// </summary>
        /// <remarks>
        /// If empty, all layers will reset to their <see cref="ControllerState.ActionOnStop.DefaultState"/>.
        /// <para></para>
        /// If this array is smaller than the
        /// <see cref="UnityEngine.Animations.AnimatorControllerPlayable.GetLayerCount"/>,
        /// any additional layers will use the last value in this array.
        /// </remarks>
        public ref ControllerState.ActionOnStop[] ActionsOnStop
            => ref _ActionsOnStop;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override float MaximumDuration
        {
            get
            {
                if (_Controller == null)
                    return 0;

                var duration = 0f;

                var clips = _Controller.animationClips;
                for (int i = 0; i < clips.Length; i++)
                {
                    var length = clips[i].length;
                    if (duration < length)
                        duration = length;
                }

                return duration;
            }
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool IsValid
#if UNITY_EDITOR
            => _Controller != null;
#else
            => false;
#endif

        /************************************************************************************************************************/

        /// <summary>Returns the <see cref="Controller"/>.</summary>
        public static implicit operator RuntimeAnimatorController(ControllerTransition<TState> transition)
            => transition?._Controller;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            if (state is ControllerState controllerState)
                controllerState.ActionsOnStop = _ActionsOnStop;

            base.Apply(state);
        }

        /************************************************************************************************************************/

        /// <summary>Adds all clips in the <see cref="Controller"/> to the collection.</summary>
        void IAnimationClipCollection.GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            if (_Controller != null)
                clips.Gather(_Controller.animationClips);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public sealed override void CopyFrom(Transition<TState> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ControllerTransition<TState> copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _Controller = default;
                _ActionsOnStop = Array.Empty<ControllerState.ActionOnStop>();
                return;
            }

            _Controller = copyFrom._Controller;
            _ActionsOnStop = copyFrom._ActionsOnStop;
            _ParameterBindings = context.GetOrCreateClone(copyFrom._ParameterBindings);
        }

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/ControllerTransition
    [Serializable]
#if ! UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class ControllerTransition : ControllerTransition<ControllerState>,
        ICopyable<ControllerTransition>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override ControllerState CreateState()
        {
#if UNITY_ASSERTIONS
            if (Controller == null)
                throw new ArgumentException(
                    $"Unable to create {nameof(ControllerState)} because the" +
                    $" {nameof(ControllerTransition)}.{nameof(Controller)} is null.");
#endif

            return State = new(Controller, ActionsOnStop)
            {
                SerializedParameterBindings = ParameterBindings
            };
        }

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerTransition"/>.</summary>
        public ControllerTransition() { }

        /// <summary>Creates a new <see cref="ControllerTransition"/> with the specified Animator Controller.</summary>
        public ControllerTransition(RuntimeAnimatorController controller)
            => Controller = controller;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ControllerTransition"/> with the specified Animator Controller.</summary>
        public static implicit operator ControllerTransition(RuntimeAnimatorController controller)
            => new(controller);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<ControllerState> Clone(CloneContext context)
            => new ControllerTransition();

        /// <inheritdoc/>
        public sealed override void CopyFrom(ControllerTransition<ControllerState> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(ControllerTransition copyFrom, CloneContext context)
            => base.CopyFrom(copyFrom, context);

        /************************************************************************************************************************/

        /// <summary>
        /// Returns a new <see cref="ControllerTransition"/>
        /// if the `target` is an <see cref="RuntimeAnimatorController"/>.
        /// </summary>
        [TryCreateTransition]
        public static ITransitionDetailed TryCreateTransition(Object target)
            => target is not RuntimeAnimatorController controller
            ? null
            : new ControllerTransition()
            {
                Controller = controller,
            };

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only] Validates that the `command` is targeting an asset.</summary>
        [UnityEditor.MenuItem("CONTEXT/" + nameof(RuntimeAnimatorController) + "/Create Transition Asset", validate = true)]
        private static bool ValidateCreateTransitionAsset(UnityEditor.MenuCommand command)
            => TryCreateTransitionAttribute.CanCreateAndSave(command.context);

        /// <summary>[Editor-Only] Tries to create an asset containing an appropriate transition for the `command`.</summary>
        [UnityEditor.MenuItem("CONTEXT/" + nameof(RuntimeAnimatorController) + "/Create Transition Asset")]
        private static void CreateTransitionAsset(UnityEditor.MenuCommand command)
            => TryCreateTransitionAttribute.TryCreateTransitionAsset(command.context, true);
#endif

        /************************************************************************************************************************/
    }
}

