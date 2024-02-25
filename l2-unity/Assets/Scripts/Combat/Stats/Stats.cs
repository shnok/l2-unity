using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats {
    [SerializeField] private int _speed;
    [SerializeField] private float _scaledSpeed;
    [SerializeField] private int _pAtkSpd;
    [SerializeField] private int _mAtkSpd;
    public int Speed { get => _speed; set => _speed = value; }
    public float ScaledSpeed { get => _scaledSpeed; set => _scaledSpeed = value; }
    public int PAtkSpd { get => _pAtkSpd; set => _pAtkSpd = value; }
    public int MAtkSpd { get => _mAtkSpd; set => _mAtkSpd = value; }
}
