using UnityEngine;

public class NpcAnimationAudioHandler : HumanoidAudioHandler
{
    [SerializeField] private string _npcClassName;

    protected override void Initialize()
    {
        base.Initialize();

        if (_race == CharacterModelSound.Default)
        {
            string[] raceParts = _npcClassName.Split("_");
            if (raceParts.Length > 1)
            {
                string raceName = raceParts[raceParts.Length - 1];
                _race = CharacterModelSoundParser.ParseRace(raceName);
            }

            _npcClassName = GetComponent<Entity>().Identity.NpcClass;
            if (!string.IsNullOrEmpty(_npcClassName))
            {
                string[] parts = _npcClassName.Split('.');
                if (parts.Length > 1)
                {
                    _npcClassName = parts[1].ToLower();
                }
            }

            if (string.IsNullOrEmpty(_npcClassName))
            {
                Debug.LogWarning("AnimationAudioHandler could not load npc class name");
                this.enabled = false;
            }
        }
    }
}
