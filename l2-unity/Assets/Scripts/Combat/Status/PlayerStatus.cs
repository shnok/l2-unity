using UnityEngine;

[System.Serializable]
public class PlayerStatus : Status {
    [SerializeField] private int _cp;
    [SerializeField] private long _pvpFlag;

    public int Cp { get => _cp; set => _cp = value; }
    public long PvpFlag { get => _pvpFlag; set => _pvpFlag = value; }

    public PlayerStatus() {}

    public void UpdateStatus(PlayerStatus status) {
        Cp = status.Cp;
        PvpFlag = status.PvpFlag;
        Hp = status.Hp;
        Mp = status.Mp;
    }
}