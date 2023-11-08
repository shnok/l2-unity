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

        Debug.Log(dayNightCycle.dayPercent);

        float mainLightLerp = dayNightCycle.dayRatio > 0 ? dayNightCycle.dayRatio : dayNightCycle.nightRatio;
        float sunRotation = Mathf.Lerp(0 - horizonOffsetDegree, 180 + horizonOffsetDegree, mainLightLerp);
        transform.eulerAngles = new Vector3(sunRotation, sunMoonYRot, transform.eulerAngles.z);

        ShareMainLightRotation();

        // Ambient light
        float ambientMinIntensity = 0f;
        float ambientFullIntensity = 1f;
        if(dayNightCycle.dayRatio > 0) {
            if(dayNightCycle.dayRatio < 0.5f) {
                RenderSettings.ambientIntensity = Mathf.Lerp(ambientMinIntensity, ambientFullIntensity, dayNightCycle.dayRatio / 0.5f);
            } else {
                RenderSettings.ambientIntensity = Mathf.Lerp(ambientFullIntensity, ambientMinIntensity, (dayNightCycle.dayRatio - 0.5f) / 0.5f);
            }
        } else {
            RenderSettings.ambientIntensity = ambientMinIntensity;
        }

        // Main light
        float mainLightMinIntensity = 0.05f;
        float mainLightFullIntensity = 1.5f;
        if(dayNightCycle.dayRatio > 0) {
            if(dayNightCycle.dayRatio < 0.5f) {
                mainLight.intensity = Mathf.Lerp(mainLightMinIntensity, mainLightFullIntensity, dayNightCycle.dayRatio / 0.5f);
            } else {
                mainLight.intensity = Mathf.Lerp(mainLightFullIntensity, mainLightMinIntensity, (dayNightCycle.dayRatio - 0.5f) / 0.5f);
            }
        } else {
            mainLight.intensity = mainLightMinIntensity;
        }

        // Update texture 
        if(dayNightCycle.dayRatio > 0) {
            skyboxMaterial.SetTexture("_SunMoon", sunTexture);
            skyboxMaterial.SetTextureScale("_SunMoon", sunTiling);
        } else {
            skyboxMaterial.SetTexture("_SunMoon", moonTexture);
            skyboxMaterial.SetTextureScale("_SunMoon", moonTiling);
        }

        // TO DO UPDATE SKY COLOR GRADIENT BASED ON TIME
        Color dayColor = new Color(1f / 255f, 116f / 255f, 178f / 255f);
        
        Color nightColor = new Color(1f / 255f, 1f / 255f, 2f / 255f);
        float nightColorIntensity = -1f;
        nightColor = nightColor * nightColorIntensity;

        if(dayNightCycle.dayRatio > 0) {
            skyboxMaterial.SetColor("_GradientColor1", dayColor);
            skyboxMaterial.SetFloat("_CloudsOpacity", 2.54f);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", 1f);
        } else {
            skyboxMaterial.SetColor("_GradientColor1", nightColor);
            skyboxMaterial.SetFloat("_CloudsOpacity", 0.12f);
            skyboxMaterial.SetFloat("_Horizon_Clouds_Opacity", 0.05f);
        }

    }



    private void ShareMainLightRotation() {
        skyboxMaterial.SetVector("_MainLightForward", transform.forward);
        skyboxMaterial.SetVector("_MainLightUp", transform.up);
        skyboxMaterial.SetVector("_MainLightRight", transform.right);
    }
}
