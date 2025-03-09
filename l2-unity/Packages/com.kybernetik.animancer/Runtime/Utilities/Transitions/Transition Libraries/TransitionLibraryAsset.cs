// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System.Collections.Generic;
using UnityEngine;

namespace Animancer.TransitionLibraries
{
    /// <summary>[Pro-Only]
    /// A <see cref="ScriptableObject"/> which serializes a <see cref="TransitionLibraryDefinition"/>
    /// and creates a <see cref="TransitionLibrary"/> from it at runtime.
    /// </summary>
    /// <remarks>
    /// <strong>Documentation:</strong>
    /// <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/libraries">
    /// Transition Libraries</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.TransitionLibraries/TransitionLibraryAsset
    [CreateAssetMenu(
        menuName = Strings.MenuPrefix + "Transition Library",
        order = Strings.AssetMenuOrder + 0)]
    [AnimancerHelpUrl(typeof(TransitionLibraryAsset))]
#if !UNITY_EDITOR
    [System.Obsolete(Validate.ProOnlyMessage)]
#endif
    public class TransitionLibraryAsset : TransitionLibraryAssetInternal { }
}
