using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SunMoonCycle : MonoBehaviour
{
    [SerializeField] public Material skyboxMaterial;

    public Light mainLight;
    public DayNightCycle dayNightCycle;
    public float horizonOffsetDegree = 10f;
    public float sunMoonYRot = 45f;

    public Texture2D sunTexture;
    public Vector2 sunTiling = new Vector2(1.5f, 1.5f);
    public Texture2D moonTexture;
    public Vector2 moonTiling = new Vector2(2.5f, 2.5f);

    void Awake() {
        dayNightCycle = GetComponent<DayNightCycle>();
        mainLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {    
        float mainLightLerpValue = dayNightCycle.worldClock.dayRatio > 0 ? dayNightCycle.worldClock.dayRatio : dayNightCycle.worldClock.nightRatio;
        float sunRotation = Mathf.Lerp(0 - horizonOffsetDegree, 180 + horizonOffsetDegree, mainLightLerpValue);
        transform.eulerAngles = new Vector3(sunRotation, sunMoonYRot, 0);

        ShareMainLightRotation();

        UpdateLightIntensity(dayNightCycle.worldClock.dawnRatio, dayNightCycle.worldClock.duskRatio);


        Color dayColor = new Color(19f / 255f, 114f / 255f, 166f / 255f); // peak at sunriseEndTime
        Color dawnColor = new Color(19f / 255f, 35f / 255f, 55f / 255f); // peak at sunriseStartTime  
        Color duskColor = new Color(180f / 255f, 152f / 255f, 135f / 255f); // peak at sunsetStartTime
        Color nightColor = new Color(1f / 255f, 1f / 255f, 2f / 255f); // peak at sunsetEndTime
        float nightColorIntensity = -1f;
        nightColor = nightColor * nightColorIntensity;

        // Lerping sky color
        Color skyColor = skyboxMaterial.GetColor("_GradientColor1");
        if(dayNightCycle.worldClock.dawnRatio > 0 && dayNightCycle.worldClock.dawnRatio < 1) {
            skyColor = Color.Lerp(nightColor, dawnColor, dayNightCycle.worldClock.dawnRatio);
        }
        if(dayNightCycle.worldClock.brightRatio > 0) {
            if(dayNightCycle.worldClock.brightRatio < 0.2f) {
                skyColor = Color.Lerp(dawnColor, dayColor, dayNightCycle.worldClock.brightRatio / 0.2f);
            } else if(dayNightCycle.worldClock.brightRatio < 1f) {
                skyColor = dayColor;
            }
        }
        if(dayNightCycle.worldClock.darkRatio > 0) {
            if(dayNightCycle.worldClock.darkRatio < 0.1f) {
                skyColor = Color.Lerp(duskColor, nightColor, dayNightCycle.worldClock.darkRatio / 0.1f);
            } else if(dayNightCycle.worldClock.darkRatio < 1f) {
                skyColor = nightColor;
            }
        }
        if(dayNightCycle.worldClock.duskRatio > 0 && dayNightCycle.worldClock.duskRatio < 1) {
            skyColor = Color.Lerp(dayColor, duskColor, dayNightCycle.worldClock.duskRatio);
        }


        Color dayFogColor = new Color(240f / 255f, 240f / 255f, 240f / 255f);
        Color nightFogColor = new Color(152f / 255f, 152f / 255f, 152f / 255f);

        // Lerping fog color
        Color fogColor = RenderSettings.fogColor;
        if(dayNightCycle.worldClock.dawnRatio > 0 && dayNightCycle.worldClock.dawnRatio < 1) {
            fogColor = Color.Lerp(fogColor, dayFogColor, dayNightCycle.worldClock.brightRatio);
        }
        if(dayNightCycle.worldClock.brightRatio > 0 && dayNightCycle.worldClock.brightRatio < 1) {
            fogColor = dayFogColor;
        }
        if(dayNightCycle.worldClock.duskRatio > 0 && dayNightCycle.worldClock.duskRatio < 1) {
            fogColor = Color.Lerp(dayFogColor, nightFogColor, dayNightCycle.worldClock.darkRatio);
        }
        if(dayNightCycle.worldClock.darkRatio > 0 && dayNightCycle.worldClock.darkRatio < 1) {
            fogColor = nightFogColor;
        }

        // Lerping cloud opacity
        float dayCloudsOpcacity = 2.54f;
        float nightCloudsOpacity = 0.12f;
        float dayHorizonCloudsOpcacity = 1f;
        float nightHorizonCloudsOpacity = 0.05f;
        if(dayNightCycle.worldClock.dawnRatio > 0 && dayNightCycle.worldClock.dawnRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(nightCloudsOpacity, dayCloudsOpcacity, dayNightCycle.worldClock.dawnRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(nightHorizonCloudsOpacity, dayHorizonCloudsOpcacity, dayNightCycle.worldClock.dawnRatio));
        }
        if(dayNightCycle.worldClock.brightRatio > 0 && dayNightCycle.worldClock.brightRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", dayCloudsOpcacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", dayHorizonCloudsOpcacity);
        }
        if(dayNightCycle.worldClock.duskRatio > 0 && dayNightCycle.worldClock.duskRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(dayCloudsOpcacity, nightCloudsOpacity, dayNightCycle.worldClock.duskRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(dayHorizonCloudsOpcacity, nightHorizonCloudsOpacity, dayNightCycle.worldClock.duskRatio));
        }
        if(dayNightCycle.worldClock.darkRatio > 0 && dayNightCycle.worldClock.darkRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", nightCloudsOpacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", nightHorizonCloudsOpacity);
        }

        RenderSettings.fogColor = fogColor;
        skyboxMaterial.SetColor("_GradientColor1", skyColor);
        UpdateMainLightTexture();
   
    }

    private void UpdateLightIntensity(float dawnRatio, float duskRatio) {
        // Ambient light intensity
        float ambientMinIntensity = 0f;
        float ambientMaxIntensity = 1f;
        RenderSettings.ambientIntensity = AdjustIntensity(ambientMinIntensity, ambientMaxIntensity, dawnRatio, duskRatio);

        // Main light intensity
        float mainLightMinIntensity = 0.05f;
        float mainLightMaxIntensity = 1.5f;
        mainLight.intensity = AdjustIntensity(mainLightMinIntensity, mainLightMaxIntensity, dawnRatio, duskRatio);
    }


    private float AdjustIntensity(float minIntensity, float fullIntensity, float dawnRatio, float duskRatio) {
        if(duskRatio > 0) {
            return Mathf.Lerp(fullIntensity, minIntensity, duskRatio);
        } else {
            return Mathf.Lerp(minIntensity, fullIntensity, dawnRatio);
        }
    }

    private void UpdateMainLightTexture() {
        // Update texture 
        if(dayNightCycle.worldClock.dayRatio > 0) {
            skyboxMaterial.SetTexture("_SunMoon", sunTexture);
            skyboxMaterial.SetTextureScale("_SunMoon", sunTiling);
        } else {
            skyboxMaterial.SetTexture("_SunMoon", moonTexture);
            skyboxMaterial.SetTextureScale("_SunMoon", moonTiling);
        }
    }

    private void ShareMainLightRotation() {
        skyboxMaterial.SetVector("_MainLightForward", transform.forward);
        skyboxMaterial.SetVector("_MainLightUp", transform.up);
        skyboxMaterial.SetVector("_MainLightRight", transform.right);
    }
}
