using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats {
    [SerializeField] private int _level;
    [SerializeField] private int _speed;
    [SerializeField] private float _scaledSpeed;
    [SerializeField] private int _pAtkSpd;
    [SerializeField] private int _mAtkSpd;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _maxCp;

    public int Level { get => _level; set => _level = value; }
    public int Speed { get => _speed; set => _speed = value; }
    public float ScaledSpeed { get => _scaledSpeed; set => _scaledSpeed = value; }
    public int PAtkSpd { get => _pAtkSpd; set => _pAtkSpd = value; }
    public int MAtkSpd { get => _mAtkSpd; set => _mAtkSpd = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int MaxMp { get => _maxMp; set => _maxMp = value; }
    public int MaxCp { get => _maxCp; set => _maxCp = value; }
}
