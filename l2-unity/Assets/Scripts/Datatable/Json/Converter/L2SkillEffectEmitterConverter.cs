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
            EffectClass =
            (jObject["EffectClass"] != null && jObject["EffectClass"].ToString().Length > 0)
            ? jObject["EffectClass"].ToString().Split(".")[1] : null,
            ScaleSize = jObject["ScaleSize"]?.Value<float>() ?? 1.0f,
            Offset = L2JsonConverterCommonFunctions.ParseVector3(jObject["Offset"]?.ToString()),
            EtcEffect = ParseEtcEffect(jObject["EtcEffect"]?.ToString()),
            EtcEffectInfo = ParseEtcEffectInfo(jObject["EtcEffectInfo"]?.ToString()),
            PawnLight = jObject["bPawnLight"]?.ToString().ToLower() == "true",
        };

        if (emitter.EffectClass == null)
        {
            if (emitter.EtcEffect != EtcEffect.EET_NONE && emitter.EtcEffectInfo != EtcEffectInfo.EEP_NONE)
            {
                emitter.EffectClass = EtcEffectToEffectName(emitter.EtcEffect, emitter.EtcEffectInfo);
            }
        }

        if (jObject["PawnLightParam"] != null)
        {
            emitter.PawnLightParam = EffectPawnLightParam.Parse(jObject["PawnLightParam"].ToString());
        }

        if (emitter.EffectClass == null)
        {
            Debug.LogError($"Skill emitter class is null.");
        }

        return emitter;
    }

    private string EtcEffectToEffectName(EtcEffect etcEffect, EtcEffectInfo etcEffectInfo)
    {
        if (etcEffect == EtcEffect.EET_SOULSHOT)
        {
            switch (etcEffectInfo)
            {
                case EtcEffectInfo.EEP_GRADENONE:
                    return "soul_N_stick";
                case EtcEffectInfo.EEP_GRADED:
                    return "soul_D_stick";
                case EtcEffectInfo.EEP_GRADEC:
                    return "soul_C_stick";
                case EtcEffectInfo.EEP_GRADEB:
                    return "soul_B_stick";
                case EtcEffectInfo.EEP_GRADEA:
                    return "soul_A_stick";
                case EtcEffectInfo.EEP_GRADES:
                    return "soul_S_stick";
            }
        }

        if (etcEffect == EtcEffect.EET_SPIRITSHOT)
        {
            switch (etcEffectInfo)
            {
                case EtcEffectInfo.EEP_GRADENONE:
                    return "spirit_N_stick";
                case EtcEffectInfo.EEP_GRADED:
                    return "spirit_D_stick";
                case EtcEffectInfo.EEP_GRADEC:
                    return "spirit_C_stick";
                case EtcEffectInfo.EEP_GRADEB:
                    return "spirit_B_stick";
                case EtcEffectInfo.EEP_GRADEA:
                    return "spirit_A_stick";
                case EtcEffectInfo.EEP_GRADES:
                    return "spirit_S_stick";
            }
        }

        return null;
    }

    private AttachMethod ParseAttachOnType(string attachOn)
    {
        if (attachOn == null)
        {
            return AttachMethod.AM_NONE;
        }

        return (AttachMethod)Enum.Parse(typeof(AttachMethod), attachOn.ToUpper());
    }


    private EtcEffect ParseEtcEffect(string etcEffect)
    {
        if (etcEffect == null)
        {
            return EtcEffect.EET_NONE;
        }

        return (EtcEffect)Enum.Parse(typeof(EtcEffect), etcEffect.ToUpper());
    }

    private EtcEffectInfo ParseEtcEffectInfo(string etcEffectInfo)
    {
        if (etcEffectInfo == null)
        {
            return EtcEffectInfo.EEP_NONE;
        }

        return (EtcEffectInfo)Enum.Parse(typeof(EtcEffectInfo), etcEffectInfo.ToUpper());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}