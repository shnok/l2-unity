// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] Persistent settings used by Animancer.</summary>
    /// <remarks>
    /// This asset automatically creates itself when first accessed.
    /// <para></para>
    /// The default location is <em>Packages/com.kybernetik.animancer/Code/Editor</em>, but you can freely move it
    /// (and the whole Animancer folder) anywhere in your project.
    /// <para></para>
    /// These settings can also be accessed via the Settings in the <see cref="Tools.AnimancerToolsWindow"/>
    /// (<c>Window/Animation/Animancer Tools</c>).
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimancerSettings
    /// 
    [AnimancerHelpUrl(typeof(AnimancerSettings))]
    public class AnimancerSettings : AnimancerSettingsInternal { }
}

#endif
