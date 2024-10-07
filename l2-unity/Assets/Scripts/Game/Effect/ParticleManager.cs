using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;
    public static ParticleManager Instance { get { return _instance; } }

    private Dictionary<string, Stack<PooledEffect>> _effectPool;

    public Dictionary<string, Stack<PooledEffect>> EffectPool
    {
        get
        {
            if (_effectPool == null)
            {
                _effectPool = new Dictionary<string, Stack<PooledEffect>>();
            }

            return _effectPool;
        }
        private set
        {
            _effectPool = value;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        PrepareEffectPool();
    }

    private void PrepareEffectPool()
    {
        foreach (KeyValuePair<string, GameObject> kvp in ParticleEffectTable.Instance.ParticleEffects)
        {
            if (EffectPool.ContainsKey(kvp.Key))
            {
                continue;
            }

            EffectPool.Add(kvp.Key, new Stack<PooledEffect>());
        }
    }

    public void SpawnSkillParticles(Entity caster, Skill skill)
    {
        List<EffectEmitter> castingActions = skill.SkillEffect.CastingActions;
        if (castingActions == null)
        {
            Debug.Log("Skill doesn't have any casting action.");
            return;
        }

        castingActions.ForEach((action) =>
        {
            Debug.Log(caster);
            Debug.Log(caster.Gear);
            Debug.Log(caster.Gear.RightHandBone);

            AttachOnType attachOn = action.AttachOn;
            Transform attachTo;
            switch (attachOn)
            {
                case AttachOnType.AM_NONE:
                    attachTo = caster.transform;
                    break;
                case AttachOnType.AM_RH:
                    attachTo = caster.Gear.RightHandBone;
                    break;
                case AttachOnType.AM_LH:
                    attachTo = caster.Gear.LeftHandBone;
                    break;
                default:
                    Debug.LogWarning($"Unhandled AttachOn: {attachOn}.");
                    attachTo = caster.transform;
                    break;
            }

            SpawnEffect(action.EffectClass, attachTo);
        });
    }

    public void SpawnEffect(string effectClass, Transform parent)
    {
        //verify if present in pool

        // if not get for particle effect table and instantiate
        if (ParticleEffectTable.Instance.ParticleEffects.TryGetValue(effectClass, out GameObject gameObject))
        {
            Debug.Log(effectClass + " " + gameObject);
            GameObject.Instantiate(gameObject, parent);
        }
    }

    public void SpawnHitParticle(Entity attacker, Entity target, bool crit, bool soulshot, int soulshotGrade)
    {

    }
}
