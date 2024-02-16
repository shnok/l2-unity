using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(MonsterEntity), typeof(Animator))]
public class MonsterAnimationAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] private string _monsterName;

    protected override void Initialize() {
        base.Initialize();

        string npcClassName = GetComponent<Entity>().Identity.NpcClass;
        if (!string.IsNullOrEmpty(npcClassName)) {
            string[] parts = npcClassName.Split('.');
            if (parts.Length > 1) {
                _monsterName = parts[1].ToLower();
            }
        }

        if (string.IsNullOrEmpty(_monsterName)) {
            Debug.LogWarning("AnimationAudioHandler could not load monster name name");
            this.enabled = false;
        }
    }

    public void PlaySound(MonsterSoundEvent soundEvent) {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _monsterName, transform.position);
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
