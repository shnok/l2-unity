using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System;

using UnityEditor;

namespace AdvancedTerrainGrass {

	[CustomEditor (typeof(GrassManager))]
	public class GrassManagerEditor : Editor {


		public int editorSelection = 0;
        public bool SavedTerrainDataIsDirty = false;
        public bool useBurstState = false;
        private static Camera sceneCam;
	    private	static Camera MainCam;

		private SerializedObject GrassManager;
		private GrassManager script;

		private SerializedProperty DrawInEditMode;
		private SerializedProperty FollowCam;
		private SerializedProperty MainCamPos;
		private SerializedProperty MainCamRot;
        private SerializedProperty rtCameraSelection;
        private SerializedProperty rtIngnoreOcclusion;
        private SerializedProperty rtBurstRadius;

        private SerializedProperty vrForceSinglePassInstanced;

        private SerializedProperty usingSRP;
        private SerializedProperty lppv;

        private SerializedProperty FreezeSizeAndColor;
		private SerializedProperty DebugStats;
		private SerializedProperty DebugCells;
		private SerializedProperty FirstTimeSynced;
		private SerializedProperty LayerEditMode;
		private SerializedProperty LayerSelection;
		private SerializedProperty Foldout_Rendersettings;
		private SerializedProperty Foldout_Advancedsettings;
		private SerializedProperty Foldout_Prototypes;

		private SerializedProperty IngnoreOcclusion;
		private SerializedProperty CameraSelection;
		private SerializedProperty CameraLayer;
		private SerializedProperty CullDistance;
		private SerializedProperty FadeLength;
		private SerializedProperty CacheDistance;
		private SerializedProperty DetailFadeStart;
		private SerializedProperty DetailFadeLength;
		private SerializedProperty VertexFadeStart;
		private SerializedProperty VertexFadeLength;
		private SerializedProperty ShadowStart;
		private SerializedProperty ShadowFadeLength;
		private SerializedProperty ShadowStartFoliage;
		private SerializedProperty ShadowFadeLengthFoliage;
        private SerializedProperty CullingGroupDistanceFactor;

        private SerializedProperty useBurst;
		private SerializedProperty BurstRadius;
		private SerializedProperty CellsPerFrame;
		private SerializedProperty DetailDensity;

		private SerializedProperty SavedTerrainData;
		private SerializedProperty NumberOfBucketsPerCellEnum;

		private SerializedProperty v_mesh;
		private SerializedProperty v_mat;
		private SerializedProperty InstanceRotation;
		private SerializedProperty WriteNormalBuffer;
		private SerializedProperty ShadowCastingMode;
		private SerializedProperty motionVectorGenerationMode;
private SerializedProperty renderingLayerMask;
		private SerializedProperty MinSize;
		private SerializedProperty MaxSize;
		private SerializedProperty Noise;
		private SerializedProperty LayerToMergeWith;
		private SerializedProperty DoSoftMerge;

		private SerializedProperty rtUseCompute;
		private SerializedProperty GrassInstanceSize;

        private SerializedProperty EnableTerrainShift;

        private MaterialEditor _materialEditor;
 		private static string baseURL = "https://docs.google.com/document/d/1JrSQVQaPkYLkbF6XJGpzcXnLcLc1G9FGzUhdW1xWoyA/view?pref=2&pli=1#heading=";


 		private bool HDRP = false;
 		private bool URP = false;


 	//	GUI
 		private GUIStyle aBox;
		private GUIStyle aBox01;
		private Texture2D primaryTex;
		private Texture2D secondaryTex;
 		GUIStyle myMiniBtn;
 		GUIStyle mainFoldoutStyle;
 		GUIStyle myMiniHelpBtn;

		public override void OnInspectorGUI () {
			GrassManager = new SerializedObject(target);
			GetProperties();
			script = (GrassManager)target;

		//	Ckeck SRP
			HDRP = false;
			URP = false;
			if (GraphicsSettings.currentRenderPipeline)
			{
			   if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition")) {
			      HDRP = true;
			      URP = false;
			   }
			   else {
			      HDRP = false;
			      URP = true;
			   }
			}


//	Styles -------------------

			Color myCol = new Color (1.0f,0.8f,0.0f,1.0f);
			Color myCol01 = new Color(0.30f,0.47f,1.0f,1.0f); // matches highlight blue //new Color(1.0f,0.3f,0.0f,1.0f); // Orange
			if (!EditorGUIUtility.isProSkin) {
				myCol = new Color(0.18f,0.35f,0.85f,1.0f);
				myCol01 = Color.blue;
			}

			var primaryCol = new Color (1.0f,0.8f,0.0f,1.0f);
			var secondaryCol = new Color(0.30f,0.47f,1.0f,1.0f); // matches highlight blue //new Color(1.0f,0.3f,0.0f,1.0f); // Orange
			if (!EditorGUIUtility.isProSkin) {
				primaryCol = new Color(0.0f,0.2f,0.9f,1.0f); // new Color(0.18f,0.35f,0.85f,1.0f);
#if UNITY_2019_3_OR_NEWER
				primaryCol = new Color(0.06f,0.275f,0.55f,1.0f);
#endif
				secondaryCol = Color.gray; //Color.blue;
			}

			GUIStyle mainLabelStyle = new GUIStyle(EditorStyles.label);
			mainLabelStyle.normal.textColor = primaryCol;

			GUIStyle primaryLabelStyle = new GUIStyle(EditorStyles.label);
			primaryLabelStyle.fontStyle = FontStyle.Bold;
			primaryLabelStyle.normal.textColor = primaryCol;

			if (primaryTex == null) {
				primaryTex = new Texture2D(1,1);
				if (!EditorGUIUtility.isProSkin) {
					primaryTex.SetPixel(0, 0, new Color(1,1,1,0.3f)); // 2019.3
				}
				else {
					primaryTex.SetPixel(0, 0, new Color(1,1,1,0.05f));
				}
				primaryTex.Apply();
			}

			if (secondaryTex == null) {
				secondaryTex = new Texture2D(1,1);
				if (!EditorGUIUtility.isProSkin) {
					secondaryTex.SetPixel(0, 0, new Color(0,0,0,0.075f));
					secondaryTex.SetPixel(0, 0, new Color(0,0,0,0.065f));
				}
				else {
					secondaryTex.SetPixel(0, 0, new Color(1,1,1,0.035f));
				}
				secondaryTex.Apply();
			}

			if (aBox == null) {
				aBox = new GUIStyle();
				aBox.padding = new RectOffset(4,4,4,4);
				aBox.normal.background = primaryTex;
			}
			if (aBox01 == null) {
				aBox01 = new GUIStyle();
				aBox01.padding = new RectOffset(14,4,4,4);
				aBox01.margin = new RectOffset(1,5,5,5);
				aBox01.normal.background = secondaryTex;
			}

			if(myMiniBtn == null) {
				myMiniBtn = new GUIStyle(EditorStyles.miniButton);
				myMiniBtn.padding = new RectOffset(2, 2, 2, 2);
			}
			// Custom Foldout
			if (mainFoldoutStyle == null) {
				mainFoldoutStyle = new GUIStyle(EditorStyles.foldout);
				mainFoldoutStyle.fontStyle = FontStyle.Bold;
				mainFoldoutStyle.normal.textColor = primaryCol;
				mainFoldoutStyle.onNormal.textColor = primaryCol;
				mainFoldoutStyle.active.textColor = primaryCol;
				mainFoldoutStyle.onActive.textColor = primaryCol;
				mainFoldoutStyle.focused.textColor = primaryCol;
				mainFoldoutStyle.onFocused.textColor = primaryCol;
			}
			// Help Btn
			if(myMiniHelpBtn == null) {
				myMiniHelpBtn = new GUIStyle(EditorStyles.miniButton);

//myMiniHelpBtn.fontSize = EditorStyles.miniLabel.fontSize - 3;

				myMiniHelpBtn.padding = new RectOffset(0, 0, 2, 2);
				myMiniHelpBtn.normal.background = null;
				myMiniHelpBtn.normal.textColor = primaryCol;
				myMiniHelpBtn.onNormal.textColor = primaryCol;
				myMiniHelpBtn.active.textColor = primaryCol;
				myMiniHelpBtn.onActive.textColor = primaryCol;
				myMiniHelpBtn.focused.textColor = primaryCol;
				myMiniHelpBtn.onFocused.textColor = primaryCol;
			}

			var ter = script.GetComponent<Terrain>();
			var terData = ter.terrainData;
			var bucketSize = terData.size.x / terData.detailWidth;
			var cellSize = (NumberOfBucketsPerCellEnum.intValue * bucketSize);
			var numberOfCells = terData.size.x / cellSize;

            // -------------------

            GUILayout.Space(16);
            var oldDrawInEditMode = DrawInEditMode.boolValue;
            var GrassDoUpdate = false;

        //	Check prototypes first

            
            EditorGUILayout.BeginVertical(aBox);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Edit Mode", primaryLabelStyle);
				if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
					Application.OpenURL(baseURL + "h.7c98ze61oz6");
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);

				EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginChangeCheck();
		            	EditorGUILayout.PropertyField(DrawInEditMode, new GUIContent("Draw in Edit Mode"));
		            EditorGUI.EndChangeCheck();
		            if (GUI.changed) {
		            	if(DrawInEditMode.boolValue) {
		                    FollowCam.boolValue = true;
		                    useBurstState = useBurst.boolValue;
		                    useBurst.boolValue = true;
		                //	We have to set these directely as we call UpdateGrass() directely
		                    script.Cam = SceneView.lastActiveSceneView.camera;
		                    script.DrawInEditMode = true;
		                    script.useBurst = true;
		                    script.UpdateGrass();
		            	}
		            	else {
		            		FollowCam.boolValue = false;
		                	script.DrawInEditMode = false;
		                	useBurst.boolValue = useBurstState;
		            	}
		            }
					if (GUILayout.Button( "Update Grass")) {
						GrassDoUpdate = true;
					}
	            EditorGUILayout.EndHorizontal();
	
	        EditorGUILayout.EndVertical();

            
            GUILayout.Space(8);
            EditorGUILayout.BeginVertical(aBox);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
						if (GUILayout.Button( "Get Prototypes") ) {
								GetPrototypes();
							}
						EditorGUILayout.PropertyField(FreezeSizeAndColor, new GUIContent("Freeze Size and Colors"));
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();

						if(HDRP || URP)
						{
							GUI.enabled = false;
						}

						if (GUILayout.Button( "Toggle Grid") ) {
							ToggleGrid();
						}
						GUI.enabled = true;
					EditorGUILayout.EndVertical();
	            EditorGUILayout.EndHorizontal();
	        EditorGUILayout.EndVertical();
            

// #if UNITY_2019_3_OR_NEWER
//             GUILayout.Space(8);
//             EditorGUILayout.BeginVertical(aBox);
// 				EditorGUILayout.PropertyField(usingURP, new GUIContent("Universal Render Pipeline"));
// 			EditorGUILayout.EndVertical();
// #else
// 			usingURP.boolValue = false;		
// #endif	

	        if(HDRP || URP) {
	        	usingSRP.boolValue = true;	
	        }
	        else {
	        	usingSRP.boolValue = false;
	        }

			GUILayout.Space(8);
            EditorGUILayout.BeginVertical(aBox);
				EditorGUILayout.PropertyField(vrForceSinglePassInstanced, new GUIContent("VR: Force Single Pass Instanced"));
			EditorGUILayout.EndVertical();

		//	Render Settings
			GUILayout.Space(8);
			EditorGUILayout.BeginVertical(aBox);
				EditorGUILayout.BeginHorizontal();
					Foldout_Rendersettings.boolValue = EditorGUILayout.Foldout(Foldout_Rendersettings.boolValue, "Render Settings", true, mainFoldoutStyle);
					if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
						Application.OpenURL(baseURL + "h.kbrux0gp7wed");
					}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);

	            if (Foldout_Rendersettings.boolValue) {

	                EditorGUILayout.PropertyField(rtCameraSelection, new GUIContent("Cameras"));

	                int selectedLayer = CameraLayer.intValue;
	                selectedLayer = EditorGUILayout.LayerField("Layer", selectedLayer);
	                CameraLayer.intValue = selectedLayer;

					EditorGUILayout.PropertyField(renderingLayerMask, new GUIContent("Rendering Layer Mask"));

	                GUILayout.Space(2);
	                EditorGUILayout.PropertyField(IngnoreOcclusion, new GUIContent("Ignore Visibility"));

	                GUILayout.Space(10);
	                EditorGUILayout.PropertyField(lppv, new GUIContent("Light Probe Proxy Volume"));

	                GUILayout.Space(10);
	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(CullDistance, new GUIContent("Cull Distance"));
	                GUILayout.Space(10);
	                GUILayout.Label("Fade Length", GUILayout.Width(76));
	                EditorGUILayout.PropertyField(FadeLength, new GUIContent(""), GUILayout.MaxWidth(50));
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(2);
	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(DetailFadeStart, new GUIContent("Detail Fade Start"));
	                GUILayout.Space(10);
	                GUILayout.Label("Fade Length", GUILayout.Width(76));
	                EditorGUILayout.PropertyField(DetailFadeLength, new GUIContent(""), GUILayout.MaxWidth(50));
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(2);
	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(VertexFadeStart, new GUIContent("Vertex Fade Start"));
	                GUILayout.Space(10);
	                GUILayout.Label("Fade Length", GUILayout.Width(76));
	                EditorGUILayout.PropertyField(VertexFadeLength, new GUIContent(""), GUILayout.MaxWidth(50));
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(2);
	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(ShadowStart, new GUIContent("Shadow Fade Start Grass"));
	                GUILayout.Space(10);
	                GUILayout.Label("Fade Length", GUILayout.Width(76));
	                EditorGUILayout.PropertyField(ShadowFadeLength, new GUIContent(""), GUILayout.MaxWidth(50));
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(2);
	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(ShadowStartFoliage, new GUIContent("Shadow Fade Start Foliage"));
	                GUILayout.Space(10);
	                GUILayout.Label("Fade Length", GUILayout.Width(76));
	                EditorGUILayout.PropertyField(ShadowFadeLengthFoliage, new GUIContent(""), GUILayout.MaxWidth(50));
	                EditorGUILayout.EndHorizontal();

	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PropertyField(CacheDistance, new GUIContent("Cache Distance"));
	                EditorGUILayout.BeginVertical(GUILayout.Width(140));
	                GUILayout.Label("");
	                EditorGUILayout.EndVertical();
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(10);
	                EditorGUILayout.PropertyField(CullingGroupDistanceFactor, new GUIContent("CullingGroup Distance Factor"));

	                GUILayout.Space(10);
	                EditorGUILayout.PropertyField(DetailDensity, new GUIContent("Grass Density"));

	                GUILayout.Space(10);
	                EditorGUI.BeginChangeCheck();
	                EditorGUILayout.PropertyField(NumberOfBucketsPerCellEnum, new GUIContent("Buckets per Cell"));
	                if (EditorGUI.EndChangeCheck()) {
	                    if (SavedTerrainData.objectReferenceValue != null)
	                        SavedTerrainDataIsDirty = true;
	                }


	                EditorGUILayout.BeginHorizontal();
	                EditorGUILayout.PrefixLabel(" ");
	                EditorGUILayout.BeginVertical();
	                GUILayout.Label("Terrain will be divided into " + numberOfCells + " x " + numberOfCells + " Cells.", EditorStyles.miniLabel);
	#if !UNITY_2019_3_OR_NEWER
					GUILayout.Space(-5);
	#endif
	                GUILayout.Label("Cell Size: " + cellSize + " x " + cellSize + "m.", EditorStyles.miniLabel);
	                EditorGUILayout.EndVertical();
	                EditorGUILayout.EndHorizontal();

	                GUILayout.Space(10);
	                EditorGUILayout.BeginHorizontal();
	#if UNITY_5_6_0 || UNITY_5_6_1 || UNITY_5_6_2
	                EditorGUILayout.PropertyField(useBurst, new GUIContent("Use Burst Init [beta]"));
	#else
	                EditorGUILayout.PropertyField(useBurst, new GUIContent("Use Burst Init"));
	#endif
	                if (useBurst.boolValue) {
	                    GUI.enabled = true;
	                }
	                else {
	                    GUI.enabled = false;
	                }
	                GUILayout.Space(10);
	                GUILayout.Label("Radius", GUILayout.Width(72));
	                EditorGUILayout.PropertyField(BurstRadius, new GUIContent(""), GUILayout.MaxWidth(50));
	                GUI.enabled = true;
	                EditorGUILayout.EndHorizontal();

	                EditorGUILayout.PropertyField(CellsPerFrame, new GUIContent("Cells per Frame"));
	                GUILayout.Space(2);
	            }
            EditorGUILayout.EndVertical();

		//	Advanced Settings
            GUILayout.Space(8);
			EditorGUILayout.BeginVertical(aBox);
			
				EditorGUILayout.BeginHorizontal();
					Foldout_Advancedsettings.boolValue = EditorGUILayout.Foldout(Foldout_Advancedsettings.boolValue, "Advanced Settings", true, mainFoldoutStyle);
					if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
						Application.OpenURL(baseURL + "h.ik42tx1oorzr");
					}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);

	            if (Foldout_Advancedsettings.boolValue) {
		            EditorGUILayout.BeginHorizontal();
		            GUILayout.Label("Compute", mainLabelStyle);
		            if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40)))
		            {
		                Application.OpenURL(baseURL + "h.x0o2mo5bvg52");
		            }
		            EditorGUILayout.EndHorizontal();
		            GUILayout.Space(4);
		            EditorGUILayout.PropertyField(rtUseCompute, new GUIContent("Enable Compute"));
					if(rtUseCompute.boolValue) {
						EditorGUILayout.PropertyField(GrassInstanceSize, new GUIContent("    Instance Size"));
					}

		            GUILayout.Space(10);
		            EditorGUILayout.BeginHorizontal();
		            GUILayout.Label("Floating Origin", mainLabelStyle);
		            if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40)))
		            {
		                Application.OpenURL(baseURL + "h.vx92jeccxcj9");
		            }
		            EditorGUILayout.EndHorizontal();
		            GUILayout.Space(4);
		            EditorGUILayout.PropertyField(EnableTerrainShift, new GUIContent("Enable"));

		            GUILayout.Space(10);
		            EditorGUILayout.BeginHorizontal();
		            GUILayout.Label("Debug", mainLabelStyle);
		            if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40)))
		            {
		                Application.OpenURL(baseURL + "h.p8m1tap48lyb");
		            }
		            EditorGUILayout.EndHorizontal();
		            GUILayout.Space(4);
		            EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(DebugStats, new GUIContent("Debug Stats"));
						EditorGUILayout.PropertyField(DebugCells, new GUIContent("    Debug Cells"));
					EditorGUILayout.EndHorizontal();
					//GUILayout.Space(2);
				}
			EditorGUILayout.EndVertical();
		//	End Advanced Settings

			if(CacheDistance.floatValue <= CullDistance.floatValue) {
				CacheDistance.floatValue = CullDistance.floatValue + cellSize;
			}

		//	Cache TerrainData
			GUILayout.Space(8);
			EditorGUILayout.BeginVertical(aBox);
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label("Saved Terrain Data", primaryLabelStyle);
					if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
						Application.OpenURL(baseURL + "h.cgihwefalakr");
					}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);
	            EditorGUILayout.PropertyField(SavedTerrainData, new GUIContent("Saved TerrainData"));
	            if (SavedTerrainDataIsDirty)
	                EditorGUILayout.HelpBox("The saved terrain data may not match the current 'Buckets per Cell' settings. Please regenerate the terrain data.", MessageType.Error);
	            if (GUILayout.Button( "Save TerrainData") ) {
					SaveTerrainData();
	                SavedTerrainDataIsDirty = false;
	            }
	        EditorGUILayout.EndVertical();

		//	Prototypes
			GUILayout.Space(8);
			EditorGUILayout.BeginVertical(aBox);
			
				EditorGUILayout.BeginHorizontal();
					Foldout_Prototypes.boolValue = EditorGUILayout.Foldout(Foldout_Prototypes.boolValue, "Prototypes", true, mainFoldoutStyle);
					if (GUILayout.Button("Help", myMiniHelpBtn, GUILayout.Width(40))) {
						Application.OpenURL(baseURL + "h.h918om4x0d42");
					}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(4);

				if(Foldout_Prototypes.boolValue) {
				//	Layer Selection
					int items;
					items = terData.detailPrototypes.Length;
				//
					if (
							items != v_mesh.arraySize
						||	items != v_mat.arraySize
						||	items != InstanceRotation.arraySize
						||	items != WriteNormalBuffer.arraySize
						||	items != ShadowCastingMode.arraySize
						||	items != motionVectorGenerationMode.arraySize
						||	items != MinSize.arraySize
						||	items != MaxSize.arraySize
						||	items != Noise.arraySize
						||	items != LayerToMergeWith.arraySize
						||	items != DoSoftMerge.arraySize

					) {
						GetPrototypes();
					}

				//	Second check if all meshes are actually assigned
					bool allMeshesAssigned = true;
					for (int i = 0; i != items; i++ ) {
						if(v_mesh.GetArrayElementAtIndex(i).objectReferenceValue == null) {
							allMeshesAssigned = false;
							break;	
						}
					}
					if (!allMeshesAssigned) {
						Debug.LogError("Not all meshes were assigned. The grass manager grabbed the prototypes from the terrain.");
						GetPrototypes();
					}

// here check for all arrays!

					if (items > 0) {
						string[] toolbarStrings = new string[] {"Single Prototype", "All Prototypes"};
						LayerEditMode.intValue = GUILayout.Toolbar(LayerEditMode.intValue, toolbarStrings);
						GUILayout.Space(4);

						Texture2D[] previews = new Texture2D[items];
						for(int i = 0; i < items; i++) {
							if (terData.detailPrototypes[i].prototype != null) {
			     				previews[i] = (Texture2D)AssetPreview.GetAssetPreview(terData.detailPrototypes[i].prototype);
			     			}
							else {
								previews[i] = (Texture2D)AssetPreview.GetMiniThumbnail(terData.detailPrototypes[i].prototypeTexture);
							}
						}

					//	Check for missing mats
						for (int i = 0; i < items; i++) {
							var tmat = v_mat.GetArrayElementAtIndex(i).objectReferenceValue as Material;
                        	if (tmat == null) {
                        		GUILayout.Space(4);
                        		EditorGUILayout.HelpBox("At least one prototype has no material assigned. Please check all Prototypes.", MessageType.Error);
                        		GUILayout.Space(8);
                        		break;
                        	}
                        }

						if(LayerEditMode.intValue == 0) {

							int thumbSize = 64; //48;
//	Show single Layer
							
							int cols = Mathf.FloorToInt( (EditorGUIUtility.currentViewWidth - thumbSize) / (float)thumbSize );
							int rows = Mathf.CeilToInt ( (float)items / cols);

							LayerSelection.intValue = GUILayout.SelectionGrid(
								LayerSelection.intValue,
								previews,
								(cols < 1) ? 1 : cols,
								GUILayout.MaxWidth(cols * thumbSize),
								GUILayout.MaxHeight(rows * thumbSize)
							);

							var currentSelection = LayerSelection.intValue;

						//	We might have deleted an element.
							if (currentSelection > (items - 1) ) {
								LayerSelection.intValue = items - 1;
								currentSelection = items - 1;
							}

							GUILayout.Space(8);
	                        GUILayout.Label("Layer " + (currentSelection + 1), mainLabelStyle);
	                        GUILayout.Space(2);
	                        EditorGUILayout.PropertyField(v_mesh.GetArrayElementAtIndex(currentSelection), new GUIContent("Mesh"));
							EditorGUILayout.PropertyField(v_mat.GetArrayElementAtIndex(currentSelection), new GUIContent("Material"));
							EditorGUILayout.PropertyField(InstanceRotation.GetArrayElementAtIndex(currentSelection), new GUIContent("Rotation Mode"));

	                    //	We may not have assigned any material.
	                        var mat = v_mat.GetArrayElementAtIndex(currentSelection).objectReferenceValue as Material;
	                        var mayWriteNormals = false;
	                        
	                        if (mat != null) {

	                        //	Instancing
	                        	mat.enableInstancing = true;

	                            if (!mat.shader.name.ToLower().Contains("foliage") && !mat.shader.name.ToLower().Contains("vertexlit")) {
	                                mayWriteNormals = true;
	                            }
		                        if ( InstanceRotation.GetArrayElementAtIndex(currentSelection).enumValueIndex != 2 || !mayWriteNormals ) {
									GUI.enabled = false;
		                        }
								else {
									GUI.enabled = true;
								}
								EditorGUILayout.PropertyField(WriteNormalBuffer.GetArrayElementAtIndex(currentSelection), new GUIContent("    Write Normal Buffer"));

		                        if (mayWriteNormals) {
		                            if (WriteNormalBuffer.GetArrayElementAtIndex(currentSelection).boolValue && InstanceRotation.GetArrayElementAtIndex(currentSelection).enumValueIndex == 2) {
		                                mat.SetFloat("_SampleNormal", 1);
		                                mat.EnableKeyword("_NORMAL");
		                            }
		                            else {
		                                mat.SetFloat("_SampleNormal", 0);
		                                mat.DisableKeyword("_NORMAL");
		                            }
		                        }
		                        else {
		                            WriteNormalBuffer.GetArrayElementAtIndex(currentSelection).boolValue = false;
		                        }

								GUI.enabled = true;

								EditorGUI.BeginChangeCheck();
									EditorGUILayout.PropertyField(ShadowCastingMode.GetArrayElementAtIndex(currentSelection), new GUIContent("Cast Shadows"));
									EditorGUILayout.PropertyField(motionVectorGenerationMode.GetArrayElementAtIndex(currentSelection), new GUIContent("Motion Vectors"));

									GUILayout.Space(4);
									//EditorGUILayout.BeginHorizontal();
										EditorGUILayout.PropertyField(MinSize.GetArrayElementAtIndex(currentSelection), new GUIContent("    Min Size"));
										EditorGUILayout.PropertyField(MaxSize.GetArrayElementAtIndex(currentSelection), new GUIContent("    Max Size"));
										EditorGUILayout.PropertyField(Noise.GetArrayElementAtIndex(currentSelection), new GUIContent("    Noise"));
									//EditorGUILayout.EndHorizontal();

								//	Set min and max size
									if (MaxSize.GetArrayElementAtIndex(currentSelection).floatValue == MinSize.GetArrayElementAtIndex(currentSelection).floatValue ) {
						        		mat.SetVector("_MinMaxScales", new Vector4 (
						        			MinSize.GetArrayElementAtIndex(currentSelection).floatValue,
						        			1.0f, 0, 0)
						        		);
						        	}
						        	else {
							        	mat.SetVector("_MinMaxScales", new Vector4 (
							        		MinSize.GetArrayElementAtIndex(currentSelection).floatValue,
											1.0f / (MaxSize.GetArrayElementAtIndex(currentSelection).floatValue - MinSize.GetArrayElementAtIndex(currentSelection).floatValue),
											0,0)
							        	);
							        }

						        if (EditorGUI.EndChangeCheck() && DrawInEditMode.boolValue) {
									GrassDoUpdate = true;
								}
							
								GUILayout.Space(4);
								var oldlayertomergewith = LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue;
								EditorGUILayout.PropertyField(LayerToMergeWith.GetArrayElementAtIndex(currentSelection), new GUIContent("Layer to merge with"));
								EditorGUILayout.PropertyField(DoSoftMerge.GetArrayElementAtIndex(currentSelection), new GUIContent("    Soft Merge"));
								GUILayout.Space(8);

								_materialEditor = (MaterialEditor) Editor.CreateEditor ( (Material)v_mat.GetArrayElementAtIndex(currentSelection).objectReferenceValue );
								if (_materialEditor != null) {
									_materialEditor.DrawHeader ();
								    _materialEditor.OnInspectorGUI ();
								}
								EditorGUIUtility.labelWidth = 0; // Reset Labelwidth which gets broken by the material inspector
		                        DestroyImmediate (_materialEditor);

		                    //	Do not merge the layer with istself
		                        if (LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue == currentSelection + 1) {
		                        	if(oldlayertomergewith != currentSelection + 1 && oldlayertomergewith < items + 1) {
		                        		LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue = oldlayertomergewith;	
		                        	}
		                        	else {
		                        		LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue = 0;
		                        	}
		                        	Debug.Log("You must not merge a layer with itself.");
		                        }
		                        if (LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue > items) {
		                        	LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue = 0;
		                        	Debug.Log("Layer to merge with does not exist.");
		                        }

		                        if (LayerToMergeWith.GetArrayElementAtIndex(currentSelection).intValue == 0) {
									DoSoftMerge.GetArrayElementAtIndex(currentSelection).boolValue = false;
								}
							}
							else {
								EditorGUILayout.HelpBox("No material assigned. Most likely it could not be created because the prefab in the terrain engine does not have any texture assigned.", MessageType.Error);		
							}
						}
						
						else {

//	////////////////////////////////////////////////////
//	Show all Layers
	                        
	                        GUILayout.Space(8);
	                        for (int i = 0; i < items; i++) {
								EditorGUILayout.BeginHorizontal();
									EditorGUILayout.BeginVertical();
										GUILayout.Label(previews[i], GUILayout.Width(32), GUILayout.Height(32));
									EditorGUILayout.EndVertical();
									EditorGUILayout.BeginVertical();
										GUILayout.Label("Layer " + (i + 1), mainLabelStyle); 
	                                    GUILayout.Space(2);

										EditorGUILayout.PropertyField(v_mesh.GetArrayElementAtIndex(i), new GUIContent("Mesh"));
									//	Mesh might be missing
	                                    var t_mesh = v_mesh.GetArrayElementAtIndex(i).objectReferenceValue;
	                                    if (t_mesh == null) {
											EditorGUILayout.HelpBox("No mesh assigned.", MessageType.Error);	                                    	
	                                    }

										EditorGUILayout.PropertyField(v_mat.GetArrayElementAtIndex(i), new GUIContent("Material"));
										EditorGUILayout.PropertyField(InstanceRotation.GetArrayElementAtIndex(i), new GUIContent("Rotation Mode"));

	                                    //	We may not have any material assigned.
	                                    var mat = v_mat.GetArrayElementAtIndex(i).objectReferenceValue as Material;
	                                    var mayWriteNormals = false;
	                                    
	                                    if (mat != null) {

	                                    //	Instancing
	                        				mat.enableInstancing = true;

	                                        if (!mat.shader.name.ToLower().Contains("foliage") && !mat.shader.name.ToLower().Contains("vertexlit")) {
	                                            mayWriteNormals = true;
	                                        }
		                                    if ( InstanceRotation.GetArrayElementAtIndex(i).enumValueIndex != 2 || !mayWriteNormals) {
												GUI.enabled = false;
											}
											EditorGUILayout.PropertyField(WriteNormalBuffer.GetArrayElementAtIndex(i), new GUIContent("    Write Normal Buffer"));

		                                    if (mayWriteNormals) {
		                                        if (WriteNormalBuffer.GetArrayElementAtIndex(i).boolValue && InstanceRotation.GetArrayElementAtIndex(i).enumValueIndex == 2) {
		                                            //GUILayout.Label("Make sure that ", EditorStyles.miniLabel);
		                                            mat.SetFloat("_SampleNormal", 1);
		                                            mat.EnableKeyword("_NORMAL");
		                                        }
		                                        else {
		                                            mat.SetFloat("_SampleNormal", 0);
		                                            mat.DisableKeyword("_NORMAL");
		                                        }
		                                    }
		                                    else {
		                                        WriteNormalBuffer.GetArrayElementAtIndex(i).boolValue = false;
		                                    }
											GUI.enabled = true;

											

											EditorGUI.BeginChangeCheck();
												EditorGUILayout.PropertyField(ShadowCastingMode.GetArrayElementAtIndex(i), new GUIContent("Cast Shadows"));
												EditorGUILayout.PropertyField(motionVectorGenerationMode.GetArrayElementAtIndex(i), new GUIContent("Motion Vectors"));
												
												GUILayout.Space(4);
												EditorGUILayout.PropertyField(MinSize.GetArrayElementAtIndex(i), new GUIContent("    Min Size"));
												EditorGUILayout.PropertyField(MaxSize.GetArrayElementAtIndex(i), new GUIContent("    Max Size"));
												EditorGUILayout.PropertyField(Noise.GetArrayElementAtIndex(i), new GUIContent("    Noise"));

											//	Set min and max size
												if(!FreezeSizeAndColor.boolValue) {
													if (MaxSize.GetArrayElementAtIndex(i).floatValue == MinSize.GetArrayElementAtIndex(i).floatValue ) {
										        		mat.SetVector("_MinMaxScales", new Vector4 (
										        			MinSize.GetArrayElementAtIndex(i).floatValue,
										        			1.0f, 0, 0)
										        		);
										        	}
										        	else {
											        	mat.SetVector("_MinMaxScales", new Vector4 (
											        		MinSize.GetArrayElementAtIndex(i).floatValue,
															1.0f / (MaxSize.GetArrayElementAtIndex(i).floatValue - MinSize.GetArrayElementAtIndex(i).floatValue),
															0,0)
											        	);
											        }
											    }

										    if (EditorGUI.EndChangeCheck() && DrawInEditMode.boolValue) {
												GrassDoUpdate = true;
											}

											GUILayout.Space(4);
											var oldlayertomergewith = LayerToMergeWith.GetArrayElementAtIndex(i).intValue;
											EditorGUILayout.PropertyField(LayerToMergeWith.GetArrayElementAtIndex(i), new GUIContent("Layer to merge with"));
											EditorGUILayout.PropertyField(DoSoftMerge.GetArrayElementAtIndex(i), new GUIContent("    Soft Merge"));
											GUILayout.Space(12);

											//	Do not merge the layer with istself
					                        if (LayerToMergeWith.GetArrayElementAtIndex(i).intValue == i + 1) {
					                        	if(oldlayertomergewith != i + 1 && oldlayertomergewith < items + 1) {
					                        		LayerToMergeWith.GetArrayElementAtIndex(i).intValue = oldlayertomergewith;	
					                        	}
					                        	else {
					                        		LayerToMergeWith.GetArrayElementAtIndex(i).intValue = 0;
					                        	}
					                        	Debug.Log("You must not merge a layer with itself.");
					                        }
					                        if (LayerToMergeWith.GetArrayElementAtIndex(i).intValue > items) {
					                        	LayerToMergeWith.GetArrayElementAtIndex(i).intValue = 0;
					                        	Debug.Log("Layer to merge with does not exist.");
					                        }
											

											if (LayerToMergeWith.GetArrayElementAtIndex(i).intValue == 0) {
												DoSoftMerge.GetArrayElementAtIndex(i).boolValue = false;
											}
										}
										else {
											EditorGUILayout.HelpBox("No material assigned. Most likely it could not be created because the prefab in the terrain engine does not have any texture assigned.", MessageType.Error);
										}

									EditorGUILayout.EndVertical();
								EditorGUILayout.EndHorizontal();

							}
						}
					}
				}
			EditorGUILayout.EndVertical();


            if (oldDrawInEditMode == false && DrawInEditMode.boolValue == true)
            {
                rtBurstRadius.floatValue = BurstRadius.floatValue;
                BurstRadius.floatValue = CullDistance.floatValue;
                rtIngnoreOcclusion.boolValue = IngnoreOcclusion.boolValue;
                IngnoreOcclusion.boolValue = true;
                rtCameraSelection.enumValueIndex = CameraSelection.enumValueIndex;
                CameraSelection.enumValueIndex = (int)GrassCameras.AllCameras;
            }
            else if (oldDrawInEditMode == true && DrawInEditMode.boolValue == false)
            {
                BurstRadius.floatValue = rtBurstRadius.floatValue;
                IngnoreOcclusion.boolValue = rtIngnoreOcclusion.boolValue;
                CameraSelection.enumValueIndex = rtCameraSelection.enumValueIndex;
            }

            GUILayout.Space(12);		
			GrassManager.ApplyModifiedProperties();
            
            // call if all is set up
            if (GrassDoUpdate) {
            	script.UpdateGrass();
            	//UpdateCamera();
            }
            
		}


// --------------------------------------------
		
		// private static void AddCamFollow() {
	    //     EditorApplication.update += OnUpdate;
	    // }
	    // private static void RemoveCamFollow() {
	    //     EditorApplication.update -= OnUpdate;
	    // }

	    // private static void OnUpdate() {
	    // 	sceneCam = SceneView.lastActiveSceneView.camera;
	    // 	MainCam = Camera.main;
	    //     // if(sceneCam != null && MainCam != null) {
	    //     //     MainCam.transform.position = sceneCam.transform.position;
	    //     //     MainCam.transform.rotation = sceneCam.transform.rotation;
	    //     // }
	    // }

	    // private void UpdateCamera() {
	    // 	sceneCam = SceneView.lastActiveSceneView.camera;
	    // 	MainCam = Camera.main;
	    //     if(sceneCam != null && MainCam != null) {
	    //         // MainCam.transform.position = sceneCam.transform.position;
	    //         // MainCam.transform.rotation = sceneCam.transform.rotation;
	    //         // var pos = MainCam.transform.position;
	    //         // pos.y += 0.1f;
	    //         // MainCam.transform.position = pos;
	    //         // pos.y -= 0.1f;
	    //         // MainCam.transform.position = pos;
	    //         // script.BurstInit();
	    //         // MainCam.Render();
	    //         // var pos = sceneCam.transform.position;
	    //         // pos.y += 0.1f;
	    //         // sceneCam.transform.position = pos;
	    //         // pos.y -= 0.1f;
	    //         // sceneCam.transform.position = pos;
	    //         // script.BurstInit();
	    //         // sceneCam.Render();

	    //     }
	    // }


// --------------------------------------------

		private void ToggleGrid() {

			GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData;

	        int id = ter.transform.gameObject.GetInstanceID();
	        var goname = "GridProjector_" + id.ToString();
	        var tempPro = GameObject.Find(goname);

		    if( tempPro == null) {

		        var go = new GameObject();
	            go.name = goname;
	            Projector proj = go.AddComponent<Projector>() as Projector;
	            proj.orthographic = true;
	            proj.orthographicSize = terData.size.x * 0.5f;
	            proj.material = new Material(Shader.Find("AdvancedTerrainGrass/GridProjector"));
	            string[] guIDS = AssetDatabase.FindAssets ("Atg_Grid");
	            if (guIDS.Length > 0) {
	            	proj.material.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(guIDS[0]), typeof(Texture));
	            }
	            else {
	            	Debug.Log("Grid Texture not found.");
	            }

	            var pbucketSize = terData.size.x / terData.detailWidth;
	            var pcellSize = (NumberOfBucketsPerCellEnum.intValue * pbucketSize);
	            var pnumberOfCells = terData.size.x / pcellSize;

	            proj.material.mainTextureScale = new Vector2(pnumberOfCells, pnumberOfCells);

	            proj.farClipPlane = terData.size.y * 2.0f;
	            go.transform.position = new Vector3(0,terData.size.y,0) + ter.GetPosition() + terData.size * 0.5f;
	            go.transform.rotation = Quaternion.Euler(90, 0, 0);
	        }
	        else {
	        	if (tempPro.GetComponent<Projector>().material != null) {
	        		DestroyImmediate(tempPro.GetComponent<Projector>().material);
	        	}
	        	DestroyImmediate(tempPro);
	        }
		}

// --------------------------------------------

		private void GetPrototypes() {
			GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData; 

	        int oldNumberOfLayers = v_mesh.arraySize;
	        int NumberOfLayers = terData.detailPrototypes.Length;
	        
	    //	Initial synchronisation with original terrain settings
	        if(!FirstTimeSynced.boolValue) {
	        	FirstTimeSynced.boolValue = true;
	        	DetailDensity.floatValue = ter.detailObjectDensity;
	        	CullDistance.floatValue = ter.detailObjectDistance;
	        	CacheDistance.floatValue = ter.detailObjectDistance * 1.2f;
	        }
	        
	    //	If the arrays grow new entries will be filled with a copy of the last entry so our "if not already assigned" fails
	        v_mesh.arraySize = NumberOfLayers;
	        v_mat.arraySize = NumberOfLayers;
	        InstanceRotation.arraySize = NumberOfLayers;
	        WriteNormalBuffer.arraySize = NumberOfLayers;
	        ShadowCastingMode.arraySize = NumberOfLayers;
	        motionVectorGenerationMode.arraySize = NumberOfLayers;
	        MinSize.arraySize = NumberOfLayers;
	        MaxSize.arraySize = NumberOfLayers;
	        Noise.arraySize = NumberOfLayers;

	        LayerToMergeWith.arraySize = NumberOfLayers;
	        DoSoftMerge.arraySize = NumberOfLayers;

	        for (int i = 0; i < NumberOfLayers; i ++) {

	        //	Do we deal with a new entry?
	        	bool hasGrown = ( (oldNumberOfLayers < NumberOfLayers) && (i == NumberOfLayers - 1) ) ? true : false;

	        	if(!FreezeSizeAndColor.boolValue) {	        
	        		MinSize.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].minHeight;
	        		MaxSize.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].maxHeight;
	        	}
	 			else {
	 				if (MinSize.GetArrayElementAtIndex(i).floatValue == Single.NaN)
	 					MinSize.GetArrayElementAtIndex(i).floatValue = 1.0f;
	 				if (MaxSize.GetArrayElementAtIndex(i).floatValue == Single.NaN)
	 					MaxSize.GetArrayElementAtIndex(i).floatValue = 2.0f;
	 			}

	        	Noise.GetArrayElementAtIndex(i).floatValue = terData.detailPrototypes[i].noiseSpread;
	        	bool materialSet = false;

	        //	As we simply size the array mesh and mat are wrong
	        	if (hasGrown) {
	        		v_mesh.GetArrayElementAtIndex(NumberOfLayers - 1).objectReferenceValue = null;
	        		v_mat.GetArrayElementAtIndex(NumberOfLayers - 1).objectReferenceValue = null;
	        	}
	        	
	        	Texture prototypeTex = null;

//	Here we deal with detail meshes
	        	if (terData.detailPrototypes[i].prototype != null) {

	        	//	We need a lot of safeguards here
	        		var t_Filter = terData.detailPrototypes[i].prototype.GetComponent<MeshFilter>();
	        		var t_Renderer = terData.detailPrototypes[i].prototype.GetComponent<Renderer>();
	        		if (t_Filter != null) {
	        			v_mesh.GetArrayElementAtIndex(i).objectReferenceValue = t_Filter.sharedMesh;
	        		}
	        		// v_mesh.GetArrayElementAtIndex(i).objectReferenceValue = terData.detailPrototypes[i].prototype.GetComponent<MeshFilter>().sharedMesh;
	        		bool vertexLit = false;

	        	//	Handle Detail meshes using the VertexLit Shader
	        		if(terData.detailPrototypes[i].renderMode == DetailRenderMode.VertexLit && v_mat.GetArrayElementAtIndex(i).objectReferenceValue == null ) {
	        			if (t_Renderer != null) {
	        				prototypeTex = t_Renderer.sharedMaterial.GetTexture("_MainTex");
	        			}
	        			vertexLit = true;
	        		}
	        	//	Handle Detail Meshes using the Grass Shader
	        		else {
	        			if (t_Renderer != null) {
	        				prototypeTex = t_Renderer.sharedMaterial.GetTexture("_MainTex");
	        			}
	        		}

        		
	        	//	Only get material if it is null
					if (prototypeTex != null && v_mat.GetArrayElementAtIndex(i).objectReferenceValue == null) {
        				var path = Path.GetDirectoryName( AssetDatabase.GetAssetPath(prototypeTex) );
	        			path = path + "/" + prototypeTex.name + ".mat";
	        			Material newMat = AssetDatabase.LoadAssetAtPath (path, typeof (Material)) as Material;
	        			if (newMat == null) {
	        				if(vertexLit){
	        					if(URP) {
	        						newMat = new Material(Shader.Find("ATG URP/VertexLit SG URP"));
	        					}
	        					else if(HDRP) {
	        						newMat = new Material(Shader.Find("ATG HDRP/VertexLit HDRP"));	
	        					}
	        					else {
	        						newMat = new Material(Shader.Find("AdvancedTerrainGrass/VertexLit Shader"));
	        					}
	        				}
	        				else {
	        					//if(usingURP.boolValue) {
	        					if(URP) {
	        						newMat = new Material(Shader.Find("ATG URP/Grass SG URP"));
	        					}
	        					else if(HDRP) {
	        						newMat = new Material(Shader.Find("ATG HDRP/Grass HDRP"));	
	        					}
	        					else {
	        						newMat = new Material(Shader.Find("AdvancedTerrainGrass/Grass Base Shader"));
	        					}
	        				}
	        				newMat.SetTexture("_MainTex", prototypeTex );
	        				AssetDatabase.CreateAsset (newMat, path);
	        			}
	        			v_mat.GetArrayElementAtIndex(i).objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath( path, typeof(Material) );
					}
					else {
						Debug.Log ("Could not create a material as no texture has been assigned.");
					}
					materialSet = true;
	        	}

	        //	Handle simple texture based grass
	        	if (!materialSet && (v_mat.GetArrayElementAtIndex(i).objectReferenceValue == null) ) {
	        		prototypeTex = terData.detailPrototypes[i].prototypeTexture;
	        		if (prototypeTex != null) {
		        		var path = Path.GetDirectoryName(  AssetDatabase.GetAssetPath(prototypeTex)   );
		        		path = path + "/" + prototypeTex.name + ".mat";
		        		Material newMat = AssetDatabase.LoadAssetAtPath (path, typeof (Material)) as Material;
		        		if (newMat == null) {
		        			if(URP) {
        						newMat = new Material(Shader.Find("ATG URP/Grass SG URP"));
        					}
        					else if(HDRP) {
        						newMat = new Material(Shader.Find("ATG HDRP/Grass HDRP"));	
        					}
        					else{
        						newMat = new Material(Shader.Find("AdvancedTerrainGrass/Grass Base Shader"));
        					}
		        			newMat.SetTexture("_MainTex", prototypeTex );
		        			AssetDatabase.CreateAsset (newMat, path);
		        		}
		        		v_mat.GetArrayElementAtIndex(i).objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath( path, typeof(Material) );
		        	}
		        }

		    //	Add default Quad for simple Grass Textures
        		if (v_mesh.GetArrayElementAtIndex(i).objectReferenceValue == null) {
        		//	Most common is width * height
        			prototypeTex = terData.detailPrototypes[i].prototypeTexture;
        			string MeshToAssign = "Atg_BaseQuad";
        			if (prototypeTex != null) {
        				if ( (prototypeTex.width / prototypeTex.height) <= 0.5f ) {
        					MeshToAssign = "Atg_BaseRectVertical";
        				}
        				else if ( (prototypeTex.width / prototypeTex.height) >= 2.0f ) {
        					MeshToAssign = "Atg_BaseRectHorizontal";
        				}
        			}
        			string[] guIDS = AssetDatabase.FindAssets (MeshToAssign);
	        		if (guIDS.Length > 0) {
						v_mesh.GetArrayElementAtIndex(i).objectReferenceValue = (Mesh)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(guIDS[0]), typeof(Mesh));
					}
					else {
						Debug.Log("Could not assign a valid Mesh because '" + MeshToAssign + "' could not be found.");
					}
				}
	        	
		    //	Update Colors
		    	if(!FreezeSizeAndColor.boolValue) {
			    	if (v_mat.GetArrayElementAtIndex(i).objectReferenceValue != null) {
			    		Material Mat = (Material) v_mat.GetArrayElementAtIndex(i).objectReferenceValue as Material;
			    		Mat.SetColor("_HealthyColor", terData.detailPrototypes[i].healthyColor);
			        	Mat.SetColor("_DryColor", terData.detailPrototypes[i].dryColor);
			        //	Handle the case that both values are equal (division by zero)
			        	if (MaxSize.GetArrayElementAtIndex(i).floatValue == MinSize.GetArrayElementAtIndex(i).floatValue ) {
			        		Mat.SetVector("_MinMaxScales", new Vector4 (
			        			MinSize.GetArrayElementAtIndex(i).floatValue,
			        			1.0f, 0, 0)
			        		);
			        	}
			        	else {
				        	Mat.SetVector("_MinMaxScales", new Vector4 (
				        		MinSize.GetArrayElementAtIndex(i).floatValue,
								1.0f / (MaxSize.GetArrayElementAtIndex(i).floatValue - MinSize.GetArrayElementAtIndex(i).floatValue),
								0,0)
				        	);
				        }
			    	}
			    }
	        }
		}

//	--------------------------------------------
// 	This function more or less equals InitCells from GrassManager.cs.
//	It merges density maps and configures Cells and CellContents and writes all out to disc.
	
	public void SaveTerrainData() {
	        GrassManager script = (GrassManager)target;
			var ter = script.GetComponent<Terrain>();
	        var terData = ter.terrainData;
	        var TerrainSize = terData.size;

	        Vector2 TerrainDetailSize = Vector2.zero;
            TerrainDetailSize.y = terData.detailHeight;
	        TerrainDetailSize.x = terData.detailWidth;
            TerrainDetailSize.y = terData.detailHeight;

            var TerrainPosition = ter.GetPosition();
            Vector3 OneOverTerrainSize;
            OneOverTerrainSize.x = 1.0f/TerrainSize.x;
            OneOverTerrainSize.y = 1.0f/TerrainSize.y;
            OneOverTerrainSize.z = 1.0f/TerrainSize.z;

	    //  We assume squared Terrains here...
            var BucketSize = TerrainSize.x / TerrainDetailSize.x;
            var NumberOfBucketsPerCell = (int)NumberOfBucketsPerCellEnum.intValue;
            var CellSize = NumberOfBucketsPerCell * BucketSize;
            var NumberOfCells = (int) (TerrainSize.x / CellSize);

        //  Merge Layers
            var NumberOfLayers = terData.detailPrototypes.Length;
            var OrigNumberOfLayers = NumberOfLayers;
            int[][] MergeArray = new int[OrigNumberOfLayers][]; // Actually too big...
            int[][] SoftMergeArray = new int[OrigNumberOfLayers][]; // Actually too big...

            int maxBucketDensity = 0;

        //  Go through all layers and determine the number of actually drawn layers - needed by Compute
            var NumberOfFinalLayers = 0;
            var FinalLayersIndices = new int[OrigNumberOfLayers];
            for (int i = 0; i < OrigNumberOfLayers; i++) {
                if (LayerToMergeWith.GetArrayElementAtIndex(i).intValue == 0) { // && LayerToMergeWith[LayerToMergeWith[i]] == 0)
                    FinalLayersIndices[i] = NumberOfFinalLayers;
                    NumberOfFinalLayers++;
                }
                else {
                    FinalLayersIndices[i] = -1;
                }
            }
            var LayersMaxDensity = new int[NumberOfFinalLayers];
            for (int i = 0; i < NumberOfFinalLayers; i++) {
                LayersMaxDensity[i] = 0;
            }

        //  Check if we have to merge detail layers
            for (int i = 0; i < OrigNumberOfLayers; i ++) {

                int t_LayerToMergeWith = LayerToMergeWith.GetArrayElementAtIndex(i).intValue;
                int index_LayerToMergeWith = t_LayerToMergeWith - 1;
                
                if( (t_LayerToMergeWith != 0) && (t_LayerToMergeWith != (i+1))  ) {
                    
                //  Check if the Layer we want to merge with does not get merged itself..
                    if ( LayerToMergeWith.GetArrayElementAtIndex(index_LayerToMergeWith).intValue == 0 ) {
                        
                        //Debug.Log("Merge Layer " + (i +1) + " with Layer " + LayerToMergeWith.GetArrayElementAtIndex(i) );

                        if ( MergeArray[ index_LayerToMergeWith ] == null ) {
                            MergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                        }

                        if ( DoSoftMerge.GetArrayElementAtIndex(i).boolValue ) {
                            if( SoftMergeArray[ index_LayerToMergeWith ] == null ) {
                                SoftMergeArray[ index_LayerToMergeWith ] = new int[ OrigNumberOfLayers - 1 ]; // Also actually too big
                            }
                        }

                    //	Find a the first free entry
                        for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                            if ( MergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                MergeArray[ index_LayerToMergeWith ][j] = 									i + 1; // as index starts at 1 (0 means: do not merge)
                                break;
                            } 
                        }
                    //  Find a the first free entry Soft Merge
                        if ( DoSoftMerge.GetArrayElementAtIndex(i).boolValue ) {
                            for (int j = 0; j < OrigNumberOfLayers - 1; j++) {
                                if ( SoftMergeArray[ index_LayerToMergeWith ][j] == 0 ) {
                                    SoftMergeArray[ index_LayerToMergeWith ][j] =                           i + 1; // as index starts at 1 (0 means: do not merge)
                                    break;
                                } 
                            }
                        }
                    }
                }
            }


            GrassCell[] tCells = new GrassCell[NumberOfCells * NumberOfCells];
        //  As we do not know the final cout we create cellcontens in a temporary list
            List<GrassCellContent> tempCellContent = new List<GrassCellContent>();
            tempCellContent.Capacity = NumberOfCells * NumberOfCells * NumberOfLayers;

            byte[][] tmapByte = new byte[NumberOfLayers][];

        //  Read and convert [,] into []
            // Here we are working on "pixels" or buckets and lay out the array like x0.y0, x0.y2, x0.y3, ....
            for(int layer = 0; layer < NumberOfLayers; layer++) {
                if (LayerToMergeWith.GetArrayElementAtIndex(layer).intValue == 0 || DoSoftMerge.GetArrayElementAtIndex(layer).boolValue ) {
					tmapByte[layer] = new byte[ (int)(TerrainDetailSize.x * TerrainDetailSize.y) ];
                //  Check merge
                    bool doMerge = false;
                    if (MergeArray[layer] != null && !DoSoftMerge.GetArrayElementAtIndex(layer).boolValue) {
                        doMerge = true;
                    }
                    for (int x = 0; x < (int)TerrainDetailSize.x; x++ ){
                        for (int y = 0; y < (int)TerrainDetailSize.y; y ++) {
                            // flipped!!!!!????????? no,not in this case.
                            int[,] temp = terData.GetDetailLayer(x, y, 1, 1, layer );
							tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( (int)temp[0,0] );                            
                        //  Merge
                            if(doMerge) {
                                for (int m = 0; m < OrigNumberOfLayers - 1; m++ ) {
                                //  Check DoSoftMerge as otherwise even soft merged layers would be added
                                    if (MergeArray[layer][m] != 0 && !DoSoftMerge.GetArrayElementAtIndex(MergeArray[layer][m] - 1).boolValue ) {
                                        temp = terData.GetDetailLayer(x, y, 1, 1, MergeArray[layer][m] - 1 ); // as index starts as 1!
										tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] = Convert.ToByte( tmapByte[layer][ x * (int)TerrainDetailSize.y + y ] + (int)temp[0,0] );                                      
                                    }
                                }
                            }  //  End of merge
                        }
                    }
                }
            }

            int density = 0;
            int CellContentOffset = 0;
            Vector3 Center;
            int ArrayOffset = 0;

            for(int x = 0; x < NumberOfCells; x++) {
                for(int z = 0; z < NumberOfCells; z++) {
                    var CurrentCellIndex = x * NumberOfCells + z;
                //  Find Height or Position.y of Cell
                    Vector2 normalizedPos;
                    
                    //  center
                    normalizedPos.x = (x * CellSize + 0.5f * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + 0.5f * CellSize) * OneOverTerrainSize.z;
                    float sampledHeight = terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  lower left
                    normalizedPos.x = (x * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  upper left
                    normalizedPos.x = (x * CellSize + CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  upper right
                    normalizedPos.x = (x * CellSize + CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  lower right
                    normalizedPos.x = (x * CellSize) * OneOverTerrainSize.x;
                    normalizedPos.y = (z * CellSize + CellSize) * OneOverTerrainSize.z;
                    sampledHeight += terData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
                    //  average
                    sampledHeight *= 0.2f;
                    

                    tCells[CurrentCellIndex] = new GrassCell();
                    tCells[CurrentCellIndex].state = 0;
                    tCells[CurrentCellIndex].index = CurrentCellIndex;
                //  Center in World Space – used by cullingspheres
                    Center.x = x * CellSize + 0.5f * CellSize  +  TerrainPosition.x;
                    Center.y = sampledHeight  +  TerrainPosition.y;
                    Center.z = z * CellSize + 0.5f * CellSize  +  TerrainPosition.z;
                    tCells[CurrentCellIndex].Center = Center;
					//tCells[CurrentCellIndex].CellContentIndexes = new List<int>();
                    tCells[CurrentCellIndex].CellContentCount = 0;

                    var tempCellContentIndexes = new List<int>();

                    int tempBucketDensity = 0;
                    int layerPointer = 0;

                    //	Create and setup all CellContens (layers) for the given Cell
                    for (int layer = 0; layer < NumberOfLayers; layer++) {
                    
                    //  Only create cellcontent entry if the layer does not get merged.
                        if (LayerToMergeWith.GetArrayElementAtIndex(layer).intValue == 0 ) {
                        //  Sum up density for all buckets 
                            for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                    //	Here we are working on cells and buckets!
                                    tempBucketDensity = (int)(tmapByte[layer][
								       (x * NumberOfBucketsPerCell) * (int)TerrainDetailSize.y + xp * (int)TerrainDetailSize.y 
								       + z * NumberOfBucketsPerCell + yp 
									]);
                                    if (tempBucketDensity > maxBucketDensity) {
                                        maxBucketDensity = tempBucketDensity;
                                    }
                                    density += tempBucketDensity;
                                }
                            }

                        //  Check for softly merged layers 
                            int NumberOfSoftlyMergedLayers = 0;

                            if(SoftMergeArray[layer] != null) {
                                for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                    if ( SoftMergeArray[layer][l] == 0) {
                                        break;
                                    }
                                    int softMergedLayer = SoftMergeArray[layer][l] - 1; // as index starts at 1
                                    int softDensity = 0;

                                    //  Sum up density for all buckets 
                                    for(int xp = 0; xp < NumberOfBucketsPerCell; xp++) {
                                        for(int yp = 0; yp < NumberOfBucketsPerCell; yp++) {
                                        //  here we are working on cells and buckets!
                                            tempBucketDensity = (int)(tmapByte[softMergedLayer][
                                               (x * NumberOfBucketsPerCell) * (int)TerrainDetailSize.y + xp * (int)TerrainDetailSize.y 
                                               + z * NumberOfBucketsPerCell + yp 
                                            ]);
                                            softDensity += tempBucketDensity;
                                        }
                                    }
                                    if (softDensity > 0) {
                                        NumberOfSoftlyMergedLayers += 1;
                                        density += softDensity;
                                    }
                                    if (NumberOfSoftlyMergedLayers * 16 > maxBucketDensity) {
                                        maxBucketDensity = NumberOfSoftlyMergedLayers * 16 + 16; // * 2;
                                    }
                                }
                            }

                        //  Compute: Store max density per layer which will determine the size of the append buffers
                            //if (UseCompute) {
                                if(density > LayersMaxDensity[layerPointer]) {
                                    LayersMaxDensity[layerPointer] = density;
                                }
                                layerPointer++;
                            //}

                            //  Skip CellContent if density = 0                    
                            if (density > 0) {
                            //  Register CellContent to Cell
								//tCells[CurrentCellIndex].CellContentIndexes.Add(CellContentOffset);
                                tempCellContentIndexes.Add(CellContentOffset);  
                                tCells[CurrentCellIndex].CellContentCount += 1;
                            //  Add new CellContent
                                var tempContent = new GrassCellContent();
                                tempContent.index = CellContentOffset;
								tempContent.Layer = layer;
                            //  Center in World Space – used by drawmeshindirect
                                tempContent.Center = Center;
                            //  Pivot of cell in local terrain space
                                tempContent.Pivot = new Vector3( x * CellSize, sampledHeight, z * CellSize );
								tempContent.PatchOffsetX = x * NumberOfBucketsPerCell * (int)TerrainDetailSize.y;
								tempContent.PatchOffsetZ = z * NumberOfBucketsPerCell;
                                tempContent.Instances = density;

                            //  Softly merged Layers
                                if( NumberOfSoftlyMergedLayers > 0 ) {
                                    List<int> tempSoffMergedLayers = new List<int>();
                                    for (int l = 0; l < OrigNumberOfLayers - 1; l++) {
                                        if(SoftMergeArray[layer][l] != 0) { //} && DoSoftMerge[  MergeArray[layer][l] - 1 ]  ) {
                                            //Debug.Log("Add softly merged layer to cell content, layer: " + ( SoftMergeArray[layer][l] - 1) + " at index " + CellContentOffset );
                                            tempSoffMergedLayers.Add( SoftMergeArray[layer][l] - 1  );
                                        }
                                    }
                                    tempContent.SoftlyMergedLayers = tempSoffMergedLayers.ToArray();
                                }

                            //  Add content to temp content list
                                tempCellContent.Add(tempContent);
                                CellContentOffset += 1;
                            }
                            density = 0;
                        }
                    }
                //  List to array
                   	tCells[CurrentCellIndex].CellContentIndexes = tempCellContentIndexes.ToArray();
                }
				ArrayOffset += (int)TerrainDetailSize.x;                
            }

            GrassCellContent[] tCellContent = tempCellContent.ToArray();
            tempCellContent.Clear();

		//	Finalize and save data
			GrassTerrainDefinitions asset = ScriptableObject.CreateInstance<GrassTerrainDefinitions>();

        //	Copy data to Scriptable Object
            asset.TerrainPosition = TerrainPosition;
            asset.DensityMaps = new List<DetailLayerMap>();
			for( int i = 0; i < tmapByte.Length; i++) {
				asset.DensityMaps.Add( new DetailLayerMap() );
				asset.DensityMaps[i].mapByte = tmapByte[i];
			}
			asset.Cells = tCells;
			asset.CellContent = tCellContent;
			asset.maxBucketDensity = maxBucketDensity;

			asset.LayersMaxDensity = LayersMaxDensity;

            Debug.Log("MaxBucketDensity: " + maxBucketDensity);

			string terPath = Path.GetDirectoryName( AssetDatabase.GetAssetPath(terData) );
			string terName = Path.GetFileNameWithoutExtension( AssetDatabase.GetAssetPath(terData) );
			if(rtUseCompute.boolValue) {
				AssetDatabase.CreateAsset(asset, terPath + "/" + terName + "_GrassTerrainData_Compute.asset");
			}
			else {
				AssetDatabase.CreateAsset(asset, terPath + "/" + terName + "_GrassTerrainData.asset");
			}
			AssetDatabase.SaveAssets();
			SavedTerrainData.objectReferenceValue = asset;
        }

// -------

		private void GetProperties() {

			DrawInEditMode = GrassManager.FindProperty("DrawInEditMode");
			FollowCam = GrassManager.FindProperty("FollowCam");
			MainCamPos = GrassManager.FindProperty("MainCamPos");
			MainCamRot = GrassManager.FindProperty("MainCamRot");
            rtCameraSelection = GrassManager.FindProperty("rtCameraSelection");
            rtIngnoreOcclusion = GrassManager.FindProperty("rtIngnoreOcclusion");
            rtBurstRadius = GrassManager.FindProperty("rtBurstRadius");

            vrForceSinglePassInstanced = GrassManager.FindProperty("vrForceSinglePassInstanced");

            usingSRP = GrassManager.FindProperty("usingSRP");
            lppv = GrassManager.FindProperty("lppv");

            FreezeSizeAndColor = GrassManager.FindProperty("FreezeSizeAndColor");
			DebugStats = GrassManager.FindProperty("DebugStats");
			DebugCells = GrassManager.FindProperty("DebugCells");
			FirstTimeSynced = GrassManager.FindProperty("FirstTimeSynced");
			LayerEditMode = GrassManager.FindProperty("LayerEditMode");
			LayerSelection = GrassManager.FindProperty("LayerSelection");
			Foldout_Rendersettings = GrassManager.FindProperty("Foldout_Rendersettings");
			Foldout_Advancedsettings = GrassManager.FindProperty("Foldout_Advancedsettings");
			Foldout_Prototypes = GrassManager.FindProperty("Foldout_Prototypes");

			IngnoreOcclusion = GrassManager.FindProperty("IngnoreOcclusion");
			CameraSelection = GrassManager.FindProperty("CameraSelection");
			CameraLayer = GrassManager.FindProperty("CameraLayer");
			CullDistance = GrassManager.FindProperty("CullDistance");
			FadeLength = GrassManager.FindProperty("FadeLength");
			CacheDistance = GrassManager.FindProperty("CacheDistance");
			DetailFadeStart  = GrassManager.FindProperty("DetailFadeStart");
			DetailFadeLength = GrassManager.FindProperty("DetailFadeLength");
			VertexFadeStart  = GrassManager.FindProperty("VertexFadeStart");
			VertexFadeLength = GrassManager.FindProperty("VertexFadeLength");
			ShadowStart = GrassManager.FindProperty("ShadowStart");
			ShadowFadeLength = GrassManager.FindProperty("ShadowFadeLength");
			ShadowStartFoliage = GrassManager.FindProperty("ShadowStartFoliage");
			ShadowFadeLengthFoliage = GrassManager.FindProperty("ShadowFadeLengthFoliage");
            CullingGroupDistanceFactor = GrassManager.FindProperty("CullingGroupDistanceFactor");

            useBurst = GrassManager.FindProperty("useBurst");
			BurstRadius = GrassManager.FindProperty("BurstRadius");
			CellsPerFrame = GrassManager.FindProperty("CellsPerFrame");
			DetailDensity = GrassManager.FindProperty("DetailDensity");

			SavedTerrainData = GrassManager.FindProperty("SavedTerrainData");
			NumberOfBucketsPerCellEnum = GrassManager.FindProperty("NumberOfBucketsPerCellEnum");

			v_mesh = GrassManager.FindProperty("v_mesh");
			v_mat = GrassManager.FindProperty("v_mat");
			InstanceRotation = GrassManager.FindProperty("InstanceRotation");
			WriteNormalBuffer = GrassManager.FindProperty("WriteNormalBuffer");
			ShadowCastingMode = GrassManager.FindProperty("ShadowCastingMode");
			motionVectorGenerationMode = GrassManager.FindProperty("motionVectorGenerationMode");
			renderingLayerMask = GrassManager.FindProperty("renderingLayerMask");
			
			MinSize = GrassManager.FindProperty("MinSize");
			MaxSize = GrassManager.FindProperty("MaxSize");
			Noise = GrassManager.FindProperty("Noise");
			LayerToMergeWith = GrassManager.FindProperty("LayerToMergeWith");
			DoSoftMerge = GrassManager.FindProperty("DoSoftMerge");

			rtUseCompute = GrassManager.FindProperty("rtUseCompute");
			GrassInstanceSize = GrassManager.FindProperty("GrassInstanceSize");

            EnableTerrainShift = GrassManager.FindProperty("EnableTerrainShift");

        }
	}
}