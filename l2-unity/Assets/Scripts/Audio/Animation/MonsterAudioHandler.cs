using System.Collections;
using FMODUnity;
using UnityEngine;

public class MonsterAudioHandler : BaseAnimationAudioHandler
{
    // [SerializeField] private string _monsterName;
    [SerializeField] protected Npcgrp _npcgrp;

    protected override void Initialize()
    {
        base.Initialize();

        // if (string.IsNullOrEmpty(_monsterName))
        // {
        //     Debug.LogWarning($"[{transform.name}] Monster name was not assigned, please pre-assign it to avoid unecessary load.");
        // }

        // string npcClassName = GetComponent<Entity>().Identity.NpcClass;
        // if (!string.IsNullOrEmpty(npcClassName))
        // {
        //     string[] parts = npcClassName.Split('.');
        //     if (parts.Length > 1)
        //     {
        //         _monsterName = parts[1].ToLower();
        //     }
        // }

        // if (string.IsNullOrEmpty(_monsterName))
        // {
        //     Debug.LogWarning("AnimationAudioHandler could not load monster name name");
        //     this.enabled = false;
        // }

        _npcgrp = NpcgrpTable.Instance.Npcgrps[_entityReferenceHolder.Entity.Identity.NpcId];
    }

    public override void PlaySound(EntitySoundEvent soundEvent)
    {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcgrp.ClassName, transform.position);
    }

    public override void PlaySwishSound()
    {
        PlaySound(EntitySoundEvent.Swish);
    }

    public override void PlayDamageSound()
    {
        PlaySound(EntitySoundEvent.Dmg);
    }

    public override void PlayDefenseSound()
    {
        EventReference soundEvent = GetRandomEvent(_npcgrp.DefenseSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }
}
