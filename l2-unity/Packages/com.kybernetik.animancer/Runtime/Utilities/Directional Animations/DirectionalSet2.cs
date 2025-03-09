// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2022 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>A generic set of objects corresponding to left/right.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalSet2_1
    /// 
    [AnimancerHelpUrl(typeof(DirectionalSet2<>))]
    public class DirectionalSet2<T> : DirectionalSet<T>
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        [SerializeField]
        private T _Left;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction2.Left"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T Left
        {
            get => _Left;
            set
            {
                AssertAllowChanges();
                _Left = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _Right;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction2.Right"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T Right
        {
            get => _Right;
            set
            {
                AssertAllowChanges();
                _Right = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Directions
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int DirectionCount
            => 2;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override string GetDirectionName(int direction)
            => ((Direction2)direction).ToString();

        /************************************************************************************************************************/

        /// <summary>Returns the object associated with the specified `direction`.</summary>
        public T Get(Direction2 direction)
            => direction switch
            {
                Direction2.Left => _Left,
                Direction2.Right => _Right,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /// <inheritdoc/>
        public override T Get(int direction)
            => Get((Direction2)direction);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override T Get(Vector2 direction)
            => Get(direction.x);

        /// <summary>Negative `direction` returns <see cref="Left"/>, 0 or positive returns <see cref="Right"/>.</summary>
        public T Get(float direction)
            => direction < 0 ? _Left : _Right;

        /************************************************************************************************************************/

        /// <summary>Sets the object associated with the specified `direction`.</summary>
        public void Set(Direction2 direction, T value)
        {
            switch (direction)
            {
                case Direction2.Left: Left = value; break;
                case Direction2.Right: Right = value; break;
                default: throw AnimancerUtilities.CreateUnsupportedArgumentException(direction);
            }
        }

        /// <inheritdoc/>
        public override void Set(int direction, T value)
            => Set((Direction2)direction, value);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 GetDirection(int direction)
            => ((Direction2)direction).ToVector2();

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 Snap(Vector2 vector)
            => Directions.SnapToDirection2(vector);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

