using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : Stats {


    [SerializeField] private int _pAtk;
    [SerializeField] private int _mAtk;
    [SerializeField] private int _pEvasion;
    [SerializeField] private int _mEvasion;
    [SerializeField] private int _pAccuracy;
    [SerializeField] private int _mAccuracy;
    [SerializeField] private int _pCritical;
    [SerializeField] private int _mCritical;
    [SerializeField] private int _mDef;
    [SerializeField] private int _pDef;
    [SerializeField] private int _shieldDef;

    [SerializeField] private int _exp;
    [SerializeField] private int _maxExp;
    [SerializeField] private int _sp;

    [SerializeField] private int _currWeight;
    [SerializeField] private int _maxWeight;

    [SerializeField] private float _attackRange;
    [SerializeField] private byte _con;
    [SerializeField] private byte _dex;
    [SerializeField] private byte _str;
    [SerializeField] private byte _wit;
    [SerializeField] private byte _men;
    [SerializeField] private byte _int;

    [SerializeField] private int _karma;
    [SerializeField] private int _pvpKills;
    [SerializeField] private int _pkKills;

    public int PAtk { get { return _pAtk; } set { _pAtk = value; } }
    public int MAtk { get { return _mAtk; } set { _mAtk = value; } }
    public int PEvasion { get { return _pEvasion; } set { _pEvasion = value; } }
    public int MEvasion { get { return _mEvasion; } set { _mEvasion = value; } }
    public int PAccuracy { get { return _pAccuracy; } set { _pAccuracy = value; } }
    public int MAccuracy { get { return _mAccuracy; } set { _mAccuracy = value; } }
    public int PCritical { get { return _pCritical; } set { _pCritical = value; } }
    public int MCritical { get { return _mCritical; } set { _mCritical = value; } }
    public int MDef { get { return _mDef; } set { _mDef = value; } }
    public int PDef { get { return _pDef; } set { _pDef = value; } }
    public int Exp { get { return _exp; } set { _exp = value; } }
    public int MaxExp { get { return _maxExp; } set { _maxExp = value; } }
    public int Sp { get { return _sp; } set { _sp = value; } }
    public int ShieldDef { get { return _shieldDef; } set { _shieldDef = value; } }
    public int CurrWeight { get { return _currWeight; } set { _currWeight = value; } }
    public int MaxWeight { get { return _maxWeight; } set { _maxWeight = value; } }

    public float AttackRange { get => _attackRange; set => _attackRange = value; }
    public byte Con { get { return _con; } set { _con = value; } }
    public byte Dex { get { return _dex; } set { _dex = value; } }
    public byte Str { get { return _str; } set { _str = value; } }
    public byte Wit { get { return _wit; } set { _wit = value; } }
    public byte Men { get { return _men; } set { _men = value; } }
    public byte Int { get { return _int; } set { _int = value; } }

    public int Karma { get { return _karma; } set { _karma = value; } }
    public int PvpKills { get { return _pvpKills; } set { _pvpKills = value; } }
    public int PkKills { get { return _pkKills; } set { _pkKills = value; } }

    // ... TODO: add extra stats
}
