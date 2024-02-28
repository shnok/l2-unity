using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : Stats {
    [SerializeField] private float _attackRange;
    [SerializeField] private byte _con;
    [SerializeField] private byte _dex;
    [SerializeField] private byte _str;
    [SerializeField] private byte _wit;
    [SerializeField] private byte _men;
    [SerializeField] private byte _int;
    public float AttackRange { get => _attackRange; set => _attackRange = value; }
    public byte Con { get { return _con; } set { _con = value; } }
    public byte Dex { get { return _dex; } set { _dex = value; } }
    public byte Str { get { return _str; } set { _str = value; } }
    public byte Wit { get { return _wit; } set { _wit = value; } }
    public byte Men { get { return _men; } set { _men = value; } }
    public byte Int { get { return _int; } set { _int = value; } }

    // ... TODO: add extra stats
}
