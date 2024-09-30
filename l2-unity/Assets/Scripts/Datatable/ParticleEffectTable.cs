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
    public GameObject[,] SoulshotCastParticles { get; private set; }
    public GameObject[,] SoulshotHitParticles { get; private set; }
    public GameObject[] DefaultHitParticles { get; private set; }
    public GameObject LocatorParticle { get; private set; }
    public GameObject LocatorReachedParticle { get; private set; }

    public void Initialize()
    {
        CacheEffects();
    }

    public void CacheEffects()
    {
        ParticleEffects = new Dictionary<string, GameObject>();

        LoadSkillParticles();

        // // Soulshot cast particle effects
        // SoulshotCastParticles = new GameObject[12, 6];
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADENONE] = LoadEffectByName("soul_N_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADED] = LoadEffectByName("soul_D_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEC] = LoadEffectByName("soul_C_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEB] = LoadEffectByName("soul_B_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADEA] = LoadEffectByName("soul_A_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SOULSHOT, (int)EtcEffectInfo.EEP_GRADES] = LoadEffectByName("soul_S_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADENONE] = LoadEffectByName("spirit_N_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADED] = LoadEffectByName("spirit_D_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEC] = LoadEffectByName("spirit_C_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEB] = LoadEffectByName("spirit_B_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADEA] = LoadEffectByName("spirit_A_stick");
        // SoulshotCastParticles[(int)EtcEffect.EET_SPIRITSHOT, (int)EtcEffectInfo.EEP_GRADES] = LoadEffectByName("spirit_S_stick");

        // Locator?
        LocatorParticle = LoadEffectByName("e_u093_a");
        LocatorReachedParticle = LoadEffectByName("e_u093_b");

        // Hit particles
        DefaultHitParticles = new GameObject[2];
        DefaultHitParticles[0] = LoadEffectByName("p_u002_a"); // Load whenever any hit occurs (with and without crit)
        DefaultHitParticles[1] = LoadEffectByName("p_u004_a"); // Load when soulshot disabled and crit occurs
        SoulshotHitParticles = new GameObject[12, 2];
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADENONE, 0] = LoadEffectByName("shot_N_atk"); // SS No-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADENONE, 1] = LoadEffectByName("shot_N_crit"); // SS No-Grade crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADED, 0] = LoadEffectByName("shot_D_atk"); // SS D-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADED, 1] = LoadEffectByName("shot_D_crit"); // SS D-Grade crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEC, 0] = LoadEffectByName("shot_C_atk"); // SS C-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEC, 1] = LoadEffectByName("shot_C_crit"); // SS C-Grade crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEB, 0] = LoadEffectByName("shot_B_atk"); // SS B-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEB, 1] = LoadEffectByName("shot_B_crit"); // SS B-Grade crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEA, 0] = LoadEffectByName("shot_A_atk"); // SS A-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADEA, 1] = LoadEffectByName("shot_A_crit"); // SS A-Grade crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADES, 0] = LoadEffectByName("shot_S_atk"); // SS S-Grade no-crit
        SoulshotHitParticles[(int)EtcEffectInfo.EEP_GRADES, 1] = LoadEffectByName("shot_S_crit"); // SS S-Grade crit
    }

    private void LoadSkillParticles()
    {
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
                        Debug.Log($"Loaded particle effect {particle} in ParticleEffectTable.");
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
                        Debug.Log($"Loaded particle effect {particle} in ParticleEffectTable.");
                    }
                });
            }
        }
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
