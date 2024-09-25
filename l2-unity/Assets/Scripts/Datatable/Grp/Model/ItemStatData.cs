using UnityEngine;

[System.Serializable]
public class ItemStatData {
    [SerializeField] private int _objectId;
    [SerializeField] private int _pDef;
    [SerializeField] private int _mDef;
    [SerializeField] private int _pAtk;
    [SerializeField] private int _mAtk;
    [SerializeField] private int _pAtkSpd;
    [SerializeField] private float _pHit;
    [SerializeField] private float _mHit;
    [SerializeField] private float _pCrit;
    [SerializeField] private float _mCrit;
    [SerializeField] private int _speed;
    [SerializeField] private int _shieldDef;
    [SerializeField] private int _shieldDefRate;
    [SerializeField] private float _pAvoid;
    [SerializeField] private float _mAvoid;
    public int ObjectId { get { return _objectId; } set { _objectId = value; } }
    public int PDef { get { return _pDef; } set { _pDef = value; } }
    public int MDef { get { return _mDef; } set { _mDef = value; } }
    public int PAtk { get { return _pAtk; } set { _pAtk = value; } }
    public int MAtk { get { return _mAtk; } set { _mAtk = value; } }
    public int PAtkSpd { get { return _pAtkSpd; } set { _pAtkSpd = value; } }
    public float PHit { get { return _pHit; } set { _pHit = value; } }
    public float MHit { get { return _mHit; } set { _mHit = value; } }
    public float PCrit { get { return _pCrit; } set { _pCrit = value; } }
    public float MCrit { get { return _mCrit; } set { _mCrit = value; } }
    public int Speed { get { return _speed; } set { _speed = value; } }
    public int ShieldDef { get { return _shieldDef; } set { _shieldDef = value; } }
    public int ShieldDefRate { get { return _shieldDefRate; } set { _shieldDefRate = value; } }
    public float PAvoid { get { return _pAvoid; } set { _pAvoid = value; } }
    public float MAvoid { get { return _mAvoid; } set { _mAvoid = value; } }
}
