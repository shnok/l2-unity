// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR && ULT_EVENTS

using Animancer.Editor;
using UnityEditor;

[assembly: PolymorphicDrawerDetails(typeof(UltEvents.UltEventBase), SeparateHeader = true)]

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] <see cref="PropertyDrawer"/> for <see cref="UltEvent"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/UltEventDrawer
    [CustomPropertyDrawer(typeof(UltEvent), true)]
    public class UltEventDrawer : UltEvents.Editor.UltEventDrawer,
        PropertyDrawers.IDiscardOnSelectionChange
    { }
}

#endif

