using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : Stats {

    [SerializeField] private int _mDef;
    [SerializeField] private int _pDef;
    [SerializeField] private int _shieldDef;

    [SerializeField] private int _exp;
    [SerializeField] private int _maxExp;
    [SerializeField] private int _sp;
    [SerializeField] private int _karma;

    [SerializeField] private float _attackRange;
    [SerializeField] private byte _con;
    [SerializeField] private byte _dex;
    [SerializeField] private byte _str;
    [SerializeField] private byte _wit;
    [SerializeField] private byte _men;
    [SerializeField] private byte _int;

    public int MDef { get { return _mDef; } set { _mDef = value; } }
    public int PDef { get { return _pDef; } set { _pDef = value; } }
    public int Exp { get { return _exp; } set { _exp = value; } }
    public int MaxExp { get { return _maxExp; } set { _maxExp = value; } }
    public int Sp { get { return _sp; } set { _sp = value; } }
    public int Karma { get { return _karma; } set { _karma = value; } }
    public int ShieldDef { get { return _shieldDef; } set { _shieldDef = value; } }

    public float AttackRange { get => _attackRange; set => _attackRange = value; }
    public byte Con { get { return _con; } set { _con = value; } }
    public byte Dex { get { return _dex; } set { _dex = value; } }
    public byte Str { get { return _str; } set { _str = value; } }
    public byte Wit { get { return _wit; } set { _wit = value; } }
    public byte Men { get { return _men; } set { _men = value; } }
    public byte Int { get { return _int; } set { _int = value; } }

    // ... TODO: add extra stats
}
