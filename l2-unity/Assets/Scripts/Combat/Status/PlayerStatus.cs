using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatus : Status {
    [SerializeField] private int _cp;
    [SerializeField] private int _maxCp;
    [SerializeField] private int _mp;
    [SerializeField] private int _maxMp;
    [SerializeField] private float _attackRange;

    public int Mp { get => _mp; set => _mp = value; }
    public int MaxMp { get => _maxMp; set => _maxMp = value; }
    public int Cp { get => _cp; set => _cp = value; }
    public int MaxCp { get => _maxCp; set => _maxCp = value; }
    public float AttackRange { get => _attackRange; set => _attackRange = value; }

    public PlayerStatus() {}
}