using System;
using UnityEngine;

[System.Serializable]
public class PooledEffect
{
    [SerializeField] private string _effectClass;
    [SerializeField] private float _effectDurationSec = 15;
    [SerializeField] private float _maximumInactiveTimeSec = 60;
    [SerializeField] private float _startTime;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private int _hitEffectIndex;

    [SerializeField] private Entity _caster;
    [SerializeField] private Entity _target;
    [SerializeField] private Skill _skill;
    [SerializeField] private Vector3 _startingPosition;
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _hitTime;
    [SerializeField] private bool _hitSuccess;
    [SerializeField] private Action _resetCallback;
    [SerializeField] private bool _isArrow;

    public float StartTime { get { return _startTime; } set { _startTime = value; } }
    public float EffectDurationSec { get { return _effectDurationSec; } set { _effectDurationSec = value; } }
    public float MaximumInactiveTimeSec { get { return _maximumInactiveTimeSec; } set { _maximumInactiveTimeSec = value; } }
    public GameObject GameObject { get { return _gameObject; } set { _gameObject = value; } }
    public int HitEffectIndex { get { return _hitEffectIndex; } set { _hitEffectIndex = value; } }
    public string EffectClass { get { return _effectClass; } set { _effectClass = value; } }

    public Entity Caster { get { return _caster; } set { _caster = value; } }
    public Entity Target { get { return _target; } set { _target = value; } }
    public Skill Skill { get { return _skill; } set { _skill = value; } }
    public float HitTime { get { return _hitTime; } set { _hitTime = value; } }
    public bool HitSuccess { get { return _hitSuccess; } set { _hitSuccess = value; } }
    public bool IsArrow { get { return _isArrow; } set { _isArrow = value; } }
    public Vector3 StartingPosition { get { return _startingPosition; } set { _startingPosition = value; } }
    public Vector3 TargetPosition { get { return _endPosition; } set { _endPosition = value; } }
    public Action ResetTimerCallback { get { return _resetCallback; } set { _resetCallback = value; } }

    public void Restart()
    {
        if (_resetCallback != null)
        {
            _resetCallback();
        }
        else
        {
            Debug.LogWarning($"[{_effectClass}] Reset callback missing.");
        }
    }
}