//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TheVegetationEngine
{
    [DisallowMultipleComponent]
    [CustomEditor(typeof(TVEElement))]
    public class TVEInspectorElement : Editor
    {
        string excludeProps = "m_Script";
        TVEElement targetScript;

        void OnEnable()
        {
            targetScript = (TVEElement)target;
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();

            var elementMaterial = targetScript.elementMaterial;

            if (elementMaterial != null)
            {
                if (!EditorUtility.IsPersistent(elementMaterial))
                {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Save the material to be able to use it in prefabs or to enable GPU Instancing support!", MessageType.Info);
                    GUILayout.Space(10);

                    if (GUILayout.Button("Save Material to Disk"))
                    {
                        var savePath = EditorUtility.SaveFilePanelInProject("Save Material", "My Element", "mat", "Save Element material to disk!");

                        if (savePath != "")
                        {
                            // Add TVE Element to be able to find the material on upgrade
                            savePath = savePath.Replace("(TVE Element)", "");
                            savePath = savePath.Replace(".mat", " (TVE Element).mat");

                            AssetDatabase.CreateAsset(elementMaterial, savePath);
                            targetScript.gameObject.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(savePath);

                            if (Application.isPlaying == false)
                            {
                                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                            }

                            AssetDatabase.Refresh();
                        }
                    }
                }
            }

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


