using UnityEngine;
using System.Collections.Generic;

public class ParticleEffectTable
{
    private static ParticleEffectTable _instance;
    public static ParticleEffectTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ParticleEffectTable();
            }

            return _instance;
        }
    }

    public Dictionary<string, GameObject> ParticleEffects { get; private set; }
    public GameObject[,] SoulshotCastParticleEffects { get; private set; }

    public void Initialize()
    {
        CacheEffects();
    }

    public void CacheEffects()
    {
        ParticleEffects = new Dictionary<string, GameObject>();
        // Skills particle effects
        foreach (KeyValuePair<int, L2SkillEffect> kvp in SkillEffectTable.Instance.SkillEffects)
        {
            if (kvp.Value.CastingActions != null && kvp.Value.CastingActions.Count > 0)
            {
                kvp.Value.CastingActions.ForEach((emitter) =>
                {
                    GameObject particle = LoadEffectEmitter(emitter);
                    if (particle != null)
                    {
                        ParticleEffects[emitter.EffectClass] = particle;
                    }
                });
            }
            if (kvp.Value.ShotActions != null && kvp.Value.ShotActions.Count > 0)
            {
                kvp.Value.ShotActions.ForEach((emitter) =>
                {
                    GameObject particle = LoadEffectEmitter(emitter);
                    if (particle != null)
                    {
                        ParticleEffects[emitter.EffectClass] = particle;
                    }
                });
            }
        }

        // Soulshot cast particle effects
        SoulshotCastParticleEffects = new GameObject[8, 10];
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADENONE] = LoadEffectByName("soul_N_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADED] = LoadEffectByName("soul_D_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEC] = LoadEffectByName("soul_C_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEB] = LoadEffectByName("soul_B_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEA] = LoadEffectByName("soul_A_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADES] = LoadEffectByName("soul_S_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADENONE] = LoadEffectByName("spirit_N_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADED] = LoadEffectByName("spirit_D_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEC] = LoadEffectByName("spirit_C_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEB] = LoadEffectByName("spirit_B_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEA] = LoadEffectByName("spirit_A_stick");
        SoulshotCastParticleEffects[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADES] = LoadEffectByName("spirit_S_stick");

        // Soulshot hit particle effects


        // Locator?

        // Default hit?
    }

    private GameObject LoadEffectEmitter(EffectEmitter emitter)
    {
        if (emitter.EffectClass == null || emitter.EffectClass.Length == 0)
        {
            return null;
        }

        if (ParticleEffects.ContainsKey(emitter.EffectClass))
        {
            return null;
        }

        return LoadEffectByName(emitter.EffectClass);
    }

    private GameObject LoadEffectByName(string name)
    {
        string path = $"Data/Effects/{name}/{name}";

        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"Can't find particle effect {name} at path {path}.");
            return null;
        }

        return Resources.Load<GameObject>(path);
    }
}
