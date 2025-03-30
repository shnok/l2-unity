// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>Left or Right.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/Direction2
    /// 
    public enum Direction2
    {
        /************************************************************************************************************************/

        /// <summary><see cref="Vector2.left"/>.</summary>
        Left,

        /// <summary><see cref="Vector2.right"/>.</summary>
        Right,

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    /// <summary>Up, Right, Down, or Left.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/Direction4
    /// 
    public enum Direction4
    {
        /************************************************************************************************************************/

        /// <summary><see cref="Vector2.up"/>.</summary>
        Up,

        /// <summary><see cref="Vector2.right"/>.</summary>
        Right,

        /// <summary><see cref="Vector2.down"/>.</summary>
        Down,

        /// <summary><see cref="Vector2.left"/>.</summary>
        Left,

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    /// <summary>Up, Right, Down, Left, or their diagonals.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/Direction8
    /// 
    public enum Direction8
    {
        /// <summary><see cref="Vector2.up"/>.</summary>
        Up,

        /// <summary><see cref="Vector2.right"/>.</summary>
        Right,

        /// <summary><see cref="Vector2.down"/>.</summary>
        Down,

        /// <summary><see cref="Vector2.left"/>.</summary>
        Left,

        /// <summary><see cref="Vector2"/>(0.7..., 0.7...).</summary>
        UpRight,

        /// <summary><see cref="Vector2"/>(0.7..., -0.7...).</summary>
        DownRight,

        /// <summary><see cref="Vector2"/>(-0.7..., -0.7...).</summary>
        DownLeft,

        /// <summary><see cref="Vector2"/>(-0.7..., 0.7...).</summary>
        UpLeft,
    }

    /************************************************************************************************************************/

    /// <summary>Utilities relating to <see cref="Direction4"/> and <see cref="Direction8"/>.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/Directions
    /// 
    public static class Directions
    {
        /************************************************************************************************************************/

        /// <summary>1 / (Square Root of 2).</summary>
        public const float OneOverSqrt2 = 0.70710678118f;

        /// <summary>A vector with a magnitude of 1 pointing up to the right.</summary>
        /// <remarks>The value is approximately (0.7, 0.7).</remarks>
        public static Vector2 UpRight => new(OneOverSqrt2, OneOverSqrt2);

        /// <summary>A vector with a magnitude of 1 pointing down to the right.</summary>
        /// <remarks>The value is approximately (0.7, -0.7).</remarks>
        public static Vector2 DownRight => new(OneOverSqrt2, -OneOverSqrt2);

        /// <summary>A vector with a magnitude of 1 pointing down to the left.</summary>
        /// <remarks>The value is approximately (-0.7, -0.7).</remarks>
        public static Vector2 DownLeft => new(-OneOverSqrt2, -OneOverSqrt2);

        /// <summary>A vector with a magnitude of 1 pointing up to the left.</summary>
        /// <remarks>The value is approximately (-0.707, 0.707).</remarks>
        public static Vector2 UpLeft => new(-OneOverSqrt2, OneOverSqrt2);

        /************************************************************************************************************************/

        /// <summary>Returns the opposite of the given `direction`.</summary>
        public static Direction2 GetOppositeDirection(this Direction2 direction)
            => (Direction2)(1 - direction);

        /// <summary>Returns the opposite of the given `direction`.</summary>
        public static Direction4 GetOppositeDirection(this Direction4 direction)
            => (Direction4)((int)(direction + 2) % 4);

        /// <summary>Returns the opposite of the given `direction`.</summary>
        public static Direction8 GetOppositeDirection(this Direction8 direction)
            => (Direction8)((int)(direction + 4) % 8);

        /************************************************************************************************************************/

        /// <summary>Returns a vector representing the specified `direction`.</summary>
        public static Vector2 ToVector2(this Direction2 direction)
            => direction switch
            {
                Direction2.Left => Vector2.left,
                Direction2.Right => Vector2.right,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /// <summary>Returns a vector representing the specified `direction`.</summary>
        public static Vector2 ToVector2(this Direction4 direction)
            => direction switch
            {
                Direction4.Up => Vector2.up,
                Direction4.Right => Vector2.right,
                Direction4.Down => Vector2.down,
                Direction4.Left => Vector2.left,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /// <summary>Returns a vector representing the specified `direction`.</summary>
        public static Vector2 ToVector2(this Direction8 direction)
            => direction switch
            {
                Direction8.Up => Vector2.up,
                Direction8.Right => Vector2.right,
                Direction8.Down => Vector2.down,
                Direction8.Left => Vector2.left,
                Direction8.UpRight => UpRight,
                Direction8.DownRight => DownRight,
                Direction8.DownLeft => DownLeft,
                Direction8.UpLeft => UpLeft,
                _ => throw AnimancerUtilities.CreateUnsupportedArgumentException(direction),
            };

        /************************************************************************************************************************/

        /// <summary>Returns the direction closest to the specified `vector.x`.</summary>
        /// <remarks>Negative `direction` returns <see cref="Left"/>, 0 or positive returns <see cref="Right"/>.</remarks>
        public static Direction2 ToDirection2(Vector2 vector)
            => ToDirection2(vector.x);

        /// <summary>Returns the direction closest to the specified `direction`.</summary>
        /// <remarks>Negative `direction` returns <see cref="Left"/>, 0 or positive returns <see cref="Right"/>.</remarks>
        public static Direction2 ToDirection2(float direction)
            => direction < 0 ? Direction2.Left : Direction2.Right;

        /************************************************************************************************************************/

        /// <summary>Returns the direction closest to the specified `vector`.</summary>
        public static Direction4 ToDirection4(Vector2 vector)
        {
            if (vector.x >= 0)
            {
                if (vector.y >= 0)
                    return vector.x > vector.y ? Direction4.Right : Direction4.Up;
                else
                    return vector.x > -vector.y ? Direction4.Right : Direction4.Down;
            }
            else
            {
                if (vector.y >= 0)
                    return vector.x < -vector.y ? Direction4.Left : Direction4.Up;
                else
                    return vector.x < vector.y ? Direction4.Left : Direction4.Down;
            }
        }

        /// <summary>Returns the direction closest to the specified `vector`.</summary>
        public static Direction4 ToDirection4(Vector2Int vector)
        {
            if (vector.x >= 0)
            {
                if (vector.y >= 0)
                    return vector.x > vector.y ? Direction4.Right : Direction4.Up;
                else
                    return vector.x > -vector.y ? Direction4.Right : Direction4.Down;
            }
            else
            {
                if (vector.y >= 0)
                    return vector.x < -vector.y ? Direction4.Left : Direction4.Up;
                else
                    return vector.x < vector.y ? Direction4.Left : Direction4.Down;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Returns the direction closest to the specified `vector`.</summary>
        public static Direction8 ToDirection8(Vector2 vector)
        {
            var angle = Mathf.Atan2(vector.y, vector.x);
            var octant = Mathf.RoundToInt(8 * angle / (2 * Mathf.PI) + 8) % 8;
            return octant switch
            {
                0 => Direction8.Right,
                1 => Direction8.UpRight,
                2 => Direction8.Up,
                3 => Direction8.UpLeft,
                4 => Direction8.Left,
                5 => Direction8.DownLeft,
                6 => Direction8.Down,
                7 => Direction8.DownRight,
                _ => throw new ArgumentOutOfRangeException("Invalid octant"),
            };
        }

        /************************************************************************************************************************/

        /// <summary>Returns a copy of the `vector` pointing in the closest <see cref="Direction2"/>.</summary>
        public static Vector2 SnapToDirection2(Vector2 vector)
            => vector.x < 0
            ? new(-vector.magnitude, 0)
            : new(vector.magnitude, 0);

        /// <summary>Returns a copy of the `vector` pointing in the closest <see cref="Direction4"/>.</summary>
        public static Vector2 SnapToDirection4(Vector2 vector)
        {
            var magnitude = vector.magnitude;
            var direction = ToDirection4(vector);
            vector = direction.ToVector2() * magnitude;
            return vector;
        }

        /// <summary>Returns a copy of the `vector` pointing in the closest <see cref="Direction8"/>.</summary>
        public static Vector2 SnapToDirection8(Vector2 vector)
        {
            var magnitude = vector.magnitude;
            var direction = ToDirection8(vector);
            vector = direction.ToVector2() * magnitude;
            return vector;
        }

        /************************************************************************************************************************/
    }
}

