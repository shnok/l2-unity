using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class L2SkillEffectEmitterConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(EffectEmitter);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        EffectEmitter emitter = new EffectEmitter
        {
            AttachOn = ParseAttachOnType(jObject["AttachOn"]?.ToString()),
            SpawnOnTarget = jObject["bSpawnOnTarget"]?.ToString().ToLower() == "true",
            RelativeToCylinder = jObject["bRelativeToCylinder"]?.ToString().ToLower() == "true",
            EffectClass = jObject["EffectClass"]?.ToString(),
            ScaleSize = jObject["ScaleSize"]?.Value<float>() ?? 1.0f,
            Offset = L2JsonConverterCommonFunctions.ParseVector3(jObject["Offset"]?.ToString()),
            EtcEffect = ParseEtcEffect(jObject["EtcEffect"]?.ToString()),
            EtcEffectInfo = ParseEtcEffectInfo(jObject["EtcEffectInfo"]?.ToString()),
            PawnLight = jObject["bPawnLight"]?.ToString().ToLower() == "true",
        };

        if (jObject["PawnLightParam"] != null)
        {
            emitter.PawnLightParam = EffectPawnLightParam.Parse(jObject["PawnLightParam"].ToString());
        }


        return emitter;
    }

    private AttachOnType ParseAttachOnType(string attachOn)
    {
        if (attachOn == null)
        {
            return AttachOnType.AM_NONE;
        }

        return (AttachOnType)Enum.Parse(typeof(AttachOnType), attachOn.ToUpper());

        // switch (attachOn)
        // {
        //     case "AM_None":
        //         return AttachOnType.AM_None;
        //     case "AM_Location":
        //         return AttachOnType.AM_Location;
        //     case "AM_RH":
        //         return AttachOnType.AM_RH;
        //     case "AM_LH":
        //         return AttachOnType.AM_LH;
        //     case "AM_RA":
        //         return AttachOnType.AM_RA;
        //     case "AM_LA":
        //         return AttachOnType.AM_LA;
        //     case "AM_WING":
        //         return AttachOnType.AM_WING;
        //     case "AM_BoneSpecified":
        //         return AttachOnType.AM_BoneSpecified;
        //     case "AM_AliasSpecified":
        //         return AttachOnType.AM_AliasSpecified;
        //     case "AM_Trail":
        //         return AttachOnType.AM_Trail;
        //     default:
        //         return AttachOnType.AM_None;
        // }
    }


    private EtcEffect ParseEtcEffect(string etcEffect)
    {
        if (etcEffect == null)
        {
            return EtcEffect.EET_NONE;
        }

        return (EtcEffect)Enum.Parse(typeof(EtcEffect), etcEffect.ToUpper());

        // switch (etcEffect)
        // {
        //     case "EET_None":
        //         return EtcEffect.EET_None;
        //     case "EET_FireCracker":
        //         return EtcEffect.EET_FireCracker;
        //     case "EET_Soulshot":
        //         return EtcEffect.EET_Soulshot;
        //     case "EET_SpiritShot":
        //         return EtcEffect.EET_SpiritShot;
        //     case "EET_Cubic":
        //         return EtcEffect.EET_Cubic;
        //     case "EET_SoundCrystal":
        //         return EtcEffect.EET_SoundCrystal;
        //     case "EET_JewelShot":
        //         return EtcEffect.EET_JewelShot;
        //     case "EET_PetJewelShot":
        //         return EtcEffect.EET_PetJewelShot;
        //     default:
        //         return EtcEffect.EET_None;
        // }
    }

    private EtcEffectInfo ParseEtcEffectInfo(string etcEffectInfo)
    {
        if (etcEffectInfo == null)
        {
            return EtcEffectInfo.EEP_NONE;
        }

        return (EtcEffectInfo)Enum.Parse(typeof(EtcEffectInfo), etcEffectInfo.ToUpper());

        // switch (etcEffectInfo)
        // {
        //     case "EEP_None":
        //         return EtcEffectInfo.EEP_None;
        //     case "EEP_FireCrackerSmall":
        //         return EtcEffectInfo.EEP_FireCrackerSmall;
        //     case "EEP_FireCrackerMiddle":
        //         return EtcEffectInfo.EEP_FireCrackerMiddle;
        //     case "EEP_FireCrackerLarge":
        //         return EtcEffectInfo.EEP_FireCrackerLarge;
        //     case "EEP_GradeNone":
        //         return EtcEffectInfo.EEP_GradeNone;
        //     case "EEP_GradeD":
        //         return EtcEffectInfo.EEP_GradeD;
        //     case "EEP_GradeC":
        //         return EtcEffectInfo.EEP_GradeC;
        //     case "EEP_GradeB":
        //         return EtcEffectInfo.EEP_GradeB;
        //     case "EEP_GradeA":
        //         return EtcEffectInfo.EEP_GradeA;
        //     case "EEP_GradeS":
        //         return EtcEffectInfo.EEP_GradeS;
        //     default:
        //         return EtcEffectInfo.EEP_None;
        // }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}