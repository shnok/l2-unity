using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    [SerializeField] private float _masterVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _musicVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _SFXVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _UIVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _ambientVolume = 1;
    [SerializeField] private bool _muteWhenNotFocused = true;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _SFXBus;
    private Bus _UIBus;
    private Bus _ambientBus;

    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        SetBuses();
    }

    private void SetBuses() {
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _SFXBus = RuntimeManager.GetBus("bus:/SFX");
        _UIBus = RuntimeManager.GetBus("bus:/UI");
        _ambientBus = RuntimeManager.GetBus("bus:/Ambient");
    }

    private void Update() {
        if(_muteWhenNotFocused && !Application.isFocused) {
            _masterBus.setVolume(0);
            _musicBus.setVolume(0);
            _SFXBus.setVolume(0);
            _UIBus.setVolume(0);
            _ambientBus.setVolume(0);
        } else {
            _masterBus.setVolume(_masterVolume * 0.75f);
            _musicBus.setVolume(_musicVolume * 0.7f);
            _SFXBus.setVolume(_SFXVolume);
            _UIBus.setVolume(_UIVolume);
            _ambientBus.setVolume(_ambientVolume * 0.45f);
        }
    }

    public void PlayMonsterSound(MonsterSoundEvent monsterSoundEvent, string npcClassName, Vector3 position) {
        string eventKey = monsterSoundEvent.ToString().ToLower();
        EventReference er = RuntimeManager.PathToEventReference("event:/MonSound/" + npcClassName + "/" + eventKey);
        if(!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlayCharacterSound(CharacterSoundEvent characterSoundEvent, CharacterRace characterRace, Vector3 position) {
        string eventKey = characterSoundEvent.ToString();
        EventReference er = RuntimeManager.PathToEventReference("event:/ChrSound/" + characterRace + "/" + characterRace + "_" + eventKey);
        if (!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlayUISound(string soundName) {
        EventReference er = RuntimeManager.PathToEventReference("event:/InterfaceSound/" + soundName);
        if (!er.IsNull) {
            PlaySound(er);
        }
    }

    public void PlayStepSound(string surfaceTag, Vector3 position) {
        string eventKey;
        surfaceTag = surfaceTag.ToLower();

        switch(surfaceTag) {
            case "dirt":
                eventKey = surfaceTag + "_run";
                break;
            case "stone":
                eventKey = surfaceTag + "_run";
                break;
            case "wood":
                eventKey = surfaceTag + "_run";
                break;
            default:
                eventKey = "default_run";
                break;

        }

        EventReference er = RuntimeManager.PathToEventReference("event:/StepSound/" + eventKey);
        if(!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlayHitSound(bool criticalHit, Vector3 position) {
        EventReference er;
        if (criticalHit) {
            er = RuntimeManager.PathToEventReference("event:/SkillSound/critical_hit");
        } else {
            er = RuntimeManager.PathToEventReference("event:/ItemSound/armor_hit_underwear");
        }

        if (!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlayItemSound(ItemSoundEvent itemSoundEvent, Vector3 position) {
        EventReference er;
        er = RuntimeManager.PathToEventReference("event:/ItemSound/" + itemSoundEvent.ToString());
       

        if (!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlaySound(EventReference sound, Vector3 postition) {
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound) {
        RuntimeManager.PlayOneShot(sound);
    }
}
