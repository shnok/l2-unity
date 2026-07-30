using UnityEngine;

[System.Serializable]
public class Status
{
    [SerializeField] private int _hp;
    [SerializeField] private int _mp;
    [SerializeField] private bool _dead;
    public int Hp { get => _hp; set => _hp = value; }
    public int Mp { get => _mp; set => _mp = value; }
    public bool IsDead { get => _dead; set => _dead = value; }

    public void UpdateStatus(Status status)
    {
        Hp = status.Hp;
        Mp = status.Mp;
    }
}