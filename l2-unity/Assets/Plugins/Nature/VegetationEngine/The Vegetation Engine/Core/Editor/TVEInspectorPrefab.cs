//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace TheVegetationEngine
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TVEPrefab))]
    public class TVEInspectorPrefab : Editor
    {
        string excludeProps = "m_Script";
        TVEPrefab targetScript;

        void OnEnable()
        {
            targetScript = (TVEPrefab)target;
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();

            var hasOverrides = false;
            var isStatic = false;

            if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabInstance(targetScript.gameObject))
            {
                var overrides = PrefabUtility.GetObjectOverrides(targetScript.gameObject);
                var overridesCount = 0;

                for (int i = 0; i < overrides.Count; i++)
                {
                    if (overrides[i].instanceObject.GetType() != typeof(UnityEngine.Transform) && overrides[i].instanceObject.GetType() != typeof(UnityEngine.GameObject))
                    {
                        overridesCount++;
                    }
                }

                if (overridesCount > 0)
                {
                    hasOverrides = true;
                    EditorGUILayout.HelpBox("Prefab instance has overrides! The Vegetation Engine works on the prefab root level and not on individual prefab instances. Make sure to apply/revert all overrides to avoid any issues!", MessageType.Warning);
                }
            }

            var staticFlags = GameObjectUtility.GetStaticEditorFlags(targetScript.gameObject);

            if (staticFlags.HasFlag(StaticEditorFlags.BatchingStatic))
            {
                isStatic = true;
                EditorGUILayout.HelpBox("Please note, vegetation prefabs cannot be used with Static Batching. Please disable the Batching Static flag on the game object header!", MessageType.Warning);
            }

            if (hasOverrides || isStatic)
            {
                GUILayout.Space(10);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Assets Converter"))
            {
                TVEAssetsConverter window = EditorWindow.GetWindow<TVEAssetsConverter>(false, "Assets Converter", true);
                window.minSize = new Vector2(480, 280);
                window.Show();
            }

            if (GUILayout.Button("Open Material Manager"))
            {
                TVEMaterialManager window = EditorWindow.GetWindow<TVEMaterialManager>(false, "Material Manager", true);
                window.minSize = new Vector2(389, 200);
                window.Show();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        void DrawInspector()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, excludeProps);

            serializedObject.ApplyModifiedProperties();
        }
    }
}


