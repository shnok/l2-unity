// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
    [AnimancerHelpUrl(typeof(DirectionalSet<>))]
    public abstract class DirectionalSet<T> : ScriptableObject
    {
        /************************************************************************************************************************/
        #region Read-Only
        /************************************************************************************************************************/

#if UNITY_ASSERTIONS
        private bool _AllowChanges;
#endif

        /// <summary>[Assert-Only] Sets a debug flag to enable or disable the ability to modify this set.</summary>
        [System.Diagnostics.Conditional(Strings.Assertions)]
        public void AllowChanges(bool allow = true)
        {
#if UNITY_ASSERTIONS
            _AllowChanges = allow;
#endif
        }

        /// <summary>[Assert-Only]
        /// Throws an <see cref="InvalidOperationException"/> if <see cref="AllowChanges"/> wasn't called.
        /// </summary>
        [System.Diagnostics.Conditional(Strings.Assertions)]
        public void AssertAllowChanges()
        {
#if UNITY_ASSERTIONS
            if (!_AllowChanges)
                throw new InvalidOperationException(
                    $"Unable to modify {this} because it is read-only." +
                    $" Call {nameof(AllowChanges)}() before making any changes.");
#endif
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Directions
        /************************************************************************************************************************/

        /// <summary>The number of directions in this set.</summary>
        public abstract int DirectionCount { get; }

        /************************************************************************************************************************/

        /// <summary>Returns the name of the specified `direction`.</summary>
        protected abstract string GetDirectionName(int direction);

        /************************************************************************************************************************/

        /// <summary>Returns the object associated with the specified `direction`.</summary>
        public abstract T Get(int direction);

        /************************************************************************************************************************/

        /// <summary>Returns the object closest to the specified `direction`.</summary>
        public abstract T Get(Vector2 direction);

        /************************************************************************************************************************/

        /// <summary>Sets the object associated with the specified `direction`.</summary>
        public abstract void Set(int direction, T value);

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Attempts to assign the `value` to one of this set's fields based on its `name` and
        /// returns the direction index of that field (or -1 if it was unable to determine the direction).
        /// </summary>
        public int SetByName(string name, T value)
        {
            var bestDirection = -1;
            var bestDirectionIndex = -1;

            var directionCount = DirectionCount;
            for (int i = 0; i < directionCount; i++)
            {
                var index = name.LastIndexOf(GetDirectionName(i));
                if (bestDirectionIndex < index)
                {
                    bestDirectionIndex = index;
                    bestDirection = i;
                }
            }

            if (bestDirection >= 0)
                Set(bestDirection, value);

            return bestDirection;
        }

        /// <summary>[Editor-Only]
        /// Attempts to assign the `value` to one of this set's fields based on its name and
        /// returns the direction index of that field (or -1 if it was unable to determine the direction).
        /// </summary>
        public int SetByName<U>(U value)
            where U : Object, T
            => SetByName(value.name, value);

        /************************************************************************************************************************/
        #region Conversion
        /************************************************************************************************************************/

        /// <summary>Returns a vector representing the specified `direction`.</summary>
        public abstract Vector2 GetDirection(int direction);

        /************************************************************************************************************************/

        /// <summary>Returns a copy of the `vector` pointing in the closest direction this set has an object for.</summary>
        public abstract Vector2 Snap(Vector2 vector);

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Gathering
        /************************************************************************************************************************/

        /// <summary>Adds all objects from this set to the `values`, starting from the specified `index`.</summary>
        public void AddTo(T[] values, int index)
        {
            var count = DirectionCount;
            for (int i = 0; i < count; i++)
                values[index + i] = Get(i);
        }

        /// <summary>Adds all objects from this set to the `values`.</summary>
        public void AddTo(List<T> values)
        {
            var count = DirectionCount;
            for (int i = 0; i < count; i++)
                values.Add(Get(i));
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Adds unit vectors corresponding to each of the objects in this set to the `directions`,
        /// starting from the specified `index`.
        /// </summary>
        public void AddTo(Vector2[] directions, int index)
        {
            var count = DirectionCount;
            for (int i = 0; i < count; i++)
                directions[index + i] = GetDirection(i);
        }

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="AddTo"/> and <see cref="AddTo"/>.</summary>
        public void AddTo(T[] values, Vector2[] directions, int index)
        {
            AddTo(values, index);
            AddTo(directions, index);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

