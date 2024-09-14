#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class UpgradeTransparentMaterials : MonoBehaviour
{
    public static string oldShaderName = "Universal Render Pipeline/Nature/SpeedTree8_PBRLit";
    public static string newShaderName = "Universal Render Pipeline/Lit";

    [MenuItem("Shnok/[Debug] Upgrade unity2022 transparent mats")]
    public static void ReplaceShaders()
    {
        if (string.IsNullOrEmpty(oldShaderName) || string.IsNullOrEmpty(newShaderName))
        {
            Debug.LogError("Please provide both old and new shader names before running.");
            return;
        }

        Shader newShader = Shader.Find(newShaderName);
        if (newShader == null)
        {
            Debug.LogError($"Could not find shader with name: {newShaderName}");
            return;
        }

        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int replacedCount = 0;

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material.shader.name == oldShaderName)
            {
                Texture t = material.mainTexture;
                Debug.Log(t);

                material.shader = Shader.Find(newShaderName);
                // Set the Cull Mode to Off (which means double-sided rendering)
                material.SetFloat("_Cull", (float)UnityEngine.Rendering.CullMode.Off);

                // Ensure the shader knows we want two-sided rendering
                material.SetFloat("_DoubleSidedEnable", 1);

                // If your shader uses the _DoubleSidedGIMode property, you might want to set it as well
                // 0 = Flip Backfaces, 1 = Mirror, 2 = None
                material.SetFloat("_DoubleSidedGIMode", 0);
                material.SetColor("_BaseColor", Color.white);
                material.SetFloat("_Smoothness", 0);
                material.SetFloat("_EnvironmentReflections", 0f);
                material.SetFloat("_SpecularHighlights", 0f);
                // Enable alpha clipping
                material.SetFloat("_AlphaClip", 1); // or true, depending on the shader
                material.mainTexture = t;
                material.SetTexture("_BaseMap", t);
                // Set the alpha clip threshold
                material.SetFloat("_Cutoff", 0.5f);

                material.EnableKeyword("_ALPHATEST_ON");

                // material.shader = newShader;
                // EditorUtility.SetDirty(material);
                replacedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Replaced shader on {replacedCount} materials.");
    }
}
#endif