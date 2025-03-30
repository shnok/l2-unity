// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>A generic set of objects corresponding to up/right/down/left with diagonals as well.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalSet8_1
    /// 
    [AnimancerHelpUrl(typeof(DirectionalSet8<>))]
    public class DirectionalSet8<T> : DirectionalSet4<T>
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        [SerializeField]
        private T _UpRight;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction8.UpRight"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T UpRight
        {
            get => _UpRight;
            set
            {
                AssertAllowChanges();
                _UpRight = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _DownRight;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction8.DownRight"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T DownRight
        {
            get => _DownRight;
            set
            {
                AssertAllowChanges();
                _DownRight = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _DownLeft;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction8.DownLeft"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T DownLeft
        {
            get => _DownLeft;
            set
            {
                AssertAllowChanges();
                _DownLeft = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _UpLeft;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction8.UpLeft"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T UpLeft
        {
            get => _UpLeft;
            set
            {
                AssertAllowChanges();
                _UpLeft = value;
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
            => 8;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override string GetDirectionName(int direction)
            => ((Direction8)direction).ToString();

        /************************************************************************************************************************/

        /// <summary>Returns the object associated with the specified `direction`.</summary>
        public T Get(Direction8 direction)
            => direction switch
            {
                Direction8.Up => Up,
                Direction8.Right => Right,
                Direction8.Down => Down,
                Direction8.Left => Left,
                Direction8.UpRight => _UpRight,
                Direction8.DownRight => _DownRight,
                Direction8.DownLeft => _DownLeft,
                Direction8.UpLeft => _UpLeft,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /// <inheritdoc/>
        public override T Get(int direction)
            => Get((Direction8)direction);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override T Get(Vector2 direction)
        {
            var angle = Mathf.Atan2(direction.y, direction.x);
            var octant = Mathf.RoundToInt(8 * angle / (2 * Mathf.PI) + 8) % 8;
            return octant switch
            {
                0 => Right,
                1 => _UpRight,
                2 => Up,
                3 => _UpLeft,
                4 => Left,
                5 => _DownLeft,
                6 => Down,
                7 => _DownRight,
                _ => throw new ArgumentOutOfRangeException("Invalid octant"),
            };
        }

        /************************************************************************************************************************/

        /// <summary>Sets the object associated with the specified `direction`.</summary>
        public void Set(Direction8 direction, T value)
        {
            switch (direction)
            {
                case Direction8.Up: Up = value; break;
                case Direction8.Right: Right = value; break;
                case Direction8.Down: Down = value; break;
                case Direction8.Left: Left = value; break;
                case Direction8.UpRight: UpRight = value; break;
                case Direction8.DownRight: DownRight = value; break;
                case Direction8.DownLeft: DownLeft = value; break;
                case Direction8.UpLeft: UpLeft = value; break;
                default: throw AnimancerUtilities.CreateUnsupportedArgumentException(direction);
            }
        }

        /// <inheritdoc/>
        public override void Set(int direction, T value)
            => Set((Direction8)direction, value);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 GetDirection(int direction)
            => ((Direction8)direction).ToVector2();

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 Snap(Vector2 vector)
            => Directions.SnapToDirection8(vector);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

