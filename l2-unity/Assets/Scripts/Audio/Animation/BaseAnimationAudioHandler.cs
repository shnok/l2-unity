using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationAudioHandler : MonoBehaviour
{
    [SerializeField] protected string _npcClassName;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected float[] _walkStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] protected float[] _runStepRatios = new float[] { 0.25f, 0.75f };
    [SerializeField] protected float _swishRatio = 0.25f;
    [SerializeField] protected float _atkRatio = 0f;
    [SerializeField] protected float _atkWaitRatio = 0f;
    [SerializeField] protected float _deathRatio = 0f;
    [SerializeField] protected float _fallRatio = 0.5f;
    [SerializeField] private int _waitSoundChance = 5;
    public int WaitSoundChance { get { return _waitSoundChance; } }
    public float[] WalkStepRatios { get { return _walkStepRatios; } }
    public float[] RunStepRatios { get { return _runStepRatios; } }
    public float SwishRatio { get { return _swishRatio; } }
    public float AtkWaitRatio { get { return _atkWaitRatio; } }
    public float AtkRatio { get { return _atkRatio; } }
    public float DeathRatio { get { return _deathRatio; } }
    public float FallRatio { get { return _fallRatio; } }

    private void Start() {
        _npcClassName = GetComponent<Entity>().Identity.NpcClass;
        if (!string.IsNullOrEmpty(_npcClassName)) {
            string[] parts = _npcClassName.Split('.');
            if (parts.Length > 1) {
                _npcClassName = parts[1].ToLower();
            }
        }

        if (string.IsNullOrEmpty(_npcClassName)) {
            Debug.LogWarning("AnimationAudioHandler could not load npc class name");
            this.enabled = false;
        }

        if (_animator == null) {
            _animator = GetComponent<Animator>();
        }

        Initialize();
    }

    protected virtual void Initialize() { }
}
