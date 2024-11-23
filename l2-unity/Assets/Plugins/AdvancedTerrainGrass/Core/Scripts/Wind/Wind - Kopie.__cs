using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTerrainGrass {

	[System.Serializable]
    public enum RTSize {
        _256 = 256,
        _512 = 512,
        _1024 = 1024
    }

	[ExecuteInEditMode]
	[RequireComponent(typeof(WindZone))]
	public class Wind : MonoBehaviour {

		
		[Header("Render Texture Settings")]
		[Space(4)]
		public RTSize Resolution = RTSize._512;
		public Texture WindBaseTex;
		public Shader WindCompositeShader;
		
		[Header("Wind Multipliers")]
		[Space(4)]
		public float Grass = 1.0f;
		public float Foliage = 1.0f; 

		[Header("Size and Speed")]
		[Space(4)]
		[Range(0.001f, 0.1f)]
		public float size = 0.01f;
		[Range(0.0001f, 0.2f)]
		[Space(5)]
		public float speed = 0.02f;
		public float speedLayer0 = 0.476f;
		public float speedLayer1 = 1.23f;
		public float speedLayer2 = 2.93f;

		[Header("Noise")]
		[Space(4)]
		public int GrassGustTiling = 4;
		public float GrassGustSpeed = 0.278f;
		
		[Header("Jitter")]
		[Space(4)]
		public float JitterFrequency = 3.127f;
		public float JitterHighFrequency = 21.0f;

	//
		private RenderTexture WindRenderTexture;
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

		private int AtgWindDirSizePID;
		private int AtgWindStrengthMultipliersPID;
		private int AtgSinTimePID;
		private int AtgGustPID;

		private int AtgWindUVsPID;
		private int AtgWindUVs1PID;
		private int AtgWindUVs2PID;
		private int AtgWindUVs3PID;

		private Vector4 WindDirectionSize = Vector4.zero;


        private Vector3 lastPosition;

		void OnEnable () {
			if(WindCompositeShader == null) {
				WindCompositeShader = Shader.Find("WindComposite");
			}
			if (WindBaseTex == null ) {
				WindBaseTex = Resources.Load("Default wind base texture") as Texture;
			}
			SetupRT();
			GetPIDs();
			trans = this.transform;
			windZone = trans.GetComponent<WindZone>();

            lastPosition = trans.position;

        }

		void SetupRT () {
			if (WindRenderTexture == null || m_material == null)
	        {
	            WindRenderTexture = new RenderTexture((int)Resolution, (int)Resolution, 0, RenderTextureFormat.ARGBHalf, /*ARGB32, /*,*/ RenderTextureReadWrite.Linear );
	            WindRenderTexture.useMipMap = true;
	            WindRenderTexture.wrapMode = TextureWrapMode.Repeat;
	            m_material = new Material(WindCompositeShader);
	        }
		}

		void GetPIDs () {
			//WindRTPID = Shader.PropertyToID("_AtgWindRT");
			AtgWindDirSizePID = Shader.PropertyToID("_AtgWindDirSize");
			AtgWindStrengthMultipliersPID = Shader.PropertyToID("_AtgWindStrengthMultipliers");
			AtgSinTimePID = Shader.PropertyToID("_AtgSinTime");
			AtgGustPID = Shader.PropertyToID("_AtgGust");
			AtgWindUVsPID = Shader.PropertyToID("_AtgWindUVs");
			AtgWindUVs1PID = Shader.PropertyToID("_AtgWindUVs1");
			AtgWindUVs2PID = Shader.PropertyToID("_AtgWindUVs2");
			AtgWindUVs3PID = Shader.PropertyToID("_AtgWindUVs3");
		}

		void OnValidate () {
			if(WindCompositeShader == null) {
				WindCompositeShader = Shader.Find("WindComposite");
			}
			if (WindBaseTex == null ) {
				WindBaseTex = Resources.Load("Default wind base texture") as Texture;
			}
		}
		
		// Update is called once per frame
		void LateUpdate () {

		//	Get wind settings from WindZone
			mainWind = windZone.windMain;
			turbulence = windZone.windTurbulence;
			
			float delta = Time.deltaTime;
			WindDirectionSize.x = trans.forward.x;
			WindDirectionSize.y = trans.forward.y;
			WindDirectionSize.z = trans.forward.z;
			WindDirectionSize.w = size;

			var windVec = new Vector2(WindDirectionSize.x, WindDirectionSize.z ) * delta * speed;

            var deltaPosition = new Vector2(lastPosition.x - trans.position.x, lastPosition.z - trans.position.z);
            deltaPosition.x *= size;
            deltaPosition.y *= size;

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

            uvs3 -= windVec * GrassGustSpeed - deltaPosition*GrassGustTiling;
			uvs3.x = uvs3.x - (int)uvs3.x;
			uvs3.y = uvs3.y - (int)uvs3.y;

		//	Set global shader variables for grass and foliage shaders
			Shader.SetGlobalVector(AtgWindDirSizePID, WindDirectionSize);
			Vector2 tempWindstrengths;
			tempWindstrengths.x = Grass * mainWind;
			tempWindstrengths.y = Foliage * mainWind;
			Shader.SetGlobalVector(AtgWindStrengthMultipliersPID, tempWindstrengths );
			Shader.SetGlobalVector(AtgGustPID, new Vector2(GrassGustTiling, turbulence + 0.5f) );	
		//	Jitter frequncies and strength
			Shader.SetGlobalVector(AtgSinTimePID, new Vector4(
				Mathf.Sin(Time.time * JitterFrequency),
				Mathf.Sin(Time.time * JitterFrequency * 0.2317f + 2.0f * Mathf.PI),
				Mathf.Sin(Time.time * JitterHighFrequency),
				turbulence * 0.1f
			));

		//	Set UVs
			Shader.SetGlobalVector(AtgWindUVsPID, uvs);
			Shader.SetGlobalVector(AtgWindUVs1PID, uvs1);
			Shader.SetGlobalVector(AtgWindUVs2PID, uvs2);
			Shader.SetGlobalVector(AtgWindUVs3PID, uvs3);

			Graphics.Blit(WindBaseTex, WindRenderTexture, m_material);
			WindRenderTexture.SetGlobalShaderProperty("_AtgWindRT"); // only accepts strings...
		}
	}
}
