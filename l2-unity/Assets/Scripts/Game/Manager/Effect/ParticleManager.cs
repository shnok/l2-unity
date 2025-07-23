using System;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private float _hitHeightOffsetMultiplier = 1.0f;

    private GameObject _arrowPrefab;

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
        Transform worldEffectsContainer = GameObject.Find("Effects").transform;
        _effectPoolContainer.transform.parent = worldEffectsContainer;
        _effectContainer.transform.parent = worldEffectsContainer;
    }

    private void Start()
    {
        PrepareEffectPool();
        _arrowPrefab = ModelTable.Instance.GetItemModelById(17);
    }

    private void PrepareEffectPool()
    {
        foreach (KeyValuePair<string, GameObject> kvp in ParticleEffectTable.Instance.ParticleEffects)
        {
            if (EffectPool.ContainsKey(kvp.Key))
            {
                continue;
            }

            // Debug.Log("Prepared effect pool: " + kvp.Key);
            EffectPool.Add(kvp.Key, new Queue<PooledEffect>());
        }

        EffectPool.Add("arrow", new Queue<PooledEffect>());

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
            PooledEffect effect = ActiveEffects.Dequeue();

            float age = Time.time - effect.StartTime;
            if (age > effect.EffectDurationSec)
            {
                Debug.Log($"Moving effect {effect.EffectClass} to pool. Age: {age} Effect Duration: {effect.EffectDurationSec}.");


                if (effect.GameObject != null)
                {
                    effect.GameObject.transform.parent = _effectPoolContainer.transform;
                    EffectPool[effect.EffectClass].Enqueue(effect);
                    effect.GameObject.SetActive(false);
                }
            }
            else
            {
                ActiveEffects.Enqueue(effect);
            }
        }

        if (ActiveHitEffects.Count > 0)
        {
            PooledEffect effect = ActiveHitEffects.Dequeue();

            float age = Time.time - effect.StartTime;
            if (age > effect.EffectDurationSec)
            {
                Debug.Log($"Moving effect {effect.GameObject.name} to pool.");


                if (effect.GameObject != null)
                {
                    effect.GameObject.transform.parent = _effectPoolContainer.transform;
                    HitEffectPool[effect.HitEffectIndex].Enqueue(effect);
                    effect.GameObject.SetActive(false);
                }
                else
                {
                    ActiveEffects.Enqueue(effect);
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

    #region Skill Particles
    public PooledEffect[] SpawnCastParticles(Entity caster, Skill skill, int hitTime)
    {
        List<EffectEmitter> castingActions = skill.SkillEffect.CastingActions;
        if (castingActions == null || castingActions.Count == 0)
        {
            Debug.Log("Skill doesn't have any casting action.");
            return null;
        }

        PooledEffect[] castEffects = new PooledEffect[castingActions.Count];

        for (int i = 0; i < castingActions.Count; i++)
        {
            EffectEmitter action = castingActions[i];
            AttachMethod attachOn = action.AttachOn;
            string effectClass = action.EffectClass;
            byte repeatCount = 1;

            if (action.EtcEffect == EtcEffect.EET_SOULSHOT)
            {
                if (caster.Gear?.WeaponType == WeaponType.bow)
                {
                    attachOn = AttachMethod.AM_LH;
                }

                if (caster.Gear?.WeaponType == WeaponType.bow || caster.Gear?.WeaponType == WeaponType.fist)
                {
                    effectClass = action.SecondaryEffectClass;
                }

                if (caster.Gear?.WeaponType == WeaponType.fist || caster.Gear?.WeaponType == WeaponType.dual)
                {
                    repeatCount = 2;
                }
            }

            for (int r = 0; r < repeatCount; r++)
            {
                if (repeatCount > 1)
                {
                    if (r == 0)
                    {
                        attachOn = AttachMethod.AM_LH;
                    }
                    else
                    {
                        attachOn = AttachMethod.AM_RH;
                    }
                }

                PooledEffect effect = SpawnEffect(effectClass);
                if (effect == null || effect.GameObject == null)
                {
                    Debug.LogError($"Can't spawn skill effect {effectClass} for skill {skill.SkillId}.");
                    return null;
                }

                effect.HitTime = hitTime / 1000f; //in seconds

                effect.GameObject.transform.parent = GetAttachTransform(caster, attachOn);

                UpdateSkillEffectTransform(caster, action, effect.GameObject.transform, effect, attachOn);

                float effectRatio = CalculateCastParticleSizeRatio(caster);
                effect.GameObject.transform.localScale = effect.GameObject.transform.localScale * effectRatio;

                ActiveEffects.Enqueue(effect);

                castEffects[i] = effect;
            }
        }

        return castEffects;
    }

    private float CalculateCastParticleSizeRatio(Entity caster)
    {
        float colRadius = caster.Appearance.CollisionRadius;
        // FDarkElf     radius 0.15     ratio 1.45
        // GiantSpider  radius 0.495    ratio 2.2

        // Using linear interpolation based on given data points:
        // (0.15, 1.45) and (0.495, 2.2)
        float ratio = 1.42f + (colRadius - 0.14f) * ((2.5f - 1.42f) / (0.495f - 0.14f));

        return Mathf.Clamp(ratio, 0.5f, 3f);
    }

    private float CalculateHitParticleSizeRatio(Entity target)
    {
        float colHeight = target.Appearance.CollisionHeight;
        /*
        a_common_people_FElf_m00  			rad: 7 				height: 0.485		ratio: 1.25
        gremlin					  			rad: 10 			height: 0.285		ratio: 1		
        a_smith_MDwarf_m00  				rad: 7 				height: 0.314		ratio: ?
        */
        float ratio = 1.25f * colHeight + 0.64375f;
        ratio *= 1.1f;
        return Mathf.Clamp(ratio, 0.5f, 3f);
    }

    public void SpawnSkillShotParticle(Entity caster, Entity target, Skill skill, float hitTime)
    {
        List<EffectEmitter> shotActions = skill.SkillEffect.ShotActions;
        if (shotActions == null || shotActions.Count == 0)
        {
            Debug.Log("Skill doesn't have any shot action.");
            return;
        }

        foreach (EffectEmitter action in shotActions)
        {
            if (action.Arrow)
            {
                SpawnArrowProjectile(caster, target, hitTime, true);
                continue;
            }

            AttachMethod attachOn = action.AttachOn;
            string effectClass = action.EffectClass;

            PooledEffect effect = SpawnEffect(effectClass);
            if (effect == null || effect.GameObject == null)
            {
                Debug.LogError($"Can't spawn skill effect {effectClass} for skill {skill.SkillId}.");
                return;
            }

            effect.Target = target;
            effect.Caster = caster;

            if ((action.SpawnOnTarget || skill.Skillgrps[0].IconType == SkillType.Physical) && !action.ChargedArrow) //To verify
                                                                                                                     // SpawnOnTarget value is inconsistent
            {
                // Transform to attach
                effect.GameObject.transform.parent = GetAttachTransform(target, attachOn);

                PlaceHitParticle(effect, caster, target, action);
            }
            else
            {
                effect.HitSuccess = true;
                effect.HitTime = hitTime;
                effect.EffectDurationSec = Mathf.Max(0.5f, hitTime - Time.time);

                // Transform to attach
                effect.GameObject.transform.parent = GetAttachTransform(caster, attachOn);

                // Set initial position
                UpdateSkillEffectTransform(caster, action, effect.GameObject.transform, effect, attachOn);

                if (action.ChargedArrow) // Effect is supposed to follow arrow position
                {
                    effect.GameObject.transform.SetPositionAndRotation(caster.Gear.Arrow.position, caster.Gear.Arrow.rotation);
                    effect.StartingPosition = effect.GameObject.transform.position;
                }

                // Remove effect for attach transform
                effect.GameObject.transform.parent = _effectContainer.transform;

                // Set initial position to current position
                effect.StartingPosition = effect.GameObject.transform.position;

                effect.Skill = skill;

                ProjectileManager.Instance.AddProjectile(effect);
            }

            ActiveEffects.Enqueue(effect);
        }
    }

    public void SpawnSkillExplosionParticle(Entity caster, Entity target, Skill skill)
    {
        List<EffectEmitter> explosionActions = skill.SkillEffect.ExplosionActions;
        if (explosionActions == null || explosionActions.Count == 0)
        {
            Debug.Log("Skill doesn't have any explosion action.");
            return;
        }

        foreach (EffectEmitter action in explosionActions)
        {
            string effectClass = action.EffectClass;

            PooledEffect effect = SpawnEffect(effectClass);
            if (effect == null || effect.GameObject == null)
            {
                Debug.LogError($"Can't spawn skill effect {effectClass} for skill {skill.SkillId}.");
                return;
            }

            effect.Target = target;
            effect.Caster = caster;

            PlaceHitParticle(effect, caster, target, action);

            ActiveEffects.Enqueue(effect);
        }
    }

    private void UpdateSkillEffectTransform(Entity caster, EffectEmitter emitter, Transform effectTransform, PooledEffect effect, AttachMethod attachMethod)
    {
        effectTransform.localPosition = new Vector3(0, caster.Appearance.CollisionHeight, 0);
        // Debug.LogWarning(emitter.EffectClass + " " + emitter.RelativeToCylinder + " " + emitter.Offset);
        if (emitter.RelativeToCylinder)
        {
            //X*=CollisionRadius, Y*=CollisionHeight, Z*=1
            effectTransform.localPosition += new Vector3(emitter.Offset.x, emitter.Offset.y * caster.Appearance.CollisionHeight, emitter.Offset.z * caster.Appearance.CollisionRadius);
            Debug.LogWarning("UpdateSkillEffectTransform: " + emitter.Offset + " " + effectTransform.localPosition);
        }
        else if (attachMethod == AttachMethod.AM_TRAIL || attachMethod == AttachMethod.AM_NONE)
        {
            // effectTransform.localPosition *= 1.2f;
            effectTransform.localPosition += emitter.Offset / 52.5f;
        }
        else
        {
            effectTransform.localPosition = emitter.Offset;
        }

        effectTransform.localScale = emitter.ScaleSize > 0 ? Vector3.one * emitter.ScaleSize : Vector3.one;
        effectTransform.localScale *= (attachMethod == AttachMethod.AM_RH || attachMethod == AttachMethod.AM_LH) ? 0.01f : 1f;
        effectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        effect.GameObject.SetActive(true);
        effect.StartTime = Time.time;
        effect.Restart();
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
                    PooledEffect effect = effectGo.GetComponent<L2Particle>().PooledEffect;
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

    public Transform GetAttachTransform(Entity entity, AttachMethod attachMethod)
    {
        Transform attachTo;
        switch (attachMethod)
        {
            case AttachMethod.AM_NONE:
                attachTo = entity.transform;
                break;
            case AttachMethod.AM_RH:
                attachTo = entity.Gear.RightHandBone;
                break;
            case AttachMethod.AM_LH:
                attachTo = entity.Gear.LeftHandBone;
                break;
            default:
                attachTo = entity.transform;
                break;
        }

        return attachTo;
    }
    #endregion


    #region Hit Particles
    public void SpawnHitParticle(Entity attacker, Entity target, Hit hit)
    {
        if (hit.hasSoulshot()) // Always spawn base hit particle with the soulshot particle
        {
            PooledEffect basecritParticle = SpawnSingleHitParticle(false, false, hit.getSsGrade());
            PlaceHitParticle(basecritParticle, attacker, target, null);
            ActiveHitEffects.Enqueue(basecritParticle);
            basecritParticle.GameObject.transform.parent = _effectContainer.transform;

            PooledEffect hitParticle = SpawnSingleHitParticle(hit.isCrit(), true, hit.getSsGrade());
            PlaceHitParticle(hitParticle, attacker, target, null);
            ActiveHitEffects.Enqueue(hitParticle);
            hitParticle.GameObject.transform.parent = _effectContainer.transform;
        }
        else
        {
            // Spawn default hit or crit particle 
            PooledEffect baseHitParticle = SpawnSingleHitParticle(hit.isCrit(), false, hit.getSsGrade());
            PlaceHitParticle(baseHitParticle, attacker, target, null);
            ActiveHitEffects.Enqueue(baseHitParticle);
            baseHitParticle.GameObject.transform.parent = _effectContainer.transform;
        }
    }

    private Vector3 CalculateHitParticlePosition(Entity attacker, Entity target, EffectEmitter action)
    {
        float particleHeight;

        if (action != null)
        {
            particleHeight = target.Appearance.CollisionHeight + target.Appearance.CollisionHeight * action.Offset.y;
        }
        else
        {
            particleHeight = target.Appearance.CollisionHeight * _hitHeightOffsetMultiplier;
        }

        // var heading = attacker.transform.position - target.transform.position;
        // float angle = Vector3.Angle(heading, target.transform.forward);
        // Vector3 cross = Vector3.Cross(heading, target.transform.forward);
        // if (cross.y >= 0) angle = -angle;
        // Vector3 direction = Quaternion.Euler(0, angle, 0) * target.transform.forward;
        // Vector3 position = target.transform.position + direction * target.Appearance.CollisionRadius + Vector3.up * particleHeight;

        Vector3 position = target.transform.position + Vector3.up * particleHeight;

        return position;
    }

    private void PlaceHitParticle(PooledEffect effect, Entity attacker, Entity target, EffectEmitter action)
    {
        effect.GameObject.SetActive(true);
        effect.StartTime = Time.time;
        effect.GameObject.transform.position = CalculateHitParticlePosition(attacker, target, action);
        effect.GameObject.transform.LookAt(attacker.transform);
        effect.GameObject.transform.eulerAngles = new Vector3(0, effect.GameObject.transform.eulerAngles.y - 90f, 0);
        effect.GameObject.transform.localScale = (action != null ? action.ScaleSize : 1) * CalculateHitParticleSizeRatio(target) * Vector3.one;
        effect.Restart();
    }

    private PooledEffect SpawnSingleHitParticle(bool crit, bool soulshot, int soulshotGrade)
    {
        int index = soulshot ? (soulshotGrade * 2 + (crit ? 1 : 0) + 2) : (crit ? 1 : 0);

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
            PooledEffect effect = effectGo.GetComponent<L2Particle>().PooledEffect;
            effect.HitEffectIndex = index;
            effect.GameObject = effectGo.gameObject;

            return effect;
        }
    }
    #endregion

    #region Arrow
    public void SpawnArrowProjectile(Entity caster, Entity target, float hitTime, bool hitSuccess)
    {
        PooledEffect arrowEffect = SpawnArrow();
        arrowEffect.GameObject.SetActive(true);
        // arrowEffect.Restart();

        arrowEffect.StartTime = Time.time;
        arrowEffect.Caster = caster;
        arrowEffect.Target = target;
        arrowEffect.HitTime = hitTime;
        arrowEffect.HitSuccess = hitSuccess;

        arrowEffect.GameObject.transform.SetPositionAndRotation(caster.Gear.Arrow.position, caster.Gear.Arrow.rotation);
        arrowEffect.StartingPosition = arrowEffect.GameObject.transform.position;
        arrowEffect.GameObject.transform.localScale = Vector3.one * 100f;
        arrowEffect.IsArrow = true;

        arrowEffect.GameObject.transform.parent = _effectContainer.transform;

        ActiveEffects.Enqueue(arrowEffect);

        ProjectileManager.Instance.AddProjectile(arrowEffect);
    }

    private PooledEffect SpawnArrow()
    {
        if (EffectPool.TryGetValue("arrow", out Queue<PooledEffect> effects))
        {
            if (effects.Count > 0)
            {
                PooledEffect readyEffect = effects.Dequeue();
                readyEffect.StartTime = Time.time;
                Debug.Log($"Retrieving effect arrow from pool.");
                if (readyEffect.GameObject != null)
                {
                    return readyEffect;
                }
                else
                {
                    Debug.LogError($"Effect arrow from pool doesn't have a gameobject!");
                }
            }
            else
            {
                GameObject arrow = GameObject.Instantiate(_arrowPrefab);
                PooledEffect arrowEffect = new PooledEffect()
                {
                    EffectClass = "arrow",
                    EffectDurationSec = 12,
                    StartTime = Time.time,
                    MaximumInactiveTimeSec = 120,
                    GameObject = arrow
                };

                return arrowEffect;
            }
        }
        else
        {
            Debug.LogError($"Trying to spawn an unknown effect with class: arrow.");
        }

        return null;
    }
    #endregion

    public void DestroyAllParticles()
    {
        DestroyQueue(_activeEffects);
        DestroyQueue(_activeHitEffects);
        DestroyPool(_effectPool);
        DestroyPool(_hitEffectPool);
    }

    private void DestroyPool<T>(Dictionary<T, Queue<PooledEffect>> pool)
    {
        if (pool == null)
        {
            return;
        }

        foreach (Queue<PooledEffect> queue in pool.Values)
        {
            DestroyQueue(queue);
        }

        // pool.Clear();
    }

    private void DestroyQueue(Queue<PooledEffect> queue)
    {
        if (queue != null && queue.Count != 0)
        {
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue.Count > 0)
                {
                    PooledEffect e = queue.Dequeue();
                    if (e != null && e.GameObject != null)
                    {
                        Destroy(e.GameObject);
                    }
                }
            }
        }
    }
}
