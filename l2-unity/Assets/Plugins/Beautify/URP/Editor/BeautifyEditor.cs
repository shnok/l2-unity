using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Beautify.Universal {
#if UNITY_2022_2_OR_NEWER
    [CustomEditor(typeof(Beautify))]
#else
    [VolumeComponentEditor(typeof(Beautify))]
#endif
    public class BeautifyEditor : VolumeComponentEditor {

        Beautify beautify;
        GUIStyle sectionGroupStyle, foldoutStyle, blackBack;
        PropertyFetcher<Beautify> propertyFetcher;
        Texture2D headerTex;
        bool pixelateExpanded;

        // settings group <setting, property reference>
        class SectionContents {
            public Dictionary<Beautify.SettingsGroup, List<MemberInfo>> groups = new Dictionary<Beautify.SettingsGroup, List<MemberInfo>>();
            public List<MemberInfo> singleFields = new List<MemberInfo>();
        }

        readonly Dictionary<Beautify.SectionGroup, SectionContents> sections = new Dictionary<Beautify.SectionGroup, SectionContents>();
        readonly Dictionary<Beautify.SettingsGroup, List<MemberInfo>> groupedFields = new Dictionary<Beautify.SettingsGroup, List<MemberInfo>>();
        readonly Dictionary<MemberInfo, SerializedDataParameter> unpackedFields = new Dictionary<MemberInfo, SerializedDataParameter>();
#if !UNITY_2021_2_OR_NEWER
        public override bool hasAdvancedMode => false;
#endif

        public override void OnEnable() {
            base.OnEnable();

            headerTex = Resources.Load<Texture2D>("beautifyHeader");
            blackBack = new GUIStyle();
            blackBack.normal.background = MakeTex(4, 4, Color.black);
            blackBack.alignment = TextAnchor.MiddleCenter;

            beautify = (Beautify)target;

            propertyFetcher = new PropertyFetcher<Beautify>(serializedObject);

            // get volume fx settings
            var settings = beautify.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(t => t.FieldType.IsSubclassOf(typeof(VolumeParameter)))
                            .Where(t => (t.IsPublic && t.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length == 0) ||
                                        (t.GetCustomAttributes(typeof(SerializeField), false).Length > 0))
                            .Where(t => t.GetCustomAttributes(typeof(HideInInspector), false).Length == 0)
                            .Where(t => t.GetCustomAttributes(typeof(Beautify.SectionGroup), false).Any());

            // group by settings first
            unpackedFields.Clear();
            sections.Clear();
            groupedFields.Clear();
            foreach (var setting in settings) {
                SectionContents sectionContents = null;

                foreach (var section in setting.GetCustomAttributes(typeof(Beautify.SectionGroup)) as IEnumerable<Beautify.SectionGroup>) {
                    if (!sections.TryGetValue(section, out sectionContents)) {
                        sectionContents = sections[section] = new SectionContents();
                    }

                    bool isGrouped = false;
                    foreach (var settingGroup in setting.GetCustomAttributes(typeof(Beautify.SettingsGroup)) as IEnumerable<Beautify.SettingsGroup>) {
                        if (!groupedFields.ContainsKey(settingGroup)) {
                            sectionContents.groups[settingGroup] = groupedFields[settingGroup] = new List<MemberInfo>();
                        }
                        groupedFields[settingGroup].Add(setting);
                        isGrouped = true;
                        unpackedFields[setting] = Unpack(propertyFetcher.Find(setting.Name));
                    }

                    if (!isGrouped) {
                        sectionContents.singleFields.Add(setting);
                        unpackedFields[setting] = Unpack(propertyFetcher.Find(setting.Name));
                    }
                }
            }
        }


        public override void OnInspectorGUI() {

            BeautifySettings.ManageBuildOptimizationStatus(false);

            serializedObject.Update();

            SetStyles();

            Beautify.TonemapOperator prevTonemap = beautify.tonemap.value;
            bool prevDirectWrite = beautify.directWrite.value;
            int prevBloomExclusionLayerMask = beautify.bloomExclusionLayerMask.overrideState ? (int)beautify.bloomExclusionLayerMask.value : 0;
            int prevAnamorphicFlaresExclusionLayerMask = beautify.anamorphicFlaresExclusionLayerMask.overrideState ? (int)beautify.anamorphicFlaresExclusionLayerMask.value : 0;

            EditorGUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal(blackBack);
                GUILayout.Label(headerTex, blackBack, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Clear Effects", "Clears all effect overrides"), EditorStyles.miniButton)) {
                    if (EditorUtility.DisplayDialog("Clear Effects", "Do you want to clear all effects?", "Yes", "Cancel")) {
                        beautify.sharpenIntensity.overrideState = false;
                        beautify.antialiasStrength.overrideState = false;
                        beautify.ditherIntensity.overrideState = false;
                        beautify.tonemap.overrideState = false;
                        beautify.saturate.overrideState = false;
                        beautify.brightness.overrideState = false;
                        beautify.contrast.overrideState = false;
                        beautify.daltonize.overrideState = false;
                        beautify.sepia.overrideState = false;
                        beautify.tintColor.overrideState = false;
                        beautify.colorTemp.overrideState = false;
                        beautify.colorTempBlend.overrideState = false;
                        beautify.lut.overrideState = false;
                        beautify.bloomIntensity.overrideState = false;
                        beautify.anamorphicFlaresIntensity.overrideState = false;
                        beautify.sunFlaresIntensity.overrideState = false;
                        beautify.lensDirtIntensity.overrideState = false;
                        beautify.chromaticAberrationIntensity.overrideState = false;
                        beautify.depthOfField.overrideState = false;
                        beautify.eyeAdaptation.overrideState = false;
                        beautify.purkinje.overrideState = false;
                        beautify.vignettingOuterRing.overrideState = false;
                        beautify.vignettingInnerRing.overrideState = false;
                        beautify.vignettingFade.overrideState = false;
                        beautify.vignettingBlink.overrideState = false;
                        beautify.outline.overrideState = false;
                        beautify.nightVision.overrideState = false;
                        beautify.thermalVision.overrideState = false;
                        beautify.frame.overrideState = false;
                        beautify.blurIntensity.overrideState = false;
                        EditorUtility.SetDirty(beautify);
                    }
                }
                if (GUILayout.Button(new GUIContent("Quick Settings", "Applies a default set of effects including color improvement, sharpening, vignette and bloom."), EditorStyles.miniButton)) {
                    if (EditorUtility.DisplayDialog("Quick Settings", "Do you want to apply a collection of example effect settings (including color improvements, dithering, sharpening, vignette and bloom)?\nYou can adjust them later as you wish.", "Yes", "No")) {
                        beautify.sharpenIntensity.Override(4f);
                        beautify.ditherIntensity.Override(0.005f);
                        beautify.brightness.Override(1.05f);
                        beautify.contrast.Override(1.02f);
                        beautify.bloomIntensity.Override(0.25f);
                        beautify.bloomThreshold.Override(0.75f);
                        beautify.vignettingOuterRing.Override(0.325f);
                        beautify.vignettingInnerRing.Override(0.925f);
                        EditorUtility.SetDirty(beautify);
                    }
                }
                if (GUILayout.Button("Help & Tips")) {
                    Application.OpenURL("https://kronnect.com/guides/beautify-urp-performance-tips");
                }
                if (GUILayout.Button("Contact Us", EditorStyles.miniButton)) {
                    Application.OpenURL("https://kronnect.com");
                }
                EditorGUILayout.EndHorizontal();


                UniversalRenderPipelineAsset pipe = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

                Camera cam = Camera.main;
                if (cam != null) {
                    UniversalAdditionalCameraData data = cam.GetComponent<UniversalAdditionalCameraData>();
                    if (data != null && !data.renderPostProcessing && !BeautifyRendererFeature.ignoringPostProcessingOption) {
                        EditorGUILayout.HelpBox("Post Processing option is disabled in the camera. Either enable it or enable the option 'Ignore Post Processing Option' in the Beautify Render Feature.", MessageType.Warning);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Go to Camera")) {
                            Selection.activeObject = cam;
                        }
                        if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                            Selection.activeObject = pipe;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                    }
                }

                if (pipe == null) {
                    EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in 'Project Settings / Graphics' !", MessageType.Error);
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                } else if (!BeautifyRendererFeature.installed) {
                    EditorGUILayout.HelpBox("Beautify Render Feature must be added to the rendering pipeline renderer.", MessageType.Error);
                    if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                        Selection.activeObject = pipe;
                    }
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                } else if (beautify.RequiresDepthTexture()) {
#if !UNITY_2021_3_OR_NEWER
                    if (!pipe.supportsCameraDepthTexture) {
                        EditorGUILayout.HelpBox("Depth Texture option may be required for certain effects. Check Universal Rendering Pipeline asset!", MessageType.Warning);
                        if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                            Selection.activeObject = pipe;
                        }
                        EditorGUILayout.Separator();
                    }
#endif
                }

                bool usesHDREffect = beautify.tonemap.value != Beautify.TonemapOperator.Linear;
                if (usesHDREffect && (QualitySettings.activeColorSpace != ColorSpace.Linear || (Camera.main != null && !Camera.main.allowHDR))) {
                    EditorGUILayout.HelpBox("Some effects, like bloom or tonemapping, works better with Linear Color Space and HDR enabled. Enable Linear color space in Player Settings and check your camera and pipeline HDR setting.", MessageType.Warning);
                }

                if ((bool)beautify.directWrite) {
#if !UNITY_2022_3_OR_NEWER
                    if (UnityEngine.XR.XRSettings.enabled) {
                        EditorGUILayout.HelpBox("Direct Write To Camera option is not compatible with VR.", MessageType.Warning);
                    }
#endif
                }

                // sections
                foreach (var section in sections) {
                    bool printSectionHeader = true;

                    // individual properties
                    foreach (var field in section.Value.singleFields) {
                        if (!unpackedFields.TryGetValue(field, out var parameter)) continue;

                        if (printSectionHeader) {
                            GUILayout.Space(6.0f);
                            Rect rect = GUILayoutUtility.GetRect(16f, 22f, sectionGroupStyle);
                            GUI.Box(rect, ObjectNames.NicifyVariableName(section.Key.GetType().Name), sectionGroupStyle);
                            printSectionHeader = false;
                        }

                        bool indent;
                        if (!IsVisible(parameter, field, out indent)) {
                            if (field.GetCustomAttribute<Beautify.ShowStrippedLabel>() != null) {
                                EditorGUILayout.BeginHorizontal();
                                GUI.enabled = false;
                                DrawPropertyField(parameter, field, indent);
                                GUILayout.Label("(Not available - Check General Options)");
                                GUI.enabled = true;
                                EditorGUILayout.EndHorizontal();
                            }
                            continue;
                        }

                        var displayName = parameter.displayName;
                        if (field.GetCustomAttribute(typeof(Beautify.DisplayName)) is Beautify.DisplayName displayNameAttrib) {
                            displayName = displayNameAttrib.name;
                        }

                        DrawPropertyField(parameter, field, indent);

                        if (beautify.disabled.value) GUI.enabled = false;
                    }
                    GUILayout.Space(6.0f);

                    // grouped properties
                    foreach (var group in section.Value.groups) {
                        Beautify.SettingsGroup settingsGroup = group.Key;
                        string groupName = ObjectNames.NicifyVariableName(settingsGroup.GetType().Name);
                        bool printGroupFoldout = true;
                        bool firstField = true;
                        bool groupHasContent = false;

                        foreach (var field in group.Value) {
                            if (!unpackedFields.TryGetValue(field, out var parameter)) continue;

                            if (printSectionHeader) {
                                GUILayout.Space(6.0f);
                                Rect rect = GUILayoutUtility.GetRect(16f, 22f, sectionGroupStyle);
                                GUI.Box(rect, ObjectNames.NicifyVariableName(section.Key.GetType().Name), sectionGroupStyle);
                                printSectionHeader = false;
                            }

                            bool indent;
                            if (!IsVisible(parameter, field, out indent)) {
                                if (firstField) {
                                    if (field.GetCustomAttribute<Beautify.ShowStrippedLabel>() != null) {
                                        EditorGUILayout.BeginHorizontal();
                                        GUI.enabled = false;
                                        EditorGUILayout.Foldout(false, groupName, true, foldoutStyle);
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Label("(Not available - Check General Options)");
                                        GUI.enabled = true;
                                        EditorGUILayout.EndHorizontal();
                                        GUILayout.Space(6.0f);
                                    }
                                    break;
                                }
                                continue;
                            }

                            firstField = false;

                            if (printGroupFoldout) {
                                printGroupFoldout = false;
                                settingsGroup.IsExpanded = EditorGUILayout.Foldout(settingsGroup.IsExpanded, groupName, true, foldoutStyle);
                                if (!settingsGroup.IsExpanded)
                                    break;
                            }

                            DrawPropertyField(parameter, field, indent);
                            groupHasContent = true;

                            if (parameter.value.propertyType == SerializedPropertyType.Boolean) {
                                if (!parameter.value.boolValue) {
                                    var hasToggleSectionBegin = field.GetCustomAttributes(typeof(Beautify.ToggleAllFields)).Any();
                                    if (hasToggleSectionBegin) break;
                                }
                            } else if (field.Name.Equals("depthOfFieldFocusMode")) {
                                if (BeautifySettings.instance != null && BeautifySettings.instance.depthOfFieldTarget == null) {
                                    SerializedProperty prop = serializedObject.FindProperty(field.Name);
                                    if (prop != null) {
                                        var value = prop.FindPropertyRelative("m_Value");
                                        if (value != null && value.enumValueIndex == (int)Beautify.DoFFocusMode.FollowTarget) {
                                            EditorGUILayout.HelpBox("Assign target in the Beautify Settings component.", MessageType.Info);
                                        }
                                    }
                                }
                            }
                        }
                        if (groupHasContent) {
                            GUILayout.Space(6.0f);
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6.0f);
            pixelateExpanded = EditorGUILayout.Foldout(pixelateExpanded, "Pixelate", true, foldoutStyle);
            EditorGUILayout.EndHorizontal();
            if (pixelateExpanded) {
                EditorGUILayout.HelpBox("Use the Downsampling option in General Settings to apply a pixelate effect.", MessageType.Info);
            }


            if (serializedObject.ApplyModifiedProperties()) {
                BeautifySettings.ManageBuildOptimizationStatus(true);

                if (beautify.directWrite.value != prevDirectWrite || beautify.bloomExclusionLayerMask != prevBloomExclusionLayerMask || beautify.anamorphicFlaresExclusionLayerMask != prevAnamorphicFlaresExclusionLayerMask || beautify.outline.value) {
                    EditorApplication.delayCall += () =>
    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
            }

            if (prevTonemap != beautify.tonemap.value && beautify.tonemap.value != Beautify.TonemapOperator.Linear) {
                beautify.saturate.value = 0;
                beautify.saturate.overrideState = true;
                beautify.contrast.value = 1f;
                beautify.contrast.overrideState = true;
            }

        }

        bool IsVisible(SerializedDataParameter property, MemberInfo field, out bool indent) {
            bool visible = true;
            indent = false;

            Beautify.DisplayConditionEnum enumCondition = field.GetCustomAttribute(typeof(Beautify.DisplayConditionEnum)) as Beautify.DisplayConditionEnum;
            Beautify.DisplayConditionBool boolCondition = field.GetCustomAttribute(typeof(Beautify.DisplayConditionBool)) as Beautify.DisplayConditionBool;
            bool isEnumCondition = enumCondition != null;
            bool isBoolCondition = boolCondition != null;
            bool canIndent = isEnumCondition ^ isBoolCondition;

            if (isEnumCondition) {
                SerializedProperty condProp = propertyFetcher.Find(enumCondition.field);
                if (condProp != null) {
                    var value = condProp.FindPropertyRelative("m_Value");
                    if (value != null) {
                        visible = false;
                        if (enumCondition.isEqual) {
                            if (value.enumValueIndex == enumCondition.enumValueIndex) {
                                indent = canIndent;
                                return true;
                            }
                        } else if (value.enumValueIndex != enumCondition.enumValueIndex) {
                            indent = canIndent;
                            return true;
                        }
                    }
                }
            }
            /* OR */
            if (isBoolCondition) {
                SerializedProperty condProp = propertyFetcher.Find(boolCondition.field);
                if (condProp != null) {
                    var value = condProp.FindPropertyRelative("m_Value");
                    if (value != null) {
                        if (value.boolValue != boolCondition.value) {
                            return false;
                        }
                        indent = value.boolValue;
                    }
                }
                /* AND */
                SerializedProperty condProp2 = propertyFetcher.Find(boolCondition.field2);
                if (condProp2 != null) {
                    var value2 = condProp2.FindPropertyRelative("m_Value");
                    if (value2 != null) {
                        if (value2.boolValue != boolCondition.value2) {
                            return false;
                        }
                        indent = indent || value2.boolValue;
                    }
                }
                indent &= canIndent;
                visible = true;
            }

            return visible;
        }

        void DrawPropertyField(SerializedDataParameter property, MemberInfo field, bool indent) {

            if (indent) {
                EditorGUI.indentLevel++;
            }

            var displayName = property.displayName;
            if (field.GetCustomAttribute(typeof(Beautify.DisplayName)) is Beautify.DisplayName displayNameAttrib) {
                displayName = displayNameAttrib.name;
            }

            if (property.value.propertyType == SerializedPropertyType.Boolean) {

                if (field.GetCustomAttribute(typeof(Beautify.GlobalOverride)) != null) {

                    BoolParameter pr = property.GetObjectRef<BoolParameter>();
                    bool prev = pr.value;

                    using (new EditorGUILayout.HorizontalScope()) {
                        float w = 17f;
                        if (EditorGUI.indentLevel > 0) w += 8f;
                        var overrideRect = GUILayoutUtility.GetRect(w, 20f, GUILayout.ExpandWidth(false));
                        overrideRect.yMin += 4f;
                        if (EditorGUI.indentLevel > 0) {
                            overrideRect.xMin += 12f;
                        }
                        bool value = GUI.Toggle(overrideRect, prev, GUIContent.none);

                        string tooltip = null;
                        if (field.GetCustomAttribute(typeof(TooltipAttribute)) is TooltipAttribute tooltipAttribute) {
                            tooltip = tooltipAttribute.tooltip;
                        }

                        using (new EditorGUI.DisabledScope(!prev)) {
                            EditorGUILayout.LabelField(new GUIContent(displayName, tooltip));
                        }

                        if (value != prev) {
                            pr.value = value;
                            SerializedProperty prop = serializedObject.FindProperty(field.Name);
                            if (prop != null) {
                                var boolProp = prop.FindPropertyRelative("m_Value");
                                if (boolProp != null) {
                                    boolProp.boolValue = value;
                                }
                                if (value) {
                                    var overrideProp = prop.FindPropertyRelative("m_OverrideState");
                                    if (overrideProp != null) {
                                        overrideProp.boolValue = true;
                                    }
                                }
                            }
                            if (field.GetCustomAttribute(typeof(Beautify.BuildToggle)) != null) {
                                BeautifySettings.SetStripShaderKeywords(beautify);
                            }
                        }
                    }

                } else {
                    PropertyField(property, new GUIContent(displayName));
                }
            } else {
                PropertyField(property, new GUIContent(displayName));
            }

            if (indent) {
                EditorGUI.indentLevel--;
            }

        }

        void SetStyles() {

            // section header style
            Color titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            GUIStyle skurikenModuleTitleStyle = "ShurikenModuleTitle";
            sectionGroupStyle = new GUIStyle(skurikenModuleTitleStyle);
            sectionGroupStyle.contentOffset = new Vector2(5f, -2f);
            sectionGroupStyle.normal.textColor = titleColor;
            sectionGroupStyle.fixedHeight = 22;
            sectionGroupStyle.fontStyle = FontStyle.Bold;

            // foldout style
            Color foldoutColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.margin = new RectOffset(6, 0, 0, 0);

        }

        [VolumeParameterDrawer(typeof(Beautify.MinMaxFloatParameter))]
        public class MaxFloatParameterDrawer : VolumeParameterDrawer {
            public override bool OnGUI(SerializedDataParameter parameter, GUIContent title) {
                if (parameter.value.propertyType == SerializedPropertyType.Vector2) {
                    var o = parameter.GetObjectRef<Beautify.MinMaxFloatParameter>();
                    var range = o.value;
                    float x = range.x;
                    float y = range.y;

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.MinMaxSlider(title, ref x, ref y, o.min, o.max);
                    x = EditorGUILayout.FloatField(x, GUILayout.Width(40));
                    y = EditorGUILayout.FloatField(y, GUILayout.Width(40));
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck()) {
                        range.x = x;
                        range.y = y;
                        o.SetValue(new Beautify.MinMaxFloatParameter(range, o.min, o.max));
                    }
                    return true;
                } else {
                    EditorGUILayout.PropertyField(parameter.value);
                    return false;
                }
            }
        }

        Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            TextureFormat tf = SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat) ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
            Texture2D result = new Texture2D(width, height, tf, false);
            result.hideFlags = HideFlags.DontSave;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

    }
}
