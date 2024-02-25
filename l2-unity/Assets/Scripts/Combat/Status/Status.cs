using UnityEngine;

[System.Serializable]
public class Status {
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _level;

    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Level { get => _level; set => _level = value; }
}