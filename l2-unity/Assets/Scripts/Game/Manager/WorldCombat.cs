using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using static StatusUpdatePacket;

public class WorldCombat : MonoBehaviour
{
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
    }

    void OnDestroy()
    {
        _instance = null;
    }


    public void InflictAttack(Entity target, Hit hit)
    {
        ApplyDamage(target, hit);
    }

    public void InflictAttack(Entity attacker, Entity target, Hit hit)
    {
        ApplyDamage(target, hit);

        if (hit.isMiss())
        {
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

    public void EntityStartAutoAttacking(Entity entity)
    {
        if (entity == PlayerEntity.Instance)
        {
            PlayerStateMachine.Instance.OnActionAllowed();
        }
        else
        {
            entity.Combat.StartAutoAttacking();
        }
    }

    public void EntityStopAutoAttacking(Entity entity)
    {
        if (entity == PlayerEntity.Instance)
        {
            PlayerStateMachine.Instance.OnStopAutoAttack();
        }
        else
        {
            entity.Combat.StopAutoAttacking();
        }
    }

    public void EntityCastSkill(Entity entity, int skillId)
    {
        Skill skill = SkillTable.Instance.GetSkill(skillId);
        CastSkill(entity, skill);
    }

    public void EntityCastSkill(Entity entity, Skill skill)
    {
        CastSkill(entity, skill);
    }

    private void CastSkill(Entity entity, Skill skill)
    {
        // Spawn particle
        ParticleManager.Instance.SpawnSkillParticles(entity, skill);

        // Cast skill sound
        if (skill.SkillSoundgrp == null || skill.SkillSoundgrp.SpellEffectSounds == null || skill.SkillSoundgrp.SpellEffectSounds.Length == 0)
        {
            Debug.LogWarning("Skill {} doesnt have any SoundGrp or can't find sound EventReference.");
            return;
        }

        EventReference soundReference = skill.SkillSoundgrp.SpellEffectSounds[0].SoundEvent;
        AudioManager.Instance.PlaySound(soundReference, entity.transform.position);
    }

    // OBSOLETE
    // private void ParticleImpact(Transform attacker, Transform target)
    // {
    //     // Calculate the position and rotation based on attacker
    //     var heading = attacker.position - target.position;
    //     float angle = Vector3.Angle(heading, target.forward);
    //     Vector3 cross = Vector3.Cross(heading, target.forward);
    //     if (cross.y >= 0) angle = -angle;
    //     Vector3 direction = Quaternion.Euler(0, angle, 0) * target.forward;

    //     float particleHeight = target.GetComponent<Entity>().Appearance.CollisionHeight * 1.25f;
    //     GameObject go = (GameObject)Instantiate(
    //         _impactParticle,
    //         target.position + direction * 0.15f + Vector3.up * particleHeight,
    //         Quaternion.identity);

    //     go.transform.LookAt(attacker);
    //     go.transform.eulerAngles = new Vector3(0, go.transform.eulerAngles.y + 180f, 0);
    // }

    public void StatusUpdate(Entity entity, List<Attribute> attributes)
    {
        // Debug.Log("Word combat: Status update");
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
                    //TODO: Where speed?
            }
        }
    }
}
