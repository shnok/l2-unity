using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] protected int _level;
    [SerializeField] protected int _runSpeed;
    [SerializeField] protected float _moveSpeedMultiplier;
    [SerializeField] protected int _walkSpeed;
    [SerializeField] protected float _scaledRunSpeed;
    [SerializeField] protected float _scaledWalkSpeed;
    [SerializeField] protected float _attackSpeedMultiplier;
    [SerializeField] private float _attackRange;
    [SerializeField] protected int _pAtkSpd;
    [SerializeField] protected int _mAtkSpd;
    [SerializeField] protected int _maxHp;
    [SerializeField] protected int _maxMp;
    [SerializeField] protected int _maxCp;
    [SerializeField] private int _karma;

    public float AttackRange { get => _attackRange; set => _attackRange = value; }
    public int Level { get => _level; set => _level = value; }
    public int RunSpeed { get => _runSpeed; set => _runSpeed = value; }
    public int WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
    public float MoveSpeedMultiplier { get => _moveSpeedMultiplier; set => _moveSpeedMultiplier = value; }
    public float AttackSpeedMultiplier { get => _attackSpeedMultiplier; set => _attackSpeedMultiplier = value; }
    public float ScaledRunSpeed { get => _scaledRunSpeed; set => _scaledRunSpeed = value; }
    public float ScaledWalkSpeed { get => _scaledWalkSpeed; set => _scaledWalkSpeed = value; }
    public int PAtkSpd { get => _pAtkSpd; set => _pAtkSpd = value; }
    public int MAtkSpd { get => _mAtkSpd; set => _mAtkSpd = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int MaxMp { get => _maxMp; set => _maxMp = value; }
    public int MaxCp { get => _maxCp; set => _maxCp = value; }
    public int Karma { get { return _karma; } set { _karma = value; } }

    public virtual void UpdateStats(Stats stats)
    {
        _level = stats.Level;
        _attackRange = stats.AttackRange;
        _runSpeed = stats.RunSpeed;
        _walkSpeed = stats.WalkSpeed;
        // _scaledRunSpeed = stats.ScaledRunSpeed;
        // _scaledWalkSpeed = stats._scaledWalkSpeed;
        // _moveSpeedMultiplier = stats.MoveSpeedMultiplier;
        // _attackSpeedMultiplier = stats.AttackSpeedMultiplier;
        _pAtkSpd = stats.PAtkSpd;
        _mAtkSpd = stats.MAtkSpd;
        _karma = stats.Karma;
    }

}
