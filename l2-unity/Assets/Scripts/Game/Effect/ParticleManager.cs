using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;
    public static ParticleManager Instance { get { return _instance; } }

    private Queue<PooledEffect> _activeEffects;
    private Dictionary<string, Queue<PooledEffect>> _effectPool;

    private Queue<PooledEffect> _activeHitEffects;
    private Dictionary<int, Queue<PooledEffect>> _hitEffectPool;

    private GameObject _effectContainer;
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

    public Queue<PooledEffect> ActiveEffects
    {
        get
        {
            if (_activeEffects == null)
            {
                _activeEffects = new Queue<PooledEffect>();
            }

            return _activeEffects;
        }
        private set
        {
            _activeEffects = value;
        }
    }

    public Queue<PooledEffect> ActiveHitEffects
    {
        get
        {
            if (_activeHitEffects == null)
            {
                _activeHitEffects = new Queue<PooledEffect>();
            }

            return _activeHitEffects;
        }
        private set
        {
            _activeHitEffects = value;
        }
    }

    public Dictionary<int, Queue<PooledEffect>> HitEffectPool
    {
        get
        {
            if (_hitEffectPool == null)
            {
                _hitEffectPool = new Dictionary<int, Queue<PooledEffect>>();
            }

            return _hitEffectPool;
        }
        private set
        {
            _hitEffectPool = value;
        }
    }


    #region Initialization
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
        _effectContainer = new GameObject("EffectContainer");
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

        Array enums = Enum.GetValues(typeof(EtcEffectInfo));
        foreach (int item in enums)
        {
            HitEffectPool[item * 2] = new Queue<PooledEffect>(); // ss grade
            HitEffectPool[item * 2 + 1] = new Queue<PooledEffect>(); // ss grade + crit
        }

        HitEffectPool[0] = new Queue<PooledEffect>(); // hit effect
        HitEffectPool[1] = new Queue<PooledEffect>(); // non-ss crit effect
    }
    #endregion

    #region Particle Management tasks
    private void FixedUpdate()
    {
        ManageActiveEffectsTask();

        PoolCleanupTask();
    }

    private void ManageActiveEffectsTask()
    {
        if (ActiveEffects.Count > 0)
        {
            PooledEffect effect = ActiveEffects.Peek();
            float age = Time.time - effect.StartTime;
            if (age > effect.EffectDurationSec)
            {
                Debug.Log($"Moving effect {effect.EffectClass} to pool.");

                ActiveEffects.Dequeue();

                if (effect.GameObject != null)
                {
                    effect.GameObject.transform.parent = _effectPoolContainer.transform;
                    EffectPool[effect.EffectClass].Enqueue(effect);
                    effect.GameObject.SetActive(false);
                }

            }
        }

        if (ActiveHitEffects.Count > 0)
        {
            PooledEffect effect = ActiveHitEffects.Peek();
            float age = Time.time - effect.StartTime;
            if (age > effect.EffectDurationSec)
            {
                Debug.Log($"Moving effect {effect.GameObject.name} to pool.");

                ActiveHitEffects.Dequeue();

                if (effect.GameObject != null)
                {
                    effect.GameObject.transform.parent = _effectPoolContainer.transform;
                    HitEffectPool[effect.HitEffectIndex].Enqueue(effect);
                    effect.GameObject.SetActive(false);
                }

            }
        }
    }

    public void PoolCleanupTask()
    {
        foreach (KeyValuePair<string, Queue<PooledEffect>> kvp in EffectPool)
        {
            CleanUpPoolQueue(kvp.Value);
        }

        foreach (KeyValuePair<int, Queue<PooledEffect>> kvp in HitEffectPool)
        {
            CleanUpPoolQueue(kvp.Value);
        }
    }

    private void CleanUpPoolQueue(Queue<PooledEffect> queue)
    {
        if (queue.Count > 0)
        {
            PooledEffect effect = queue.Peek();
            float age = Time.time - effect.StartTime;
            if (age > effect.MaximumInactiveTimeSec)
            {
                queue.Dequeue();

                if (effect.GameObject != null)
                {
                    Debug.Log($"Removing effect {effect.GameObject.name} from pool.");
                    GameObject.Destroy(effect.GameObject);
                }
            }
        }
    }

    #endregion

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
            AttachMethod attachOn = action.AttachOn;
            Transform attachTo;
            switch (attachOn)
            {
                case AttachMethod.AM_NONE:
                    attachTo = caster.transform;
                    break;
                case AttachMethod.AM_RH:
                    attachTo = caster.Gear.RightHandBone;
                    break;
                case AttachMethod.AM_LH:
                    attachTo = caster.Gear.LeftHandBone;
                    break;
                default:
                    Debug.LogWarning($"Unhandled AttachOn: {attachOn}.");
                    attachTo = caster.transform;
                    break;
            }

            PooledEffect effect = SpawnEffect(action.EffectClass);
            if (effect == null || effect.GameObject == null)
            {
                Debug.LogError($"Can't spawn skill effect {skill.EffectId} for skill {skill.SkillId}.");
                return;
            }

            effect.GameObject.transform.parent = attachTo;

            UpdateSkillEffectTransform(action, effect.GameObject.transform, effect);
            ActiveEffects.Enqueue(effect);
        });
    }

    private void UpdateSkillEffectTransform(EffectEmitter emitter, Transform effectTransform, PooledEffect effect)
    {
        effectTransform.localPosition = emitter.Offset;
        effectTransform.localScale = emitter.ScaleSize > 0 ? Vector3.one * emitter.ScaleSize : Vector3.one;
        effectTransform.localScale *= _globalEffectScaling;
        effectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        effect.GameObject.SetActive(true);
        effect.StartTime = Time.time;
    }

    public void SpawnEffectByClass(string effectClass, Vector3 location)
    {
        PooledEffect effect = SpawnEffect(effectClass);

        Transform effectTransform = effect.GameObject.transform;
        effectTransform.parent = _effectContainer.transform;
        effectTransform.localPosition = location;
        effectTransform.localScale = _globalEffectScaling * Vector3.one;

        ActiveEffects.Enqueue(effect);
    }

    public PooledEffect SpawnEffect(string effectClass)
    {
        //verify if present in pool
        if (EffectPool.TryGetValue(effectClass, out Queue<PooledEffect> effects))
        {
            if (effects.Count > 0)
            {
                PooledEffect readyEffect = effects.Dequeue();
                readyEffect.StartTime = Time.time;
                Debug.Log($"Retrieving effect {effectClass} from pool.");
                if (readyEffect.GameObject != null)
                {
                    return readyEffect;
                }
                else
                {
                    Debug.LogError($"Effect {effectClass} from pool doesn't have a gameobject!");
                }
            }
            else
            {
                // if not get for particle effect table and instantiate
                if (ParticleEffectTable.Instance.ParticleEffects.TryGetValue(effectClass, out GameObject gameObject))
                {
                    Debug.Log($"Created new effect {effectClass}.");
                    GameObject effectGo = GameObject.Instantiate(gameObject);
                    PooledEffect effect = effectGo.GetComponent<ParticleTimerResetGroup>().PooledEffect;
                    effect.GameObject = effectGo.gameObject;
                    effect.EffectClass = effectClass;
                    return effect;
                }
                else
                {
                    Debug.LogError($"Effect {effectClass} doesn't exist!");
                }
            }

        }
        else
        {
            Debug.LogError($"Trying to spawn an unknown effect with class: {effectClass}.");
        }

        return null;
    }


    #region Hit Particles
    public void SpawnHitParticle(Entity attacker, Entity target, bool crit, bool soulshot, int soulshotGrade)
    {
        // Spawn default hit particle from pool at index 0
        PooledEffect baseHitParticle = SpawnSingleHitParticle(false, false, soulshotGrade);

        PooledEffect hitParticle = SpawnSingleHitParticle(crit, soulshot, soulshotGrade);

        Vector3 particlePosition = CalculateParticlePosition(attacker, target);

        PlaceHitParticle(baseHitParticle, attacker, particlePosition);
        PlaceHitParticle(hitParticle, attacker, particlePosition);

        ActiveHitEffects.Enqueue(baseHitParticle);
        ActiveHitEffects.Enqueue(hitParticle);
    }

    private Vector3 CalculateParticlePosition(Entity attacker, Entity target)
    {
        var heading = attacker.transform.position - target.transform.position;
        float angle = Vector3.Angle(heading, target.transform.forward);
        Vector3 cross = Vector3.Cross(heading, target.transform.forward);
        if (cross.y >= 0) angle = -angle;
        Vector3 direction = Quaternion.Euler(0, angle, 0) * target.transform.forward;
        float particleHeight = target.Appearance.CollisionHeight * 1.25f;
        Vector3 position = target.transform.position + direction * target.Appearance.CollisionRadius + Vector3.up * particleHeight;

        return position;
    }

    private void PlaceHitParticle(PooledEffect effect, Entity attacker, Vector3 position)
    {
        effect.GameObject.SetActive(true);
        effect.GameObject.transform.parent = _effectContainer.transform;
        effect.StartTime = Time.time;
        effect.GameObject.transform.position = position;
        effect.GameObject.transform.LookAt(attacker.transform);
        effect.GameObject.transform.eulerAngles = new Vector3(0, effect.GameObject.transform.eulerAngles.y - 90f, 0);
    }

    private PooledEffect SpawnSingleHitParticle(bool crit, bool soulshot, int soulshotGrade)
    {
        int hitIndex = (!soulshot && crit) ? 1 : 0;
        int ssIndex = soulshotGrade * 2 + (crit ? 1 : 0);
        int index = ssIndex > 0 ? ssIndex : hitIndex;

        Queue<PooledEffect> queue = HitEffectPool[index];
        if (queue.Count > 0)
        {
            // Get particle from pool
            return queue.Dequeue();
        }
        else
        {
            // Create new particle
            GameObject go;
            if (soulshot)
            {
                go = ParticleEffectTable.Instance.SoulshotHitParticles[soulshotGrade, crit ? 1 : 0];
            }
            else
            {
                go = ParticleEffectTable.Instance.DefaultHitParticles[crit ? 1 : 0];
            }

            if (go == null)
            {
                Debug.LogError($"Hit particle gameobject is null. Paraneters: crit:{crit} soulshot:{soulshot} soulshotGrade:{soulshotGrade}");
                return null;
            }

            Debug.Log($"Created new hit effect.");

            GameObject effectGo = GameObject.Instantiate(go);
            PooledEffect effect = effectGo.GetComponent<ParticleTimerResetGroup>().PooledEffect;
            effect.HitEffectIndex = index;
            effect.GameObject = effectGo.gameObject;

            return effect;
        }
    }
    #endregion
}
