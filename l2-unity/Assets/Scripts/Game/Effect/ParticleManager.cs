using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;
    public static ParticleManager Instance { get { return _instance; } }

    private Dictionary<string, Queue<PooledEffect>> _activeEffects;
    private Dictionary<string, Queue<PooledEffect>> _effectPool;

    private GameObject _effectPoolContainer;
    [SerializeField] private float _globalEffectScaling = 0.01f;

    public Dictionary<string, Queue<PooledEffect>> EffectPool
    {
        get
        {
            if (_effectPool == null)
            {
                _effectPool = new Dictionary<string, Queue<PooledEffect>>();
            }

            return _effectPool;
        }
        private set
        {
            _effectPool = value;
        }
    }
    public Dictionary<string, Queue<PooledEffect>> ActiveEffects
    {
        get
        {
            if (_activeEffects == null)
            {
                _activeEffects = new Dictionary<string, Queue<PooledEffect>>();
            }

            return _activeEffects;
        }
        private set
        {
            _activeEffects = value;
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

        _effectPoolContainer = new GameObject("EffectPool");
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

            Debug.Log("Prepared effect pool: " + kvp.Key);
            EffectPool.Add(kvp.Key, new Queue<PooledEffect>());
        }

        foreach (KeyValuePair<string, GameObject> kvp in ParticleEffectTable.Instance.ParticleEffects)
        {
            if (ActiveEffects.ContainsKey(kvp.Key))
            {
                continue;
            }

            ActiveEffects.Add(kvp.Key, new Queue<PooledEffect>());
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

            SpawnEffect(action, attachTo);
        });
    }

    private void FixedUpdate()
    {
        ManageActiveEffectsTask();

        QueueCleanupTask();
    }

    private void ManageActiveEffectsTask()
    {
        foreach (KeyValuePair<string, Queue<PooledEffect>> kvp in ActiveEffects)
        {
            if (kvp.Value.Count > 0)
            {
                PooledEffect effect = kvp.Value.Peek();
                float age = Time.time - effect.StartTime;
                if (age > effect.EffectDurationSec)
                {
                    Debug.Log($"Moving effect {kvp.Key} to pool.");

                    kvp.Value.Dequeue();

                    if (effect.GameObject != null)
                    {
                        effect.GameObject.transform.parent = _effectPoolContainer.transform;
                        EffectPool[kvp.Key].Enqueue(effect);
                        effect.GameObject.SetActive(false);
                    }

                }
            }
        }
    }

    public void QueueCleanupTask()
    {
        foreach (KeyValuePair<string, Queue<PooledEffect>> kvp in EffectPool)
        {
            if (kvp.Value.Count > 0)
            {
                PooledEffect effect = kvp.Value.Peek();
                float age = Time.time - effect.StartTime;
                if (age > effect.MaximumInactiveTimeSec)
                {
                    Debug.Log($"Removing effect {kvp.Key} from pool.");

                    kvp.Value.Dequeue();

                    if (effect.GameObject != null)
                    {
                        GameObject.Destroy(effect.GameObject);
                    }

                }
            }
        }
    }

    public void SpawnEffect(EffectEmitter emitter, Transform parent)
    {
        //verify if present in pool
        if (EffectPool.TryGetValue(emitter.EffectClass, out Queue<PooledEffect> effects))
        {
            if (effects.Count > 0)
            {
                PooledEffect readyEffect = effects.Dequeue();
                readyEffect.StartTime = Time.time;
                Debug.Log($"Retrieving effect {emitter.EffectClass} from pool.");
                if (readyEffect.GameObject != null)
                {
                    Transform effectTransform = readyEffect.GameObject.transform;
                    effectTransform.parent = parent;
                    UpdateEffectTransform(emitter, effectTransform, readyEffect);
                }
                else
                {
                    Debug.LogError($"Effect {emitter.EffectClass} from pool doesn't have a gameobject!");
                }
            }
            else
            {
                // if not get for particle effect table and instantiate
                if (ParticleEffectTable.Instance.ParticleEffects.TryGetValue(emitter.EffectClass, out GameObject gameObject))
                {
                    Debug.Log($"Created new effect {emitter.EffectClass}.");
                    Transform effectTransform = GameObject.Instantiate(gameObject, parent).transform;
                    PooledEffect effect = effectTransform.GetComponent<ParticleTimerResetGroup>().PooledEffect;
                    effect.GameObject = effectTransform.gameObject;
                    UpdateEffectTransform(emitter, effectTransform, effect);
                }
                else
                {
                    Debug.LogError($"Effect {emitter.EffectClass} doesn't exist!");
                }
            }

        }
        else
        {
            Debug.LogError($"Trying to spawn an unknown effect with class: {emitter.EffectClass}.");
        }
    }

    private void UpdateEffectTransform(EffectEmitter emitter, Transform effectTransform, PooledEffect effect)
    {
        effectTransform.localPosition = emitter.Offset;
        effectTransform.localScale = emitter.ScaleSize > 0 ? Vector3.one * emitter.ScaleSize : Vector3.one;
        effectTransform.localScale *= _globalEffectScaling;
        effectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        effect.GameObject.SetActive(true);
        effect.StartTime = Time.time;

        if (ActiveEffects.TryGetValue(emitter.EffectClass, out Queue<PooledEffect> effects))
        {
            effects.Enqueue(effect);
        }
    }

    public void SpawnHitParticle(Entity attacker, Entity target, bool crit, bool soulshot, int soulshotGrade)
    {

    }
}
