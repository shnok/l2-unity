using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NpcEntity), typeof(Animator))]
public class MonsterAnimationAudioHandler : BaseAnimationAudioHandler
{
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
