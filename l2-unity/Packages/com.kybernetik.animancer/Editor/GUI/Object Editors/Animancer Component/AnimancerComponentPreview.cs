// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using UnityEditor;

namespace Animancer.Editor.Previews
{
    /// <summary>[Editor-Only]
    /// An interactive preview which displays the internal details of an <see cref="AnimancerComponent"/>.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor.Previews/AnimancerComponentPreview
    [CustomPreview(typeof(AnimancerComponent))]
    public class AnimancerComponentPreview : AnimancerComponentPreviewInternal { }
}

#endif
