// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_PHYSICS_3D

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A component which takes the root motion from an <see cref="Animator"/> and applies it to a
    /// <see cref="CharacterController"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/other/root-motion#redirecting-root-motion">
    /// Redirecting Root Motion</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/RedirectRootMotionToCharacterController
    /// 
    [AddComponentMenu("Animancer/Redirect Root Motion To Character Controller")]
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(RedirectRootMotionToCharacterController))]
    public class RedirectRootMotionToCharacterController : RedirectRootMotion<CharacterController>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector3 Position
        {
            get => Target.transform.position;
            set => Target.Move(value - Position);
        }

        /// <inheritdoc/>
        public override Quaternion Rotation
        {
            get => Target.transform.rotation;
            set => Target.transform.rotation = value;
        }

        /// <inheritdoc/>
        protected override void OnAnimatorMove()
        {
            if (!ApplyRootMotion)
                return;

            Target.Move(Animator.deltaPosition);
            Target.transform.rotation *= Animator.deltaRotation;
        }

        /************************************************************************************************************************/
    }
}

#endif

