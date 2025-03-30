// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2022 Kybernetik //

using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>A set of up/right/down/left animations.</summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/directional-sets">
    /// Directional Animation Sets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/DirectionalAnimationSet2
    /// 
    [CreateAssetMenu(
        menuName = Strings.MenuPrefix + "Directional Animation Set/2 Directions",
        order = Strings.AssetMenuOrder + 3)]
    [AnimancerHelpUrl(typeof(DirectionalAnimationSet2))]
    public class DirectionalAnimationSet2 : DirectionalSet2<AnimationClip>,
        IAnimationClipSource
    {
        /************************************************************************************************************************/

        /// <summary>[<see cref="IAnimationClipSource"/>] Adds all animations from this set to the `clips`.</summary>
        void IAnimationClipSource.GetAnimationClips(List<AnimationClip> clips)
            => AddTo(clips);

        /************************************************************************************************************************/
    }
}

