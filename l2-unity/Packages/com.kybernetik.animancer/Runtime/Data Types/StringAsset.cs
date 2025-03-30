// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> which holds a <see cref="StringReference"/>
    /// based on its <see cref="Object.name"/>.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/StringAsset
    [AnimancerHelpUrl(typeof(StringAsset))]
    [CreateAssetMenu(
        menuName = Strings.MenuPrefix + "String Asset",
        order = Strings.AssetMenuOrder + 2)]
    public class StringAsset : StringAssetInternal
    {
        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        [Tooltip("An unused Editor-Only field where you can explain what this asset is used for")]
        [SerializeField, TextArea(2, 25)]
        private string _EditorComment;

        /// <summary>[Editor-Only] [<see cref="SerializeField"/>]
        /// An unused Editor-Only field where you can explain what this asset is used for.
        /// </summary>
        public ref string EditorComment
            => ref _EditorComment;

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// Returns a <see cref="StringAsset"/> with the specified `name` if one exists in the project.
        /// </summary>
        /// <remarks>If multiple assets have the same `name`, any one of them will be returned.</remarks>
        public static StringAsset Find(
            StringReference name,
            out string path)
        {
            var filter = $"{name} t:{nameof(StringAsset)}";
            var guids = UnityEditor.AssetDatabase.FindAssets(filter);

            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<StringAsset>(path);
                if (asset != null && asset.Name == name)
                    return asset;
            }

            path = null;
            return null;
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only] Saves a new <see cref="StringAsset"/> in the `directory` and returns it.</summary>
        /// <remarks>If no `directory` is specified, this method will ask the user to select a directory manually.</remarks>
        public static StringAsset Create(
            StringReference name,
            ref string directory,
            out string path)
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = UnityEditor.EditorUtility.SaveFolderPanel(
                    $"Select Folder to save String Asset - {name}",
                    "Assets",
                    "");

                if (string.IsNullOrEmpty(directory))
                {
                    path = null;
                    return null;
                }
            }

            var newAsset = CreateInstance<StringAsset>();
            newAsset.name = name;

            var workingDirectory = Environment.CurrentDirectory.Replace('\\', '/');
            if (directory.StartsWith(workingDirectory))
                directory = directory[(workingDirectory.Length + 1)..];

            path = System.IO.Path.Combine(directory, name + ".asset");

            UnityEditor.AssetDatabase.CreateAsset(newAsset, path);

            Debug.Log($"Created {nameof(StringAsset)}: {path}", newAsset);

            return newAsset;
        }

        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// If a <see cref="StringAsset"/> exists with the specified `name`, this method returns it.
        /// If multiple assets have the same name, any one of them will be returned.
        /// Otherwise, a new asset will be saved in the `createDirectory` and returned.
        /// </summary>
        /// <remarks>
        /// If no `createDirectory` is specified, this method will ask the user to select a directory manually.
        /// </remarks>
        public static StringAsset FindOrCreate(
            StringReference name,
            string createDirectory,
            out string path)
        {
            var asset = Find(name, out path);
            return asset != null
                ? asset
                : Create(name, ref createDirectory, out path);
        }

        /************************************************************************************************************************/
#endif
    }
}
