// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom editor for <see cref="TransitionAssetBase"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimancerTransitionAssetBaseEditor
    [CustomEditor(typeof(TransitionAssetBase), true), CanEditMultipleObjects]
    public class AnimancerTransitionAssetBaseEditor : ScriptableObjectEditor { }
}

#endif

