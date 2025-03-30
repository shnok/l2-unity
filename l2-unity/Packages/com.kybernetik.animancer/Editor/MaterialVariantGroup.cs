// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only]
    /// A utility for automatically swapping materials based on supported shaders.
    /// </summary>
    /// <remarks>
    /// This system is used to allow materials to be used in any render pipeline.
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/MaterialVariantGroup
    //[CreateAssetMenu(
    //     menuName = Strings.MenuPrefix + "Material Variant Group",
    //     order = Strings.AssetMenuOrder + 5)]
    public class MaterialVariantGroup : ScriptableObject
    {
        /************************************************************************************************************************/

        [SerializeField]
        private Material _Material;

        /// <summary>The target material.</summary>
        public ref Material Material
            => ref _Material;

        [SerializeField]
        private Material[] _Variants;

        /// <summary>The variants to try applying to the target material.</summary>
        public ref Material[] Variants
            => ref _Variants;

        /************************************************************************************************************************/

        /// <summary>Applies the first variant that successfully loaded its shader.</summary>
        protected virtual void OnEnable()
        {
            if (_Material == null ||
                _Variants.IsNullOrEmpty())
                return;

            foreach (var variant in _Variants)
            {
                if (variant == null ||
                    variant.shader == null ||
                    !variant.shader.isSupported)
                    continue;

                if (_Material.shader != variant.shader)
                {
                    _Material.shader = variant.shader;
                    _Material.CopyPropertiesFromMaterial(variant);
                }

                return;
            }
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Loads all assets of this type to ensure their <see cref="OnEnable"/> is called.
        /// </summary>
        private class Initializer : AssetPostprocessor
        {
            /************************************************************************************************************************/

            /// <summary>Loads all assets of this type to ensure their <see cref="OnEnable"/> is called.</summary>
            private static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths,
                bool didDomainReload)
            {
                if (!didDomainReload)
                    return;

                var filter = $"t:{nameof(MaterialVariantGroup)}";

                var guids = AssetDatabase.FindAssets(filter);
                if (guids.Length == 0)
                    return;

                for (int i = 0; i < guids.Length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    AssetDatabase.LoadAssetAtPath<MaterialVariantGroup>(path);
                }
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
    }
}

#endif

