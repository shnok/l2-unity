// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/MixerTransition2D
    [Serializable]
#if ! UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class MixerTransition2D : MixerTransition<Vector2MixerState, Vector2>,
        ICopyable<MixerTransition2D>
    {
        /************************************************************************************************************************/

        /// <summary>
        /// A type of <see cref="ManualMixerState"/> which can be
        /// created by a <see cref="MixerTransition2D"/>.
        /// </summary>
        public enum MixerType
        {
            /// <summary><see cref="CartesianMixerState"/></summary>
            Cartesian,

            /// <summary><see cref="DirectionalMixerState"/></summary>
            Directional,
        }

        [SerializeField]
        private MixerType _Type;

        /// <summary>[<see cref="SerializeField"/>]
        /// The type of <see cref="ManualMixerState"/> that this transition will create.
        /// </summary>
        public ref MixerType Type => ref _Type;

        /// <summary>The name of the serialized backing field of <see cref="Type"/>.</summary>
        public const string TypeField = nameof(_Type);

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.MixerParameterBinding)]
        private StringAsset _ParameterNameX;

        /// <summary>[<see cref="SerializeField"/>] The <see cref="Vector2MixerState.ParameterNameX"/>.</summary>
        public ref StringAsset ParameterNameX => ref _ParameterNameX;

        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip(Strings.Tooltips.MixerParameterBinding)]
        private StringAsset _ParameterNameY;

        /// <summary>[<see cref="SerializeField"/>] The <see cref="Vector2MixerState.ParameterNameY"/>.</summary>
        public ref StringAsset ParameterNameY => ref _ParameterNameY;

        /************************************************************************************************************************/

        /// <summary>Creates and returns a new state depending on the <see cref="Type"/>.</summary>
        /// <remarks>
        /// Note that using methods like <see cref="AnimancerGraph.Play(ITransition)"/> will also call
        /// <see cref="ITransition.Apply"/>, so if you call this method manually you may want to call that method
        /// as well. Or you can just use <see cref="AnimancerUtilities.CreateStateAndApply"/>.
        /// <para></para>
        /// This method also assigns it as the <see cref="Transition{TState}.State"/>.
        /// </remarks>
        public override Vector2MixerState CreateState()
        {
            State = _Type switch
            {
                MixerType.Cartesian => new CartesianMixerState(),
                MixerType.Directional => new DirectionalMixerState(),
                _ => throw new ArgumentOutOfRangeException(nameof(_Type)),
            };

            State.ParameterNameX = ParameterNameX;
            State.ParameterNameY = ParameterNameY;

            InitializeState();
            return State;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Transition<Vector2MixerState> Clone(CloneContext context)
            => new MixerTransition2D();

        /// <inheritdoc/>
        public sealed override void CopyFrom(MixerTransition<Vector2MixerState, Vector2> copyFrom, CloneContext context)
            => this.CopyFromBase(copyFrom, context);

        /// <inheritdoc/>
        public virtual void CopyFrom(MixerTransition2D copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _Type = default;
                return;
            }

            _Type = copyFrom._Type;
            _ParameterNameX = copyFrom._ParameterNameX;
            _ParameterNameY = copyFrom._ParameterNameY;
        }

        /************************************************************************************************************************/
    }
}

