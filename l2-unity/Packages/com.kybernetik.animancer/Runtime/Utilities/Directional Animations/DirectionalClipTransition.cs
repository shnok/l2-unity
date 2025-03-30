// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ClipTransition"/> which gets its clip from a
    /// <see cref="DirectionalSet{T}"/> of <see cref="AnimationClip"/>s.
    /// </summary>
    /// 
    /// <remarks>
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// <para></para>
    /// <strong>Example:</strong><code>
    /// // Leave the Clip field empty in the Inspector and assign its AnimationSet instead.
    /// [SerializeField] private DirectionalClipTransition _Transition;
    /// 
    /// ...
    /// 
    /// // Then you can just call SetDirection and Play it like any other transition.
    /// // All of the transition's details like Fade Duration and Events will be applied to whichever clip is plays.
    /// _Transition.SetDirection(Vector2.right);
    /// _Animancer.Play(_Transition);
    /// </code></remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalClipTransition
    /// 
    [Serializable]
    public class DirectionalClipTransition : ClipTransition,
        ICopyable<DirectionalClipTransition>
    {
        /************************************************************************************************************************/

        [SerializeField]
        [Tooltip("The animations used to determine the " + nameof(Clip))]
        private DirectionalSet<AnimationClip> _AnimationSet;

        /// <summary>[<see cref="SerializeField"/>] 
        /// The <see cref="DirectionalSet{T}"/> used to determine the <see cref="ClipTransition.Clip"/>.
        /// </summary>
        public ref DirectionalSet<AnimationClip> AnimationSet
            => ref _AnimationSet;

        /// <summary>The name of the serialized backing field of <see cref="AnimationSet"/>.</summary>
        public const string AnimationSetField = nameof(_AnimationSet);

        /************************************************************************************************************************/

        /// <summary>Sets the <see cref="ClipTransition.Clip"/> from the <see cref="AnimationSet"/>.</summary>
        public void SetDirection(Vector2 direction)
            => Clip = _AnimationSet.Get(direction);

        /// <summary>Sets the <see cref="ClipTransition.Clip"/> from the <see cref="AnimationSet"/>.</summary>
        public void SetDirection(int direction)
            => Clip = _AnimationSet.Get(direction);

        /// <summary>Sets the <see cref="ClipTransition.Clip"/> from the <see cref="AnimationSet"/>.</summary>
        public void SetDirection(Direction4 direction)
            => SetDirection((int)direction);

        /// <summary>Sets the <see cref="ClipTransition.Clip"/> from the <see cref="AnimationSet"/>.</summary>
        public void SetDirection(Direction8 direction)
            => SetDirection((int)direction);

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            base.GatherAnimationClips(clips);
            clips.GatherFromSource(_AnimationSet);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public virtual void CopyFrom(DirectionalClipTransition copyFrom, CloneContext context)
        {
            base.CopyFrom(copyFrom, context);

            if (copyFrom == null)
            {
                _AnimationSet = default;
                return;
            }

            _AnimationSet = copyFrom._AnimationSet;
        }

        /************************************************************************************************************************/
    }
}

