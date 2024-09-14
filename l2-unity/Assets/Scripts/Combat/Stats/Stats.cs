using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] private int _level;
    [SerializeField] private int _runSpeed;
    [SerializeField] private int _walkSpeed;
    [SerializeField] private float _scaledRunSpeed;
    [SerializeField] private float _scaledWalkSpeed;
    [SerializeField] private int _pAtkSpd;
    [SerializeField] private int _mAtkSpd;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxMp;
    [SerializeField] private int _maxCp;

    public int Level { get => _level; set => _level = value; }
    public int RunSpeed { get => _runSpeed; set => _runSpeed = value; }
    public int WalkSpeed { get => _walkSpeed; set => _walkSpeed = value; }
    public float ScaledRunSpeed { get => _scaledRunSpeed; set => _scaledRunSpeed = value; }
    public float ScaledWalkSpeed { get => _scaledWalkSpeed; set => _scaledWalkSpeed = value; }
    public int PAtkSpd { get => _pAtkSpd; set => _pAtkSpd = value; }
    public int MAtkSpd { get => _mAtkSpd; set => _mAtkSpd = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int MaxMp { get => _maxMp; set => _maxMp = value; }
    public int MaxCp { get => _maxCp; set => _maxCp = value; }

    public virtual void UpdateStats(Stats stats)
    {
        _level = stats.Level;
        _runSpeed = stats.RunSpeed;
        _walkSpeed = stats.WalkSpeed;
        _scaledRunSpeed = stats.ScaledRunSpeed;
        ScaledWalkSpeed = stats._scaledWalkSpeed;
        _pAtkSpd = stats.PAtkSpd;
        _mAtkSpd = stats.MAtkSpd;
        _maxHp = stats.MaxHp;
        _maxMp = stats.MaxMp;
        _maxCp = stats.MaxCp;
    }
}
