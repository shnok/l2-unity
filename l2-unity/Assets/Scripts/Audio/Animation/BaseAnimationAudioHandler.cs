using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class BaseAnimationAudioHandler : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _entityReferenceHolder;
    [SerializeField] protected float[] _walkStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] protected float[] _runStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] protected float _swishRatio = 0.25f;
    [SerializeField] protected float _atkRatio = 0f;
    [SerializeField] protected float _atkWaitRatio = 0f;
    [SerializeField] protected float _deathRatio = 0f;
    [SerializeField] protected float _fallRatio = 0.5f;
    [SerializeField] private int _waitSoundChance = 5;
    [SerializeField] private int _runBreathChance = 5;
    public int WaitSoundChance { get { return _waitSoundChance; } }
    public int RunBreathChance { get { return _runBreathChance; } }
    public float[] WalkStepRatios { get { return _walkStepRatios; } }
    public float[] RunStepRatios { get { return _runStepRatios; } }
    public float SwishRatio { get { return _swishRatio; } }
    public float AtkWaitRatio { get { return _atkWaitRatio; } }
    public float AtkRatio { get { return _atkRatio; } }
    public float DeathRatio { get { return _deathRatio; } }
    public float FallRatio { get { return _fallRatio; } }
    public Animator Animator { get { return _entityReferenceHolder.Animator; } }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (_entityReferenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _entityReferenceHolder = GetComponent<EntityReferenceHolder>();
        }
    }

    // Entity sounds
    public virtual void PlaySound(EntitySoundEvent soundEvent)
    {
    }


    public void PlaySoundAtRatio(EntitySoundEvent soundEvent, float ratio)
    {
        if (ratio == 0)
        {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(EntitySoundEvent soundEvent, float ratio)
    {
        while ((Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio)
        {
            yield return null;
        }

        PlaySound(soundEvent);
    }

    public void PlaySoundAtRatio(EventReference soundEvent, float ratio)
    {
        if (ratio == 0)
        {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(EventReference soundEvent, float ratio)
    {
        while ((Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio)
        {
            yield return null;
        }

        PlaySound(soundEvent);
    }

    public virtual void PlaySound(EventReference soundEvent)
    {
        AudioManager.Instance.PlaySound(soundEvent, transform.position);
    }

    // Item sounds
    public virtual void PlaySound(ItemSoundEvent soundEvent)
    {
        AudioManager.Instance.PlayItemSound(soundEvent, transform.position);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio)
    {
        if (ratio == 0)
        {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }


    public IEnumerator PlaySoundAtRatioCoroutine(ItemSoundEvent soundEvent, float ratio)
    {
        while ((Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio)
        {
            yield return null;
        }

        PlaySound(soundEvent);
    }


    public virtual void PlayCritSound()
    {
        AudioManager.Instance.Play3DSoundByReferenceName("SkillSound/critical_hit", transform.position);
    }

    public virtual void PlaySoulshotSound()
    {
        AudioManager.Instance.Play3DSoundByReferenceName("SkillSound/soul_shot_shot", transform.position);
    }

    public virtual void PlayDefenseSound()
    {
    }

    public virtual void PlayDamageSound()
    {
    }

    public virtual void PlaySwishSound()
    {
    }

    protected EventReference GetRandomEvent(List<EventReference> events)
    {
        if (events == null)
        {
            return new EventReference();
        }

        return events[UnityEngine.Random.Range(0, events.Count)];
    }
}
