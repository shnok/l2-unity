using FMODUnity;
using UnityEngine;

public class NpcAudioHandler : HumanoidAudioHandler
{
    [SerializeField] private string _npcClassName;
    [SerializeField] protected Npcgrp _npcgrp;

    protected override void Initialize()
    {
        base.Initialize();

        // if (_race == CharacterModelSound.Default)
        // {
        //     Debug.LogWarning($"[{transform.name}] Character race was not selected, please pre-assign it to avoid unecessary load.");
        //     string[] raceParts = _npcClassName.Split("_");
        //     if (raceParts.Length > 1)
        //     {
        //         string raceName = raceParts[raceParts.Length - 1];
        //         _race = CharacterModelSoundParser.ParseRace(raceName);
        //     }

        //     _npcClassName = GetComponent<Entity>().Identity.NpcClass;
        //     if (!string.IsNullOrEmpty(_npcClassName))
        //     {
        //         string[] parts = _npcClassName.Split('.');
        //         if (parts.Length > 1)
        //         {
        //             _npcClassName = parts[1].ToLower();
        //         }
        //     }

        //     if (string.IsNullOrEmpty(_npcClassName))
        //     {
        //         Debug.LogWarning("AnimationAudioHandler could not load npc class name");
        //         this.enabled = false;
        //     }
        // }

        _npcgrp = NpcgrpTable.Instance.Npcgrps[_entityReferenceHolder.Entity.Identity.NpcId];
        // _chargrp = ChargrpTable.Instance.CharGrps[_entityReferenceHolder.Entity.RaceId];
    }

    public override void PlaySound(EntitySoundEvent soundEvent)
    {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcgrp.ClassName, transform.position);
    }

    public override void PlaySwishSound()
    {
        EventReference soundEvent = GetRandomEvent(_npcgrp.AttackSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public override void PlayDamageSound()
    {
        EventReference soundEvent = GetRandomEvent(_npcgrp.DamageSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public override void PlayDefenseSound()
    {
        EventReference soundEvent = GetRandomEvent(_npcgrp.DefenseSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public override void PlayAtkSoundAtRatio(float ratio)
    {
        EventReference soundEvent = GetRandomEvent(_npcgrp.AttackSoundsEvents);
        if (!soundEvent.IsNull)
        {
            PlaySoundAtRatio(soundEvent, ratio);
        }
    }
}
