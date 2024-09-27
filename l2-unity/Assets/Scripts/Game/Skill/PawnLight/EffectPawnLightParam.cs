using System;
using System.Text.RegularExpressions;
using UnityEditor.Search;
using UnityEngine;

[System.Serializable]
public class EffectPawnLightParam
{
    [SerializeField] private EPawnLightType _pawnLightType;
    [SerializeField] private ELightType _lightType;
    [SerializeField] private ELightEffect _lightEffectType;
    [SerializeField] private string _lightColor;
    [SerializeField] private float _lightRadius;
    [SerializeField] private ELightCoordSystem _lightCoordSystem;

    public EPawnLightType PawnLightType { get { return _pawnLightType; } set { _pawnLightType = value; } }
    public ELightType LightType { get { return _lightType; } set { _lightType = value; } }
    public ELightEffect LightEffectType { get { return _lightEffectType; } set { _lightEffectType = value; } }
    public string LightColor { get { return _lightColor; } set { _lightColor = value; } }
    public float LightRadius { get { return _lightRadius; } set { _lightRadius = value; } }
    public ELightCoordSystem LightCoordSystem { get { return _lightCoordSystem; } set { _lightCoordSystem = value; } }

    public static EffectPawnLightParam Parse(string input)
    {
        EffectPawnLightParam effect = new EffectPawnLightParam();
        input = input.Trim('(', ')');

        string pattern = @"(\w+)=(\([^)]+\)|[^,]+)"; // TODO: Use this regex in system grp files

        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            string key = match.Groups[1].Value;
            string value = match.Groups[2].Value;

            switch (key)
            {
                case "PawnLightType":
                    effect.PawnLightType = (EPawnLightType)Enum.Parse(typeof(EPawnLightType), value.ToUpper());
                    break;
                case "LightType":
                    effect.LightType = (ELightType)Enum.Parse(typeof(ELightType), value.ToUpper());
                    break;
                case "LightEffectType":
                    effect.LightEffectType = (ELightEffect)Enum.Parse(typeof(ELightEffect), value.ToUpper());
                    break;
                case "LightColor":
                    effect.LightColor = value;
                    break;
                case "LightRadius":
                    effect.LightRadius = float.Parse(value);
                    break;
                case "LightCoordSystem":
                    effect.LightCoordSystem = (ELightCoordSystem)Enum.Parse(typeof(ELightCoordSystem), value.ToUpper());
                    break;
            }
        }

        return effect;
    }

    //TODO: Handle skilleffect lights, not a priority for now...

    //"bPawnLight": "True","PawnLightParam": "(PawnLightType=EPLT_SKILL,LightType=LT_Steady,LightEffectType=LE_Cylinder,LightColor=(W=0.900000,X=0.500000,Y=0.200000,Z=0.200000),LightRadius=30.000000,LightCoordSystem=PTCS_RelativeRotation)"}],
}