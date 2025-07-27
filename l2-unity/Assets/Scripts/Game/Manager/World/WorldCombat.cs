using System.Collections.Generic;
using System.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using static StatusUpdatePacket;

public class WorldCombat : MonoBehaviour
{
    [SerializeField] private List<Hit> _hits;
    [SerializeField] private float _projectilesSpeed = 20f;
    private EventProcessor _eventProcessor;
    private WorldSpawner _worldSpawner;

    private static WorldCombat _instance;
    public static WorldCombat Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _hits = new List<Hit>();
        _eventProcessor = EventProcessor.Instance;
        _worldSpawner = GetComponent<WorldSpawner>();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void FixedUpdate()
    {
        ApplyHits();
    }

    private void ApplyHits()
    {
        float now = Time.time;
        for (int i = _hits.Count - 1; i >= 0; i--)
        {
            Hit hit = _hits[i];
            if (now >= hit.HitTime)
            {
                _hits.RemoveAt(i);
                InflictAttack(hit.Attacker, hit.Target, hit);
                // Debug.LogWarning($"Hit: Apply hit! Attacker: {hit.Attacker.gameObject.name}");
            }
        }
    }

    public void InflictAttack(Entity attacker, Entity target, Hit hit)
    {
        if (attacker.IsDead)
        {
            Debug.LogWarning($"Hit: Attacker {attacker.gameObject.name} is dead.");
            return;
        }

        ApplyDamage(target, hit);

        if (hit.isMiss())
        {
            // Debug.LogWarning("Hit: Hit is miss.");
            attacker.ReferenceHolder.AudioHandler.PlaySwishSound();
            return;
        }

        // Instantiate hit particle
        ParticleManager.Instance.SpawnHitParticle(attacker, target, hit);
    }

    private void ApplyDamage(Entity target, Hit hit)
    {
        // Apply damage to target
        target.Combat.ApplyDamage(hit);
    }

    public Task OnMagicSkillUse(MagicSkillUsePacket packet)
    {
        return _worldSpawner.ExecuteWithEntitiesAsync(packet.ObjectId, packet.TargetId, (targeter, targeted) =>
        {
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                PlayerSkill.Instance.OnSkillUsed(packet.SkillId, packet.ReuseDelay);
            }

            EntityCastSkill(targeter, targeted, packet.SkillId, packet.HitTime, packet.ReuseDelay);
        });
    }


    public Task OnMagicSkillCanceled(MagicSkillCanceledPacket packet)
    {
        return _worldSpawner.ExecuteWithEntityAsync(packet.ObjectId, (entity) =>
               {
                   if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
                   {
                       PlayerStateMachine.Instance.OnMagicSkillCanceled();
                       NameplatesManagerGame.Instance.StopCasting();
                   }
                   else
                   {
                       entity.ReferenceHolder.NewAnimationController.Wait();
                   }

                   EntityCancelCastSkill(entity);
               });
    }

    private void EntityCancelCastSkill(Entity entity)
    {
        entity.Combat.AbortCast();
    }

    public Task OnMagicSkillLaunched(MagicSkillLaunchedPacked packet)
    {

        // TODO: Handle multiple targets
        // TODO: Handle skill explosion
        if (packet.TargetCount == 0)
        {
            return _worldSpawner.ExecuteWithEntityAsync(packet.ObjectId, (targeter) =>
            {
                // EntityShootSkill(targeter, targeter, packet.SkillId);
            });
        }
        else
        {
            return _worldSpawner.ExecuteWithEntitiesAsync(packet.ObjectId, packet.Targets[0], (targeter, targeted) =>
            {
                // EntityShootSkill(targeter, targeted, packet.SkillId);
            });
        }

    }

    public void EntityCastSkill(Entity entity, Entity target, int skillId, int hitTime, int reuseDelay)
    {
        // Debug.LogWarning($"EntityCastSkill: {entity.transform.name} Skill: {skillId}");
        Skill skill = SkillTable.Instance.GetSkill(skillId);
        CastSkill(entity, target, skill, hitTime, reuseDelay);
    }

    public void OnSkillNotAllowed(int skillId)
    {
        Debug.LogWarning($"SkillNotAllowed: ID: {skillId}");
        PlayerStateMachine.Instance.OnSkillNotAllowed(skillId);
    }

    public void OnSkillAllowed(int hitTime)
    {
        PlayerStateMachine.Instance.OnSkillAllowed(hitTime);
    }

    public void EntityCastSkill(Entity entity, int skillId)
    {
        Debug.LogWarning($"EntityCastSkill: {entity.transform.name} Skill: {skillId}");
        Skill skill = SkillTable.Instance.GetSkill(skillId);
        CastSkill(entity, null, skill, -1, -1);
    }

    // Start default skill casting
    private void CastSkill(Entity entity, Entity target, Skill skill, int hitTime, int reuseDelay)
    {
        if (entity == PlayerEntity.Instance)
        {
            OnSkillAllowed(hitTime);
        }

        // Spawn particle
        PooledEffect[] castEffects = ParticleManager.Instance.SpawnCastParticles(entity, skill, hitTime);

        //Play skill cast animation
        if (skill.Skillgrps[0].CastAnimation != SkillCastAnimation.None)
            entity.CastSkill(skill, target, hitTime, reuseDelay, castEffects);

        // Cast skill sound
        if (skill.SkillSoundgrp == null || skill.SkillSoundgrp.SpellEffectSounds == null || skill.SkillSoundgrp.SpellEffectSounds.Length == 0)
        {
            Debug.LogWarning($"Skill {skill.SkillId} doesnt have any SoundGrp or can't find sound EventReference.");
            return;
        }

        EventReference soundReference = skill.SkillSoundgrp.SpellEffectSounds[0].SoundEvent;
        AudioManager.Instance.PlaySound(soundReference, entity.transform.position);
    }

    // Shoot skill projectile
    public void EntityShootSkill(Entity sender, Entity target, Skill skill, float hitTime)
    {
        // Spawn particle
        ParticleManager.Instance.SpawnSkillShotParticle(sender, target, skill, hitTime);

        // Shoot skill sound
        if (skill.SkillSoundgrp == null || skill.SkillSoundgrp.SpellEffectSounds == null || skill.SkillSoundgrp.SpellEffectSounds.Length < 2)
        {
            Debug.LogWarning($"Skill {skill.SkillId} doesnt have any SoundGrp or can't find sound EventReference.");
            return;
        }

        EventReference soundReference = skill.SkillSoundgrp.SpellEffectSounds[1].SoundEvent;

        AudioManager.Instance.PlaySound(soundReference, sender.transform.position);
    }

    // Projectile hit target
    public void SkillProjectileHitTarget(Entity entity, Entity target, Skill skill)
    {
        // Skill has an explosion action ? (was a projectile) ?
        if (skill.SkillEffect.ExplosionActions == null || skill.SkillEffect.ExplosionActions.Count == 0)
        {
            Debug.LogWarning($"Skill {skill.SkillId} doesnt have any explosion action.");
            return;
        }

        // Spawn explosion particle
        ParticleManager.Instance.SpawnSkillExplosionParticle(entity, target, skill);

        if (skill.SkillSoundgrp == null || skill.SkillSoundgrp.SpellEffectSounds == null || skill.SkillSoundgrp.SpellEffectSounds.Length < 3)
        {
            Debug.LogWarning($"Skill {skill.SkillId} doesnt have any SoundGrp or can't find sound EventReference.");
            return;
        }

        EventReference soundReference = skill.SkillSoundgrp.SpellEffectSounds[2].SoundEvent;
        AudioManager.Instance.PlaySound(soundReference, target.transform.position);
    }

    public Task UpdateEntityTarget(int id, int targetId, Vector3 position)
    {
        return _worldSpawner.ExecuteWithEntitiesAsync(id, targetId, (targeter, targeted) =>
        {
            ((NetworkEntityReferenceHolder)targeter.ReferenceHolder).NetworkTransformReceive.SetNewPosition(position);
            targeter.Combat.TargetId = targetId;
            targeter.Combat.Target = targeted;
        });
    }

    public Task UpdateMyTarget(int id, int targetId)
    {
        return _worldSpawner.ExecuteWithEntitiesAsync(id, targetId, (targeter, targeted) =>
        {
            targeter.Combat.TargetId = targetId;
            targeter.Combat.Target = targeted;
        });
    }

    public Task UnsetEntityTarget(int id)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            if (id == PlayerEntity.Instance.Identity.Id)
            {
                TargetManager.Instance.ClearTarget(false);
            }

            e.Combat.TargetId = -1;
            e.Combat.Target = null;
        });
    }

    public Task StatusUpdate(int id, List<StatusUpdatePacket.Attribute> attributes)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            StatusUpdate(e, attributes);
            if (e == PlayerEntity.Instance)
            {
                // InventoryWindow.Instance.RefreshWeight(((PlayerStats)e.Stats).CurrWeight, ((PlayerStats)e.Stats).MaxWeight);
                CharacterInfoWindow.Instance.UpdateValues();
            }
        });
    }

    public Task EntityDied(int id, bool toVillageAllowed, bool toClanHallAllowed, bool toCastleAllowed, bool toSiegeHQAllowed, bool sweepable, bool fixedResAllowed)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            if (id == GameClient.Instance.CurrentPlayerId)
            {
                PlayerController.Instance.ResetDestination(false);
                RestartLocationWindow.Instance.ShowWindowWithParams(toVillageAllowed, toClanHallAllowed, toCastleAllowed, toSiegeHQAllowed, fixedResAllowed);
            }

            e.ReferenceHolder.Combat.OnDeath();
        });
    }

    public Task EntityRevived(int id)
    {
        return _worldSpawner.ExecuteWithEntityAsync(id, e =>
        {
            e.ReferenceHolder.Combat.OnRevive();
        });
    }

    public Task EntityAttacks(Vector3 attackerPosition, int sender, Hit hit, int hitIndex)
    {
        return _worldSpawner.ExecuteWithEntitiesAsync(sender, hit.TargetId, (senderEntity, targetEntity) =>
        {
            hit.Attacker = senderEntity;
            hit.Target = targetEntity;

            // float atkEndTime = Time.time + senderEntity.AnimationController.PAtkSpd / 1000f;
            float atkEndTime = 0;
            if (senderEntity.Gear.WeaponType == WeaponType.bow)
            {
                hit.HitTime = Time.time + senderEntity.AnimationController.PAtkSpd / 1000f;
            }
            else
            {
                hit.HitTime = Time.time + senderEntity.AnimationController.PAtkSpd / (2f * (hitIndex + 1)) / 1000f;
            }

            _hits.Add(hit);

            // Debug.Log($"Hit scheduled in {senderEntity.AnimationController.PAtkSpd / 2f} ms - Now: {Time.time} - HitTime: {hit.HitTime}");
            if (hitIndex > 0)
            {
                return;
            }

            if (sender != GameClient.Instance.CurrentPlayerId)
            {
                NetworkEntityReferenceHolder referenceHolder = (NetworkEntityReferenceHolder)senderEntity.ReferenceHolder;

                // Update attacker target 
                if (referenceHolder.Combat.Target != targetEntity)
                {
                    referenceHolder.Combat.Target = targetEntity;
                    referenceHolder.Combat.AttackTarget = targetEntity;
                }

                // Debug.Log("Attacker position: " + attackerPosition);
                referenceHolder.NetworkTransformReceive.SetNewPosition(attackerPosition, false);

                // User destination does not matter, only move direction does
                if (senderEntity.Identity.EntityType != EntityType.User)
                {
                    referenceHolder.NetworkCharacterControllerReceive.ResetDestination();
                }
            }
            else
            {
                // NetworkTransformShare.Instance.SharePosition();
                PlayerTransformReceive.Instance.SetNewPosition(attackerPosition);
            }

            // Update attacked target 
            if (hit.TargetId != GameClient.Instance.CurrentPlayerId)
            {
                if (((NetworkEntityReferenceHolder)targetEntity.ReferenceHolder).Combat.Target == null)
                {
                    ((NetworkEntityReferenceHolder)targetEntity.ReferenceHolder).Combat.Target = senderEntity;
                    ((NetworkEntityReferenceHolder)targetEntity.ReferenceHolder).Combat.AttackTarget = senderEntity;
                }
            }

            senderEntity.OnStopMoving();
            senderEntity.Combat.AttackOnce(hit.HitTime, atkEndTime, !hit.isMiss(), targetEntity);
        });
    }

    public void StatusUpdate(Entity entity, List<Attribute> attributes)
    {
        // Debug.Log("Word combat: Status update");
        if (entity == null || entity.Status == null || entity.Stats == null)
        {
            Debug.LogWarning("Trying to update a not ready entity.");
            return;
        }

        Status status = entity.Status;
        Stats stats = entity.Stats;

        foreach (Attribute attribute in attributes)
        {
            switch ((AttributeType)attribute.id)
            {
                case AttributeType.LEVEL:
                    stats.Level = attribute.value;
                    break;
                case AttributeType.EXP:
                    ((PlayerStats)stats).Exp = attribute.value;
                    break;
                case AttributeType.STR:
                    ((PlayerStats)stats).Str = (byte)attribute.value;
                    break;
                case AttributeType.DEX:
                    ((PlayerStats)stats).Dex = (byte)attribute.value;
                    break;
                case AttributeType.CON:
                    ((PlayerStats)stats).Con = (byte)attribute.value;
                    break;
                case AttributeType.INT:
                    ((PlayerStats)stats).Int = (byte)attribute.value;
                    break;
                case AttributeType.WIT:
                    ((PlayerStats)stats).Wit = (byte)attribute.value;
                    break;
                case AttributeType.MEN:
                    ((PlayerStats)stats).Men = (byte)attribute.value;
                    break;
                case AttributeType.CUR_HP:
                    status.Hp = attribute.value;
                    break;
                case AttributeType.MAX_HP:
                    stats.MaxHp = attribute.value;
                    break;
                case AttributeType.CUR_MP:
                    status.Mp = attribute.value;
                    break;
                case AttributeType.MAX_MP:
                    stats.MaxMp = attribute.value;
                    break;
                case AttributeType.SP:
                    ((PlayerStats)stats).Sp = attribute.value;
                    break;
                case AttributeType.CUR_LOAD:
                    ((PlayerStats)stats).CurrWeight = attribute.value;
                    break;
                case AttributeType.MAX_LOAD:
                    ((PlayerStats)stats).MaxWeight = attribute.value;
                    break;
                case AttributeType.P_ATK:
                    ((PlayerStats)stats).PAtk = attribute.value;
                    break;
                case AttributeType.ATK_SPD:
                    stats.PAtkSpd = attribute.value;
                    entity.UpdatePAtkSpeed(stats.PAtkSpd);
                    break;
                case AttributeType.P_DEF:
                    ((PlayerStats)stats).PDef = attribute.value;
                    break;
                case AttributeType.P_EVASION:
                    ((PlayerStats)stats).PEvasion = attribute.value;
                    break;
                case AttributeType.P_ACCURACY:
                    ((PlayerStats)stats).PAccuracy = attribute.value;
                    break;
                case AttributeType.P_CRITICAL:
                    ((PlayerStats)stats).PCritical = attribute.value;
                    break;
                case AttributeType.M_EVASION:
                    ((PlayerStats)stats).MEvasion = attribute.value;
                    break;
                case AttributeType.M_ACCURACY:
                    ((PlayerStats)stats).MAccuracy = attribute.value;
                    break;
                case AttributeType.M_CRITICAL:
                    ((PlayerStats)stats).MCritical = attribute.value;
                    break;
                case AttributeType.M_ATK:
                    ((PlayerStats)stats).MAtk = attribute.value;
                    break;
                case AttributeType.CAST_SPD:
                    stats.MAtkSpd = attribute.value;
                    entity.UpdateMAtkSpeed(stats.MAtkSpd);
                    break;
                case AttributeType.M_DEF:
                    ((PlayerStats)stats).MDef = attribute.value;
                    break;
                case AttributeType.PVP_FLAG:
                    break;
                case AttributeType.KARMA:
                    break;
                case AttributeType.CUR_CP:
                    ((PlayerStatus)status).Cp = attribute.value;
                    break;
                case AttributeType.MAX_CP:
                    stats.MaxCp = attribute.value;
                    break;
            }
        }
    }

    #region Maths
    public float CalculateTimeToHitTarget(Entity senderEntity, Entity targetEntity)
    {
        return Vector3.Distance(senderEntity.transform.position, targetEntity.transform.position) / _projectilesSpeed; //16 meters per second
    }

    public float GetRealAttackRange(Entity attacker, Entity target)
    {
        // return attacker.Appearance.CollisionRadius + target.Appearance.CollisionRadius + attacker.Stats.AttackRange;
        // return attacker.Appearance.CollisionRadius + target.Appearance.CollisionRadius;
        return attacker.Stats.AttackRange;
    }

    public float GetInteractRange(Entity attacker, Entity target)
    {
        return Mathf.Min(
            attacker.Appearance.CollisionRadius + target.Appearance.CollisionRadius + 0.4f, // added extra distance to not be too close
            2.857142857142857f); // hardcoded maximum interaction distance in the server
    }
    #endregion

    public void ExAutoSoulshotReceived(int itemId, bool enable)
    {
        _eventProcessor.QueueEvent(() => PlayerShortcuts.Instance.ToggleShortcutItem(itemId, enable));
    }

    public Task OnFightStanceStart(int entityId)
    {
        return _worldSpawner.ExecuteWithEntityAsync(entityId, (e) =>
        {
            e.Combat.FightStanceStart();
        });
    }

    public Task OnFightStanceStop(int entityId)
    {
        return _worldSpawner.ExecuteWithEntityAsync(entityId, (e) =>
        {
            e.Combat.FightStanceStop();
        });
    }

    public void SetupGauge(SetupGaugePacket.GaugeColor color, int time, int maxTime)
    {
        _eventProcessor.QueueEvent(() =>
        {
            NameplatesManagerGame.Instance.StartCasting(color, maxTime != time ? maxTime - time : time);
        });
    }

    public void EntityShootArrow(Entity caster, Entity target, float hitTimeSec, bool hitSuccess)
    {
        ParticleManager.Instance.SpawnArrowProjectile(caster, target, hitTimeSec, hitSuccess);
    }

    public Task RelationChanged(int owner, int karma, int pvpFlag)
    {
        return _worldSpawner.ExecuteWithEntityAsync(owner, e =>
        {
            e.Stats.Karma = karma;
            e.Identity.PvpFlag = pvpFlag;
        });
    }
}
