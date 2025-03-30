// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>A generic set of objects corresponding to up/right/down/left.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalSet4_1
    /// 
    [AnimancerHelpUrl(typeof(DirectionalSet4<>))]
    public class DirectionalSet4<T> : DirectionalSet<T>
    {
        /************************************************************************************************************************/
        #region Fields and Properties
        /************************************************************************************************************************/

        [SerializeField]
        private T _Up;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction4.Up"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T Up
        {
            get => _Up;
            set
            {
                AssertAllowChanges();
                _Up = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _Right;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction4.Right"/>.</summary>
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

        [SerializeField]
        private T _Down;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction4.Down"/>.</summary>
        /// <exception cref="ArgumentException"><see cref="AllowChanges"/> was not called before setting this value.</exception>
        public T Down
        {
            get => _Down;
            set
            {
                AssertAllowChanges();
                _Down = value;
                AnimancerUtilities.SetDirty(this);
            }
        }

        /************************************************************************************************************************/

        [SerializeField]
        private T _Left;

        /// <summary>[<see cref="SerializeField"/>] The object for <see cref="Direction4.Left"/>.</summary>
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
        #endregion
        /************************************************************************************************************************/
        #region Directions
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int DirectionCount
            => 4;

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override string GetDirectionName(int direction)
            => ((Direction4)direction).ToString();

        /************************************************************************************************************************/

        /// <summary>Returns the object associated with the specified `direction`.</summary>
        public T Get(Direction4 direction)
            => direction switch
            {
                Direction4.Up => _Up,
                Direction4.Right => _Right,
                Direction4.Down => _Down,
                Direction4.Left => _Left,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /// <inheritdoc/>
        public override T Get(int direction)
            => Get((Direction4)direction);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override T Get(Vector2 direction)
        {
            if (direction.x >= 0)
            {
                if (direction.y >= 0)
                    return direction.x > direction.y ? _Right : _Up;
                else
                    return direction.x > -direction.y ? _Right : _Down;
            }
            else
            {
                if (direction.y >= 0)
                    return direction.x < -direction.y ? _Left : _Up;
                else
                    return direction.x < direction.y ? _Left : _Down;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Sets the object associated with the specified `direction`.</summary>
        public void Set(Direction4 direction, T value)
        {
            switch (direction)
            {
                case Direction4.Up: Up = value; break;
                case Direction4.Right: Right = value; break;
                case Direction4.Down: Down = value; break;
                case Direction4.Left: Left = value; break;
                default: throw AnimancerUtilities.CreateUnsupportedArgumentException(direction);
            }
        }

        /// <inheritdoc/>
        public override void Set(int direction, T value)
            => Set((Direction4)direction, value);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 GetDirection(int direction)
            => ((Direction4)direction).ToVector2();

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector2 Snap(Vector2 vector)
            => Directions.SnapToDirection4(vector);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

