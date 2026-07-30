using UnityEngine;

[System.Serializable]
public class PlayerStats : Stats
{
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
    [SerializeField] private float _expPercent;
    [SerializeField] private int _sp;

    [SerializeField] private int _currWeight;
    [SerializeField] private int _maxWeight;

    [SerializeField] private byte _con;
    [SerializeField] private byte _dex;
    [SerializeField] private byte _str;
    [SerializeField] private byte _wit;
    [SerializeField] private byte _men;
    [SerializeField] private byte _int;

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
    public float ExpPercent { get { return _expPercent; } set { _expPercent = value; } }
    public int Sp { get { return _sp; } set { _sp = value; } }
    public int ShieldDef { get { return _shieldDef; } set { _shieldDef = value; } }
    public int CurrWeight { get { return _currWeight; } set { _currWeight = value; } }
    public int MaxWeight { get { return _maxWeight; } set { _maxWeight = value; } }

    public byte Con { get { return _con; } set { _con = value; } }
    public byte Dex { get { return _dex; } set { _dex = value; } }
    public byte Str { get { return _str; } set { _str = value; } }
    public byte Wit { get { return _wit; } set { _wit = value; } }
    public byte Men { get { return _men; } set { _men = value; } }
    public byte Int { get { return _int; } set { _int = value; } }

    public int PvpKills { get { return _pvpKills; } set { _pvpKills = value; } }
    public int PkKills { get { return _pkKills; } set { _pkKills = value; } }

    public void UpdateStats(PlayerStats stats)
    {
        base.UpdateStats(stats);

        _pAtkSpd = stats.PAtkSpd;
        _mAtkSpd = stats.MAtkSpd;
        _maxHp = stats.MaxHp;
        _maxMp = stats.MaxMp;
        _maxCp = stats.MaxCp;

        _pAtk = stats.PAtk;
        _mAtk = stats.MAtk;
        _pEvasion = stats.PEvasion;
        _mEvasion = stats.MEvasion;
        _pAccuracy = stats.PAccuracy;
        _mAccuracy = stats.MAccuracy;
        _pCritical = stats.PCritical;
        _mCritical = stats.MCritical;
        _mDef = stats.MDef;
        _pDef = stats.PDef;
        _shieldDef = stats.ShieldDef;

        _exp = stats.Exp;
        _expPercent = stats.ExpPercent;
        _sp = stats.Sp;

        _currWeight = stats.CurrWeight;
        _maxWeight = stats.MaxWeight;

        _con = stats.Con;
        _dex = stats.Dex;
        _str = stats.Str;
        _wit = stats.Wit;
        _men = stats.Men;
        _int = stats.Int;

        _pvpKills = stats.PvpKills;
        _pkKills = stats.PkKills;
    }
}
