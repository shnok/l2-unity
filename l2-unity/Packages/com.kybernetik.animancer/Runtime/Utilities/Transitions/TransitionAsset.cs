// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Animancer
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer/TransitionAsset
    [CreateAssetMenu(
        menuName = Strings.MenuPrefix + "Transition Asset",
        order = Strings.AssetMenuOrder + 1)]
    [AnimancerHelpUrl(typeof(TransitionAsset))]
    public class TransitionAsset : TransitionAsset<ITransitionDetailed>
    {
        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Sets the <see cref="TransitionAssetBase.CreateInstance"/>.</summary>
        [InitializeOnLoadMethod]
        private static void SetMainImplementation()
            => CreateInstance = transition =>
            {
                var asset = CreateInstance<TransitionAsset>();
                asset.Transition = transition;
                return asset;
            };

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void Reset()
        {
            Transition = new ClipTransition();
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Validates that the `mainAsset` is actually an asset.</summary>
        public static bool ValidateCreate(Object mainAsset)
        {
            var path = AssetDatabase.GetAssetPath(mainAsset);
            return !string.IsNullOrEmpty(path);
        }

        /// <summary>[Editor-Only] Creates a <see cref="TransitionAsset"/> next to the `mainAsset`.</summary>
        public static TransitionAsset Create(Object mainAsset)
        {
            var path = AssetDatabase.GetAssetPath(mainAsset);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(
                    $"Can't create {nameof(TransitionAsset)} for something that isn't an asset.",
                    mainAsset);

                return null;
            }

            path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, $"{mainAsset.name}.asset");
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var asset = CreateInstance<TransitionAsset>();
            AssetDatabase.CreateAsset(asset, path);
            Selection.activeObject = asset;
            return asset;
        }

        /************************************************************************************************************************/
#endif
        /************************************************************************************************************************/
    }
}

