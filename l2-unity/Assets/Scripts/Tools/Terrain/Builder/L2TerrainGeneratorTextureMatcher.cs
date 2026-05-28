using System.Collections.Generic;
using UnityEngine;
using static JBooth.MicroSplat.MicroSplatPropData;

public class L2TerrainGeneratorTextureMatcher
{
    public Dictionary<string, string> textureMatches;
    public Dictionary<string, float> scaleMatches;
    public Dictionary<string, List<PerTexFloatVal>> pertexFloatMatches;
    public Dictionary<string, List<PerTexColorVal>> pertextColorMatches;

    public struct PerTexFloatVal
    {
        public PerTexFloat ptf;
        public float value;
    }

    public struct PerTexColorVal
    {
        public PerTexColor ptf;
        public Color value;
    }


    private static L2TerrainGeneratorTextureMatcher _instance;
    public static L2TerrainGeneratorTextureMatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new L2TerrainGeneratorTextureMatcher();
            }

            return _instance;
        }
    }

    private L2TerrainGeneratorTextureMatcher()
    {
        textureMatches = new Dictionary<string, string>();
        textureMatches.Add("Base", "Swamp_Soil_tjmhfcjl_1K");
        textureMatches.Add("SL_G", "Wild_Grass_pjwgW0_1K");
        textureMatches.Add("SL_S3", "Grass_Patchy_pjvtA0_1K");
        // textureMatches.Add("SL_WR", ""); //L2 texture is good enough
        // textureMatches.Add("WR_02", ""); //L2 texture is good enough
        textureMatches.Add("SL_S6", "Soil_Sand_pjErQ0_1K");
        textureMatches.Add("SL_G3", "Wild_Grass_pjwgW0_1K");
        textureMatches.Add("SL_G2", "Wild_Grass_pjwgW0_1K");
        textureMatches.Add("SL_S1", "Thai_Beach_Sand_tefnah1q_1K");
        textureMatches.Add("SL_R1", "Rough_Soil_Detail_Texture_se4mcazf0_1K");
        textureMatches.Add("SL_C", "Icelandic_Jagged_Slate_Rock_shfsaida_1K");
        textureMatches.Add("SL_G4", "grass_and_rubble_pjwdt0_1k");
        textureMatches.Add("SL_C1", "grass_and_rubble_pjwdt0_1k");

        scaleMatches = new Dictionary<string, float>();
        scaleMatches.Add("Base", 3);
        scaleMatches.Add("SL_G", 5);
        scaleMatches.Add("SL_S3", 4);
        scaleMatches.Add("SL_WR", 3); //L2 texture is good enough
        scaleMatches.Add("WR_02", 3); //L2 texture is good enough
        scaleMatches.Add("SL_S6", 2);
        scaleMatches.Add("SL_G3", 5);
        scaleMatches.Add("SL_G2", 6);
        scaleMatches.Add("SL_S1", 1.2f);
        scaleMatches.Add("SL_R1", 1.25f);
        scaleMatches.Add("SL_C", 1);
        scaleMatches.Add("SL_G4", 7);
        scaleMatches.Add("SL_C1", 7);

        pertexFloatMatches = new Dictionary<string, List<PerTexFloatVal>>();
        pertexFloatMatches.Add("Base", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0.03f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1.37f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_G", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = .33f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 0.37f },
        });
        pertexFloatMatches.Add("SL_S3", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = -0.07f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0.137f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 0.31f },
        });
        pertexFloatMatches.Add("SL_WR", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("WR_02", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_S6", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0.01f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0.83f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 0.63f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_G3", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0.73f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 0.95f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_G2", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0.74f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1.1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_S1", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 0.394f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 0.2f },
        });
        pertexFloatMatches.Add("SL_R1", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0 },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1.1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_C", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0.46f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 0.83f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_G4", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1.1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });
        pertexFloatMatches.Add("SL_C1", new List<PerTexFloatVal>
        {
            new PerTexFloatVal() { ptf = PerTexFloat.Brightness, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.ColorIntensity, value = 0f },
            new PerTexFloatVal() { ptf = PerTexFloat.Contrast, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.Saturation, value = 1.1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightOffset, value = 1f },
            new PerTexFloatVal() { ptf = PerTexFloat.HeightContrast, value = 1f },
        });

        pertextColorMatches = new Dictionary<string, List<PerTexColorVal>>();

        pertextColorMatches.Add("Base", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_G", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_S3", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  new Color(238 / 255f, 238 / 255f, 238 / 255f) },
        });
        pertextColorMatches.Add("SL_WR", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_S6", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  new Color(253 / 255f, 242 / 255f, 242 / 255f) },
        });
        pertextColorMatches.Add("SL_G3", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_G2", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_S1", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  new Color(255 / 255f, 249 / 255f, 237 / 255f) },
        });
        pertextColorMatches.Add("SL_R1", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_C", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  new Color(154 / 255f, 130 / 255f, 101 / 255f) },
        });
        pertextColorMatches.Add("SL_G4", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
        pertextColorMatches.Add("SL_C1", new List<PerTexColorVal>
        {
            new PerTexColorVal() { ptf = PerTexColor.Tint, value =  Color.white },
        });
    }

}