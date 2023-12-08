using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(NpcEntity), typeof(Animator))]
public class MonsterAnimationAudioHandler : MonoBehaviour
{
    public string npcClassName;
    public Animator animator;

    private void Start() {
        npcClassName = GetComponent<NpcEntity>().Identity.NpcClass;
        if(!string.IsNullOrEmpty(npcClassName)) {
            string[] parts = npcClassName.Split('.');
            if(parts.Length > 1) {
                npcClassName = parts[1].ToLower();
            }
        }

        if(string.IsNullOrEmpty(npcClassName)) {
            Debug.LogWarning("MonsterAnimationAudioHandler could not load npc class name");
            this.enabled = false;
        }

        if(animator == null) {
            animator = GetComponent<Animator>();
        }
    }

    public void PlaySound(MonsterSoundEvent soundEvent) {
        AudioManager.GetInstance().PlayMonsterSound(soundEvent, npcClassName, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        MonsterSoundEvent soundEvent = (MonsterSoundEvent) type;
        AudioManager.GetInstance().PlayMonsterSound(soundEvent, npcClassName, transform.position);
    }

    public void PlaySoundAtRatio(MonsterSoundEvent soundEvent, float ratio) {
        Debug.LogWarning("PlaySoundAtRatio " + soundEvent + " " + ratio);
        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(MonsterSoundEvent soundEvent, float ratio) {
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }
}
