using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace AdvancedTerrainGrass {

	[System.Serializable]
    public enum RTSize {
        _256 = 256,
        _384 = 384,
        _512 = 512,
        _1024 = 1024
    }

    [System.Serializable]
    public enum GustMixLayer {
    	Layer_0 = 0,
        Layer_1 = 1,
        Layer_2 = 2
    }

	[ExecuteAlways]
	[RequireComponent(typeof(WindZone))]
	public class Wind : MonoBehaviour {

		[Space(4)]
		public bool UpdateInEditMode = true;

		
		
		[Header("Wind Multipliers")]
		[Space(4)]
		public float Grass = 1.0f;
		public float Foliage = 1.0f;

		[Header("Wind Mapping")]
		[Space(4)]
		[Range(0.1f, 1.0f)]
		public float WindToFrequencyChange = 0.25f;
		[Range(0.0f, 4.0f)]
		public float MaxTurbulence = 0.5f;
		public AnimationCurve WindToTurbulence = new AnimationCurve(new Keyframe(0, 1), new Keyframe(5, 1));

		[Header("Speed")]
		[Space(4)]
		[Tooltip("Base Wind Speed in km/h at Main = 1 (WindZone)")]
        public float BaseWindSpeed = 15;
		
		[Space(4)]
		public float speedLayer0 = 1.0f;
		public float speedLayer1 = 1.137f;
		public float speedLayer2 = 1.376f;

		[Header("Noise")]
		[Space(4)]
		public int GrassGustTiling = 4;
		public float GrassGustSpeed = 0.278f;
		public GustMixLayer LayerToMixWith = GustMixLayer.Layer_1; 

		[Header("Render Texture Settings")]
		[Space(4)]
		public RTSize Resolution = RTSize._384;
		[Tooltip("Size of the Wind RenderTexture in World Space")]
		public float SizeInWorldSpace = 100.0f;
		[Space(4)]
		public Texture WindBaseTex;
		public Shader WindCompositeShader;

	//	Due to motion vectors we need current and previous wind texture
		private RenderTexture WindRenderTextureA;
		private RenderTexture WindRenderTextureB;
		private bool renderIntoA = true;
		
		private Material m_material;

		private Vector2 uvs = new Vector2(0,0);
		private Vector2 uvs1 = new Vector2(0,0);
		private Vector2 uvs2 = new Vector2(0,0);
		private Vector2 uvs3 = new Vector2(0,0);

		//private int WindRTPID;

		private Transform trans;
		private WindZone windZone;
		private float mainWind;
		private float turbulence;
		private float windpulseMagnitude;
		private float windPulseFrequency; 

		private static readonly int AtgWindDirSizePID = Shader.PropertyToID("_AtgWindDirSize");
		private static readonly int AtgWindStrengthMultipliersPID = Shader.PropertyToID("_AtgWindStrengthMultipliers");
		private static readonly int AtgSinTimePID = Shader.PropertyToID("_AtgSinTime");
		private static readonly int AtgGustPID = Shader.PropertyToID("_AtgGust");
		private static readonly int AtgGustMixLayerPID = Shader.PropertyToID("_AtgGustMixLayer");

		private static readonly int AtgBaseWindPID = Shader.PropertyToID("_AtgBaseWind");
		private static readonly int AtgBendFrequencyPID = Shader.PropertyToID("_AtgBendFrequency");
		private static readonly int AtgBendFrequencyLastFramePID = Shader.PropertyToID("_AtgBendFrequencyLastFrame");

		private static readonly int AtgWindUVsPID = Shader.PropertyToID("_AtgWindUVs");
		private static readonly int AtgWindUVs1PID = Shader.PropertyToID("_AtgWindUVs1");
		private static readonly int AtgWindUVs2PID = Shader.PropertyToID("_AtgWindUVs2");
		private static readonly int AtgWindUVs3PID = Shader.PropertyToID("_AtgWindUVs3");

		private static Vector3[] MixLayers = new [] { new Vector3(1f,0f,0f), new Vector3(0f,1f,0f), new Vector3(0f,0f,1f)  };

		private Vector4 WindDirectionSize = Vector4.zero;
		private float WindTurbulence;

        private Vector3 lastPosition;
        private double lastTimeStamp = 0.0f;

        private double domainTime_Wind = 0.0f;
		private double domainTime_Wind_LastFrame = 0.0f;
		private float temp_WindFrequency = 0.25f;
		private float freqSpeed = 0.0125f;					// Drives the duration of frequency changes

        private float BaseWind;

		void OnEnable () {
			if(WindCompositeShader == null) {
				WindCompositeShader = Shader.Find("WindComposite");
			}
			if (WindBaseTex == null ) {
				WindBaseTex = Resources.Load("Default wind base texture") as Texture;
			}
			SetupRT();
			trans = this.transform;
			windZone = trans.GetComponent<WindZone>();

            lastPosition = trans.position;

            #if UNITY_EDITOR
				EditorApplication.update += OnEditorUpdate;
			#endif

        }

        void OnDisable () {
			if (WindRenderTextureA != null) {
				WindRenderTextureA.Release();
				UnityEngine.Object.DestroyImmediate(WindRenderTextureA);
			}
			if (WindRenderTextureB != null) {
				WindRenderTextureB.Release();
				UnityEngine.Object.DestroyImmediate(WindRenderTextureB);
			}
			if (m_material != null) {
				UnityEngine.Object.DestroyImmediate(m_material);
				m_material = null;
			}
			#if UNITY_EDITOR
				EditorApplication.update -= OnEditorUpdate;
			#endif
		}
		#if UNITY_EDITOR
			void OnEditorUpdate() {
				if(!Application.isPlaying) { //}  && UpdateInEditMode) {
					Update();
				//	Unity 2019.1.10 on macOS using Metal also needs this
					//SceneView.RepaintAll(); 
				}
			}
		#endif

		void SetupRT () {
			var rtf = RenderTextureFormat.RGHalf;
			if (WindRenderTextureA == null)
	        {
	            WindRenderTextureA = new RenderTexture((int)Resolution, (int)Resolution, 0, rtf, /*ARGB32, /*,*/ RenderTextureReadWrite.Linear );
	            WindRenderTextureA.useMipMap = true;
	            WindRenderTextureA.wrapMode = TextureWrapMode.Repeat;
	        }
	        if (WindRenderTextureB == null)
	        {
	            WindRenderTextureB = new RenderTexture((int)Resolution, (int)Resolution, 0, rtf, /*ARGB32, /*,*/ RenderTextureReadWrite.Linear );
	            WindRenderTextureB.useMipMap = true;
	            WindRenderTextureB.wrapMode = TextureWrapMode.Repeat;	
	        }
	        if (m_material == null)
	        {
	        	m_material = new Material(WindCompositeShader);
	        }
		}

		void OnValidate () {
			if(WindCompositeShader == null) {
				WindCompositeShader = Shader.Find("WindComposite");
			}
			if (WindBaseTex == null ) {
				WindBaseTex = Resources.Load("Default wind base texture") as Texture;
			}
		}

		void Update () {

		#if UNITY_EDITOR
			if (UnityEditor.BuildPipeline.isBuildingPlayer)
				return;
		#endif

			domainTime_Wind_LastFrame = domainTime_Wind;

		//	Get wind settings from WindZone
			mainWind = windZone.windMain;
			turbulence = windZone.windTurbulence;
			windpulseMagnitude = windZone.windPulseMagnitude;
			windPulseFrequency = windZone.windPulseFrequency;
			
			float delta = Time.deltaTime;
			#if UNITY_EDITOR
				if(!Application.isPlaying) {
					//delta = (float)(EditorApplication.timeSinceStartup - lastTimeStamp);
					//lastTimeStamp = EditorApplication.timeSinceStartup;
					delta = (float)(Time.realtimeSinceStartup - lastTimeStamp);
					lastTimeStamp = Time.realtimeSinceStartup;
				}
			#endif

		//	Animated BaseWind as from the wind zone. This is constant over the entire terrain :(
			BaseWind = mainWind + windpulseMagnitude * (1.0f + Mathf.Sin(Time.time * windPulseFrequency) + 1.0f + Mathf.Sin(Time.time * windPulseFrequency * 3.0f) ) * 0.5f;
			
		//	Update the custom time
			temp_WindFrequency = Mathf.MoveTowards(temp_WindFrequency, mainWind * WindToFrequencyChange, freqSpeed);
			domainTime_Wind += delta * (1.0f + temp_WindFrequency);
			// var domainSinTime_Wind = (float)Math.Sin(domainTime_Wind * 0.5);
			// var domainShiftedSinTime_Wind = (float)(domainSinTime_Wind + domainTime_Wind) * 0.5f;
			// Shader.SetGlobalVector(BendFrequencyPID, new Vector4(
			// 	(float)domainTime_Wind,
			// 	domainSinTime_Wind,
			// 	domainShiftedSinTime_Wind,
			// 	0
			// 	)
			// );
			Shader.SetGlobalFloat(AtgBendFrequencyPID, (float)domainTime_Wind);
			Shader.SetGlobalFloat(AtgBendFrequencyLastFramePID, (float)domainTime_Wind_LastFrame);


		//	Wind Texture
			var SizeInUvSpace = 1.0f / SizeInWorldSpace;	

			WindDirectionSize.x = trans.forward.x;
			WindDirectionSize.y = trans.forward.y;
			WindDirectionSize.z = trans.forward.z;
			WindDirectionSize.w = SizeInUvSpace;

			//var windVec = new Vector2(WindDirectionSize.x, WindDirectionSize.z ) * delta * speed;
			//var windVec = new Vector2(WindDirectionSize.x, WindDirectionSize.z ) * delta /* mainWind */ * WindToGrassspeed.Evaluate(mainWind) * (BaseWindSpeed * 0.2777f * SizeInUvSpace);
var windVec = new Vector2(WindDirectionSize.x, WindDirectionSize.z ) * delta * mainWind * (BaseWindSpeed * 0.2777f * SizeInUvSpace);

            var deltaPosition = new Vector2(lastPosition.x - trans.position.x, lastPosition.z - trans.position.z);
            deltaPosition.x *= SizeInUvSpace;
            deltaPosition.y *= SizeInUvSpace;

            lastPosition = trans.position;

            uvs -= windVec * speedLayer0 - deltaPosition;
			uvs.x = uvs.x - (int)uvs.x;
			uvs.y = uvs.y - (int)uvs.y;

            uvs1 -= windVec * speedLayer1 - deltaPosition;
			uvs1.x = uvs1.x - (int)uvs1.x;
			uvs1.y = uvs1.y - (int)uvs1.y;

            uvs2 -= windVec * speedLayer2 - deltaPosition;
			uvs2.x = uvs2.x - (int)uvs2.x;
			uvs2.y = uvs2.y - (int)uvs2.y;

            uvs3 -= windVec * GrassGustSpeed - deltaPosition * GrassGustTiling;
			uvs3.x = uvs3.x - (int)uvs3.x;
			uvs3.y = uvs3.y - (int)uvs3.y;

		//	Set global shader variables for grass and foliage shaders
			Shader.SetGlobalVector(AtgWindDirSizePID, WindDirectionSize);
			Vector2 tempWindstrengths;
			tempWindstrengths.x = Grass * mainWind;
			tempWindstrengths.y = Foliage * mainWind;
			Shader.SetGlobalVector(AtgWindStrengthMultipliersPID, tempWindstrengths );

			WindTurbulence = MaxTurbulence * WindToTurbulence.Evaluate(mainWind);
			//Shader.SetGlobalVector(AtgGustPID, new Vector2(GrassGustTiling, turbulence + 0.5f) );
			Shader.SetGlobalVector(AtgGustPID, new Vector2(GrassGustTiling, WindTurbulence) );	
		//	Set Mix Layer
			Shader.SetGlobalVector(AtgGustMixLayerPID, MixLayers[(int)LayerToMixWith]);
		
			Shader.SetGlobalFloat(AtgBaseWindPID, mainWind);	

		//	Set UVs
			Shader.SetGlobalVector(AtgWindUVsPID, uvs);
			Shader.SetGlobalVector(AtgWindUVs1PID, uvs1);
			Shader.SetGlobalVector(AtgWindUVs2PID, uvs2);
			Shader.SetGlobalVector(AtgWindUVs3PID, uvs3);

		#if UNITY_EDITOR
			if (m_material != null && WindRenderTextureA != null && WindRenderTextureB != null) {
				if (UpdateInEditMode || Application.isPlaying) {
		#endif
					if(renderIntoA)
					{
						Graphics.Blit(WindBaseTex, WindRenderTextureA, m_material);
						WindRenderTextureA.SetGlobalShaderProperty("_AtgWindRT"); // only accepts strings...	
						WindRenderTextureB.SetGlobalShaderProperty("_AtgWindRTPrevious");
					}
					else
					{
						Graphics.Blit(WindBaseTex, WindRenderTextureB, m_material);
						WindRenderTextureB.SetGlobalShaderProperty("_AtgWindRT"); // only accepts strings...
						WindRenderTextureA.SetGlobalShaderProperty("_AtgWindRTPrevious");
					}
					renderIntoA = !renderIntoA;
					
		#if UNITY_EDITOR
				}
			}
		#endif
		}
	}
}
