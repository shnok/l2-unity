using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NpcEntity), typeof(Animator))]
public class MonsterAnimationAudioHandler : MonoBehaviour
{
    [SerializeField] private string _npcClassName;
    [SerializeField] private Animator _animator;
    [SerializeField] private float[] _walkStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] private float[] _runStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] private float _swishRatio = 0.25f;
    [SerializeField] private float _atkRatio = 0f;
    [SerializeField] private float _atkWaitRatio = 0f;
    [SerializeField] private float _deathRatio = 0f;
    [SerializeField] private float _fallRatio = 0.7f;
    [SerializeField] private int _waitSoundChance = 5;

    public float[] WalkStepRatios { get { return _walkStepRatios; } }
    public float[] RunStepRatios { get { return _runStepRatios; } }
    public float SwishRatio { get { return _swishRatio; } }
    public float AtkWaitRatio { get { return _atkWaitRatio; } }
    public float AtkRatio { get { return _atkRatio; } }
    public float DeathRatio { get { return _deathRatio; } }
    public float FallRatio { get { return _fallRatio; } }
    public int WaitSoundChance { get { return _waitSoundChance; } }

    private void Start() {
        _npcClassName = GetComponent<NpcEntity>().Identity.NpcClass;
        if(!string.IsNullOrEmpty(_npcClassName)) {
            string[] parts = _npcClassName.Split('.');
            if(parts.Length > 1) {
                _npcClassName = parts[1].ToLower();
            }
        }

        if(string.IsNullOrEmpty(_npcClassName)) {
            Debug.LogWarning("MonsterAnimationAudioHandler could not load npc class name");
            this.enabled = false;
        }

        if(_animator == null) {
            _animator = GetComponent<Animator>();
        }
    }

    public void PlaySound(MonsterSoundEvent soundEvent) {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcClassName, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        MonsterSoundEvent soundEvent = (MonsterSoundEvent) type;
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcClassName, transform.position);
    }

    public void PlaySoundAtRatio(MonsterSoundEvent soundEvent, float ratio) {
        if(ratio == 0) {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(MonsterSoundEvent soundEvent, float ratio) {
        while((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }
}
