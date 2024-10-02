using System.Collections;
using UnityEngine;

public class MonsterAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] private string _monsterName;

    protected override void Initialize()
    {
        base.Initialize();

        if (string.IsNullOrEmpty(_monsterName))
        {
            Debug.LogWarning($"[{transform.name}] Monster name was not assigned, please pre-assign it to avoid unecessary load.");
        }

        string npcClassName = GetComponent<Entity>().Identity.NpcClass;
        if (!string.IsNullOrEmpty(npcClassName))
        {
            string[] parts = npcClassName.Split('.');
            if (parts.Length > 1)
            {
                _monsterName = parts[1].ToLower();
            }
        }

        if (string.IsNullOrEmpty(_monsterName))
        {
            Debug.LogWarning("AnimationAudioHandler could not load monster name name");
            this.enabled = false;
        }
    }

    public override void PlaySound(EntitySoundEvent soundEvent)
    {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _monsterName, transform.position);
    }
}
