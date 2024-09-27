using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class SkillEffectTable
{
    private static SkillEffectTable _instance;
    public static SkillEffectTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillEffectTable();
            }

            return _instance;
        }
    }

    public Dictionary<int, L2SkillEffect> SkillEffects { get; private set; }

    public void Initialize()
    {
        CacheEffects();
    }

    public void CacheEffects()
    {
        SkillEffects = new Dictionary<int, L2SkillEffect>();

        foreach (KeyValuePair<int, Skill> kvp in SkillTable.Instance.Skills)
        {
            if (SkillEffects.ContainsKey(kvp.Key))
            {
                continue;
            }

            L2SkillEffect skillEffect = ParseEffectId(kvp.Value.EffectId);

            if (skillEffect != null)
            {
                SkillEffects.TryAdd(kvp.Value.EffectId, skillEffect);
            }
        }
    }

    private L2SkillEffect ParseEffectId(int id)
    {

        string dataPath = Path.Combine(Application.streamingAssetsPath, $"Data/LineageEffects/{id}.json");
        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("File not found: " + dataPath);
            return null;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            L2SkillEffect skillEffect = ParseL2SkillEffect(reader.ReadToEnd());
            GameManager.Instance.debugEffect = skillEffect;

            return skillEffect;
        }
    }


    public static L2SkillEffect ParseL2SkillEffect(string json)
    {
        return JsonConvert.DeserializeObject<L2SkillEffect>(json, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new L2SkillEffectEmitterConverter() }
        });
    }
}

