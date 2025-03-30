// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using Animancer.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Animancer
{
    /// <summary>Plays a single <see cref="AnimationClip"/>.</summary>
    /// 
    /// <remarks>
    /// <para></para>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/playing/component-types">
    /// Component Types</see>
    /// <para></para>
    /// <strong>Sample:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/samples/fine-control/doors">Doors</see>
    /// </remarks>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer/SoloAnimation
    /// 
    [AddComponentMenu(Strings.MenuPrefix + "Solo Animation")]
    [AnimancerHelpUrl(typeof(SoloAnimation))]
    [DefaultExecutionOrder(DefaultExecutionOrder)]
    public class SoloAnimation : SoloAnimationInternal { }
}
