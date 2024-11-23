using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTerrainGrass {

	public class SwitchGrassSettings : MonoBehaviour {

		public List <GrassManager> GrassManagers;
		public int ActiveGrassManagers = 0;
		public Terrain[] ters;

		// Use this for initialization
		void Start () {
			ters = Terrain.activeTerrains;
			int NumberOfTerrains = ters.Length;
			for (int i = 0; i < NumberOfTerrains; i ++) {
				var tempGrassManager = ters[i].GetComponent<GrassManager>();
				if (tempGrassManager != null) {
					GrassManagers.Add( tempGrassManager );
					ActiveGrassManagers ++;
				}
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		//	Restore original settings
			if (Input.GetKey (KeyCode.Alpha1) ) {
				for (int i = 0; i < ActiveGrassManagers; i++) {
	                GrassManagers[i].RefreshGrassRenderingSettings(
	                    GrassManagers[i].DetailDensity,           //  float t_DetailDensity,
	                    GrassManagers[i].CullDistance,            //  float t_CullDistance,
	                    GrassManagers[i].FadeLength,              //  float t_FadeLength,
	                    GrassManagers[i].CacheDistance,           //  float t_CacheDistance,
	                    GrassManagers[i].DetailFadeStart,         //  float t_DetailFadeStart,
	                    GrassManagers[i].DetailFadeLength,        //  float t_DetailFadeLength,
	                    GrassManagers[i].ShadowStart,             //  float t_ShadowStart,
	                    GrassManagers[i].ShadowFadeLength,        //  float t_ShadowFadeLength,
	                    GrassManagers[i].ShadowStartFoliage,      //  float t_ShadowStartFoliage,
	                    GrassManagers[i].ShadowStartFoliage       //  float t_ShadowFadeLengthFoliage
	                );
            	}
        	}
                        
        //	Set 
            if (Input.GetKey (KeyCode.Alpha2) ) {
            	for (int i = 0; i < ActiveGrassManagers; i++) {
	                GrassManagers[i].RefreshGrassRenderingSettings(
	                    2,              //  float t_DetailDensity,
	                    60,             //  float t_CullDistance,
	                    20,             //  float t_FadeLength,
	                    80,             //  float t_CacheDistance,
	                    10,             //  float t_DetailFadeStart,
	                    15,             //  float t_DetailFadeLength,
	                    10,             //  float t_ShadowStart,
	                    15,             //  float t_ShadowFadeLength,
	                    15,             //  float t_ShadowStartFoliage,
	                    20              //  float t_ShadowFadeLengthFoliage
	                );
	            }
            }

/*
            if (Input.GetKey (KeyCode.Alpha3) ) {

                RefreshGrassRenderingSettings(
                    0.5f,                      //  float t_DetailDensity,
                    CullDistance,            //  float t_CullDistance,
                    FadeLength,              //  float t_FadeLength,
                    CacheDistance,           //  float t_CacheDistance,
                    DetailFadeStart,         //  float t_DetailFadeStart,
                    DetailFadeLength,        //  float t_DetailFadeLength,
                    ShadowStart,             //  float t_ShadowStart,
                    ShadowFadeLength,        //  float t_ShadowFadeLength,
                    ShadowStartFoliage,      //  float t_ShadowStartFoliage,
                    ShadowStartFoliage       //  float t_ShadowFadeLengthFoliage
                );
            }
            */
		}
	}
}