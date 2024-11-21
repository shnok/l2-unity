//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TheVegetationEngine;
using Boxophobic.StyledGUI;

public class TVEShaderElementGUI : ShaderGUI
{
    bool multiSelection = false;
    bool showInternalProperties = false;
    bool showHiddenProperties = false;
    bool showAdditionalInfo = false;

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        AssignDefaultTexture(material, newShader);
        TVEUtils.SetElementSettings(material);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        var material0 = materialEditor.target as Material;
        var materials = materialEditor.targets;

        if (materials.Length > 1)
            multiSelection = true;

        DrawDynamicInspector(material0, materialEditor, props);

        foreach (Material material in materials)
        {
            TVEUtils.SetElementSettings(material);
        }
    }

    void DrawDynamicInspector(Material material, MaterialEditor materialEditor, MaterialProperty[] props)
    {
        TVEUtils.DrawShaderBanner(material);

        var customPropsList = new List<MaterialProperty>();

        if (multiSelection)
        {
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (prop.flags == MaterialProperty.PropFlags.HideInInspector && !showHiddenProperties)
                    continue;

                customPropsList.Add(prop);
            }
        }
        else
        {
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                if (prop.flags == MaterialProperty.PropFlags.HideInInspector && !showHiddenProperties)
                {
                    continue;
                }

                if (material.HasProperty("_ElementMode"))
                {
                    if (material.GetInt("_ElementMode") == 1 && prop.name == "_MainColor")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalColor1")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalColor2")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalColor3")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalColor4")
                        continue;

                    if (material.GetInt("_ElementMode") == 1 && prop.name == "_MainValue")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalValue1")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalValue2")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalValue3")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_AdditionalValue4")
                        continue;

                    if (material.GetInt("_ElementMode") == 0 && prop.name == "_SeasonRemap")
                        continue;
                }

                // Keep with auto-register for upgrading
                if (!material.shader.name.Contains("Motion Advanced"))
                {
                    if (prop.name == "_SpeedTresholdValue")
                        continue;
                    if (prop.name == "_ElementDirectionMode")
                        continue;
                }

                if (material.HasProperty("_ElementMotionMode"))
                {
                    var mode = material.GetInt("_ElementMotionMode");

                    if (mode == 13)
                    {
                        if (prop.name == "_MotionPower")
                            continue;
                    }
                }

                if (material.HasProperty("_ElementDirectionMode"))
                {
                    var mode = material.GetInt("_ElementDirectionMode");

                    if (mode != 30)
                    {
                        if (prop.name == "_SpeedTresholdValue")
                            continue;
                    }
                }

                if (material.HasProperty("_ElementRaycastMode"))
                {
                    if (material.GetInt("_ElementRaycastMode") == 0 && prop.name == "_RaycastLayerMask")
                        continue;
                    if (material.GetInt("_ElementRaycastMode") == 0 && prop.name == "_RaycastDistanceEndValue")
                        continue;
                }

                customPropsList.Add(prop);
            }
        }

        //Draw Custom GUI
        for (int i = 0; i < customPropsList.Count; i++)
        {
            var displayName = customPropsList[i].displayName;

            var debug = "";

            if (showInternalProperties)
            {
                debug = "  (" + customPropsList[i].name + ")";
            }

            materialEditor.ShaderProperty(customPropsList[i], displayName + debug);
        }

        GUILayout.Space(10);

        StyledGUI.DrawInspectorCategory("Advanced Settings");

        GUILayout.Space(10);

        if (EditorUtility.IsPersistent(material) /*&& (material.HasProperty("_ElementRaycastMode") && material.GetFloat("_ElementRaycastMode") < 0.5f)*/)
        {
            materialEditor.EnableInstancingField();
            GUILayout.Space(10);
        }
        else
        {
            material.enableInstancing = false;
        }

        showInternalProperties = EditorGUILayout.Toggle("Show Internal Properties", showInternalProperties);
        showHiddenProperties = EditorGUILayout.Toggle("Show Hidden Properties", showHiddenProperties);
        showAdditionalInfo = EditorGUILayout.Toggle("Show Additional Info", showAdditionalInfo);

        if (showAdditionalInfo)
        {
            TVEUtils.DrawTechnicalDetails(material);
        }

        GUILayout.Space(20);

        TVEUtils.DrawPoweredByTheVegetationEngine();
    }

    void AssignDefaultTexture(Material material, Shader shader)
    {
        if (shader.name.Contains("Interaction") || shader.name.Contains("Advanced"))
        {
            material.SetTexture("_MainTex", Resources.Load<Material>("Internal Interaction").GetTexture("_MainTex"));
            material.SetTexture("_NoiseTex", Resources.Load<Material>("Internal Interaction").GetTexture("_NoiseTex"));
        }
        else
        {
            material.SetTexture("_MainTex", Resources.Load<Material>("Internal Colors").GetTexture("_MainTex"));
            material.SetTexture("_NoiseTex", Resources.Load<Material>("Internal Interaction").GetTexture("_NoiseTex"));
        }
    }
}

