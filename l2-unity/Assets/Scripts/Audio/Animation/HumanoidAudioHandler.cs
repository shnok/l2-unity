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

    public void PlaySoundFromAnimationClip(int type)
    {
        EntitySoundEvent soundEvent = (EntitySoundEvent)type;
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
        EventReference soundEvent = GetRandomEvent(_chargrp.DamageSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public override void PlayDefenseSound()
    {
        EventReference soundEvent = GetRandomEvent(_chargrp.DefenseSoundsEvents);
        if (!soundEvent.IsNull)
        {
            AudioManager.Instance.PlaySound(soundEvent, transform.position);
        }
    }

    public virtual void PlayAtkSoundAtRatio(float ratio)
    {
        WeaponAnimType weaponAnim = ((HumanoidAnimationController)_entityReferenceHolder.AnimationController).WeaponAnim;
        switch (weaponAnim)
        {
            case WeaponAnimType.hand:
            case WeaponAnimType.dual:
            case WeaponAnimType.shield:
            case WeaponAnimType._1HS:
                PlaySoundAtRatio(EntitySoundEvent.Atk_1H, ratio);
                break;
            case WeaponAnimType._2HS:
            case WeaponAnimType.pole:
            case WeaponAnimType.bow:
                PlaySoundAtRatio(EntitySoundEvent.Atk_2H, ratio);
                break;
        }
    }
}
