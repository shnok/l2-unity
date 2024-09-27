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