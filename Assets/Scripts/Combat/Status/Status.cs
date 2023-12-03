using UnityEngine;

[System.Serializable]
public abstract class Status {
    [SerializeField]
    private int hp;
    [SerializeField]
    private int maxHp;
    [SerializeField]
    private int level;
    public int Hp { get => hp; set => hp = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Level { get => level; set => level = value; }

    public Status() { }
}