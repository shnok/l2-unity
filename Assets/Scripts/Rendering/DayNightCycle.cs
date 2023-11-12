using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SunMoonCycle : MonoBehaviour
{
    [SerializeField] public Material skyboxMaterial;

    public Light mainLight;
    public WorldClock clock;
    public float horizonOffsetDegree = 10f;
    public float mainLightRotY = 45f;

    public Texture2D sunTexture;
    public Vector2 sunTiling = new Vector2(1.5f, 1.5f);
    public Texture2D moonTexture;
    public Vector2 moonTiling = new Vector2(2.5f, 2.5f);

    public Color dayColor = new Color(19f / 255f, 114f / 255f, 166f / 255f); // peak at sunriseEndTime
    public Color dawnColor = new Color(19f / 255f, 35f / 255f, 55f / 255f); // peak at sunriseStartTime  
    public Color duskColor = new Color(180f / 255f, 152f / 255f, 135f / 255f); // peak at sunsetStartTime
    public Color nightColor = new Color(1f / 255f, 1f / 255f, 2f / 255f) * -1f; // peak at sunsetEndTime
    public Color dayFogColor = new Color(240f / 255f, 240f / 255f, 240f / 255f);
    public Color nightFogColor = new Color(152f / 255f, 152f / 255f, 152f / 255f);

    public Color mainLightDayColor = new Color(255f / 255f, 240f / 255f, 225f / 255f);
    public Color mainLightNightColor = new Color(255f / 255f, 240f / 255f, 225f / 255f);
    public Color mainLightduskColor = new Color(255f / 255f, 206f / 255f, 158f / 255f);
    public Color mainLightDawnColor = new Color(255f / 255f, 206f / 255f, 158f / 255f);

    public float dayCloudsOpcacity = 2.54f;
    public float nightCloudsOpacity = 0.12f;
    public float dayHorizonCloudsOpcacity = 1f;
    public float nightHorizonCloudsOpacity = 0.05f;

    public float ambientMinIntensity = 0f;
    public float ambientMaxIntensity = 1f;

    public float mainLightMinIntensity = 0.05f;
    public float mainLightMaxIntensity = 1.5f;

    void Awake() {
        clock = GetComponent<WorldClock>();
        mainLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {    
        float mainLightLerpValue = clock.worldClock.dayRatio > 0 ? clock.worldClock.dayRatio : clock.worldClock.nightRatio;
        float sunRotation = Mathf.Lerp(0 - horizonOffsetDegree, 180 + horizonOffsetDegree, mainLightLerpValue);
        transform.eulerAngles = new Vector3(sunRotation, mainLightRotY, 0);

        // Update main light rotation with sky material
        ShareMainLightRotation();

        // Lerping lights
        UpdateLightIntensity();

        // Lerping sky color
        UpdateSkyColor();

        // Lerping fog color
        UpdateFogColor();

        // Lerping cloud opacity
        UpdateCloudsOpacity();

        // Update main light texture
        UpdateMainLightTexture();

        UpdateLightColor();
    }

    private void UpdateSkyColor() {
        Color skyColor = skyboxMaterial.GetColor("_GradientColor1");
        if(clock.worldClock.dawnRatio > 0 && clock.worldClock.dawnRatio < 1) {
            skyColor = Color.Lerp(nightColor, dawnColor, clock.worldClock.dawnRatio);
        }
        if(clock.worldClock.brightRatio > 0) {
            if(clock.worldClock.brightRatio < 0.2f) {
                skyColor = Color.Lerp(dawnColor, dayColor, clock.worldClock.brightRatio / 0.2f);
            } else if(clock.worldClock.brightRatio < 1f) {
                skyColor = dayColor;
            }
        }
        if(clock.worldClock.darkRatio > 0) {
            if(clock.worldClock.darkRatio < 0.1f) {
                skyColor = Color.Lerp(duskColor, nightColor, clock.worldClock.darkRatio / 0.1f);
            } else if(clock.worldClock.darkRatio < 1f) {
                skyColor = nightColor;
            }
        }
        if(clock.worldClock.duskRatio > 0 && clock.worldClock.duskRatio < 1) {
            skyColor = Color.Lerp(dayColor, duskColor, clock.worldClock.duskRatio);
        }

        skyboxMaterial.SetColor("_GradientColor1", skyColor);
    }

    private void UpdateLightColor() {
        Color lightColor = mainLight.color;
        if(clock.worldClock.dawnRatio > 0 && clock.worldClock.dawnRatio < 1) {
            lightColor = Color.Lerp(mainLightNightColor, mainLightDawnColor, clock.worldClock.dawnRatio);
        }
        if(clock.worldClock.brightRatio > 0) {
            if(clock.worldClock.brightRatio < 0.2f) {
                lightColor = Color.Lerp(mainLightDawnColor, mainLightDayColor, clock.worldClock.brightRatio / 0.2f);
            } else if(clock.worldClock.brightRatio < 1f) {
                lightColor = mainLightDayColor;
            }
        }
        if(clock.worldClock.darkRatio > 0) {
            if(clock.worldClock.darkRatio < 0.1f) {
                lightColor = Color.Lerp(mainLightduskColor, mainLightNightColor, clock.worldClock.darkRatio / 0.1f);
            } else if(clock.worldClock.darkRatio < 1f) {
                lightColor = mainLightNightColor;
            }
        }
        if(clock.worldClock.duskRatio > 0 && clock.worldClock.duskRatio < 1) {
            lightColor = Color.Lerp(mainLightDayColor, mainLightduskColor, clock.worldClock.duskRatio);
        }
        mainLight.color = lightColor;
    }

    private void UpdateFogColor() {
        Color fogColor = RenderSettings.fogColor;
        if(clock.worldClock.dawnRatio > 0 && clock.worldClock.dawnRatio < 1) {
            fogColor = Color.Lerp(fogColor, dayFogColor, clock.worldClock.dawnRatio);
        }
        if(clock.worldClock.brightRatio > 0 && clock.worldClock.brightRatio < 1) {
            fogColor = dayFogColor;
        }
        if(clock.worldClock.duskRatio > 0 && clock.worldClock.duskRatio < 1) {
            fogColor = Color.Lerp(dayFogColor, nightFogColor, clock.worldClock.duskRatio);
        }
        if(clock.worldClock.darkRatio > 0 && clock.worldClock.darkRatio < 1) {
            fogColor = nightFogColor;
        }
        RenderSettings.fogColor = fogColor;
    }

    private void UpdateLightIntensity() {
        // Ambient light intensity
        RenderSettings.ambientIntensity = AdjustIntensity(ambientMinIntensity, ambientMaxIntensity, clock.worldClock.dawnRatio, clock.worldClock.duskRatio); ;

        // Main light intensity
        mainLight.intensity = AdjustIntensity(mainLightMinIntensity, mainLightMaxIntensity, clock.worldClock.dawnRatio, clock.worldClock.duskRatio);
    }


    private float AdjustIntensity(float minIntensity, float fullIntensity, float dawnRatio, float duskRatio) {
        if(duskRatio > 0) {
            return Mathf.Lerp(fullIntensity, minIntensity, duskRatio);
        } else {
            return Mathf.Lerp(minIntensity, fullIntensity, dawnRatio);
        }
    }

    private void UpdateCloudsOpacity() {
        if(clock.worldClock.dawnRatio > 0 && clock.worldClock.dawnRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(nightCloudsOpacity, dayCloudsOpcacity, clock.worldClock.dawnRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(nightHorizonCloudsOpacity, dayHorizonCloudsOpcacity, clock.worldClock.dawnRatio));
        }
        if(clock.worldClock.brightRatio > 0 && clock.worldClock.brightRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", dayCloudsOpcacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", dayHorizonCloudsOpcacity);
        }
        if(clock.worldClock.duskRatio > 0 && clock.worldClock.duskRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(dayCloudsOpcacity, nightCloudsOpacity, clock.worldClock.duskRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(dayHorizonCloudsOpcacity, nightHorizonCloudsOpacity, clock.worldClock.duskRatio));
        }
        if(clock.worldClock.darkRatio > 0 && clock.worldClock.darkRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", nightCloudsOpacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", nightHorizonCloudsOpacity);
        }
    }

    private void UpdateMainLightTexture() {
        // Update texture 
        if(clock.worldClock.dayRatio > 0) {
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
