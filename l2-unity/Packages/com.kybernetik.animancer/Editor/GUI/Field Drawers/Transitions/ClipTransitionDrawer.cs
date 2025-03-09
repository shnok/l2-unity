// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using UnityEditor;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/ClipTransitionDrawer
    [CustomPropertyDrawer(typeof(ClipTransition), true)]
    public class ClipTransitionDrawer : TransitionDrawer
    {
        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ClipTransitionDrawer"/>.</summary>
        public ClipTransitionDrawer()
            : base(ClipTransition.ClipFieldName)
        { }

        /************************************************************************************************************************/
    }
}

#endif

