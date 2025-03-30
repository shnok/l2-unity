// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2025 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom Inspector for <see cref="DirectionalAnimationSet4"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/DirectionalAnimationSet4Editor
    [CustomEditor(typeof(DirectionalAnimationSet4), true)]
    public class DirectionalAnimationSet4Editor : DirectionalAnimationSetEditor { }

    /// <summary>[Editor-Only] A custom Inspector for <see cref="DirectionalAnimationSet8"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/DirectionalAnimationSet8Editor
    [CustomEditor(typeof(DirectionalAnimationSet8), true)]
    public class DirectionalAnimationSet8Editor : DirectionalAnimationSetEditor { }

    /// <summary>[Editor-Only]
    /// A custom Inspector for 
    /// <see cref="DirectionalAnimationSet4"/> and <see cref="DirectionalAnimationSet8"/>.
    /// </summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/DirectionalAnimationSetEditor
    [CanEditMultipleObjects]
    public class DirectionalAnimationSetEditor : ScriptableObjectEditor
    {
        /************************************************************************************************************************/

        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet2) + "/Find Animations")]
        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet4) + "/Find Animations")]
        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet8) + "/Find Animations")]
        private static void FindSimilarAnimations(MenuCommand command)
        {
            var set = (DirectionalSet<AnimationClip>)command.context;

            var directory = AssetDatabase.GetAssetPath(set);
            directory = Path.GetDirectoryName(directory);

            var guids = AssetDatabase.FindAssets(
                $"{set.name} t:{nameof(AnimationClip)}",
                new string[] { directory });

            using (new ModifySerializedField(set, "Find Animations"))
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                    if (clip == null)
                        continue;

                    set.SetByName(clip);
                }
            }
        }

        /************************************************************************************************************************/

        [MenuItem(
            itemName: Strings.CreateMenuPrefix + "Directional Animation Set/From Selection",
            priority = Strings.AssetMenuOrder + 5)]
        private static void CreateDirectionalAnimationSet()
        {
            var nameToAnimations = new Dictionary<string, List<AnimationClip>>();

            var selection = Selection.objects;
            for (int i = 0; i < selection.Length; i++)
            {
                var clip = selection[i] as AnimationClip;
                if (clip == null)
                    continue;

                var name = clip.name;
                for (Direction4 direction = 0; direction < (Direction4)4; direction++)
                {
                    name = name.Replace(direction.ToString(), "");
                }

                if (!nameToAnimations.TryGetValue(name, out var clips))
                {
                    clips = new();
                    nameToAnimations.Add(name, clips);
                }

                clips.Add(clip);
            }

            if (nameToAnimations.Count == 0)
                throw new InvalidOperationException("No animation clips are selected");

            var sets = new List<Object>();
            foreach (var nameAndAnimations in nameToAnimations)
            {
                var count = nameAndAnimations.Value.Count;
                DirectionalSet<AnimationClip> set = count <= 2
                    ? CreateInstance<DirectionalAnimationSet2>()
                    : count <= 4
                    ? CreateInstance<DirectionalAnimationSet4>()
                    : CreateInstance<DirectionalAnimationSet8>();

                set.AllowChanges();
                for (int i = 0; i < nameAndAnimations.Value.Count; i++)
                    set.SetByName(nameAndAnimations.Value[i]);

                var path = AssetDatabase.GetAssetPath(nameAndAnimations.Value[0]);
                path = $"{Path.GetDirectoryName(path)}/{nameAndAnimations.Key}.asset";
                AssetDatabase.CreateAsset(set, path);

                sets.Add(set);
            }

            Selection.objects = sets.ToArray();
        }

        /************************************************************************************************************************/

        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet2) + "/Toggle Looping")]
        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet4) + "/Toggle Looping")]
        [MenuItem("CONTEXT/" + nameof(DirectionalAnimationSet8) + "/Toggle Looping")]
        private static void ToggleLooping(MenuCommand command)
        {
            var set = (DirectionalSet<AnimationClip>)command.context;

            var count = set.DirectionCount;
            for (int i = 0; i < count; i++)
            {
                var clip = set.Get(i);
                if (clip == null)
                    continue;

                var isLooping = !clip.isLooping;
                for (i = 0; i < count; i++)
                {
                    clip = set.Get(i);
                    if (clip == null)
                        continue;

                    AnimancerEditorUtilities.SetLooping(clip, isLooping);
                }

                break;
            }
        }

        /************************************************************************************************************************/
    }
}

#endif

