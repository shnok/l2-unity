using UnityEngine;
using System.Collections.Generic;
using System;

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

        // Locator?
        LocatorParticle = LoadEffectByName("e_u093_a");
        LocatorReachedParticle = LoadEffectByName("e_u093_b");

        // Hit particles
        DefaultHitParticles = new GameObject[2];
        DefaultHitParticles[0] = LoadEffectByName("p_u002_a"); // Load whenever any hit occurs (with and without crit)
        DefaultHitParticles[1] = LoadEffectByName("p_u004_a"); // Load when soulshot disabled and crit occurs
        SoulshotHitParticles = new GameObject[12, 2];
        SoulshotHitParticles[0, 0] = LoadEffectByName("shot_N_atk"); // SS No-Grade no-crit
        SoulshotHitParticles[0, 1] = LoadEffectByName("shot_N_cr"); // SS No-Grade crit
        SoulshotHitParticles[1, 0] = LoadEffectByName("shot_D_atk"); // SS D-Grade no-crit
        SoulshotHitParticles[1, 1] = LoadEffectByName("shot_D_cr"); // SS D-Grade crit
        SoulshotHitParticles[2, 0] = LoadEffectByName("shot_C_atk"); // SS C-Grade no-crit
        SoulshotHitParticles[2, 1] = LoadEffectByName("shot_C_cr"); // SS C-Grade crit
        SoulshotHitParticles[3, 0] = LoadEffectByName("shot_B_atk"); // SS B-Grade no-crit
        SoulshotHitParticles[3, 1] = LoadEffectByName("shot_B_cr"); // SS B-Grade crit
        SoulshotHitParticles[4, 0] = LoadEffectByName("shot_A_atk"); // SS A-Grade no-crit
        SoulshotHitParticles[4, 1] = LoadEffectByName("shot_A_cr"); // SS A-Grade crit
        SoulshotHitParticles[5, 0] = LoadEffectByName("shot_S_atk"); // SS S-Grade no-crit
        SoulshotHitParticles[5, 1] = LoadEffectByName("shot_S_cr"); // SS S-Grade crit
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
                    string effectClass = emitter.EffectClass;

                    GameObject particle = LoadEffectEmitter(effectClass);
                    if (particle != null)
                    {
                        ParticleEffects[effectClass] = particle;
                        // Debug.Log($"Loaded particle effect {particle} in ParticleEffectTable.");
                    }


                    effectClass = emitter.SecondaryEffectClass;

                    if (effectClass != null)
                    {
                        particle = LoadEffectEmitter(effectClass);
                        if (particle != null)
                        {
                            ParticleEffects[effectClass] = particle;
                            // Debug.Log($"Loaded secondary particle effect {particle} in ParticleEffectTable.");
                        }
                    }
                });
            }

            if (kvp.Value.ShotActions != null && kvp.Value.ShotActions.Count > 0)
            {
                kvp.Value.ShotActions.ForEach((emitter) =>
                {
                    string effectClass = emitter.EffectClass;

                    GameObject particle = LoadEffectEmitter(effectClass);
                    if (particle != null)
                    {
                        ParticleEffects[effectClass] = particle;
                        // Debug.Log($"Loaded particle effect {particle} in ParticleEffectTable.");
                    }
                });
            }


            if (kvp.Value.ExplosionActions != null && kvp.Value.ExplosionActions.Count > 0)
            {
                kvp.Value.ExplosionActions.ForEach((emitter) =>
                {
                    string effectClass = emitter.EffectClass;

                    GameObject particle = LoadEffectEmitter(effectClass);
                    if (particle != null)
                    {
                        ParticleEffects[effectClass] = particle;
                        Debug.Log($"Loaded particle effect {particle} in ParticleEffectTable.");
                    }
                });
            }
        }
    }

    private GameObject LoadEffectEmitter(string effectClass)
    {
        if (effectClass == null || effectClass.Length == 0)
        {
            return null;
        }

        if (ParticleEffects.ContainsKey(effectClass))
        {
            return null;
        }

        return LoadEffectByName(effectClass);
    }

    private GameObject LoadEffectByName(string name)
    {
        string path = $"Data/Effects/{name}/{name}";

        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            // Debug.LogWarning($"Can't find particle effect {name} at path {path}.");
            return null;
        }

        // Debug.Log($"Successfully loaded particle effect {name}.");

        return prefab;
    }
}
