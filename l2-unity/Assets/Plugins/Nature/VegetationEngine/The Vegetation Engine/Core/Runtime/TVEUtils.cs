//Cristian Pop - https://boxophobic.com/

using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using Boxophobic.Constants;
#endif

namespace TheVegetationEngine
{
    public class TVEUtils
    {
        // Settings Utils
        public static void SetMaterialSettings(Material material)
        {
            var shaderName = material.shader.name;

            // Skip the Lite shaders
            if (material.HasProperty("_IsLiteShader"))
            {
                return;
            }

            if (!material.HasProperty("_IsTVEShader"))
            {
                return;
            }

            if (material.HasProperty("_IsVersion"))
            {
                var version = material.GetInt("_IsVersion");

                // fix wrong version added in shader
                if (version == 1200)
                {
                    version = 1050;
                }

                // Chanage shader early
                if (version < 900)
                {
                    // Mobile shaders
                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Cross Vertex Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Vertex Lit (Mobile)");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                        material.SetFloat("_MotionValue_20", 0);
                        material.SetFloat("_MotionValue_30", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Cross Simple Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Simple Lit (Mobile)");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                        material.SetFloat("_MotionValue_20", 0);
                        material.SetFloat("_MotionValue_30", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Grass Vertex Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Vertex Lit (Mobile)");

                        material.SetFloat("_MotionValue_20", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Grass Simple Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Simple Lit (Mobile)");

                        material.SetFloat("_MotionValue_20", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Uber Vertex Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Vertex Lit (Mobile)");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Uber Simple Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Simple Lit (Mobile)");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                    }

                    // Default Shaders
                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Cross Standard Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Standard Lit");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                        material.SetFloat("_MotionValue_20", 0);
                        material.SetFloat("_MotionValue_30", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Cross Subsurface Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Subsurface Lit");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                        material.SetFloat("_MotionValue_20", 0);
                        material.SetFloat("_MotionValue_30", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Grass Standard Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Standard Lit");

                        material.SetFloat("_MotionValue_20", 0);
                        material.SetFloat("_MotionValue_30", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Grass Subsurface Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Subsurface Lit");

                        material.SetFloat("_MotionValue_20", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Uber Standard Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Standard Lit");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Uber Subsurface Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Subsurface Lit");

                        material.SetFloat("_SizeFadeStartValue", 0);
                        material.SetFloat("_SizeFadeEndValue", 0);
                    }
                }

                if (version < 1100)
                {
                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Bark Standard Lit"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Standard Lit");

                        material.SetFloat("_GlobalColors", 0);
                        material.SetFloat("_GlobalAlpha", 0);
                        material.SetFloat("_SubsurfaceValue", 0);
                        material.SetFloat("_MotionAmplitude_32", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                        material.SetPropertyLock("_GlobalColors", true);
                        material.SetPropertyLock("_GlobalAlpha", true);
                        material.SetPropertyLock("_SubsurfaceValue", true);
                        material.SetPropertyLock("_MotionAmplitude_32", true);
#endif
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Bark Vertex Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Vertex Lit (Mobile)");

                        material.SetFloat("_GlobalColors", 0);
                        material.SetFloat("_GlobalAlpha", 0);
                        material.SetFloat("_SubsurfaceValue", 0);
                        material.SetFloat("_MotionAmplitude_32", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                        material.SetPropertyLock("_GlobalColors", true);
                        material.SetPropertyLock("_GlobalAlpha", true);
                        material.SetPropertyLock("_SubsurfaceValue", true);
                        material.SetPropertyLock("_MotionAmplitude_32", true);
#endif
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Bark Simple Lit (Mobile)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Simple Lit (Mobile)");

                        material.SetFloat("_GlobalColors", 0);
                        material.SetFloat("_GlobalAlpha", 0);
                        material.SetFloat("_SubsurfaceValue", 0);
                        material.SetFloat("_MotionAmplitude_32", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                        material.SetPropertyLock("_GlobalColors", true);
                        material.SetPropertyLock("_GlobalAlpha", true);
                        material.SetPropertyLock("_SubsurfaceValue", true);
                        material.SetPropertyLock("_MotionAmplitude_32", true);
#endif
                    }

                    if (material.shader.name == ("Hidden/BOXOPHOBIC/The Vegetation Engine/Geometry/Bark Standard Lit (Blanket)"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Geometry/Plant Standard Lit (Blanket)");

                        material.SetFloat("_GlobalColors", 0);
                        material.SetFloat("_GlobalAlpha", 0);
                        material.SetFloat("_SubsurfaceValue", 0);
                        material.SetFloat("_MotionAmplitude_32", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                        material.SetPropertyLock("_GlobalColors", true);
                        material.SetPropertyLock("_GlobalAlpha", true);
                        material.SetPropertyLock("_SubsurfaceValue", true);
                        material.SetPropertyLock("_MotionAmplitude_32", true);
#endif
                    }

                    // Disable Coloring for Props
                    if (material.shader.name.Contains("Prop"))
                    {
                        material.SetFloat("_GlobalColors", 0);
                        material.SetFloat("_GlobalAlpha", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                        material.SetPropertyLock("_GlobalColors", true);
                        material.SetPropertyLock("_GlobalAlpha", true);
#endif
                    }
                }

                // Update settings
                if (version < 500)
                {
                    if (material.HasProperty("_RenderPriority"))
                    {
                        if (material.GetInt("_RenderPriority") != 0)
                        {
                            material.SetInt("_RenderQueue", 1);
                        }
                    }

                    material.SetInt("_IsVersion", 500);
                }

                if (version < 600)
                {
                    if (material.HasProperty("_LayerReactValue"))
                    {
                        material.SetInt("_LayerVertexValue", material.GetInt("_LayerReactValue"));
                    }

                    material.SetInt("_IsVersion", 600);
                }

                if (version < 620)
                {
                    if (material.HasProperty("_VertexRollingMode"))
                    {
                        material.SetInt("_MotionValue_20", material.GetInt("_VertexRollingMode"));
                    }

                    material.SetInt("_IsVersion", 620);
                }

                if (version < 630)
                {
                    material.DisableKeyword("TVE_DETAIL_BLEND_OVERLAY");
                    material.DisableKeyword("TVE_DETAIL_BLEND_REPLACE");

                    material.SetInt("_IsVersion", 630);
                }

                if (version < 640)
                {
                    if (material.HasProperty("_Cutoff"))
                    {
                        material.SetFloat("_AlphaCutoffValue", material.GetFloat("_Cutoff"));
                    }

                    material.SetInt("_IsVersion", 640);
                }

                if (version < 650)
                {
                    if (material.HasProperty("_Cutoff"))
                    {
                        material.SetFloat("_AlphaClipValue", material.GetFloat("_Cutoff"));
                    }

                    if (material.HasProperty("_MotionValue_20"))
                    {
                        material.SetFloat("_MotionValue_20", 1);
                    }

                    // Guess best values for squash motion
                    if (material.HasProperty("_MotionScale_20") && material.HasProperty("_MaxBoundsInfo"))
                    {
                        var bounds = material.GetVector("_MaxBoundsInfo");
                        var scale = Mathf.Round((1.0f / bounds.y * 10.0f * 0.5f) * 10) / 10;

                        if (scale > 1)
                        {
                            scale = Mathf.Clamp(Mathf.FloorToInt(scale), 0, 20);
                        }

                        material.SetFloat("_MotionScale_20", scale);
                    }

                    if (material.shader.name.Contains("Bark"))
                    {
                        material.SetFloat("_DetailCoordMode", 1);
                    }

                    material.DisableKeyword("TVE_ALPHA_CLIP");
                    material.DisableKeyword("TVE_DETAIL_MODE_ON");
                    material.DisableKeyword("TVE_DETAIL_MODE_OFF");
                    material.DisableKeyword("TVE_DETAIL_TYPE_VERTEX_BLUE");
                    material.DisableKeyword("TVE_DETAIL_TYPE_PROJECTION");
                    material.DisableKeyword("TVE_IS_VEGETATION_SHADER");
                    material.DisableKeyword("TVE_IS_GRASS_SHADER");

                    material.SetInt("_IsVersion", 650);
                }

                if (version < 710)
                {
                    if (material.HasProperty("_MotionScale_20"))
                    {
                        var scale = material.GetFloat("_MotionScale_20");

                        material.SetFloat("_MotionScale_20", scale * 10.0f);
                    }

                    material.SetInt("_IsVersion", 710);
                }

                if (version < 800)
                {
                    if (material.HasProperty("_ColorsMaskMinValue") && material.HasProperty("_ColorsMaskMaxValue"))
                    {
                        var min = material.GetFloat("_ColorsMaskMinValue");
                        var max = material.GetFloat("_ColorsMaskMaxValue");

                        material.SetFloat("_MainMaskMinValue", min);
                        material.SetFloat("_MainMaskMaxValue", max);
                    }

                    if (material.HasProperty("_LeavesFilterMode") && material.HasProperty("_LeavesFilterColor"))
                    {
                        var mode = material.GetInt("_LeavesFilterMode");
                        var color = material.GetColor("_LeavesFilterColor");

                        if (mode == 1)
                        {
                            if (color.r < 0.1f && color.g < 0.1f && color.b < 0.1f)
                            {
                                material.SetFloat("_GlobalColors", 0);
                                material.SetFloat("_MotionValue_30", 0);
                            }
                        }
                    }

                    if (material.HasProperty("_DetailMeshValue"))
                    {
                        material.SetFloat("_DetailMeshValue", 0);
                        material.SetFloat("_DetailBlendMinValue", 0.4f);
                        material.SetFloat("_DetailBlendMaxValue", 0.6f);
                    }

                    material.SetInt("_IsVersion", 800);
                }

                if (version < 810)
                {
                    if (material.HasProperty("_GlobalColors"))
                    {
                        var value = material.GetFloat("_GlobalColors");

                        material.SetFloat("_GlobalColors", Mathf.Clamp01(value * 2.0f));
                    }

                    if (material.HasProperty("_VertexOcclusionColor"))
                    {
                        var color = material.GetColor("_VertexOcclusionColor");
                        var alpha = (color.r + color.g + color.b + 0.001f) / 3.0f;

                        color.a = Mathf.Clamp01(alpha);

                        material.SetColor("_VertexOcclusionColor", color);
                    }

                    material.SetInt("_IsIdentifier", 0);
                    material.SetInt("_IsVersion", 810);
                }

                if (version < 830)
                {
                    material.SetFloat("_OverlayProjectionValue", 0.6f);

                    material.SetInt("_IsVersion", 830);
                }

                if (version < 850)
                {
                    if (material.HasProperty("_DetailOpaqueMode"))
                    {
                        var mode = material.GetInt("_DetailOpaqueMode");

                        material.SetInt("_DetailFadeMode", 1 - mode);
                    }

                    if (material.HasProperty("_DetailTypeMode"))
                    {
                        var mode = material.GetInt("_DetailTypeMode");

                        if (mode == 1)
                        {
                            material.SetInt("_DetailCoordMode", 2);
                        }

                        // Transfer Type to Mesh variable
                        material.SetInt("_DetailMeshMode", material.GetInt("_DetailTypeMode"));
                    }

                    if (material.HasProperty("_DetailCoordMode"))
                    {
                        // Transfer Detail Coord to Second Coord
                        material.SetInt("_SecondUVsMode", material.GetInt("_DetailCoordMode"));
                    }

                    if (material.HasProperty("_EmissiveFlagMode"))
                    {
                        int mode = material.GetInt("_EmissiveFlagMode");

                        if (mode == 0)
                        {
                            material.SetInt("_EmissiveFlagMode", 0);
                        }
                        else if (mode == 10)
                        {
                            material.SetInt("_EmissiveFlagMode", 1);
                        }
                        else if (mode == 20)
                        {
                            material.SetInt("_EmissiveFlagMode", 2);
                        }
                        else if (mode == 30)
                        {
                            material.SetInt("_EmissiveFlagMode", 3);
                        }
                    }

                    if (material.HasProperty("_EmissiveIntensityParams"))
                    {
                        var value = 1.0f;
                        var param = material.GetVector("_EmissiveIntensityParams");

                        if (param.w == 0)
                        {
                            value = param.y;
                            material.SetInt("_EmissiveIntensityMode", 0);
                        }
                        else
                        {
                            value = param.z;
                            material.SetInt("_EmissiveIntensityMode", 1);
                        }

                        material.SetFloat("_EmissiveIntensityValue", value);
                    }

                    material.SetInt("_IsVersion", 850);
                }

                if (version < 900)
                {
                    material.SetFloat("_DetailMeshValue", 1);
                    material.SetFloat("_DetailMaskValue", 1);

                    material.SetInt("_IsVersion", 900);
                }

                if (version < 1000)
                {
                    material.SetInt("_IsIdentifier", (int)UnityEngine.Random.Range(1, 100));

                    if (material.HasProperty("_DetailMeshInvertMode"))
                    {
                        var mode = material.GetInt("_DetailMeshInvertMode");

                        if (mode == 0)
                        {
                            material.SetFloat("_DetailMeshMinValue", 0);
                            material.SetFloat("_DetailMeshMaxValue", 1);
                        }
                        else
                        {
                            material.SetFloat("_DetailMeshMinValue", 1);
                            material.SetFloat("_DetailMeshMaxValue", 0);
                        }
                    }

                    if (material.HasProperty("_DetailMaskInvertMode"))
                    {
                        var mode = material.GetInt("_DetailMaskInvertMode");

                        if (mode == 0)
                        {
                            material.SetFloat("_DetailMaskMinValue", 0);
                            material.SetFloat("_DetailMaskMaxValue", 1);
                        }
                        else
                        {
                            material.SetFloat("_DetailMaskMinValue", 1);
                            material.SetFloat("_DetailMaskMaxValue", 0);
                        }
                    }

                    material.SetInt("_IsVersion", 1000);
                }

                if (version < 1100)
                {
                    if (material.HasProperty("_MotionValue_20"))
                    {
                        var mode = material.GetInt("_MotionValue_20");

                        if (mode == 0)
                        {
                            material.SetFloat("_MotionAmplitude_20", 0);
                            material.SetFloat("_MotionAmplitude_22", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                            material.SetPropertyLock("_MotionAmplitude_20", true);
                            material.SetPropertyLock("_MotionAmplitude_22", true);
#endif
                        }
                    }

                    if (material.HasProperty("_MotionValue_30"))
                    {
                        var mode = material.GetInt("_MotionValue_30");

                        if (mode == 0)
                        {
                            material.SetFloat("_MotionAmplitude_32", 0);

#if UNITY_2022_1_OR_NEWER && UNITY_EDITOR
                            material.SetPropertyLock("_MotionAmplitude_32", true);
#endif
                        }
                    }

                    material.SetInt("_IsVersion", 1100);
                }

                // Bumped version because 1200 was used before by mistake
                if (version < 1201)
                {
                    if (material.HasProperty("_EmissiveColor"))
                    {
                        var color = material.GetColor("_EmissiveColor");

                        if (material.GetColor("_EmissiveColor").r > 0 || material.GetColor("_EmissiveColor").g > 0 || material.GetColor("_EmissiveColor").b > 0)
                        {
                            material.SetInt("_EmissiveMode", 1);
                        }
                    }

                    material.SetInt("_IsVersion", 1201);
                }

                // Refresh is needed to apply new keywords
                if (version < 1230)
                {
                    material.SetInt("_IsVersion", 1230);
                }
            }

            // Set Internal Render Values
            if (material.HasProperty("_RenderMode"))
            {
                material.SetInt("_render_mode", material.GetInt("_RenderMode"));
            }

            if (material.HasProperty("_RenderCull"))
            {
                material.SetInt("_render_cull", material.GetInt("_RenderCull"));
            }

            if (material.HasProperty("_RenderZWrite"))
            {
                material.SetInt("_render_zw", material.GetInt("_RenderZWrite"));
            }

            if (material.HasProperty("_RenderClip"))
            {
                material.SetInt("_render_clip", material.GetInt("_RenderClip"));
            }

            if (material.HasProperty("_RenderSpecular"))
            {
                material.SetInt("_render_specular", material.GetInt("_RenderSpecular"));
            }

            // Set Render Mode
            if (material.HasProperty("_RenderMode"))
            {
                int mode = material.GetInt("_RenderMode");
                int queue = 0;
                int priority = 0;
                int decals = 0;
                int clip = 0;

                if (material.HasProperty("_RenderQueue") && material.HasProperty("_RenderPriority"))
                {
                    queue = material.GetInt("_RenderQueue");
                    priority = material.GetInt("_RenderPriority");
                }

                if (material.GetTag("RenderPipeline", false) == "HDRenderPipeline")
                {
                    if (material.HasProperty("_RenderDecals"))
                    {
                        decals = material.GetInt("_RenderDecals");
                    }
                }

                if (material.HasProperty("_RenderClip"))
                {
                    clip = material.GetInt("_RenderClip");
                }

                // User Defined, render type changes needed
                if (queue == 2)
                {
                    if (material.renderQueue == 2000)
                    {
                        material.SetOverrideTag("RenderType", "Opaque");
                    }

                    if (material.renderQueue > 2449 && material.renderQueue < 3000)
                    {
                        material.SetOverrideTag("RenderType", "AlphaTest");
                    }

                    if (material.renderQueue > 2999)
                    {
                        material.SetOverrideTag("RenderType", "Transparent");
                    }
                }

                // Opaque
                if (mode == 0)
                {
                    if (queue != 2)
                    {
                        material.SetOverrideTag("RenderType", "AlphaTest");
                        //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest + priority;

                        if (clip == 0)
                        {
                            if (decals == 0)
                            {
                                material.renderQueue = 2000 + priority;
                            }
                            else
                            {
                                material.renderQueue = 2225 + priority;
                            }
                        }
                        else
                        {
                            if (decals == 0)
                            {
                                material.renderQueue = 2450 + priority;
                            }
                            else
                            {
                                material.renderQueue = 2475 + priority;
                            }
                        }
                    }

                    // Standard and Universal Render Pipeline
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_render_zw", 1);
                    material.SetInt("_render_premul", 0);

                    // Set Main Color alpha to 1
                    if (material.HasProperty("_MainColor"))
                    {
                        var color = material.GetColor("_MainColor");
                        material.SetColor("_MainColor", new Color(color.r, color.g, color.b, 1.0f));
                    }

                    if (material.HasProperty("_MainColorTwo"))
                    {
                        var color = material.GetColor("_MainColorTwo");
                        material.SetColor("_MainColorTwo", new Color(color.r, color.g, color.b, 1.0f));
                    }

                    // HD Render Pipeline
                    material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.DisableKeyword("_ENABLE_FOG_ON_TRANSPARENT");

                    material.DisableKeyword("_BLENDMODE_ALPHA");
                    material.DisableKeyword("_BLENDMODE_ADD");
                    material.DisableKeyword("_BLENDMODE_PRE_MULTIPLY");

                    material.SetInt("_RenderQueueType", 1);
                    material.SetInt("_SurfaceType", 0);
                    material.SetInt("_BlendMode", 0);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_AlphaSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_AlphaDstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.SetInt("_TransparentZWrite", 1);
                    material.SetInt("_ZTestDepthEqualForOpaque", 3);

                    if (clip == 0)
                    {
                        material.SetInt("_ZTestGBuffer", 4);
                    }
                    else
                    {
                        material.SetInt("_ZTestGBuffer", 3);
                    }

                    //material.SetInt("_ZTestGBuffer", 4);
                    material.SetInt("_ZTestTransparent", 4);

                    material.SetShaderPassEnabled("TransparentBackface", false);
                    material.SetShaderPassEnabled("TransparentBackfaceDebugDisplay", false);
                    material.SetShaderPassEnabled("TransparentDepthPrepass", false);
                    material.SetShaderPassEnabled("TransparentDepthPostpass", false);
                }
                // Transparent
                else
                {
                    if (queue != 2)
                    {
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + priority;
                    }

                    int zwrite = 1;

                    if (material.HasProperty("_RenderZWrite"))
                    {
                        zwrite = material.GetInt("_RenderZWrite");
                    }

                    // Standard and Universal Render Pipeline
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_render_premul", 0);

                    // HD Render Pipeline
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.EnableKeyword("_ENABLE_FOG_ON_TRANSPARENT");

                    material.EnableKeyword("_BLENDMODE_ALPHA");
                    material.DisableKeyword("_BLENDMODE_ADD");
                    material.DisableKeyword("_BLENDMODE_PRE_MULTIPLY");

                    material.SetInt("_RenderQueueType", 5);
                    material.SetInt("_SurfaceType", 1);
                    material.SetInt("_BlendMode", 0);
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_AlphaSrcBlend", 1);
                    material.SetInt("_AlphaDstBlend", 10);
                    material.SetInt("_ZWrite", zwrite);
                    material.SetInt("_TransparentZWrite", zwrite);
                    material.SetInt("_ZTestDepthEqualForOpaque", 4);
                    material.SetInt("_ZTestGBuffer", 4);
                    material.SetInt("_ZTestTransparent", 4);

                    material.SetShaderPassEnabled("TransparentBackface", true);
                    material.SetShaderPassEnabled("TransparentBackfaceDebugDisplay", true);
                    material.SetShaderPassEnabled("TransparentDepthPrepass", true);
                    material.SetShaderPassEnabled("TransparentDepthPostpass", true);
                }
            }

            if (shaderName.Contains("Prop"))
            {
                material.SetShaderPassEnabled("MotionVectors", false);
            }
            else
            {
                material.SetShaderPassEnabled("MotionVectors", true);
            }

            // Set Receive Mode in HDRP
            if (material.GetTag("RenderPipeline", false) == "HDRenderPipeline")
            {
                if (material.HasProperty("_RenderDecals"))
                {
                    int decals = material.GetInt("_RenderDecals");

                    if (decals == 0)
                    {
                        material.EnableKeyword("_DISABLE_DECALS");
                    }
                    else
                    {
                        material.DisableKeyword("_DISABLE_DECALS");
                    }
                }

                if (material.HasProperty("_RenderSSR"))
                {
                    int ssr = material.GetInt("_RenderSSR");

                    if (ssr == 0)
                    {
                        material.EnableKeyword("_DISABLE_SSR");

                        material.SetInt("_StencilRef", 0);
                        material.SetInt("_StencilRefDepth", 0);
                        material.SetInt("_StencilRefDistortionVec", 4);
                        material.SetInt("_StencilRefGBuffer", 2);
                        material.SetInt("_StencilRefMV", 32);
                        material.SetInt("_StencilWriteMask", 6);
                        material.SetInt("_StencilWriteMaskDepth", 8);
                        material.SetInt("_StencilWriteMaskDistortionVec", 4);
                        material.SetInt("_StencilWriteMaskGBuffer", 14);
                        material.SetInt("_StencilWriteMaskMV", 40);
                    }
                    else
                    {
                        material.DisableKeyword("_DISABLE_SSR");

                        material.SetInt("_StencilRef", 0);
                        material.SetInt("_StencilRefDepth", 8);
                        material.SetInt("_StencilRefDistortionVec", 4);
                        material.SetInt("_StencilRefGBuffer", 10);
                        material.SetInt("_StencilRefMV", 40);
                        material.SetInt("_StencilWriteMask", 6);
                        material.SetInt("_StencilWriteMaskDepth", 8);
                        material.SetInt("_StencilWriteMaskDistortionVec", 4);
                        material.SetInt("_StencilWriteMaskGBuffer", 14);
                        material.SetInt("_StencilWriteMaskMV", 40);
                    }
                }
            }

            // Set Cull Mode
            if (material.HasProperty("_RenderCull"))
            {
                int cull = material.GetInt("_RenderCull");

                material.SetInt("_CullMode", cull);
                material.SetInt("_TransparentCullMode", cull);
                material.SetInt("_CullModeForward", cull);

                // Needed for HD Render Pipeline
                material.DisableKeyword("_DOUBLESIDED_ON");
            }

            // Set Clip Mode
            if (material.HasProperty("_RenderClip"))
            {
                int clip = material.GetInt("_RenderClip");
                float cutoff = 0.5f;

                if (material.HasProperty("_AlphaClipValue"))
                {
                    cutoff = material.GetFloat("_AlphaClipValue");
                }

                if (clip == 0)
                {
                    material.DisableKeyword("TVE_ALPHA_CLIP");

                    material.SetInt("_render_coverage", 0);
                }
                else
                {
                    material.EnableKeyword("TVE_ALPHA_CLIP");

                    if (material.HasProperty("_RenderCoverage") && material.HasProperty("_AlphaFeatherValue"))
                    {
                        material.SetInt("_render_coverage", material.GetInt("_RenderCoverage"));
                    }
                    else
                    {
                        material.SetInt("_render_coverage", 0);
                    }
                }

                material.SetFloat("_Cutoff", cutoff);

                // HD Render Pipeline
                material.SetFloat("_AlphaCutoff", cutoff);
                material.SetFloat("_AlphaCutoffPostpass", cutoff);
                material.SetFloat("_AlphaCutoffPrepass", cutoff);
                material.SetFloat("_AlphaCutoffShadow", cutoff);
            }
            else
            {
                // Impostors don't have render clip
                if (!material.HasProperty("_AlphaFeatherValue"))
                {
                    material.SetInt("_RenderCoverage", 0);
                }
            }

            // Set Normals Mode
            if (material.HasProperty("_RenderNormals") && material.HasProperty("_render_normals"))
            {
                int normals = material.GetInt("_RenderNormals");

                // Standard, Universal, HD Render Pipeline
                // Flip 0
                if (normals == 0)
                {
                    material.SetVector("_render_normals", new Vector4(-1, -1, -1, 0));
                    material.SetVector("_DoubleSidedConstants", new Vector4(-1, -1, -1, 0));
                }
                // Mirror 1
                else if (normals == 1)
                {
                    material.SetVector("_render_normals", new Vector4(1, 1, -1, 0));
                    material.SetVector("_DoubleSidedConstants", new Vector4(1, 1, -1, 0));
                }
                // None 2
                else if (normals == 2)
                {
                    material.SetVector("_render_normals", new Vector4(1, 1, 1, 0));
                    material.SetVector("_DoubleSidedConstants", new Vector4(1, 1, 1, 0));
                }
            }

            if (material.HasProperty("_RenderDirect") && material.HasProperty("_render_direct"))
            {
                float value = material.GetFloat("_RenderDirect");

                material.SetFloat("_render_direct", value);
            }

            if (material.HasProperty("_RenderShadow") && material.HasProperty("_render_shadow"))
            {
                float value = material.GetFloat("_RenderShadow");

                material.SetFloat("_render_shadow", value);
            }

            if (material.HasProperty("_RenderAmbient") && material.HasProperty("_render_ambient"))
            {
                float value = material.GetFloat("_RenderAmbient");

                material.SetFloat("_render_ambient", value);
            }

#if UNITY_EDITOR
            // Assign Default HD Foliage profile
            if (material.HasProperty("_SubsurfaceDiffusion"))
            {
                // Workaround when the old HDRP 12 diffusion is not found
                if (material.GetFloat("_SubsurfaceDiffusion") == 3.5648174285888672f && AssetDatabase.GUIDToAssetPath("78322c7f82657514ebe48203160e3f39") == "")
                {
                    material.SetFloat("_SubsurfaceDiffusion", 0);
                }

                // Workaround when the old HDRP 14 diffusion is not found
                if (material.GetFloat("_SubsurfaceDiffusion") == 2.6486763954162598f && AssetDatabase.GUIDToAssetPath("879ffae44eefa4412bb327928f1a96dd") == "")
                {
                    material.SetFloat("_SubsurfaceDiffusion", 0);
                }

                // Search for one of Unity's diffusion profile
                if (material.GetFloat("_SubsurfaceDiffusion") == 0)
                {
                    // HDRP 12 Profile
                    if (AssetDatabase.GUIDToAssetPath("78322c7f82657514ebe48203160e3f39") != "")
                    {
                        material.SetFloat("_SubsurfaceDiffusion", 3.5648174285888672f);
                        material.SetVector("_SubsurfaceDiffusion_Asset", new Vector4(228889264007084710000000000000000000000f, 0.000000000000000000000000012389357880079404f, 0.00000000000000000000000000000000000076932702684439582f, 0.00018220426863990724f));
                    }

                    // HDRP 14 Profile
                    if (AssetDatabase.GUIDToAssetPath("879ffae44eefa4412bb327928f1a96dd") != "")
                    {
                        material.SetFloat("_SubsurfaceDiffusion", 2.6486763954162598f);
                        material.SetVector("_SubsurfaceDiffusion_Asset", new Vector4(-36985449400010195000000f, 20.616847991943359f, -0.00000000000000000000000000052916750040661612f, -1352014335655804900f));
                    }

                    // HDRP 16 Profile
                    if (AssetDatabase.GUIDToAssetPath("2384dbf2c1c420f45a792fbc315fbfb1") != "")
                    {
                        material.SetFloat("_SubsurfaceDiffusion", 3.8956573009490967f);
                        material.SetVector("_SubsurfaceDiffusion_Asset", new Vector4(-8695930962161997000000000000000f, -50949593547561853000000000000000f, -0.010710084810853004f, -0.0000000055696536271909736f));
                    }
                }
            }
#endif

            // Set Detail Mode
            if (material.HasProperty("_DetailMode") && material.HasProperty("_SecondColor"))
            {
                if (material.GetInt("_DetailMode") == 0)
                {
                    material.DisableKeyword("TVE_DETAIL");
                }
                else
                {
                    material.EnableKeyword("TVE_DETAIL");
                }

                if (material.HasProperty("_SecondUVsMode"))
                {
                    var mode = material.GetInt("_SecondUVsMode");

                    // Main
                    if (mode == 0)
                    {
                        material.SetVector("_second_uvs_mode", new Vector4(1, 0, 0, 0));
                    }
                    // Baked
                    else if (mode == 1)
                    {
                        material.SetVector("_second_uvs_mode", new Vector4(0, 1, 0, 0));
                    }
                    // Planar
                    else if (mode == 2)
                    {
                        material.SetVector("_second_uvs_mode", new Vector4(0, 0, 1, 0));
                    }
                    // Unused
                    else if (mode == 3)
                    {
                        material.SetVector("_second_uvs_mode", new Vector4(0, 0, 0, 1));
                    }
                }
            }

            // Set Blend Mode
            if (material.HasProperty("_TerrainMode"))
            {
                if (material.GetInt("_TerrainMode") == 0)
                {
                    material.DisableKeyword("TVE_TERRAIN");
                }
                else
                {
                    material.EnableKeyword("TVE_TERRAIN");
                }
            }

            #region TERRAIN SETTINGS
            if (material.HasProperty("_TerrainLayersMode"))
            {
                var mode = material.GetInt("_TerrainLayersMode");

                if (mode == 4)
                {
                    material.EnableKeyword("TVE_TERRAIN_04");
                    material.DisableKeyword("TVE_TERRAIN_08");
                    material.DisableKeyword("TVE_TERRAIN_12");
                    material.DisableKeyword("TVE_TERRAIN_16");
                }

                if (mode == 8)
                {
                    material.DisableKeyword("TVE_TERRAIN_04");
                    material.EnableKeyword("TVE_TERRAIN_08");
                    material.DisableKeyword("TVE_TERRAIN_12");
                    material.DisableKeyword("TVE_TERRAIN_16");
                }

                if (mode == 12)
                {
                    material.DisableKeyword("TVE_TERRAIN_04");
                    material.DisableKeyword("TVE_TERRAIN_08");
                    material.EnableKeyword("TVE_TERRAIN_12");
                    material.DisableKeyword("TVE_TERRAIN_16");
                }

                if (mode == 16)
                {
                    material.DisableKeyword("TVE_TERRAIN_04");
                    material.DisableKeyword("TVE_TERRAIN_08");
                    material.DisableKeyword("TVE_TERRAIN_12");
                    material.EnableKeyword("TVE_TERRAIN_16");
                }
            }

            if (material.HasProperty("_TerrainBlendMode"))
            {
                var mode = material.GetInt("_TerrainBlendMode");

                if (mode == 0)
                {
                    material.DisableKeyword("TVE_HEIGHT_BLEND");
                }

                if (mode == 1)
                {
                    material.EnableKeyword("TVE_HEIGHT_BLEND");
                }
            }

            if (material.HasProperty("_TerrainTexMode"))
            {
                var mode = material.GetInt("_TerrainTexMode");

                if (mode == 0)
                {
                    material.DisableKeyword("TVE_PACKED_TEX");
                }

                if (mode == 1)
                {
                    material.EnableKeyword("TVE_PACKED_TEX");
                }
            }

            // Set Terrain Mode
            if (material.HasProperty("_LayerSampleMode1"))
            {
                for (int i = 1; i < 17; i++)
                {
                    var prop = "_LayerSampleMode" + i;

                    if (material.HasProperty(prop))
                    {
                        var layer = i.ToString("00");
                        var mode = material.GetInt(prop);

                        if (mode == 10)
                        {
                            material.EnableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_3D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_3D");
                        }

                        if (mode == 20)
                        {
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_2D");
                            material.EnableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_3D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_3D");
                        }

                        if (mode == 30)
                        {
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_3D");
                            material.EnableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_3D");
                        }

                        if (mode == 40)
                        {
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_2D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_PLANAR_3D");
                            material.DisableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_2D");
                            material.EnableKeyword("TVE_SAMPLE_" + layer + "_STOCHASTIC_3D");
                        }
                    }
                }
            }
            #endregion

            if (material.HasProperty("_emissive_intensity_value"))
            {
                // Set Emissive Mode
                if (material.HasProperty("_EmissiveMode"))
                {
                    if (material.GetInt("_EmissiveMode") == 0)
                    {
                        material.DisableKeyword("TVE_EMISSIVE");
                    }
                    else
                    {
                        material.EnableKeyword("TVE_EMISSIVE");
                    }
                }

                // Set Intensity Mode
                if (material.HasProperty("_EmissiveIntensityMode") && material.HasProperty("_EmissiveIntensityValue"))
                {
                    float mode = material.GetInt("_EmissiveIntensityMode");
                    float value = material.GetFloat("_EmissiveIntensityValue");

                    if (mode == 0)
                    {
                        material.SetFloat("_emissive_intensity_value", value);
                    }
                    else if (mode == 1)
                    {
                        material.SetFloat("_emissive_intensity_value", (12.5f / 100.0f) * Mathf.Pow(2f, value));
                    }
                }

                // Set GI Mode
                if (material.HasProperty("_EmissiveFlagMode"))
                {
                    int mode = material.GetInt("_EmissiveFlagMode");

                    if (mode == 0)
                    {
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                    }
                    else if (mode == 1)
                    {
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
                    }
                    else if (mode == 2)
                    {
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                    }
                    else if (mode == 3)
                    {
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    }
                }
            }

            // Set Batching Mode
            if (material.HasProperty("_VertexDataMode"))
            {
                int batching = material.GetInt("_VertexDataMode");

                if (batching == 0)
                {
                    material.DisableKeyword("TVE_BATCHING");
                }
                else
                {
                    material.EnableKeyword("TVE_BATCHING");
                }
            }

            // Set Legacy props for external bakers
            if (material.HasProperty("_AlphaClipValue"))
            {
                material.SetFloat("_Cutoff", material.GetFloat("_AlphaClipValue"));
            }

            // Set Legacy props for external bakers
            if (material.HasProperty("_MainColor"))
            {
                material.SetColor("_Color", material.GetColor("_MainColor"));
            }

            // Set BlinnPhong Spec Color
            if (material.HasProperty("_SpecColor"))
            {
                material.SetColor("_SpecColor", Color.white);
            }

            if (material.HasProperty("_MainAlbedoTex"))
            {
                material.SetTexture("_MainTex", material.GetTexture("_MainAlbedoTex"));
            }

            if (material.HasProperty("_MainNormalTex"))
            {
                material.SetTexture("_BumpMap", material.GetTexture("_MainNormalTex"));
            }

            if (material.HasProperty("_MainUVs"))
            {
                material.SetTextureScale("_MainTex", new Vector2(material.GetVector("_MainUVs").x, material.GetVector("_MainUVs").y));
                material.SetTextureOffset("_MainTex", new Vector2(material.GetVector("_MainUVs").z, material.GetVector("_MainUVs").w));

                material.SetTextureScale("_BumpMap", new Vector2(material.GetVector("_MainUVs").x, material.GetVector("_MainUVs").y));
                material.SetTextureOffset("_BumpMap", new Vector2(material.GetVector("_MainUVs").z, material.GetVector("_MainUVs").w));
            }

            if (material.HasProperty("_SubsurfaceValue"))
            {
                // Legacy Surface Shader
                material.SetFloat("_Translucency", material.GetFloat("_SubsurfaceScatteringValue"));
                material.SetFloat("_TransNormalDistortion", material.GetFloat("_SubsurfaceNormalValue"));

                // Lit Template
                material.SetFloat("_TransStrength", material.GetFloat("_SubsurfaceScatteringValue"));
                material.SetFloat("_TransNormal", material.GetFloat("_SubsurfaceNormalValue"));
                material.SetFloat("_TransScattering", material.GetFloat("_SubsurfaceAngleValue"));
                material.SetFloat("_TransDirect", material.GetFloat("_SubsurfaceDirectValue"));
                material.SetFloat("_TransAmbient", material.GetFloat("_SubsurfaceAmbientValue"));
                material.SetFloat("_TransShadow", material.GetFloat("_SubsurfaceShadowValue"));
            }

#if UNITY_EDITOR

            // Add ID for material sharing debug
            if (material.HasProperty("_IsIdentifier"))
            {
                var id = material.GetInt("_IsIdentifier");

                if (id == 0)
                {
                    material.SetInt("_IsIdentifier", (int)UnityEngine.Random.Range(1, 100));
                }
            }

            // Detect if the shaders is custom compiled
            if (AssetDatabase.GetAssetPath(material.shader).Contains("Core"))
            {
                material.SetInt("_IsCustomShader", 0);
            }
            else
            {
                material.SetInt("_IsCustomShader", 1);
            }

            // Set internals for impostor baking 
            if (material.HasProperty("_VertexOcclusionColor"))
            {
                material.SetInt("_HasOcclusion", 1);
            }
            else
            {
                material.SetInt("_HasOcclusion", 0);
            }

            if (material.HasProperty("_GradientColorOne"))
            {
                material.SetInt("_HasGradient", 1);
            }
            else
            {
                material.SetInt("_HasGradient", 0);
            }

            if (material.HasProperty("_emissive_intensity_value"))
            {
                material.SetInt("_HasEmissive", 1);
            }
            else
            {
                material.SetInt("_HasEmissive", 0);
            }

            // Enable Nature Rendered support
            material.SetOverrideTag("NatureRendererInstancing", "True");

            // Set Internal shader type
            if (shaderName.Contains("Vertex Lit"))
            {
                material.SetInt("_IsVertexShader", 1);
                material.SetInt("_IsSimpleShader", 0);
                material.SetInt("_IsStandardShader", 0);
                material.SetInt("_IsSubsurfaceShader", 0);
            }

            if (shaderName.Contains("Simple Lit"))
            {
                material.SetInt("_IsVertexShader", 0);
                material.SetInt("_IsSimpleShader", 1);
                material.SetInt("_IsStandardShader", 0);
                material.SetInt("_IsSubsurfaceShader", 0);
            }

            if (shaderName.Contains("Standard Lit"))
            {
                material.SetInt("_IsVertexShader", 0);
                material.SetInt("_IsSimpleShader", 0);
                material.SetInt("_IsStandardShader", 1);
                material.SetInt("_IsSubsurfaceShader", 0);
            }

            if (shaderName.Contains("Subsurface Lit"))
            {
                material.SetInt("_IsVertexShader", 0);
                material.SetInt("_IsSimpleShader", 0);
                material.SetInt("_IsStandardShader", 0);
                material.SetInt("_IsSubsurfaceShader", 1);
            }
#endif
        }

        public static void SetElementSettings(Material material)
        {
            if (!material.HasProperty("_IsElementShader"))
            {
                return;
            }

            material.SetShaderPassEnabled("VolumePass", false);

            if (material.HasProperty("_IsVersion"))
            {
                var version = material.GetInt("_IsVersion");

                if (version < 600)
                {
                    if (material.HasProperty("_ElementLayerValue"))
                    {
                        var oldLayer = material.GetInt("_ElementLayerValue");

                        if (material.GetInt("_ElementLayerValue") > 0)
                        {
                            material.SetInt("_ElementLayerMask", (int)Mathf.Pow(2, oldLayer));
                            material.SetInt("_ElementLayerValue", -1);
                        }
                    }

                    if (material.HasProperty("_InvertX"))
                    {
                        material.SetInt("_ElementInvertMode", material.GetInt("_InvertX"));
                    }

                    if (material.HasProperty("_ElementFadeSupport"))
                    {
                        material.SetInt("_ElementVolumeFadeMode", material.GetInt("_ElementFadeSupport"));
                    }

                    material.SetInt("_IsVersion", 600);
                }

                if (version < 700)
                {
                    // Requires revalidation
                    material.SetInt("_IsVersion", 700);
                }

                if (version < 800)
                {
                    if (material.shader.name.Contains("Interaction"))
                    {
                        if (material.HasProperty("_ElementDirectionMode"))
                        {
                            if (material.GetInt("_ElementDirectionMode") == 1)
                            {
                                material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Elements/Default/Motion Advanced");
                                material.SetInt("_ElementDirectionMode", 30);
                            }
                        }
                    }

                    if (material.shader.name.Contains("Orientation"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Elements/Default/Motion Interaction");
                    }

                    if (material.shader.name.Contains("Turbulence"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Elements/Default/Motion Advanced");
                        material.SetInt("_ElementDirectionMode", 10);
                    }

                    if (material.shader.name.Contains("Wind Control"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Elements/Default/Wind Power");
                    }

                    if (material.shader.name.Contains("Wind Direction"))
                    {
                        material.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Elements/Default/Motion Advanced");
                        material.SetInt("_ElementDirectionMode", 10);
                    }

                    material.SetInt("_IsVersion", 800);
                }

                if (version < 810)
                {
                    if (material.HasProperty("_MainTexMinValue") && material.HasProperty("_MainTexMaxValue"))
                    {
                        var min = material.GetFloat("_MainTexMinValue");
                        var max = material.GetFloat("_MainTexMaxValue");

                        material.SetFloat("_MainMaskAlphaMinValue", min);
                        material.SetFloat("_MainMaskAlphaMaxValue", max);
                    }

                    material.SetInt("_IsVersion", 810);
                }
            }

            if (material.HasProperty("_IsColorsElement"))
            {
                material.SetOverrideTag("ElementType", "ColorsElement");
            }
            else if (material.HasProperty("_IsExtrasElement"))
            {
                material.SetOverrideTag("ElementType", "ExtrasElement");
            }
            else if (material.HasProperty("_IsMotionElement"))
            {
                material.SetOverrideTag("ElementType", "MotionElement");
            }
            else if (material.HasProperty("_IsVertexElement"))
            {
                material.SetOverrideTag("ElementType", "VertexElement");
            }

            //if (material.HasProperty("_ElementColorsMode"))
            //{
            //    var effect = material.GetInt("_ElementColorsMode");

            //    material.SetInt("_render_colormask", effect);
            //}

            if (material.HasProperty("_ElementMotionMode"))
            {
                var effect = material.GetInt("_ElementMotionMode");

                material.SetInt("_render_colormask", effect);

                if (effect == 13)
                {
                    material.SetFloat("_MotionPower", 0);
                }
            }

            if (material.HasProperty("_ElementBlendRGB"))
            {
                var blend = material.GetInt("_ElementBlendRGB");

                if (blend == 0)
                {
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                if (blend == 1)
                {
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.Zero);
                }
                if (blend == 2)
                {
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.One);
                }
            }

            if (material.HasProperty("_ElementBlendA"))
            {
                var blend = material.GetInt("_ElementBlendA");

                if (blend == 0)
                {
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.Zero);
                }
                else
                {
                    material.SetInt("_render_src", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_render_dst", (int)UnityEngine.Rendering.BlendMode.One);
                }
            }

            if (material.HasProperty("_ElementDirectionMode"))
            {
                var direction = material.GetInt("_ElementDirectionMode");

                if (direction == 10)
                {
                    material.SetVector("_element_direction_mode", new Vector4(1, 0, 0, 0));
                }

                if (direction == 20)
                {
                    material.SetVector("_element_direction_mode", new Vector4(0, 1, 0, 0));
                }

                if (direction == 30)
                {
                    material.SetVector("_element_direction_mode", new Vector4(0, 0, 1, 0));
                }

                if (direction == 40)
                {
                    material.SetVector("_element_direction_mode", new Vector4(0, 0, 0, 1));
                }
            }

            //if (material.HasProperty("_ElementRaycastMode"))
            //{
            //    var raycast = material.GetInt("_ElementRaycastMode");

            //    if (raycast == 1)
            //    {
            //        material.enableInstancing = false;
            //    }
            //}

#if UNITY_EDITOR
            if (material.HasProperty("_ElementLayerMask"))
            {
                var layers = material.GetInt("_ElementLayerMask");

                if (layers > 1)
                {
                    material.SetInt("_ElementLayerMessage", 1);
                }
                else
                {
                    material.SetInt("_ElementLayerMessage", 0);
                }

                if (layers == -1)
                {
                    material.SetInt("_ElementLayerWarning", 1);
                }
                else
                {
                    material.SetInt("_ElementLayerWarning", 0);
                }
            }
#endif
        }

        // Element Utils
        public static GameObject CreateElement(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, Material material, bool customMaterial)
        {
            material.name = "Element Default";

            var gameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Internal Element"));
            gameObject.name = "Element " + Path.GetFileNameWithoutExtension(material.shader.name);

            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = localRotation;
            gameObject.transform.localScale = localScale;

            gameObject.AddComponent<TVEElement>();

            if (customMaterial)
            {
                gameObject.GetComponent<TVEElement>().customMaterial = material;
            }
            else
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = material;
            }

            gameObject.transform.parent = parent;

            return gameObject;
        }

        public static GameObject CreateElement(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Transform parent, Material material)
        {
            return CreateElement(localPosition, localRotation, localScale, parent, material, false);
        }

        public static GameObject CreateElement(Terrain terrain, Material material, bool customMaterial)
        {
            material.name = "Element Terrain";

            var gameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Internal Element"));
            gameObject.name = "Element " + terrain.name;

            CopyTerrainDataToElement(terrain, TVETerrainDataMode.HeightTexture, material);

            gameObject.AddComponent<TVEElement>();

            if (customMaterial)
            {
                gameObject.GetComponent<TVEElement>().customMaterial = material;
            }
            else
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = material;
            }

            var position = terrain.transform.position;
            var bounds = terrain.terrainData.bounds;
            gameObject.transform.localPosition = new Vector3(bounds.center.x + position.x, bounds.min.y + position.y, bounds.center.z + position.z);
            gameObject.transform.localScale = new Vector3(terrain.terrainData.size.x, 1, terrain.terrainData.size.z);

            gameObject.GetComponent<TVEElement>().terrainData = terrain;

            return gameObject;
        }

        public static GameObject CreateElement(Terrain terrain, Material material)
        {
            return CreateElement(terrain, material, false);
        }

        public static GameObject CreateElement(GameObject gameObject, Material material, bool customMaterial)
        {
            material.name = "Element";

            gameObject.AddComponent<TVEElement>();

            if (customMaterial)
            {
                gameObject.GetComponent<TVEElement>().customMaterial = material;
            }
            else
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = material;
            }

            return gameObject;
        }

        public static GameObject CreateElement(GameObject gameObject, Material material)
        {
            return CreateElement(gameObject, material, false);
        }

        public static void CopyTerrainDataToElement(Terrain terrain, TVETerrainDataMode terrainMask, Material material)
        {
            if (terrain == null)
            {
                return;
            }

            // Support for regular elements
            if (material.HasProperty("_MainTex"))
            {
                if (terrainMask == TVETerrainDataMode.HeightTexture)
                {
                    material.SetTexture("_MainTex", terrain.terrainData.heightmapTexture);
                }
                if (terrainMask == TVETerrainDataMode.NormalTexture)
                {
                    material.SetTexture("_MainTex", terrain.normalmapTexture);
                }
                if (terrainMask == TVETerrainDataMode.HolesTexture)
                {
                    material.SetTexture("_MainTex", terrain.terrainData.holesTexture);
                }
            }

            // Support for terrain elements
            material.SetVector("_TerrainPosition", terrain.transform.position);
            material.SetVector("_TerrainSize", terrain.terrainData.size);
            material.SetTexture("_TerrainHeightTex", terrain.terrainData.heightmapTexture);
            material.SetTexture("_TerrainHolesTex", terrain.terrainData.holesTexture);
            material.SetTexture("_TerrainNormalTex", terrain.normalmapTexture);

            // Support for terrain elements
            if (terrain.terrainData.alphamapTextureCount == 1)
            {
                material.SetTexture("_ControlTex1", terrain.terrainData.alphamapTextures[0]);
            }

            if (terrain.terrainData.alphamapTextureCount == 2)
            {
                material.SetTexture("_ControlTex2", terrain.terrainData.alphamapTextures[1]);
            }

            if (terrain.terrainData.alphamapTextureCount == 3)
            {
                material.SetTexture("_ControlTex3", terrain.terrainData.alphamapTextures[2]);
            }

            if (terrain.terrainData.alphamapTextureCount == 4)
            {
                material.SetTexture("_ControlTex4", terrain.terrainData.alphamapTextures[2]);
            }
        }

        // Mesh Utils
        public static Mesh CreatePackedMesh(TVEModelData meshData)
        {
            Mesh mesh = UnityEngine.Object.Instantiate(meshData.mesh);

            var vertexCount = mesh.vertexCount;

            var bounds = mesh.bounds;
            var maxX = Mathf.Max(Mathf.Abs(bounds.min.x), Mathf.Abs(bounds.max.x));
            var maxZ = Mathf.Max(Mathf.Abs(bounds.min.z), Mathf.Abs(bounds.max.z));
            var maxR = Mathf.Max(maxX, maxZ) / 100f;
            var maxH = Mathf.Max(Mathf.Abs(bounds.min.y), Mathf.Abs(bounds.max.y)) / 100f;

            if (meshData.height == 0)
            {
                meshData.height = maxH;
            }

            if (meshData.radius == 0)
            {
                meshData.radius = maxR;
            }

            var dummyFloat = new List<float>(vertexCount);
            var dummyVector2 = new List<Vector2>(vertexCount);
            var dummyVector3 = new List<Vector3>(vertexCount);
            var dummyVector4 = new List<Vector4>(vertexCount);

            var colors = new List<Color>(vertexCount);
            var UV0 = new List<Vector4>(vertexCount);
            var UV2 = new List<Vector4>(vertexCount);
            var UV4 = new List<Vector4>(vertexCount);

            for (int i = 0; i < vertexCount; i++)
            {
                dummyFloat.Add(1);
                dummyVector2.Add(Vector2.zero);
                dummyVector3.Add(Vector3.zero);
                dummyVector4.Add(Vector4.zero);
            }

            mesh.GetColors(colors);
            mesh.GetUVs(0, UV0);
            mesh.GetUVs(1, UV2);
            mesh.GetUVs(3, UV4);

            if (UV2.Count == 0)
            {
                UV2 = dummyVector4;
            }

            if (UV4.Count == 0)
            {
                UV4 = dummyVector4;
            }

            if (meshData.variationMask == null)
            {
                meshData.variationMask = dummyFloat;
            }

            if (meshData.occlusionMask == null)
            {
                meshData.occlusionMask = dummyFloat;
            }

            if (meshData.detailMask == null)
            {
                meshData.detailMask = dummyFloat;
            }

            if (meshData.heightMask == null)
            {
                meshData.heightMask = dummyFloat;
            }

            if (meshData.motion2Mask == null)
            {
                meshData.motion2Mask = dummyFloat;
            }

            if (meshData.motion3Mask == null)
            {
                meshData.motion3Mask = dummyFloat;
            }

            if (meshData.detailCoord == null)
            {
                meshData.detailCoord = dummyVector2;
            }

            if (meshData.detailCoord == null)
            {
                meshData.pivotPositions = dummyVector3;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                colors[i] = new Color(meshData.variationMask[i], meshData.occlusionMask[i], meshData.detailMask[i], meshData.heightMask[i]);
                UV0[i] = new Vector4(UV0[i].x, UV0[i].y, MathVector2ToFloat(meshData.motion2Mask[i], meshData.motion3Mask[i]), MathVector2ToFloat(meshData.height / 100f, meshData.radius / 100f));
                UV2[i] = new Vector4(UV2[i].x, UV2[i].y, meshData.detailCoord[i].x, meshData.detailCoord[i].y);
                UV4[i] = new Vector4(meshData.pivotPositions[i].x, meshData.pivotPositions[i].z, meshData.pivotPositions[i].y, 0);
            }

            mesh.SetColors(colors);
            mesh.SetUVs(0, UV0);
            mesh.SetUVs(1, UV2);
            mesh.SetUVs(3, UV4);

            dummyFloat.Clear();
            dummyVector2.Clear();
            dummyVector3.Clear();
            dummyVector4.Clear();

            return mesh;
        }

        public static Mesh CombinePackedMeshes(List<GameObject> gameObjects, bool mergeSubMeshes, bool usePrebakedPivots)
        {
            var mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;

            var combineInstances = new CombineInstance[gameObjects.Count];

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var instanceMesh = UnityEngine.Object.Instantiate(gameObjects[i].GetComponent<MeshFilter>().sharedMesh);
                var meshRenderer = gameObjects[i].GetComponent<MeshRenderer>();

                var vertexCount = instanceMesh.vertexCount;
                var UV4 = new List<Vector3>(vertexCount);
                var newUV4 = new List<Vector4>(vertexCount);

                instanceMesh.GetUVs(3, UV4);

                if (usePrebakedPivots)
                {
                    for (int v = 0; v < vertexCount; v++)
                    {
                        var currentPivot = new Vector3(UV4[v].x, UV4[v].z, UV4[v].y);
                        var transformedPivot = gameObjects[i].transform.TransformPoint(currentPivot);
                        var swizzeledPivots = new Vector4(transformedPivot.x, transformedPivot.z, transformedPivot.y, 0);

                        newUV4.Add(swizzeledPivots);
                    }
                }
                else
                {
                    for (int v = 0; v < vertexCount; v++)
                    {
                        var currentPivot = gameObjects[i].transform.position;
                        var swizzeledPivots = new Vector4(currentPivot.x, currentPivot.z, currentPivot.y, 0);

                        newUV4.Add(swizzeledPivots);
                    }
                }

                instanceMesh.SetUVs(3, newUV4);

                combineInstances[i].mesh = instanceMesh;
                combineInstances[i].transform = meshRenderer.transform.localToWorldMatrix;
                combineInstances[i].lightmapScaleOffset = meshRenderer.lightmapScaleOffset;
                combineInstances[i].realtimeLightmapScaleOffset = meshRenderer.realtimeLightmapScaleOffset;
            }

            mesh.CombineMeshes(combineInstances, mergeSubMeshes, true, true);

            return mesh;
        }

        public static Mesh CombinePackedMeshes(List<GameObject> gameObjects, bool mergeSubMeshes)
        {
            return CombinePackedMeshes(gameObjects, mergeSubMeshes, true);
        }

        public static Mesh CombineColliderMeshes(List<GameObject> gameObjects)
        {
            var mesh = new Mesh();
            var combineInstances = new CombineInstance[gameObjects.Count];

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var instanceMesh = UnityEngine.Object.Instantiate(gameObjects[i].GetComponent<MeshFilter>().sharedMesh);
                var meshRenderer = gameObjects[i].GetComponent<MeshRenderer>();
                var transformMatrix = meshRenderer.transform.localToWorldMatrix;

                combineInstances[i].mesh = instanceMesh;
                combineInstances[i].transform = transformMatrix;
                combineInstances[i].lightmapScaleOffset = meshRenderer.lightmapScaleOffset;
                combineInstances[i].realtimeLightmapScaleOffset = meshRenderer.realtimeLightmapScaleOffset;
            }

            mesh.CombineMeshes(combineInstances, true, true, false);

            return mesh;
        }

        public static List<Mesh> SplitPackedMesh(Mesh mesh)
        {
            var spliMeshes = new List<Mesh>();

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Mesh submesh = GetSubmesh(mesh, i);

                spliMeshes.Add(submesh);
            }

            return spliMeshes;
        }

        public static Mesh GetSubmesh(Mesh mesh, int submeshIndex)
        {
            int[] triangles = mesh.GetTriangles(submeshIndex, true);

            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            Color[] colors = mesh.colors;
            List<Vector4> UV0 = new List<Vector4>();
            List<Vector4> UV2 = new List<Vector4>();
            List<Vector4> UV4 = new List<Vector4>();

            mesh.GetUVs(0, UV0);
            mesh.GetUVs(1, UV2);
            mesh.GetUVs(3, UV4);

            // Create a HashSet to store unique vertices and a dictionary to map old indices to new indices
            HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();
            Dictionary<int, int> indexMap = new Dictionary<int, int>();
            Dictionary<Vector3, int> vertexMap = new Dictionary<Vector3, int>();

            // Loop through the triangles and add their vertices to the HashSet
            foreach (int triangleIndex in triangles)
            {
                Vector3 vertex = vertices[triangleIndex];

                if (!uniqueVertices.Contains(vertex))
                {
                    uniqueVertices.Add(vertex);

                    int newIndex = uniqueVertices.Count - 1;

                    indexMap[triangleIndex] = newIndex;
                    vertexMap[vertex] = newIndex;
                }
            }

            Vector3[] newVertices = new Vector3[uniqueVertices.Count];
            Vector3[] newNormals = new Vector3[uniqueVertices.Count];
            Vector4[] newTangents = new Vector4[uniqueVertices.Count];
            Color[] newColors = new Color[uniqueVertices.Count];
            List<Vector4> newUV0 = new List<Vector4>();
            List<Vector4> newUV2 = new List<Vector4>();
            List<Vector4> newUV4 = new List<Vector4>();

            uniqueVertices.CopyTo(newVertices);

            foreach (KeyValuePair<int, int> pair in indexMap)
            {
                int oldIndex = pair.Key;
                int newIndex = pair.Value;
                newNormals[newIndex] = normals[oldIndex];
                newTangents[newIndex] = tangents[oldIndex];
                newColors[newIndex] = colors[oldIndex];
                newUV0.Add(UV0[oldIndex]);
                newUV2.Add(UV2[oldIndex]);
                newUV4.Add(UV4[oldIndex]);
            }

            int[] newTriangles = new int[triangles.Length];

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int newIndex1 = vertexMap[vertices[triangles[i + 0]]];
                int newIndex2 = vertexMap[vertices[triangles[i + 1]]];
                int newIndex3 = vertexMap[vertices[triangles[i + 2]]];
                newTriangles[i + 0] = newIndex1;
                newTriangles[i + 1] = newIndex2;
                newTriangles[i + 2] = newIndex3;
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = newVertices;
            newMesh.normals = newNormals;
            newMesh.tangents = newTangents;
            newMesh.colors = newColors;
            newMesh.SetUVs(0, newUV0);
            newMesh.SetUVs(1, newUV2);
            newMesh.SetUVs(3, newUV4);
            newMesh.triangles = newTriangles;

            return newMesh;
        }

#if UNITY_EDITOR
        public static TVEModelSettings PreProcessMesh(string meshPath)
        {
            TVEModelSettings meshSettings = new TVEModelSettings();
            meshSettings.meshPath = meshPath;

            var modelImporter = AssetImporter.GetAtPath(meshPath) as ModelImporter;

            if (modelImporter != null)
            {
                var doPrecessing = false;

                if (!modelImporter.isReadable)
                {
                    doPrecessing = true;
                }

                if (modelImporter.keepQuads)
                {
                    doPrecessing = true;
                }

                if (modelImporter.meshCompression != ModelImporterMeshCompression.Off)
                {
                    doPrecessing = true;
                }

                if (doPrecessing)
                {
                    meshSettings.isReadable = modelImporter.isReadable;
                    meshSettings.keepQuads = modelImporter.keepQuads;
                    meshSettings.meshCompression = modelImporter.meshCompression;

                    modelImporter.isReadable = true;
                    modelImporter.keepQuads = false;
                    modelImporter.meshCompression = ModelImporterMeshCompression.Off;
                    modelImporter.SaveAndReimport();
                    AssetDatabase.Refresh();

                    meshSettings.requiresProcessing = true;
                }
            }

            //string fileText = File.ReadAllText(meshPath);
            //fileText = fileText.Replace("m_IsReadable: 0", "m_IsReadable: 1");
            //File.WriteAllText(meshPath, fileText);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            return meshSettings;
        }

        public static void PostProcessMesh(string meshPath, TVEModelSettings meshSettings)
        {
            if (meshSettings.requiresProcessing)
            {
                var modelImporter = AssetImporter.GetAtPath(meshPath) as ModelImporter;

                if (modelImporter != null)
                {
                    modelImporter.isReadable = meshSettings.isReadable;
                    modelImporter.keepQuads = meshSettings.keepQuads;
                    modelImporter.meshCompression = meshSettings.meshCompression;
                    modelImporter.SaveAndReimport();
                }
            }
        }

        public static void CreateModifiableMeshes(Mesh mesh)
        {
            var meshPath = AssetDatabase.GetAssetPath(mesh);

            var meshBase = UnityEngine.Object.Instantiate(mesh);
            var meshMotion = UnityEngine.Object.Instantiate(mesh);

            var vertexCount = mesh.vertexCount;

            var colors = new List<Color>(vertexCount);
            var UV0 = new List<Vector4>(vertexCount);
            var UV2 = new List<Vector4>(vertexCount);

            mesh.GetColors(colors);
            mesh.GetUVs(0, UV0);
            mesh.GetUVs(1, UV2);

            var dataUV3 = new List<Vector2>(vertexCount);
            var dataBase = new List<Color>(vertexCount);
            var dataMotion = new List<Color>(vertexCount);

            for (int i = 0; i < vertexCount; i++)
            {
                // Store Detail UVs
                dataUV3.Add(new Vector4(UV2[i].z, UV2[i].w, 0, 0));

                // Store Variation, Occlusion and Detail Mask
                dataBase.Add(new Color(colors[i].r, colors[i].g, colors[i].b, 0));

                // Store HeightTexture Mask, Branch Mask and Leaves Mask
                var motionMasks = MathFloatFromVector2(UV0[i].z);
                dataMotion.Add(new Color(colors[i].a, motionMasks.x, motionMasks.y, 0));
            }

            meshBase.SetUVs(2, dataUV3);
            meshBase.SetColors(dataBase);
            meshMotion.SetColors(dataMotion);

            var basePath = meshPath.Replace("TVE Model", "Modifiable Base");
            var motionPath = meshPath.Replace("TVE Model", "Modifiable Motion");

            if (!File.Exists(basePath))
            {
                AssetDatabase.CreateAsset(meshBase, basePath);
            }
            else
            {
                var meshFile = AssetDatabase.LoadAssetAtPath<Mesh>(basePath);
                meshFile.Clear();
                EditorUtility.CopySerialized(meshBase, meshFile);
            }

            if (!File.Exists(motionPath))
            {
                AssetDatabase.CreateAsset(meshMotion, motionPath);
            }
            else
            {
                var meshFile = AssetDatabase.LoadAssetAtPath<Mesh>(motionPath);
                meshFile.Clear();
                EditorUtility.CopySerialized(meshMotion, meshFile);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CombineModifiableMeshes(Mesh mesh, Mesh meshBase, Mesh meshMotion)
        {
            var newMesh = UnityEngine.Object.Instantiate(mesh);
            newMesh.name = mesh.name;

            var vertexCount = mesh.vertexCount;

            var newColors = new List<Color>(vertexCount);
            var newUV0 = new List<Vector4>(vertexCount);
            var newUV2 = new List<Vector4>(vertexCount);

            mesh.GetColors(newColors);
            mesh.GetUVs(0, newUV0);
            mesh.GetUVs(1, newUV2);

            var dataUV3 = new List<Vector2>(vertexCount);
            var dataBase = new List<Color>(vertexCount);
            var dataMotion = new List<Color>(vertexCount);

            meshBase.GetUVs(3, dataUV3);
            meshBase.GetColors(dataBase);
            meshMotion.GetColors(dataMotion);

            for (int i = 0; i < vertexCount; i++)
            {
                newColors[i] = new Color(dataBase[i].r, dataBase[i].g, dataBase[i].b, dataMotion[i].r);
                newUV0[i] = new Vector4(newUV0[i].x, newUV0[i].y, TVEUtils.MathVector2ToFloat(dataMotion[i].g, dataMotion[i].b), newUV0[i].w);
                newUV2[i] = new Vector4(newUV2[i].x, newUV2[i].y, dataUV3[i].x, dataUV3[i].y);
            }

            newMesh.SetColors(newColors);
            newMesh.SetUVs(0, newUV0);
            newMesh.SetUVs(1, newUV2);

            mesh.Clear();

            if (!mesh.isReadable)
            {
                newMesh.UploadMeshData(true);
            }

            EditorUtility.CopySerialized(newMesh, mesh);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Packer Utils
        public static TVEPackerData InitPackerData()
        {
            TVEPackerData packerData = new TVEPackerData();

            packerData.blitMaterial = new Material(Shader.Find("Hidden/BOXOPHOBIC/The Vegetation Engine/Helpers/Packer"));

            packerData.maskChannels = new int[4];
            packerData.maskCoords = new int[4];
            //maskLayers = new int[4];
            packerData.maskActions0 = new int[4];
            packerData.maskActions1 = new int[4];
            packerData.maskActions2 = new int[4];
            packerData.maskTextures = new Texture[4];

            for (int i = 0; i < 4; i++)
            {
                packerData.maskChannels[i] = 0;
                packerData.maskCoords[i] = 0;
                //maskLayers[i] = 0;
                packerData.maskActions0[i] = 0;
                packerData.maskActions1[i] = 0;
                packerData.maskActions2[i] = 0;
                packerData.maskTextures[i] = null;
            }

            return packerData;
        }

        public static Texture PackAndSaveTexture(string dataPath, TVEPackerData packerData)
        {
            Texture packedTexture;

            int importSize = GetPackedTextureImportSize(packerData);

            List<TVETextureSettings> uniqueTextureSettings = new List<TVETextureSettings>();

            for (int i = 0; i < packerData.maskTextures.Length; i++)
            {
                var texture = packerData.maskTextures[i];

                if (texture != null)
                {
                    var texturePath = AssetDatabase.GetAssetPath(texture);

                    bool exists = false;

                    for (int s = 0; s < uniqueTextureSettings.Count; s++)
                    {
                        if (texturePath == uniqueTextureSettings[s].texturePath)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        var textureSettings = TVEUtils.PreProcessTexture(texturePath);
                        uniqueTextureSettings.Add(textureSettings);
                    }
                }
            }

            //Set Packer Metallic channel
            if (packerData.maskTextures[0] != null)
            {
                packerData.blitMaterial.SetTexture("_Packer_TexR", packerData.maskTextures[0]);
                packerData.blitMaterial.SetInt("_Packer_ChannelR", packerData.maskChannels[0]);
                packerData.blitMaterial.SetInt("_Packer_CoordR", packerData.maskCoords[0]);
                //blitMaterial.SetInt("_Packer_LayerR", maskLayers[0]);
                packerData.blitMaterial.SetInt("_Packer_Action0R", packerData.maskActions0[0]);
                packerData.blitMaterial.SetInt("_Packer_Action1R", packerData.maskActions1[0]);
                packerData.blitMaterial.SetInt("_Packer_Action2R", packerData.maskActions2[0]);
            }
            else
            {
                packerData.blitMaterial.SetInt("_Packer_ChannelR", 0);
                packerData.blitMaterial.SetFloat("_Packer_FloatR", 1.0f);
            }

            //Set Packer Occlusion channel
            if (packerData.maskTextures[1] != null)
            {
                packerData.blitMaterial.SetTexture("_Packer_TexG", packerData.maskTextures[1]);
                packerData.blitMaterial.SetInt("_Packer_ChannelG", packerData.maskChannels[1]);
                packerData.blitMaterial.SetInt("_Packer_CoordG", packerData.maskCoords[1]);
                //blitMaterial.SetInt("_Packer_LayerG", maskLayers[1]);
                packerData.blitMaterial.SetInt("_Packer_Action0G", packerData.maskActions0[1]);
                packerData.blitMaterial.SetInt("_Packer_Action1G", packerData.maskActions1[1]);
                packerData.blitMaterial.SetInt("_Packer_Action2G", packerData.maskActions2[1]);
            }
            else
            {
                packerData.blitMaterial.SetInt("_Packer_ChannelG", 0);
                packerData.blitMaterial.SetFloat("_Packer_FloatG", 1.0f);
            }

            //Set Packer Mask channel
            if (packerData.maskTextures[2] != null)
            {
                packerData.blitMaterial.SetTexture("_Packer_TexB", packerData.maskTextures[2]);
                packerData.blitMaterial.SetInt("_Packer_ChannelB", packerData.maskChannels[2]);
                packerData.blitMaterial.SetInt("_Packer_CoordB", packerData.maskCoords[2]);
                //blitMaterial.SetInt("_Packer_LayerB", maskLayers[2]);
                packerData.blitMaterial.SetInt("_Packer_Action0B", packerData.maskActions0[2]);
                packerData.blitMaterial.SetInt("_Packer_Action1B", packerData.maskActions1[2]);
                packerData.blitMaterial.SetInt("_Packer_Action2B", packerData.maskActions2[2]);
            }
            else
            {
                packerData.blitMaterial.SetInt("_Packer_ChannelB", 0);
                packerData.blitMaterial.SetFloat("_Packer_FloatB", 1.0f);
            }

            //Set Packer Smothness channel
            if (packerData.maskTextures[3] != null)
            {
                packerData.blitMaterial.SetTexture("_Packer_TexA", packerData.maskTextures[3]);
                packerData.blitMaterial.SetInt("_Packer_ChannelA", packerData.maskChannels[3]);
                packerData.blitMaterial.SetInt("_Packer_CoordA", packerData.maskCoords[3]);
                //blitMaterial.SetInt("_Packer_LayerA", maskLayers[3]);
                packerData.blitMaterial.SetInt("_Packer_Action0A", packerData.maskActions0[3]);
                packerData.blitMaterial.SetInt("_Packer_Action1A", packerData.maskActions1[3]);
                packerData.blitMaterial.SetInt("_Packer_Action2A", packerData.maskActions2[3]);
            }
            else
            {
                packerData.blitMaterial.SetInt("_Packer_ChannelA", 0);
                packerData.blitMaterial.SetFloat("_Packer_FloatA", 1.0f);
            }

            packerData.blitMaterial.SetInt("_Packer_TransformSpace", packerData.transformSpace);

            Vector2 pixelSize = GetPackedTexturePixelSize(packerData);

            RenderTexture renderTexure = new RenderTexture((int)pixelSize.x, (int)pixelSize.y, 0, RenderTextureFormat.ARGBFloat);
            Texture2D packedTexure = new Texture2D(renderTexure.width, renderTexure.height, TextureFormat.RGBAFloat, false);

            if (packerData.blitMesh != null)
            {
                var currentRenderTexture = RenderTexture.active;

                Graphics.SetRenderTarget(renderTexure);

                GL.Clear(false, true, new Color(0.5f, 0.5f, 1f, 0f), 0f);

                GL.PushMatrix();
                GL.LoadOrtho();

                packerData.blitMaterial.SetPass(packerData.blitPass);

                Graphics.DrawMeshNow(packerData.blitMesh, Matrix4x4.identity);

                RenderTexture.active = renderTexure;
                packedTexure.ReadPixels(new Rect(0, 0, renderTexure.width, renderTexure.height), 0, 0);
                packedTexure.Apply();
                RenderTexture.active = currentRenderTexture;

                Graphics.SetRenderTarget(null);
                GL.PopMatrix();
            }
            else
            {
                var currentRenderTexture = RenderTexture.active;

                Graphics.Blit(Texture2D.whiteTexture, renderTexure, packerData.blitMaterial, packerData.blitPass);
                RenderTexture.active = renderTexure;
                packedTexure.ReadPixels(new Rect(0, 0, renderTexure.width, renderTexure.height), 0, 0);
                packedTexure.Apply();
                RenderTexture.active = currentRenderTexture;
            }

            renderTexure.Release();

            if (dataPath.EndsWith(".asset"))
            {
                if (File.Exists(dataPath))
                {
                    var assetTexture = AssetDatabase.LoadAssetAtPath<Texture>(dataPath);

                    EditorUtility.CopySerialized(packedTexure, assetTexture);
                }
                else
                {
                    AssetDatabase.CreateAsset(packedTexure, dataPath);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                byte[] bytes;

                if (dataPath.EndsWith("png"))
                {
                    bytes = packedTexure.EncodeToPNG();
                }
                else if (dataPath.EndsWith("tga"))
                {
                    bytes = packedTexure.EncodeToTGA();
                }
                else
                {
                    bytes = packedTexure.EncodeToEXR();
                }

                File.WriteAllBytes(dataPath, bytes);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                TextureImporter texImporter = AssetImporter.GetAtPath(dataPath) as TextureImporter;

                if (packerData.saveAsDefault)
                {
                    texImporter.textureType = TextureImporterType.Default;
                }
                else
                {
                    texImporter.textureType = TextureImporterType.NormalMap;
                }

                texImporter.maxTextureSize = importSize;
                texImporter.sRGBTexture = packerData.saveAsSRGB;
                texImporter.alphaSource = TextureImporterAlphaSource.FromInput;

                texImporter.SaveAndReimport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            packedTexture = AssetDatabase.LoadAssetAtPath<Texture>(dataPath);

            for (int i = 0; i < uniqueTextureSettings.Count; i++)
            {
                var textureSettings = uniqueTextureSettings[i];
                TVEUtils.PostProcessTexture(textureSettings.texturePath, textureSettings);
            }

            return packedTexture;
        }

        static int GetPackedTextureImportSize(TVEPackerData packerData)
        {
            int initSize = 32;

            for (int i = 0; i < packerData.maskTextures.Length; i++)
            {
                var texture = packerData.maskTextures[i];

                if (texture != null)
                {
                    string texPath = AssetDatabase.GetAssetPath(texture);
                    TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;

                    initSize = Mathf.Max(initSize, texImporter.maxTextureSize);
                }
            }

            return initSize;
        }

        static Vector2 GetPackedTexturePixelSize(TVEPackerData packerData)
        {
            int x = 32;
            int y = 32;

            for (int i = 0; i < packerData.maskTextures.Length; i++)
            {
                var texture = packerData.maskTextures[i];

                if (texture != null)
                {
                    x = Mathf.Max(x, texture.width);
                    y = Mathf.Max(y, texture.height);
                }
            }

            return new Vector2(x, y);
        }

        public static TVETextureSettings PreProcessTexture(string texturePath)
        {
            TVETextureSettings textureSettings = new TVETextureSettings();
            textureSettings.texturePath = texturePath;

            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            textureSettings.textureCompression = importer.textureCompression;
            textureSettings.maxTextureSize = importer.maxTextureSize;

            importer.ReadTextureSettings(textureSettings.textureSettings);

            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = false;
            importer.maxTextureSize = 8192;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            importer.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return textureSettings;
        }

        public static void PostProcessTexture(string texturePath, TVETextureSettings textureSettings)
        {
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            importer.textureCompression = textureSettings.textureCompression;
            importer.maxTextureSize = textureSettings.maxTextureSize;

            importer.SetTextureSettings(textureSettings.textureSettings);

            importer.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // GameObject Utils
        public static void GetChildRecursive(GameObject go, List<GameObject> gameObjects)
        {
            foreach (Transform child in go.transform)
            {
                if (child == null)
                    continue;

                gameObjects.Add(child.gameObject);
                GetChildRecursive(child.gameObject, gameObjects);
            }
        }

        public static void GetChildRecursive(GameObject go, List<TVEGameObjectData> gameObjectsData)
        {
            foreach (Transform child in go.transform)
            {
                if (child == null)
                    continue;

                var gameObjectData = new TVEGameObjectData();
                gameObjectData.gameObject = child.gameObject;

                gameObjectsData.Add(gameObjectData);
                GetChildRecursive(child.gameObject, gameObjectsData);
            }
        }
#endif

        // Math Utils
        public static float MathRemap(float value, float low1, float high1, float low2, float high2)
        {
            return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
        }

        public static float MathVector2ToFloat(float x, float y)
        {
            Vector2 output;

            output.x = Mathf.Floor(x * (2048 - 1));
            output.y = Mathf.Floor(y * (2048 - 1));

            return (output.x * 2048) + output.y;
        }

        public static Vector2 MathFloatFromVector2(float input)
        {
            Vector2 output;

            output.y = input % 2048f;
            output.x = Mathf.Floor(input / 2048f);

            return output / (2048f - 1);
        }

        // Texture Utils
        public static Color GetGlobalTextureData(Vector3 position, int layer, TVERenderData renderData, Texture2DArray texture2DArray)
        {
            if (layer > renderData.bufferSize)
            {
                Debug.Log("<b>[The Vegetation Engine]</b> The requested global texture layer does not exist!");
                return Color.black;
            }

            if (texture2DArray == null || texture2DArray.depth != renderData.textureObject.volumeDepth)
            {
                texture2DArray = new Texture2DArray(1, 1, renderData.textureObject.volumeDepth, TextureFormat.RGBAHalf, false);
            }

            var volumePosition = renderData.internalPosition;
            var volumeScale = renderData.internalScale;

            var normalizedPositionX = Mathf.Clamp(TVEUtils.MathRemap(position.x, volumePosition.x + (-volumeScale.x / 2), volumePosition.x + (volumeScale.x / 2), 0, 1), 0.001f, 1);
            var normalizedPositionZ = Mathf.Clamp(TVEUtils.MathRemap(position.z, volumePosition.z + (-volumeScale.z / 2), volumePosition.z + (volumeScale.z / 2), 0, 1), 0.001f, 1);

            var pixelPositionX = Mathf.RoundToInt(normalizedPositionX * renderData.textureObject.width - 1);
            var pixelPositionZ = Mathf.RoundToInt(normalizedPositionZ * renderData.textureObject.height - 1);

            var asyncGPUReadback = AsyncGPUReadback.Request(renderData.textureObject, 0, pixelPositionX, 1, pixelPositionZ, 1, layer, 1);
            asyncGPUReadback.WaitForCompletion();

            if (!asyncGPUReadback.hasError)
            {
                texture2DArray.SetPixelData(asyncGPUReadback.GetData<byte>(), 0, layer);
                texture2DArray.Apply();

                //AsyncGPUReadback.Request(renderData.texObject, 0, (AsyncGPUReadbackRequest asyncAction) =>
                //{
                //    texture2DArray.SetPixelData(asyncAction.GetData<byte>(), 0, 0);
                //    texture2DArray.Apply();
                //});

                var texture2DPixels = texture2DArray.GetPixels(layer, 0);

                return texture2DPixels[0];
            }
            else
            {
                return Color.black;
            }
        }


#if UNITY_EDITOR
        // Material Utils
        public static void UnloadMaterialFromMemory(Material material)
        {
            var shader = material.shader;

            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var propName = ShaderUtil.GetPropertyName(shader, i);
                    var texture = material.GetTexture(propName);

                    if (texture != null)
                    {
                        Resources.UnloadAsset(texture);
                    }
                }
            }
        }

        public static void CopyMaterialProperties(Material oldMaterial, Material newMaterial)
        {
            var oldShader = oldMaterial.shader;
            var newShader = newMaterial.shader;

            for (int i = 0; i < ShaderUtil.GetPropertyCount(oldShader); i++)
            {
                for (int j = 0; j < ShaderUtil.GetPropertyCount(newShader); j++)
                {
                    var propertyName = ShaderUtil.GetPropertyName(oldShader, i);
                    var propertyType = ShaderUtil.GetPropertyType(oldShader, i);

                    if (propertyName == ShaderUtil.GetPropertyName(newShader, j))
                    {
                        if (propertyType == ShaderUtil.ShaderPropertyType.Color || propertyType == ShaderUtil.ShaderPropertyType.Vector)
                        {
#if UNITY_2022_1_OR_NEWER
                        if (!oldMaterial.IsPropertyLocked(propertyName))
                        {
                            newMaterial.SetVector(propertyName, oldMaterial.GetVector(propertyName));
                        }
#else
                        newMaterial.SetVector(propertyName, oldMaterial.GetVector(propertyName));
#endif
                        }

                        if (propertyType == ShaderUtil.ShaderPropertyType.Float || propertyType == ShaderUtil.ShaderPropertyType.Range)
                        {
#if UNITY_2022_1_OR_NEWER
                        if (!oldMaterial.IsPropertyLocked(propertyName))
                        {
                            newMaterial.SetFloat(propertyName, oldMaterial.GetFloat(propertyName));
                        }
#else
                            newMaterial.SetFloat(propertyName, oldMaterial.GetFloat(propertyName));
#endif
                        }

                        if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
#if UNITY_2022_1_OR_NEWER
                        if (!oldMaterial.IsPropertyLocked(propertyName))
                        {
                            newMaterial.SetTexture(propertyName, oldMaterial.GetTexture(propertyName));
                        }
#else
                            newMaterial.SetTexture(propertyName, oldMaterial.GetTexture(propertyName));
#endif
                        }
                    }
                }
            }
        }

        public static void CopyMaterialPropertiesFromBlock(MaterialPropertyBlock propertyBlock, Material newMaterial)
        {
            var newShader = newMaterial.shader;

            for (int i = 0; i < ShaderUtil.GetPropertyCount(newShader); i++)
            {
                var propertyName = ShaderUtil.GetPropertyName(newShader, i);
                var propertyType = ShaderUtil.GetPropertyType(newShader, i);

                if (propertyType == ShaderUtil.ShaderPropertyType.Color || propertyType == ShaderUtil.ShaderPropertyType.Vector)
                {
                    if (propertyBlock.HasColor(propertyName))
                    {
                        newMaterial.SetVector(propertyName, propertyBlock.GetColor(propertyName));
                    }
                }

                if (propertyType == ShaderUtil.ShaderPropertyType.Float || propertyType == ShaderUtil.ShaderPropertyType.Range)
                {
                    if (propertyBlock.HasFloat(propertyName))
                    {
                        newMaterial.SetFloat(propertyName, propertyBlock.GetFloat(propertyName));
                    }
                }

                if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    if (propertyBlock.HasTexture(propertyName))
                    {
                        newMaterial.SetTexture(propertyName, propertyBlock.GetTexture(propertyName));
                    }
                }
            }
        }

        // Shader Utils
        public static bool IsValidTVEShader(string shaderPath)
        {
            bool valid = false;

            if (!shaderPath.Contains("GPUI") && !shaderPath.Contains("Helper") && !shaderPath.Contains("Legacy") && !shaderPath.Contains("Terrain Shaders"))
            {
                var shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);

                if (shader != null && !shader.name.Contains("Landscape"))
                {
                    var material = new Material(shader);

                    if (material.HasProperty("_IsTVEShader"))
                    {
                        valid = true;
                    }
                }
            }

            return valid;
        }

        public static List<string> GetCoreShaderPaths()
        {
            var coreShaderPaths = new List<string>();

            var allShaderPaths = Directory.GetFiles("Assets/", "*.shader", SearchOption.AllDirectories);

            for (int i = 0; i < allShaderPaths.Length; i++)
            {
                if (IsValidTVEShader(allShaderPaths[i]))
                {
                    coreShaderPaths.Add(allShaderPaths[i]);
                }
            }

            return coreShaderPaths;
        }

        public static string[] ShaderModelOptions =
        {
            "Shader Model 2.0",
            "Shader Model 2.5",
            "Shader Model 3.0",
            "Shader Model 3.5",
            "Shader Model 4.0",
            "Shader Model 4.5",
            "Shader Model 4.6",
            "Shader Model 5.0",
        };

        public static string[] RenderEngineOptions =
        {
            "Unity Default Renderer",
            "Vegetation Studio (Instanced Indirect)",
            "Vegetation Studio 1.4.5+ (Instanced Indirect)",
            "Nature Renderer (Procedural Instancing)",
            "Foliage Renderer (Instanced Indirect)",
            "Flora Renderer (Instanced Indirect)",
            "GPU Instancer (Instanced Indirect)",
            "Renderer Stack (Instanced Indirect)",
            "Instant Renderer (Instanced Indirect)",
            "Disable SRP Batcher Compatibility",
        };

        public static TVEShaderSettings GetShaderSettings(string shaderPath)
        {
            TVEShaderSettings shaderSettings = new TVEShaderSettings();
            shaderSettings.shaderPath = shaderPath;

            StreamReader reader = new StreamReader(shaderPath);

            string lines = reader.ReadToEnd();

            for (int i = 0; i < RenderEngineOptions.Length; i++)
            {
                var renderEngine = RenderEngineOptions[i];

                if (lines.Contains(renderEngine))
                {
                    shaderSettings.renderEngine = renderEngine;
                    break;
                }
            }

            for (int i = 0; i < ShaderModelOptions.Length; i++)
            {
                var shaderModel = ShaderModelOptions[i].Replace("Shader Model", "#pragma target");

                if (lines.Contains(shaderModel))
                {
                    shaderSettings.shaderModel = ShaderModelOptions[i];
                    break;
                }
            }

            reader.Close();

            return shaderSettings;
        }

        public static void SetShaderSettings(string shaderPath, TVEShaderSettings shaderSettings)
        {
            var renderEngine = shaderSettings.renderEngine;
            var shaderModel = shaderSettings.shaderModel.Replace("Shader Model", "#pragma target");

            string[] engineVegetationStudio = new string[]
            {
            "           //Vegetation Studio (Instanced Indirect)",
            "           #include \"XXX/Core/Includes/VS_Indirect.cginc\"",
            "           #pragma instancing_options procedural:setup forwardadd",
            "           #pragma multi_compile GPU_FRUSTUM_ON __",
            };

            string[] engineVegetationStudioHD = new string[]
            {
            "           //Vegetation Studio (Instanced Indirect)",
            "           #include \"XXX/Core/Includes/VS_IndirectHD.cginc\"",
            "           #pragma instancing_options procedural:setupVSPro forwardadd",
            "           #pragma multi_compile GPU_FRUSTUM_ON __",
            };

            string[] engineVegetationStudio145 = new string[]
            {
            "           //Vegetation Studio 1.4.5+ (Instanced Indirect)",
            "           #include \"XXX/Core/Includes/VS_Indirect145.cginc\"",
            "           #pragma instancing_options procedural:setupVSPro forwardadd",
            "           #pragma multi_compile GPU_FRUSTUM_ON __",
            };

            string[] engineNatureRenderer = new string[]
            {
            "           //Nature Renderer (Procedural Instancing)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:SetupNatureRenderer forwardadd",
            };

            string[] engineFoliageRenderer = new string[]
            {
            "           //Foliage Renderer (Instanced Indirect)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:setupFoliageRenderer forwardadd",
            };

            string[] engineFloraRenderer = new string[]
            {
            "           //Flora Renderer (Instanced Indirect)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:FloraInstancingSetup forwardadd",
            };

            string[] engineGPUInstancer = new string[]
            {
            "           //GPU Instancer (Instanced Indirect)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:setupGPUI forwardadd",
            };

            string[] engineRendererStack = new string[]
            {
            "           //Renderer Stack (Instanced Indirect)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:GPUInstancedIndirectInclude forwardadd",
            };

            string[] engineInstantRenderer = new string[]
            {
            "           //Instant Renderer (Instanced Indirect)",
            "           #include \"XXX\"",
            "           #pragma instancing_options procedural:setupInstantRenderer forwardadd",
            };

            var assetFolder = TVEUtils.FindAsset("The Vegetation Engine.pdf");
            assetFolder = assetFolder.Replace("/The Vegetation Engine.pdf", "");

            var cgincNatureRenderer = "Assets/Visual Design Cafe/Nature Shaders/Common/Nodes/Integrations/Nature Renderer.cginc";

            if (!File.Exists(cgincNatureRenderer))
            {
                cgincNatureRenderer = TVEUtils.FindAsset("Nature Renderer.cginc");
            }

            var cgincFoliageRenderer = "Packages/com.jbooth.foliagerenderer/Shaders/FoliageRendererInstancing.cginc";

            if (!File.Exists(cgincFoliageRenderer))
            {
                cgincFoliageRenderer = TVEUtils.FindAsset("FoliageRendererInstancing.cginc");
            }

            var cgincFloraRenderer = "Packages/com.ma.flora/ShaderLibrary/Flora.hlsl";

            if (!File.Exists(cgincFloraRenderer))
            {
                cgincFloraRenderer = TVEUtils.FindAsset("Flora.hlsl");
            }

            var cgincGPUInstancer = "Assets/GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc";

            if (!File.Exists(cgincGPUInstancer))
            {
                cgincGPUInstancer = TVEUtils.FindAsset("GPUInstancerInclude.cginc");
            }

            var cgincRendererStack = "Assets/VladislavTsurikov/RendererStack/Shaders/Include/GPUInstancedIndirectInclude.cginc";

            if (!File.Exists(cgincRendererStack))
            {
                cgincRendererStack = TVEUtils.FindAsset("GPUInstancedIndirectInclude.cginc");
            }

            var cgincInstantRenderer = "Assets/VladislavTsurikov/InstantRenderer/Shaders/Include/InstantRendererInclude.cginc";

            if (!File.Exists(cgincInstantRenderer))
            {
                cgincInstantRenderer = TVEUtils.FindAsset("GPUInstancerInclude.cginc");
            }

            // Add correct paths for VSP and GPUI
            engineVegetationStudio[1] = engineVegetationStudio[1].Replace("XXX", assetFolder);
            engineVegetationStudioHD[1] = engineVegetationStudioHD[1].Replace("XXX", assetFolder);
            engineVegetationStudio145[1] = engineVegetationStudio145[1].Replace("XXX", assetFolder);
            engineNatureRenderer[1] = engineNatureRenderer[1].Replace("XXX", cgincNatureRenderer);
            engineFoliageRenderer[1] = engineFoliageRenderer[1].Replace("XXX", cgincFoliageRenderer);
            engineFloraRenderer[1] = engineFloraRenderer[1].Replace("XXX", cgincFloraRenderer);
            engineGPUInstancer[1] = engineGPUInstancer[1].Replace("XXX", cgincGPUInstancer);
            engineRendererStack[1] = engineRendererStack[1].Replace("XXX", cgincRendererStack);
            engineInstantRenderer[1] = engineInstantRenderer[1].Replace("XXX", cgincInstantRenderer);

            var isHDPipeline = false;

            StreamReader reader = new StreamReader(shaderPath);

            List<string> lines = new List<string>();

            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }

            reader.Close();

            int count = lines.Count;

            if (shaderModel != "From Shader")
            {
                for (int i = 0; i < count; i++)
                {
                    if (lines[i].Contains("#pragma target"))
                    {
                        lines[i] = shaderModel;
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                {
                    int c = 0;
                    int j = i + 1;

                    while (lines[j].Contains("SHADER INJECTION POINT END") == false)
                    {
                        j++;
                        c++;
                    }

                    lines.RemoveRange(i + 1, c);
                    count = count - c;
                }
            }

            count = lines.Count;

            for (int i = 0; i < count; i++)
            {
                if (lines[i].Contains("HDRenderPipeline"))
                {
                    isHDPipeline = true;
                }

                if (lines[i].Contains("[HideInInspector] _DisableSRPBatcher"))
                {
                    lines.RemoveAt(i);
                    count--;
                }
            }

            //Inject 3rd Party Support
            if (renderEngine.Contains("Vegetation Studio (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        if (isHDPipeline)
                        {
                            lines.InsertRange(i + 1, engineVegetationStudioHD);
                        }
                        else
                        {
                            lines.InsertRange(i + 1, engineVegetationStudio);
                        }
                    }
                }
            }

            if (renderEngine.Contains("Vegetation Studio 1.4.5+ (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineVegetationStudio145);
                    }
                }
            }

            if (renderEngine.Contains("Nature Renderer (Procedural Instancing)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineNatureRenderer);
                    }
                }
            }

            if (renderEngine.Contains("Foliage Renderer (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineFoliageRenderer);
                    }
                }
            }

            if (renderEngine.Contains("Flora Renderer (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineFloraRenderer);
                    }
                }
            }

            if (renderEngine.Contains("GPU Instancer (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineGPUInstancer);
                    }
                }
            }

            if (renderEngine.Contains("Renderer Stack (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineRendererStack);
                    }
                }
            }

            if (renderEngine.Contains("Instant Renderer (Instanced Indirect)"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("SHADER INJECTION POINT BEGIN"))
                    {
                        lines.InsertRange(i + 1, engineInstantRenderer);
                    }
                }
            }

            if (renderEngine.Contains("Disable SRP Batcher Compatibility"))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].EndsWith("Properties"))
                    {
                        lines.Insert(i + 2, "		[HideInInspector] _DisableSRPBatcher(\"_DisableSRPBatcher\", Float) = 0 //Disable SRP Batcher Compatibility");
                    }
                }
            }

            //for (int i = 0; i < lines.Count; i++)
            //{
            //    // Disable ASE Drawers
            //    if (lines[i].Contains("[ASEBegin]"))
            //    {
            //        lines[i] = lines[i].Replace("[ASEBegin]", "");
            //    }

            //    if (lines[i].Contains("[ASEnd]"))
            //    {
            //        lines[i] = lines[i].Replace("[ASEnd]", "");
            //    }
            //}

#if !AMPLIFY_SHADER_EDITOR && !UNITY_2020_2_OR_NEWER

            // Add diffusion profile support for HDRP 7
            if (isHDPipeline)
            {
                if (shaderPath.Contains("Subsurface Lit"))
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("[DiffusionProfile]"))
                        {
                            lines[i] = lines[i].Replace("[DiffusionProfile]", "[StyledDiffusionMaterial(_SubsurfaceDiffusion)]");
                        }
                    }
                }
            }

#elif AMPLIFY_SHADER_EDITOR && !UNITY_2020_2_OR_NEWER

            // Add diffusion profile support
            if (isHDPipeline)
            {
                if (shaderAssetPath.Contains("Subsurface Lit"))
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Contains("[HideInInspector][Space(10)][ASEDiffusionProfile(_SubsurfaceDiffusion)]"))
                        {
                            lines[i] = lines[i].Replace("[HideInInspector][Space(10)][ASEDiffusionProfile(_SubsurfaceDiffusion)]", "[Space(10)][ASEDiffusionProfile(_SubsurfaceDiffusion)]");
                        }

                        if (lines[i].Contains("[DiffusionProfile]") && !lines[i].Contains("[HideInInspector]"))
                        {
                            lines[i] = lines[i].Replace("[DiffusionProfile]", "[HideInInspector][DiffusionProfile]");
                        }

                        if (lines[i].Contains("[StyledDiffusionMaterial(_SubsurfaceDiffusion)]"))
                        {
                            lines[i] = lines[i].Replace("[StyledDiffusionMaterial(_SubsurfaceDiffusion)]", "[HideInInspector][StyledDiffusionMaterial(_SubsurfaceDiffusion)]");
                        }
                    }
                }
            }
#endif

            StreamWriter writer = new StreamWriter(shaderPath);

            for (int i = 0; i < lines.Count; i++)
            {
                writer.WriteLine(lines[i]);
            }

            writer.Close();

            lines = new List<string>();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetDatabase.ImportAsset(shaderPath);
        }

        public static void CopyOrReplaceShader(string oldShaderPath, string newShaderPath, string newShaderName)
        {
            if (File.Exists(newShaderPath))
            {
                // Copy old shader content
                StreamReader reader = new StreamReader(oldShaderPath);

                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                reader.Close();

                for (int i = 0; i < 10; i++)
                {
                    if (lines[i].StartsWith("Shader"))
                    {
                        lines[i] = "Shader \"" + newShaderName + "\"";
                    }
                }

                StreamWriter writer = new StreamWriter(newShaderPath, false);

                for (int i = 0; i < lines.Count; i++)
                {
                    writer.WriteLine(lines[i]);
                }

                writer.Close();

                lines = new List<string>();

                AssetDatabase.ImportAsset(newShaderPath);
                AssetDatabase.Refresh();
            }
            else
            {
                AssetDatabase.CopyAsset(oldShaderPath, newShaderPath);
                AssetDatabase.Refresh();
            }
        }

        // GUI Utils
        public static void DrawShaderBanner(Material material)
        {
            GUIStyle titleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subTitleStyle = new GUIStyle("label")
            {
                richText = true,
                alignment = TextAnchor.MiddleRight
            };

            var splitLine = material.shader.name.Split(char.Parse("/"));
            var splitCount = splitLine.Length;
            var shaderName = splitLine[splitCount - 1];
            var shaderCategory = splitLine[splitCount - 2];
            var shaderType = splitLine[splitCount - 3];

            var subtitle = "";

            if (shaderType == "The Vegetation Engine")
            {
                if (AssetDatabase.GetAssetPath(material.shader).Contains("Core"))
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        subtitle = "<color=#ffdb78>" + "Core" + " / " + shaderCategory + "</color>";
                    }
                    else
                    {
                        subtitle = "Core" + " / " + shaderCategory;
                    }
                }
                else
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        subtitle = "<color=#8affc4>" + "Custom" + " / " + shaderCategory + "</color>";
                    }
                    else
                    {
                        subtitle = "<b><color=#008c60>" + "Custom" + " / " + shaderCategory + "</color></b>";
                    }
                }
            }
            else
            {
                if (shaderType.Contains("The Vegetation Engine "))
                {
                    shaderType = shaderType.Replace("The Vegetation Engine ", "");

                    if (EditorGUIUtility.isProSkin)
                    {
                        subtitle = "<color=#8affc4>" + shaderType + " / " + shaderCategory + "</color>";
                    }
                    else
                    {
                        subtitle = "<b><color=#008c60>" + "Custom" + " / " + shaderCategory + "</color></b>";
                    }
                }
            }

            GUILayout.Space(10);

            var fullRect = GUILayoutUtility.GetRect(0, 0, 36, 0);
            var fillRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 36);
            var subRect = new Rect(0, fullRect.position.y - 2, fullRect.xMax, 36);
            var lineRect = new Rect(0, fullRect.position.y, fullRect.xMax + 3, 1);

            Color color;
            Color guiColor;

            if (EditorGUIUtility.isProSkin)
            {
                color = CONSTANT.ColorDarkGray;
                guiColor = CONSTANT.ColorLightGray;
            }
            else
            {
                color = CONSTANT.ColorLightGray;
                guiColor = CONSTANT.ColorDarkGray;
            }

            EditorGUI.DrawRect(fillRect, color);
            EditorGUI.DrawRect(lineRect, CONSTANT.LineColor);

            GUI.Label(fullRect, "<size=16><color=#" + ColorUtility.ToHtmlStringRGB(guiColor) + ">" + shaderName + "</color></size>", titleStyle);
            GUI.Label(subRect, "<size=10>" + subtitle + "</size>", subTitleStyle);

            GUILayout.Space(10);
        }

        public static void DrawBatchingSupport(Material material)
        {
            if (material.HasProperty("_VertexDataMode"))
            {
                var batching = material.GetInt("_VertexDataMode");

                bool toggle = false;

                if (batching > 0.5f)
                {
                    toggle = true;

                    EditorGUILayout.HelpBox("Use the Batching Support option when the object is statically batched. All vertex calculations are done in world space and features like Baked Pivots and Size options are not supported because the object pivot data is missing with static batching.", MessageType.Info);

                    GUILayout.Space(10);
                }

                toggle = EditorGUILayout.Toggle("Enable Batching Support", toggle);

                if (toggle)
                {
                    material.SetInt("_VertexDataMode", 1);
                }
                else
                {
                    material.SetInt("_VertexDataMode", 0);
                }
            }
        }

        public static void DrawDynamicSupport(Material material)
        {
            if (material.HasProperty("_VertexDynamicMode"))
            {
                var dynamic = material.GetInt("_VertexDynamicMode");

                bool toggle = false;

                if (dynamic > 0.5f)
                {
                    toggle = true;

                    if (material.HasProperty("_VertexPivotMode"))
                    {
                        GUILayout.Space(10);
                    }

                    EditorGUILayout.HelpBox("Use the Dynamic Support option when the object is moving or rotating. Usable when cutting tree or with scrollable environments! ", MessageType.Info);

                    GUILayout.Space(10);
                }

                toggle = EditorGUILayout.Toggle("Enable Dynamic Support", toggle);

                if (toggle)
                {
                    material.SetInt("_VertexDynamicMode", 1);
                }
                else
                {
                    material.SetInt("_VertexDynamicMode", 0);
                }
            }
        }

        public static void DrawPivotsSupport(Material material)
        {
            if (material.HasProperty("_VertexPivotMode"))
            {
                var pivot = material.GetInt("_VertexPivotMode");

                bool toggle = false;

                if (pivot > 0.5f)
                {
                    toggle = true;

                    if (material.shader.name.Contains("Impostors"))
                    {
                        EditorGUILayout.HelpBox("Pre Baked Pivots are not supported for impostor shaders!", MessageType.Error);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("The Pre Baked Pivots Support feature allows for using per mesh element interaction and elements influence. The option requires pre-baked pivots on prefab conversion!", MessageType.Info);
                    }

                    GUILayout.Space(10);
                }

                toggle = EditorGUILayout.Toggle("Enable Pre Baked Pivots ", toggle);

                if (toggle)
                {
                    material.SetInt("_VertexPivotMode", 1);
                }
                else
                {
                    material.SetInt("_VertexPivotMode", 0);
                }
            }
        }

        public static void DrawTechnicalDetails(Material material)
        {
            GUILayout.Space(10);

            var shaderName = material.shader.name;

            var styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
            };

            if (shaderName.Contains("Vertex Lit"))
            {
                DrawTechincalLabel("Shader Complexity: Cheap", styleLabel);
            }

            if (shaderName.Contains("Simple Lit"))
            {
                DrawTechincalLabel("Shader Complexity: Optimized", styleLabel);
            }

            if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
            {
                DrawTechincalLabel("Shader Complexity: Balanced", styleLabel);
            }

            if (!material.HasProperty("_IsElementShader"))
            {
                if (material.GetTag("RenderPipeline", false) == "HDRenderPipeline")
                {
                    DrawTechincalLabel("Render Pipeline: High Definition Render Pipeline", styleLabel);
                }
                else if (material.GetTag("RenderPipeline", false) == "UniversalPipeline")
                {
                    DrawTechincalLabel("Render Pipeline: Universal Render Pipeline", styleLabel);
                }
                else
                {
                    DrawTechincalLabel("Render Pipeline: Standard Render Pipeline", styleLabel);
                }
            }
            else
            {
                DrawTechincalLabel("Render Pipeline: Any Render Pipeline", styleLabel);
            }

            DrawTechincalLabel("Render Queue: " + material.renderQueue.ToString(), styleLabel);

            if (shaderName.Contains("Standard Lit") || shaderName.Contains("Simple Lit"))
            {
                DrawTechincalLabel("Render Path: Rendered in both Forward and Deferred path", styleLabel);
            }

            if (shaderName.Contains("Subsurface Lit"))
            {
                DrawTechincalLabel("Render Path: Always rendered in Forward path", styleLabel);
            }

            if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
            {
                DrawTechincalLabel("Lighting Model: Physicaly Based Shading", styleLabel);
            }

            if (shaderName.Contains("Vertex Lit"))
            {
                DrawTechincalLabel("Lighting Model: Cheap Vertex Shading", styleLabel);
            }

            if (shaderName.Contains("Simple Lit"))
            {
                DrawTechincalLabel("Lighting Model: Blinn Phong Shading", styleLabel);
            }

            if (shaderName.Contains("Simple Lit") || shaderName.Contains("Standard Lit"))
            {
                DrawTechincalLabel("Subsurface Model: Subsurface Scattering Approximation", styleLabel);
            }

            if (shaderName.Contains("Subsurface Lit"))
            {
                DrawTechincalLabel("Subsurface Model: Translucency Subsurface Scattering", styleLabel);
            }

            if (material.HasProperty("_IsPropShader"))
            {
                DrawTechincalLabel("Batching Support: Yes", styleLabel);
            }
            else if (material.HasProperty("_IsImpostorShader") || material.HasProperty("_IsElementShader"))
            {
                DrawTechincalLabel("Batching Support: No", styleLabel);
            }
            else
            {
                DrawTechincalLabel("Batching Support: No", styleLabel);
            }

            var elementTag = material.GetTag("ElementType", false, "");

            if (elementTag != "")
            {
                DrawTechincalLabel("Element Type Tag: " + elementTag, styleLabel);
            }
        }

        public static void DrawActiveKeywords(Material material)
        {
            GUILayout.Space(10);

            var styleLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
            };

            var keywords = material.enabledKeywords;

            for (int i = 0; i < keywords.Length; i++)
            {
                DrawTechincalLabel(keywords[i].name, styleLabel);
            }
        }


        public static void DrawTechincalLabel(string label, GUIStyle style)
        {
            GUILayout.Label("<size=10>" + label + "</size>", style);
        }

        public static void DrawCopySettingsFromObject(Material material)
        {
            Object inputObject = null;
            inputObject = (Object)EditorGUILayout.ObjectField("Copy Settings From Object", inputObject, typeof(Object), true);

            if (inputObject != null)
            {
                if (inputObject.GetType() == typeof(GameObject))
                {
                    var gameObject = (GameObject)inputObject;

                    if (gameObject.GetComponent<TVETerrain>() != null)
                    {
                        var terrain = gameObject.GetComponent<TVETerrain>();

                        if (terrain.terrainMaterial != null)
                        {
                            CopyMaterialProperties(terrain.terrainMaterial, material);
                        }

                        if (terrain.terrainPropertyBlock != null)
                        {
                            CopyMaterialPropertiesFromBlock(terrain.terrainPropertyBlock, material);
                        }
                    }
                    else
                    {
                        var oldMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;

                        if (oldMaterials != null)
                        {
                            for (int i = 0; i < oldMaterials.Length; i++)
                            {
                                var oldMaterial = oldMaterials[i];

                                if (oldMaterial != null)
                                {
                                    CopyMaterialProperties(oldMaterial, material);

                                    if (oldMaterial.HasProperty("_IsPlantShader"))
                                    {
                                        var newShaderName = material.shader.name;
                                        newShaderName = newShaderName.Replace("Vertex", "XXX");
                                        newShaderName = newShaderName.Replace("Simple", "XXX");
                                        newShaderName = newShaderName.Replace("Standard", "XXX");
                                        newShaderName = newShaderName.Replace("Subsurface", "XXX");

                                        if (oldMaterial.shader.name.Contains("Vertex"))
                                        {
                                            newShaderName = newShaderName.Replace("XXX", "Vertex");
                                        }

                                        if (oldMaterial.shader.name.Contains("Simple"))
                                        {
                                            newShaderName = newShaderName.Replace("XXX", "Simple");
                                        }

                                        if (oldMaterial.shader.name.Contains("Standard"))
                                        {
                                            newShaderName = newShaderName.Replace("XXX", "Standard");
                                        }

                                        if (oldMaterial.shader.name.Contains("Subsurface"))
                                        {
                                            newShaderName = newShaderName.Replace("XXX", "Subsurface");
                                        }

                                        if (Shader.Find(newShaderName) != null)
                                        {
                                            material.shader = Shader.Find(newShaderName);
                                        }

                                        if (!oldMaterial.HasProperty("_SubsurfaceValue"))
                                        {
                                            material.SetFloat("_SubsurfaceValue", 0);
                                        }
                                    }

                                    material.SetFloat("_IsInitialized", 1);

                                }
                            }
                        }
                    }
                }

                if (inputObject.GetType() == typeof(Material))
                {
                    var oldMaterial = (Material)inputObject;

                    if (oldMaterial != null)
                    {
                        CopyMaterialProperties(oldMaterial, material);

                        if (oldMaterial.HasProperty("_IsPlantShader"))
                        {
                            var newShaderName = material.shader.name;
                            newShaderName = newShaderName.Replace("Vertex", "XXX");
                            newShaderName = newShaderName.Replace("Simple", "XXX");
                            newShaderName = newShaderName.Replace("Standard", "XXX");
                            newShaderName = newShaderName.Replace("Subsurface", "XXX");

                            if (oldMaterial.shader.name.Contains("Vertex"))
                            {
                                newShaderName = newShaderName.Replace("XXX", "Vertex");
                            }

                            if (oldMaterial.shader.name.Contains("Simple"))
                            {
                                newShaderName = newShaderName.Replace("XXX", "Simple");
                            }

                            if (oldMaterial.shader.name.Contains("Standard"))
                            {
                                newShaderName = newShaderName.Replace("XXX", "Standard");
                            }

                            if (oldMaterial.shader.name.Contains("Subsurface"))
                            {
                                newShaderName = newShaderName.Replace("XXX", "Subsurface");
                            }

                            if (Shader.Find(newShaderName) != null)
                            {
                                material.shader = Shader.Find(newShaderName);
                            }

                            if (!oldMaterial.HasProperty("_SubsurfaceValue"))
                            {
                                material.SetFloat("_SubsurfaceValue", 0);
                            }
                        }

                        material.SetFloat("_IsInitialized", 1);
                    }
                }

                inputObject = null;
            }
        }

        public static void DrawRenderQueue(Material material, MaterialEditor materialEditor)
        {
            if (material.HasProperty("_RenderQueue") && material.HasProperty("_RenderPriority"))
            {
                GUILayout.Space(10);

                var queue = material.GetInt("_RenderQueue");
                var priority = material.GetInt("_RenderPriority");

                queue = EditorGUILayout.Popup("Render Queue Mode", queue, new string[] { "Auto", "Priority", "User Defined" });

                if (queue == 0)
                {
                    priority = 0;
                }
                else if (queue == 1)
                {
                    priority = EditorGUILayout.IntSlider("Render Priority", priority, -100, 100);
                }
                else
                {
                    priority = 0;
                    materialEditor.RenderQueueField();
                }

                material.SetInt("_RenderQueue", queue);
                material.SetInt("_RenderPriority", priority);
            }
        }

        public static void DrawPoweredByTheVegetationEngine()
        {
            var styleLabelCentered = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
            };

            Rect lastRect0 = GUILayoutUtility.GetLastRect();
            EditorGUI.DrawRect(new Rect(0, lastRect0.yMax, 1000, 1), new Color(0, 0, 0, 0.4f));

            GUILayout.Space(10);

            GUILayout.Label("<size=10><color=#808080>Powered by The Vegetation Engine</color></size>", styleLabelCentered);

            Rect labelRect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(labelRect, "", new GUIStyle()))
            {
                Application.OpenURL("http://u3d.as/1H9u");
            }

            GUILayout.Space(5);
        }

        // Property Utils
        public static bool GetPropertyVisibility(Material material, string internalName)
        {
            bool valid = true;
            var shaderName = material.shader.name;

            if (internalName == "unity_Lightmaps")
                valid = false;

            if (internalName == "unity_LightmapsInd")
                valid = false;

            if (internalName == "unity_ShadowMasks")
                valid = false;

            if (internalName.Contains("_Banner"))
                valid = false;

            if (internalName == "_SpecColor")
                valid = false;

            if (material.HasProperty("_RenderMode"))
            {
                if (material.GetInt("_RenderMode") == 0 && internalName == "_RenderZWrite")
                    valid = false;
            }

            bool hasRenderNormals = false;

            if (material.HasProperty("_render_normals"))
            {
                hasRenderNormals = true;
            }

            if (!hasRenderNormals)
            {
                if (internalName == "_RenderNormals")
                    valid = false;
            }

            if (!shaderName.Contains("Vertex Lit"))
            {
                if (internalName == "_RenderDirect")
                    valid = false;
                if (internalName == "_RenderShadow")
                    valid = false;
                if (internalName == "_RenderAmbient")
                    valid = false;
            }

            if (material.HasProperty("_RenderCull"))
            {
                if (material.GetInt("_RenderCull") == 2 && internalName == "_RenderNormals")
                    valid = false;
            }

            if (material.HasProperty("_RenderClip"))
            {
                if (material.GetInt("_RenderClip") == 0)
                {
                    if (internalName == "_RenderCoverage")
                        valid = false;
                    if (internalName == "_AlphaClipValue")
                        valid = false;
                    if (internalName == "_AlphaFeatherValue")
                        valid = false;
                    if (internalName == "_FadeMode")
                        valid = false;
                    if (internalName == "_FadeGlobalValue")
                        valid = false;
                    if (internalName == "_FadeConstantValue")
                        valid = false;
                    if (internalName == "_FadeCameraValue")
                        valid = false;
                    if (internalName == "_FadeGlancingValue")
                        valid = false;
                    if (internalName == "_FadeHorizontalValue")
                        valid = false;
                    if (internalName == "_FadeVerticalValue")
                        valid = false;
                    if (internalName == "_SpaceRenderFade")
                        valid = false;
                    if (internalName == "_MainAlphaValue")
                        valid = false;
                    if (internalName == "_SecondAlphaValue")
                        valid = false;
                    if (internalName == "_DetailAlphaMode")
                        valid = false;
                    if (internalName == "_DetailFadeMode")
                        valid = false;
                    if (internalName == "_EmissiveAlphaValue")
                        valid = false;
                }
            }

            if (!material.HasProperty("_AlphaFeatherValue"))
            {
                if (internalName == "_RenderCoverage")
                    valid = false;
            }

            if (material.GetTag("RenderPipeline", false) != "HDRenderPipeline")
            {
                if (internalName == "_RenderDecals")
                    valid = false;
                if (internalName == "_RenderSSR")
                    valid = false;
            }

            if (material.HasProperty("_RenderPacked"))
            {
                if (material.GetInt("_RenderPacked") == 1 && internalName == "_MainOcclusionValue")
                    valid = false;
                if (material.GetInt("_RenderPacked") == 1 && internalName == "_SecondOcclusionValue")
                    valid = false;
                if (material.GetInt("_RenderPacked") == 1 && internalName == "_TerrainOcclusionValue")
                    valid = false;
            }

            bool showFadeSpace = false;

            if (material.HasProperty("_FadeGlobalValue") || material.HasProperty("_FadeConstantValue") || material.HasProperty("_FadeCameraValue") || material.HasProperty("_FadeGlancingValue") || material.HasProperty("_FadeHorizontalValue"))
            {
                showFadeSpace = true;
            }

            if (!showFadeSpace)
            {
                if (internalName == "_SpaceRenderFade")
                    valid = false;
            }

            bool showGlobalsCat = false;

            if (material.HasProperty("_GlobalColors") || material.HasProperty("_GlobalOverlay") || material.HasProperty("_GlobalWetness") || material.HasProperty("_GlobalEmissive") || material.HasProperty("_GlobalSize") || material.HasProperty("_GlobalConform") || material.HasProperty("_GlobalHeight"))
            {
                showGlobalsCat = true;
            }

            if (!showGlobalsCat)
            {
                if (internalName == "_CategoryGlobals")
                    valid = false;
            }

            bool showGlobalLayers = false;

            if (material.HasProperty("_LayerColorsValue") || material.HasProperty("_LayerExtrasValue") || material.HasProperty("_LayerMotionValue") || material.HasProperty("_LayerReactValue"))
            {
                showGlobalLayers = true;
            }

            if (!showGlobalLayers)
            {
                if (internalName == "_SpaceGlobalLayers")
                    valid = false;
            }

            bool showGlobalLocals = false;

            if (material.HasProperty("_ColorsIntensityValue") || material.HasProperty("_ColorsVariationValue") || material.HasProperty("_AlphaVariationValue") || material.HasProperty("_OverlayVariationValue") || material.HasProperty("_OverlayProjectionValue") || material.HasProperty("_ConformOffsetValue"))
            {
                showGlobalLocals = true;
            }

            if (!showGlobalLocals)
            {
                if (internalName == "_SpaceGlobalLocals")
                    valid = false;
            }

            bool showGlobalOptions = false;

            if (material.HasProperty("_ColorsPositionMode") || material.HasProperty("_ExtrasPositionMode"))
            {
                showGlobalOptions = true;
            }

            if (!showGlobalOptions)
            {
                if (internalName == "_SpaceGlobalOptions")
                    valid = false;
            }

            bool showMainMaskMessage = false;

            if (material.HasProperty("_MainMaskMinValue"))
            {
                showMainMaskMessage = true;
            }

            if (!showMainMaskMessage)
            {
                if (internalName == "_MessageMainMask")
                    valid = false;
            }

            if (material.HasProperty("_MainColorMode"))
            {
                if (material.GetInt("_MainColorMode") == 0)
                {
                    if (internalName == "_MainColorTwo")
                        valid = false;
                }
            }

            if (!material.HasProperty("_SecondColor"))
            {
                if (internalName == "_CategoryDetail")
                    valid = false;
                if (internalName == "_SecondUVsMode")
                    valid = false;
            }

            if (material.HasProperty("_SecondColorMode"))
            {
                if (material.GetInt("_SecondColorMode") == 0)
                {
                    if (internalName == "_SecondColorTwo")
                        valid = false;
                }
            }

            bool showTerrainSettings = false;

            if (material.HasProperty("_ThirdColor") || material.HasProperty("_TerrainColor"))
            {
                showTerrainSettings = true;
            }

            if (!showTerrainSettings)
            {
                if (internalName == "_CategoryTerrain")
                    valid = false;
                if (internalName == "_MessageTerrain")
                    valid = false;
                if (internalName == "_ThirdUVsMode")
                    valid = false;
            }

            bool showSecondMaskMessage = false;

            if (material.HasProperty("_SecondMaskMinValue"))
            {
                showSecondMaskMessage = true;
            }

            if (!showSecondMaskMessage)
            {
                if (internalName == "_MessageSecondMask")
                    valid = false;
            }

            if (!material.HasProperty("_VertexOcclusionColor"))
            {
                if (internalName == "_CategoryOcclusion")
                    valid = false;
                if (internalName == "_EndOcclusion")
                    valid = false;
                if (internalName == "_MessageOcclusion")
                    valid = false;
            }

            if (!material.HasProperty("_GradientColorOne"))
            {
                if (internalName == "_CategoryGradient")
                    valid = false;
            }

            if (!material.HasProperty("_NoiseColorOne"))
            {
                if (internalName == "_CategoryNoise")
                    valid = false;
            }

            if (!material.HasProperty("_MatcapValue"))
            {
                if (internalName == "_CategoryMatcap")
                    valid = false;
            }

            if (!material.HasProperty("_RimLightColor"))
            {
                if (internalName == "_CategoryRimLight")
                    valid = false;
            }

            if (material.HasProperty("_SubsurfaceValue"))
            {
                if (material.GetTag("RenderPipeline", false) != "HDRenderPipeline" || shaderName.Contains("Standard"))
                {
                    if (internalName == "_SubsurfaceDiffusion")
                        valid = false;
                    if (internalName == "_SpaceSubsurface")
                        valid = false;
                    if (internalName == "_MessageSubsurface")
                        valid = false;
                }

                // Standard Render Pipeline
                if (internalName == "_Translucency")
                    valid = false;
                if (internalName == "_TransNormalDistortion")
                    valid = false;
                if (internalName == "_TransScattering")
                    valid = false;
                if (internalName == "_TransDirect")
                    valid = false;
                if (internalName == "_TransAmbient")
                    valid = false;
                if (internalName == "_TransShadow")
                    valid = false;

                // Universal Render Pipeline
                if (internalName == "_TransStrength")
                    valid = false;
                if (internalName == "_TransNormal")
                    valid = false;

                if (material.GetTag("RenderPipeline", false) == "HDRenderPipeline" || shaderName.Contains("Standard Lit") || shaderName.Contains("Simple Lit") || shaderName.Contains("Vertex Lit"))
                {
                    if (internalName == "_SubsurfaceNormalValue")
                        valid = false;
                    if (internalName == "_SubsurfaceDirectValue")
                        valid = false;
                    if (internalName == "_SubsurfaceAmbientValue")
                        valid = false;
                    if (internalName == "_SubsurfaceShadowValue")
                        valid = false;
                }
            }
            else
            {
                if (internalName == "_CategorySubsurface")
                    valid = false;
                if (internalName == "_SubsurfaceDiffusion")
                    valid = false;
                if (internalName == "_SpaceSubsurface")
                    valid = false;
                if (internalName == "_MessageSubsurface")
                    valid = false;

                if (internalName == "_SubsurfaceScatteringValue")
                    valid = false;
                if (internalName == "_SubsurfaceAngleValue")
                    valid = false;
                if (internalName == "_SubsurfaceNormalValue")
                    valid = false;
                if (internalName == "_SubsurfaceDirectValue")
                    valid = false;
                if (internalName == "_SubsurfaceAmbientValue")
                    valid = false;
                if (internalName == "_SubsurfaceShadowValue")
                    valid = false;
            }

            if (!material.HasProperty("_emissive_intensity_value"))
            {
                if (internalName == "_CategoryEmissive")
                    valid = false;
                if (internalName == "_EmissiveMode")
                    valid = false;
                if (internalName == "_EmissiveFlagMode")
                    valid = false;
                if (internalName == "_EmissiveIntensityMode")
                    valid = false;
                if (internalName == "_EmissiveIntensityValue")
                    valid = false;
            }

            if (material.HasProperty("_EmissiveMode"))
            {
                if (material.GetInt("_EmissiveMode") == 0)
                {
                    if (internalName == "_GlobalEmissive")
                        valid = false;
                }
            }

            if (!material.HasProperty("_PerspectivePushValue"))
            {
                if (internalName == "_CategoryPerspective")
                    valid = false;
                if (internalName == "_EndPerspective")
                    valid = false;
            }

            if (!material.HasProperty("_SizeFadeStartValue"))
            {
                if (internalName == "_CategorySizeFade")
                    valid = false;
                if (internalName == "_EndSizeFade")
                    valid = false;
            }

            bool hasMotion = false;

            if (material.HasProperty("_MotionHighlightColor") || material.HasProperty("_MotionAmplitude_10") || material.HasProperty("_MotionAmplitude_20") || material.HasProperty("_MotionAmplitude_30"))
            {
                hasMotion = true;
            }

            if (!hasMotion)
            {
                if (internalName == "_CategoryMotion")
                    valid = false;
            }

            bool hasMotionGlobals = false;

            if (material.HasProperty("_MotionHighlightColor") || material.HasProperty("_MotionFacingValue"))
            {
                hasMotionGlobals = true;
            }

            if (!hasMotionGlobals)
            {
                if (internalName == "_SpaceMotionGlobals")
                    valid = false;
            }

            //bool hasMotionLocals = false;

            //if (material.HasProperty("_MotionValue_20") || material.HasProperty("_MotionValue_30") || material.HasProperty("_MotionNormal_32") || material.HasProperty("_MainMaskMotionValue"))
            //{
            //    hasMotionLocals = true;
            //}

            //if (!hasMotionLocals)
            //{
            //    if (internalName == "_SpaceMotionLocals")
            //        valid = false;
            //}

            if (material.HasProperty("_VertexDataMode"))
            {
                if (material.GetInt("_VertexDataMode") == 1)
                {
                    if (internalName == "_ColorsPositionMode")
                        valid = false;
                    if (internalName == "_ExtrasPositionMode")
                        valid = false;
                    if (internalName == "_SpaceGlobalPosition")
                        valid = false;
                    if (internalName == "_NoisePositionMode")
                        valid = false;
                    if (internalName == "_GlobalSize")
                        valid = false;
                    if (internalName == "_CategorySizeFade")
                        valid = false;
                    if (internalName == "_SizeFadeStartValue")
                        valid = false;
                    if (internalName == "_SizeFadeEndValue")
                        valid = false;
                    if (internalName == "_SpaceMotionGlobals")
                        valid = false;
                    if (internalName == "_MotionFacingValue")
                        valid = false;
                    if (internalName == "_MotionPosition_10")
                        valid = false;
                    if (internalName == "_MotionAmplitude_22")
                        valid = false;
                }
            }

            if (material.HasProperty("_VertexVariationMode"))
            {
                var value = material.GetInt("_VertexVariationMode");

                if (value == 0 || !showGlobalsCat)
                {
                    if (internalName == "_MessageMotionVariation")
                        valid = false;
                }

                if (value == 0 || !hasMotion)
                {
                    if (internalName == "_MessageMotionVariation")
                        valid = false;
                }
            }

            return valid;
        }

        public static string GetPropertyDisplay(Material material, MaterialProperty property)
        {
            var displayName = property.displayName;
            var internalName = property.name;
            var shaderName = material.shader.name;

            if (internalName == "_AI_Parallax")
            {
                GUILayout.Space(10);
            }

            if (internalName == "_SecondAlbedoTex")
            {
                GUILayout.Space(10);
            }

            if (internalName == "_ThirdAlbedoTex")
            {
                GUILayout.Space(10);
            }

            if (internalName == "_EmissiveTex")
            {
                GUILayout.Space(10);
            }

            if (internalName == "_AI_Clip")
            {
                displayName = "Impostor Alpha Treshold";
            }

            if (internalName == "_MainColor")
            {
                if (material.HasProperty("_MainColorMode"))
                {
                    if (material.GetInt("_MainColorMode") == 1)
                    {
                        displayName = displayName + "A";
                    }
                }
            }

            if (internalName == "_SecondColor")
            {
                if (material.HasProperty("_SecondColorMode"))
                {
                    if (material.GetInt("_SecondColorMode") == 1)
                    {
                        displayName = displayName + "A";
                    }
                }
            }

            if (EditorGUIUtility.currentViewWidth > 550)
            {
                if (internalName == "_MainMetallicValue")
                {
                    if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
                    {
                        displayName = displayName + " (Mask Red)";
                    }
                }

                if (internalName == "_MainOcclusionValue")
                {
                    displayName = displayName + " (Mask Green)";
                }

                if (internalName == "_MainSmoothnessValue")
                {
                    if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
                    {
                        displayName = displayName + " (Mask Alpha)";
                    }
                }

                if (internalName == "_MainMaskRemap")
                {
                    displayName = displayName + " (Mask Blue)";
                }

                if (internalName == "_SecondMetallicValue")
                {
                    if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
                    {
                        displayName = displayName + " (Mask Red)";
                    }
                }

                if (internalName == "_SecondOcclusionValue")
                {
                    displayName = displayName + " (Mask Green)";
                }

                if (internalName == "_SecondSmoothnessValue")
                {
                    if (shaderName.Contains("Standard Lit") || shaderName.Contains("Subsurface Lit"))
                    {
                        displayName = displayName + " (Mask Alpha)";
                    }
                }

                if (internalName == "_SecondMaskRemap")
                {
                    displayName = displayName + " (Mask Blue)";
                }

                if (internalName == "_DetailMeshMode" || internalName == "_DetailMeshRemap")
                {
                    if (material.HasProperty("_DetailMeshMode"))
                    {
                        if (material.GetInt("_DetailMeshMode") == 0)
                        {
                            displayName = displayName + " (Vertex Blue)";
                        }
                        else if (material.GetInt("_DetailMeshMode") == 1)
                        {
                            displayName = displayName + " (World Normals)";
                        }
                    }
                }

                if (internalName == "_DetailMaskMode" || internalName == "_DetailMaskRemap")
                {
                    displayName = displayName + " (Mask Blue)";
                }

                if (internalName == "_VertexOcclusionRemap")
                {
                    displayName = displayName + " (Vertex Green)";
                }

                if (internalName == "_GradientMaskRemap")
                {
                    displayName = displayName + " (Vertex Alpha)";
                }
            }

            return displayName;
        }

        // Asset Utils
        public static string[] FindAssets(string filter, bool sort)
        {
            var assetPaths = AssetDatabase.FindAssets("glob:\"" + filter + "\"");

            if (sort)
            {
                assetPaths = assetPaths.OrderBy(f => new FileInfo(f).Name).ToArray();
            }

            for (int i = 0; i < assetPaths.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            }

            return assetPaths;
        }

        public static string FindAsset(string filter)
        {
            var assetPath = "";

            var assetGUIDs = AssetDatabase.FindAssets("glob:\"" + filter + "\"");

            if (assetGUIDs != null && assetGUIDs.Length > 0)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
            }

            return assetPath;
        }

        public static string GetTVEFolder()
        {
            var folder = TVEUtils.FindAsset("The Vegetation Engine.pdf");
            folder = folder.Replace("/The Vegetation Engine.pdf", "");

            return folder;
        }

        public static string GetUserFolder()
        {
            var folder = TVEUtils.FindAsset("User.pdf");
            folder = folder.Replace("/User.pdf", "");
            folder += "/The Vegetation Engine";

            return folder;
        }

        public static TVEPathData GetPathData(string assetPath)
        {
            var pathData = new TVEPathData();

            pathData.folder = Path.GetDirectoryName(assetPath);
            pathData.extention = Path.GetExtension(assetPath);

            assetPath = Path.GetFileNameWithoutExtension(assetPath);
            assetPath = assetPath.Replace("(", "");
            assetPath = assetPath.Replace(")", "");

            var splitLine = assetPath.Split(char.Parse(" "));
            var splitCount = splitLine.Length;

            pathData.type = splitLine[splitCount - 1];
            pathData.suffix = splitLine[splitCount - 2];

            assetPath = assetPath.Replace(pathData.type, "");
            assetPath = assetPath.Replace(pathData.suffix, "");

            // Old Assets might not have an ID
            if (splitCount > 3)
            {
                pathData.GUID = splitLine[splitCount - 3];
                assetPath = assetPath.Replace(pathData.GUID, "");
            }

            assetPath = assetPath.TrimEnd();

            pathData.name = assetPath;

            return pathData;
        }

        public static bool HasLabel(string path, string check)
        {
            bool valid = false;

            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            var labelsArr = AssetDatabase.GetLabels(asset);
            var labeldList = new List<string>();

            labeldList.AddRange(labelsArr);

            if (labeldList.Contains(check))
            {
                valid = true;
            }

            labeldList.Clear();

            return valid;
        }
#endif
    }
}
