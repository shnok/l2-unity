// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A component which takes the root motion from an <see cref="UnityEngine.Animator"/>
    /// and applies it to a different object.
    /// </summary>
    /// 
    /// <remarks>
    /// This can be useful if the character's <see cref="Rigidbody"/> or <see cref="CharacterController"/> is on a
    /// parent of the <see cref="UnityEngine.Animator"/> to keep the model separate from the logical components.
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/other/root-motion#redirecting-root-motion">
    /// Redirecting Root Motion</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/RedirectRootMotion
    /// 
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(RedirectRootMotion))]
    [RequireComponent(typeof(Animator))]
    public abstract class RedirectRootMotion : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The Animator which provides the root motion")]
        private Animator _Animator;

        /// <summary>The <see cref="UnityEngine.Animator"/> which provides the root motion.</summary>
        public ref Animator Animator => ref _Animator;

        /************************************************************************************************************************/

        /// <summary>The current position of the target.</summary>
        public abstract Vector3 Position { get; set; }

        /// <summary>The current rotation of the target.</summary>
        public abstract Quaternion Rotation { get; set; }

        /************************************************************************************************************************/

        /// <summary>Is <see cref="Animator.applyRootMotion"/> enabled?</summary>
        public virtual bool ApplyRootMotion
            => Animator != null;

        /************************************************************************************************************************/

        /// <summary>Automatically finds the <see cref="Animator"/>.</summary>
        protected virtual void OnValidate()
        {
            TryGetComponent(out _Animator);
        }

        /************************************************************************************************************************/

        /// <summary>Applies the root motion from the <see cref="Animator"/> to the <see cref="Target"/>.</summary>
        protected virtual void OnAnimatorMove()
        {
            if (!ApplyRootMotion)
                return;

            Position += Animator.deltaPosition;
            Rotation *= Animator.deltaRotation;
        }

        /************************************************************************************************************************/
    }

    /// <summary>A <see cref="RedirectRootMotion"/> with a generic <see cref="Target"/>.</remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/RedirectRootMotion_1
    /// 
    [HelpURL("https://kybernetik.com.au/animancer/api/Animancer/" + nameof(RedirectRootMotion<T>) + "_1")]
    public abstract class RedirectRootMotion<T> : RedirectRootMotion
        where T : Object
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The object which the root motion will be applied to")]
        private T _Target;

        /// <summary>The object which the root motion will be applied to.</summary>
        public ref T Target => ref _Target;

        /************************************************************************************************************************/

        /// <summary>
        /// Returns true if the <see cref="Target"/> and <see cref="RedirectRootMotion.Animator"/> are set and
        /// <see cref="Animator.applyRootMotion"/> is enabled.
        /// </summary>
        public override bool ApplyRootMotion
            => Target != null
            && base.ApplyRootMotion;

        /************************************************************************************************************************/

        /// <summary>Automatically finds the <see cref="RedirectRootMotion.Animator"/> and <see cref="Target"/>.</summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_Target == null)
            {
                var parent = transform.parent;
                if (parent != null)
                    _Target = parent.GetComponentInParent<T>();

                if (_Target == null)
                    TryGetComponent(out _Target);
            }
        }

        /************************************************************************************************************************/
    }
}

