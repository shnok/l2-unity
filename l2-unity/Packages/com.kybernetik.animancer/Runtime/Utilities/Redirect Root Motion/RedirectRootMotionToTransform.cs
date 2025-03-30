// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A component which takes the root motion from an <see cref="Animator"/> and applies it to a
    /// <see cref="Transform"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/other/root-motion#redirecting-root-motion">
    /// Redirecting Root Motion</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/RedirectRootMotionToTransform
    /// 
    [AddComponentMenu("Animancer/Redirect Root Motion To Transform")]
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(RedirectRootMotionToTransform))]
    public class RedirectRootMotionToTransform : RedirectRootMotion<Transform>
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override Vector3 Position
        {
            get => Target.position;
            set => Target.position = value;
        }

        /// <inheritdoc/>
        public override Quaternion Rotation
        {
            get => Target.rotation;
            set => Target.rotation = value;
        }

        /************************************************************************************************************************/
    }
}

