using UnityEngine;

[System.Serializable]
public abstract class Status {
    [SerializeField] private int _hp;
    [SerializeField] private int _maxHp;
    [SerializeField] private int _level;
    [SerializeField] private float _moveSpeed;
    public int Hp { get => _hp; set => _hp = value; }
    public int MaxHp { get => _maxHp; set => _maxHp = value; }
    public int Level { get => _level; set => _level = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    public Status() { }
}