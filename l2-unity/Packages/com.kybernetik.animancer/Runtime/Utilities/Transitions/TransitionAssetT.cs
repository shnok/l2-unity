// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/TransitionAsset_1
    [AnimancerHelpUrl(typeof(TransitionAsset<ITransitionDetailed>))]
    public class TransitionAsset<TTransition> : TransitionAssetBase
        where TTransition : ITransitionDetailed
    {
        /************************************************************************************************************************/

        [SerializeReference]
        private TTransition _Transition;

        /// <summary>[<see cref="SerializeReference"/>]
        /// The <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.
        /// </summary>
        /// <remarks>
        /// WARNING: the <see cref="Transition{TState}.State"/> holds the most recently played state, so
        /// if you're sharing this transition between multiple objects it will only remember one of them.
        /// <para></para>
        /// You can use <see cref="AnimancerStateDictionary.GetOrCreate(ITransition)"/>
        /// or <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/>
        /// to get or create the state for a specific object.
        /// </remarks>
        public TTransition Transition
        {
            get
            {
                AssertTransition();
                return _Transition;
            }
            set => _Transition = value;
        }

        /// <summary>Returns the <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.</summary>
        public override ITransitionDetailed GetTransition()
        {
            AssertTransition();
            return _Transition;
        }

        /************************************************************************************************************************/

        /// <summary>[Assert-Conditional]
        /// Throws a <see cref="NullReferenceException"/> if the <see cref="Transition"/> is null.
        /// </summary>
        [System.Diagnostics.Conditional(Strings.Assertions)]
        private void AssertTransition()
        {
            if (_Transition == null)
                throw new NullReferenceException(
                    $"'{name}' {nameof(Transition)} is null." +
                    $" {nameof(HasTransition)} can be used to check without triggering this error.");
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override bool IsValid => _Transition.IsValid();

        /// <inheritdoc/>
        public override float FadeDuration => _Transition.FadeDuration;

        /// <inheritdoc/>
        public override object Key => _Transition.Key;

        /// <inheritdoc/>
        public override FadeMode FadeMode => _Transition.FadeMode;

        /// <inheritdoc/>
        public override AnimancerState CreateState() => _Transition.CreateState();

        /// <inheritdoc/>
        public override void Apply(AnimancerState state)
        {
            _Transition.Apply(state);
            state.SetDebugName(this);
        }

        /// <inheritdoc/>
        public override void GetAnimationClips(List<AnimationClip> clips)
            => clips.GatherFromSource(_Transition);

        /************************************************************************************************************************/

        /// <summary>Is the <see cref="Transition"/> assigned (i.e. not <c>null</c>)?</summary>
        public bool HasTransition => _Transition != null;

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only]
        /// Assigns a default <typeparamref name="TTransition"/> to the <see cref="Transition"/> field.
        /// </summary>
        protected virtual void Reset()
        {
            _Transition = AnimancerReflection.CreateDefaultInstance<TTransition>();
        }
#endif

        /************************************************************************************************************************/
    }
}

