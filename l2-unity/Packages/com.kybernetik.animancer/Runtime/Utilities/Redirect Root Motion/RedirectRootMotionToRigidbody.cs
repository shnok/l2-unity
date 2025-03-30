// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_PHYSICS_3D

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A component which takes the root motion from an <see cref="Animator"/> and applies it to a
    /// <see cref="Rigidbody"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/other/root-motion#redirecting-root-motion">
    /// Redirecting Root Motion</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/RedirectRootMotionToRigidbody
    /// 
    [AddComponentMenu("Animancer/Redirect Root Motion To Rigidbody")]
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(RedirectRootMotionToRigidbody))]
    public class RedirectRootMotionToRigidbody : RedirectRootMotion<Rigidbody>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector3 Position
        {
            get => Target.position;
            set => Target.MovePosition(value);
        }

        /// <inheritdoc/>
        public override Quaternion Rotation
        {
            get => Target.rotation;
            set => Target.MoveRotation(value);
        }

        /************************************************************************************************************************/
    }
}

#endif

