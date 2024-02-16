using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationAudioHandler : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
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

    private void Start() {
        Initialize();
    }

    public void PlaySound(ItemSoundEvent soundEvent) {
        AudioManager.Instance.PlayItemSound(soundEvent, transform.position);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio) {
        if (ratio == 0) {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(ItemSoundEvent soundEvent, float ratio) {
        while ((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }

    protected virtual void Initialize() {
        if (_animator == null) {
            _animator = GetComponent<Animator>();
        }
    }
}
