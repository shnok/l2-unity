// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using Animancer.Editor;
using UnityEditor;

[assembly: PolymorphicDrawerDetails(typeof(UnityEngine.Events.UnityEventBase), SeparateHeader = true)]

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] <see cref="PropertyDrawer"/> for <see cref="UnityEvent"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/UnityEventDrawer
    [CustomPropertyDrawer(typeof(UnityEvent), true)]
    public class UnityEventDrawer : UnityEditorInternal.UnityEventDrawer { }
}

#endif

