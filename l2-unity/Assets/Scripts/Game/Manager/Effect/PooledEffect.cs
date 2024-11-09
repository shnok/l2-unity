using System;
using UnityEngine;

[System.Serializable]
public class PooledEffect
{
    [SerializeField] private string _effectClass;
    [SerializeField] private float _effectDurationSec;
    [SerializeField] private float _maximumInactiveTimeSec;
    [SerializeField] private float _startTime;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private int _hitEffectIndex;

    [SerializeField] private Entity _caster;
    [SerializeField] private Entity _target;
    [SerializeField] private Vector3 _startingPosition;
    [SerializeField] private float _hitTime;
    [SerializeField] private bool _hitSuccess;

    public float StartTime { get { return _startTime; } set { _startTime = value; } }
    public float EffectDurationSec { get { return _effectDurationSec; } set { _effectDurationSec = value; } }
    public float MaximumInactiveTimeSec { get { return _maximumInactiveTimeSec; } set { _maximumInactiveTimeSec = value; } }
    public GameObject GameObject { get { return _gameObject; } set { _gameObject = value; } }
    public int HitEffectIndex { get { return _hitEffectIndex; } set { _hitEffectIndex = value; } }
    public string EffectClass { get { return _effectClass; } set { _effectClass = value; } }

    public Entity Caster { get { return _caster; } set { _caster = value; } }
    public Entity Target { get { return _target; } set { _target = value; } }
    public float HitTime { get { return _hitTime; } set { _hitTime = value; } }
    public bool HitSuccess { get { return _hitSuccess; } set { _hitSuccess = value; } }
    public Vector3 StartingPosition { get { return _startingPosition; } set { _startingPosition = value; } }
}