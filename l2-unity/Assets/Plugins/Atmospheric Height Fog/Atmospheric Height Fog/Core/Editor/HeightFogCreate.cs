// Cristian Pop - https://boxophobic.com/

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AtmosphericHeightFog
{
    public class HeightFogCreate
    {
        [MenuItem("GameObject/BOXOPHOBIC/Atmospheric Height Fog/Global", false, 7)]
        static void CreateGlobalVolume()
        {
            if (GameObject.Find("Height Fog Global") != null)
            {
                Debug.Log("[Atmospheric Height Fog] " + "Height Fog Global is already added to your scene!");
                return;
            }

            GameObject go = new GameObject();
            go.name = "Height Fog Global";
            go.AddComponent<HeightFogGlobal>();

            if (Selection.activeGameObject != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;
            }

            Selection.activeGameObject = go;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/BOXOPHOBIC/Atmospheric Height Fog/Override Volume (Box)", false, 7)]
        static void CreateOverrideBoxVolume()
        {
            if (GameObject.Find("Height Fog Global") == null)
            {
                Debug.Log("[Atmospheric Height Fog] " + "Height Fog Global must be added to the scene first!");
                return;
            }

            GameObject go = new GameObject();
            go.name = "Height Fog Override (Box)";
            go.AddComponent<BoxCollider>();
            go.GetComponent<BoxCollider>().isTrigger = true;
            go.AddComponent<HeightFogOverride>();

            var sceneCamera = SceneView.lastActiveSceneView.camera;

            if (sceneCamera != null)
            {
                go.transform.position = sceneCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }

            if (Selection.activeGameObject != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;
            }

            Selection.activeGameObject = go;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/BOXOPHOBIC/Atmospheric Height Fog/Override Volume (Sphere)", false, 7)]
        static void CreateOverrideSphereVolume()
        {
            if (GameObject.Find("Height Fog Global") == null)
            {
                Debug.Log("[Atmospheric Height Fog] " + "Height Fog Global must be added to the scene first!");
                return;
            }

            GameObject go = new GameObject();
            go.name = "Height Fog Override (Sphere)";
            go.AddComponent<SphereCollider>();
            go.GetComponent<SphereCollider>().isTrigger = true;
            go.AddComponent<HeightFogOverride>();

            var sceneCamera = SceneView.lastActiveSceneView.camera;

            if (sceneCamera != null)
            {
                go.transform.position = sceneCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }

            if (Selection.activeGameObject != null)
            {
                go.transform.parent = Selection.activeGameObject.transform;
            }

            Selection.activeGameObject = go;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

