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

    public float dawnRatio;
    public float dayRatio;
    public float duskRatio; 
    public float nightRatio;


    void Awake() {
        dayNightCycle = GetComponent<DayNightCycle>();
        mainLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {    
        float mainLightLerp = dayNightCycle.dayRatio > 0 ? dayNightCycle.dayRatio : dayNightCycle.nightRatio;
        float sunRotation = Mathf.Lerp(0 - horizonOffsetDegree, 180 + horizonOffsetDegree, mainLightLerp);
        transform.eulerAngles = new Vector3(sunRotation, sunMoonYRot, 0);

        ShareMainLightRotation();

        // Define time when dawn end and dusk start
        float dawnStartRatio = -0.10f; // night
        float dawnEndRatio = 0.15f; // day
        float duskStartRatio = 0.85f; // day
        float duskEndRatio = 0.99f; // day

        dawnRatio = CalculatePeriodRatio(dawnStartRatio, dawnEndRatio);
        dayRatio = CalculatePeriodRatio(dawnEndRatio, duskStartRatio);
        duskRatio = CalculatePeriodRatio(duskStartRatio, duskEndRatio);
        nightRatio = CalculatePeriodRatio(-.99f, dawnStartRatio);


        // Ambient light intensity
        float ambientMinIntensity = 0f;
        float ambientFullIntensity = 1f;
        RenderSettings.ambientIntensity = AdjustIntensity(ambientMinIntensity, ambientFullIntensity, dawnRatio, duskRatio);

        // Main light intensity
        float mainLightMinIntensity = 0.05f;
        float mainLightFullIntensity = 1.5f;
        mainLight.intensity = AdjustIntensity(mainLightMinIntensity, mainLightFullIntensity, dawnRatio, duskRatio);


        Color dayColor = new Color(19f / 255f, 114f / 255f, 166f / 255f); // peak at dawnEndRatio
        Color dawnColor = new Color(19f / 255f, 35f / 255f, 55f / 255f); // peak at dawnStartRatio  
        Color duskColor = new Color(180f / 255f, 152f / 255f, 135f / 255f); // peak at duskStartRatio
        Color nightColor = new Color(1f / 255f, 1f / 255f, 2f / 255f); // peak at duskEndRatio
        float nightColorIntensity = -1f;
        nightColor = nightColor * nightColorIntensity;

        // Lerping sky color
        Color skyColor = skyboxMaterial.GetColor("_GradientColor1");
        if(dawnRatio > 0 && dawnRatio < 1) {
            skyColor = Color.Lerp(nightColor, dawnColor, dawnRatio);
        }
        if(dayRatio > 0) {
            if(dayRatio < 0.2f) {
                skyColor = Color.Lerp(dawnColor, dayColor, dayRatio / 0.2f);
            } else if(dayRatio < 1f) {
                skyColor = dayColor;
            }
        }
        if(nightRatio > 0) {
            if(nightRatio < 0.1f) {
                skyColor = Color.Lerp(duskColor, nightColor, nightRatio / 0.1f);
            } else if(nightRatio < 1f) {
                skyColor = nightColor;
            }
        }
        if(duskRatio > 0 && duskRatio < 1) {
            skyColor = Color.Lerp(dayColor, duskColor, duskRatio);
        }


        Color dayFogColor = new Color(240f / 255f, 240f / 255f, 240f / 255f);
        Color nightFogColor = new Color(152f / 255f, 152f / 255f, 152f / 255f);

        // Lerping fog color
        Color fogColor = RenderSettings.fogColor;
        if(dawnRatio > 0 && dawnRatio < 1) {
            fogColor = Color.Lerp(fogColor, dayFogColor, dayRatio);
        }
        if(dayRatio > 0 && dayRatio < 1) {
            fogColor = dayFogColor;
        }
        if(duskRatio > 0 && duskRatio < 1) {
            fogColor = Color.Lerp(dayFogColor, nightFogColor, nightRatio);
        }
        if(nightRatio > 0 && nightRatio < 1) {
            fogColor = nightFogColor;
        }

        // Lerping cloud opacity
        float cloudsOpacity = skyboxMaterial.GetFloat("_Clouds_Opacity");
        float horizonCloudsOpacity = skyboxMaterial.GetFloat("_Horizon_Clouds_Opacity");
        float dayCloudsOpcacity = 2.54f;
        float nightCloudsOpacity = 0.12f;
        float dayHorizonCloudsOpcacity = 1f;
        float nightHorizonCloudsOpacity = 0.05f;
        if(dawnRatio > 0 && dawnRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(nightCloudsOpacity, dayCloudsOpcacity, dawnRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(nightHorizonCloudsOpacity, dayHorizonCloudsOpcacity, dawnRatio));
        }
        if(dayRatio > 0 && dayRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", dayCloudsOpcacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", dayHorizonCloudsOpcacity);
        }
        if(duskRatio > 0 && duskRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", Mathf.Lerp(dayCloudsOpcacity, nightCloudsOpacity, duskRatio));
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", Mathf.Lerp(dayHorizonCloudsOpcacity, nightHorizonCloudsOpacity, duskRatio));
        }
        if(nightRatio > 0 && nightRatio < 1) {
            skyboxMaterial.SetFloat("_Clouds_Opacity", nightCloudsOpacity);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", nightHorizonCloudsOpacity);
        }

   
        RenderSettings.fogColor = fogColor;
        skyboxMaterial.SetColor("_GradientColor1", skyColor);
        UpdateMainLightTexture();
   
    }

    private float CalculatePeriodRatio(float startRatio, float endRatio) {
        float periodDuration = (startRatio < 0) ? (Mathf.Abs(startRatio) + endRatio) : (endRatio - startRatio);

        float ratio = 0;
        if(dayNightCycle.dayRatio >= 0 && dayNightCycle.nightRatio == 0) {
            // Day
            if(dayNightCycle.dayRatio <= endRatio) {
                // In Range        
                if(startRatio < 0) {
                    ratio += Mathf.Abs(startRatio);
                    ratio += dayNightCycle.dayRatio;
                } else if(dayNightCycle.dayRatio >= startRatio) {
                    ratio -= startRatio;
                    ratio += dayNightCycle.dayRatio;
                } else {
                    ratio = 0;
                }

                ratio = Mathf.Clamp(ratio / periodDuration, 0, 1);
            } else {
                ratio = 1;
            }
        } else {
            // Night
            if(startRatio < 0 && dayNightCycle.nightRatio > (1 + startRatio)) {
                ratio += Mathf.Clamp((dayNightCycle.nightRatio - (1 + startRatio)) / periodDuration, 0, 1);
            } else {
                ratio = 0;
            }
        }

        return ratio;
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
        if(dayNightCycle.dayRatio > 0) {
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
