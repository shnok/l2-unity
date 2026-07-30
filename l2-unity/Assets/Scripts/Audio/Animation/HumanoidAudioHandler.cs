using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class HumanoidAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] protected CharacterModelSound _race;
    [SerializeField] protected SurfaceDetector _surfaceDetector;
    [SerializeField] protected Chargrp _chargrp;

    protected override void Initialize()
    {
        base.Initialize();
        if (_surfaceDetector == null)
        {
            Debug.LogWarning($"[{transform.name}] SurfaceDetector was not assigned, please pre-assign it to avoid unecessary load.");
            _surfaceDetector = GetComponent<SurfaceDetector>();
        }

        _chargrp = ChargrpTable.Instance.CharGrps[_entityReferenceHolder.Entity.RaceId];
    }

    public override void PlaySound(EntitySoundEvent soundEvent)
    {
        if (soundEvent == EntitySoundEvent.Step)
        {
            PlayStepSound();
            return;
        }

        AudioManager.Instance.PlayCharacterSound(soundEvent, _race, transform.position);
    }

    private void PlayStepSound()
    {
        string currentSurface = _surfaceDetector.GetSurfaceTag();
        AudioManager.Instance.PlayStepSound(currentSurface, transform.position);
    }

    public override void PlaySwishSound()
    {
        WeaponType weaponType = _entityReferenceHolder.Gear.WeaponType;
        AudioManager.Instance.PlaySwishSound(weaponType, transform.position);
    }

    public override void PlayDamageSound()
    {
        PlaySound(EntitySoundEvent.Dmg);
    }

    public override void PlayDefenseSound()
    {
        EventReference soundEvent = GetRandomEvent(_chargrp.DefenseSoundsEvents);
        //TODO: Adapt based on armor type (heavy light robe)
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public virtual void PlayAtkSound()
    {
        // Dont always play atk voiceline (20 % chance?)
        if (!RandomUtils.ShouldEventHappen(20))
        {
            return;
        }

        WeaponAnimType weaponAnim = ((NewHumanoidAnimationController)_entityReferenceHolder.NewAnimationController).WeaponAnim;
        switch (weaponAnim)
        {
            case WeaponAnimType.hand:
            case WeaponAnimType.dual:
            case WeaponAnimType.shield:
            case WeaponAnimType._1HS:
                // PlaySoundAtRatio(EntitySoundEvent.Atk_1H, _atkRatio);
                PlaySound(EntitySoundEvent.Atk_1H);
                break;
            case WeaponAnimType._2HS:
            case WeaponAnimType.pole:
            case WeaponAnimType.bow:
                // PlaySoundAtRatio(EntitySoundEvent.Atk_2H, _atkRatio);
                PlaySound(EntitySoundEvent.Atk_2H);
                break;
        }
    }

    public virtual void PlaySpAtkSound()
    {
        WeaponAnimType weaponAnim = ((NewHumanoidAnimationController)_entityReferenceHolder.NewAnimationController).WeaponAnim;
        switch (weaponAnim)
        {
            case WeaponAnimType.hand:
            case WeaponAnimType.dual:
            case WeaponAnimType.bow:
            case WeaponAnimType.shield:
                break;
            case WeaponAnimType._1HS:
                PlaySound(EntitySoundEvent.Atk_1H_S);
                break;
            case WeaponAnimType._2HS:
                PlaySound(EntitySoundEvent.Atk_2H_S);
                break;
            case WeaponAnimType.pole:
                PlaySound(EntitySoundEvent.Atk_pole_S);
                break;
        }
    }


    public virtual void PlayPreAtkSound(int spAtkIndex)
    {
        WeaponAnimType weaponAnim = ((NewHumanoidAnimationController)_entityReferenceHolder.NewAnimationController).WeaponAnim;

        if (weaponAnim == WeaponAnimType.bow)
        {
            return; // bow attacks dont have voicelines
        }

        if (spAtkIndex == 0) //TODO: Verify and update
        {
            PlaySound(EntitySoundEvent.PreAtk_1);
        }
    }

    public override void PlaySkillVoice(string voice)
    {
        string eventName = $"event:/ChrSound/Skill/{voice}";
        // Debug.LogWarning(eventName);
        EventReference er = RuntimeManager.PathToEventReference(eventName);
        if (!er.IsNull)
        {
            AudioManager.Instance.PlaySound(er, transform.position);
        }
    }

}
