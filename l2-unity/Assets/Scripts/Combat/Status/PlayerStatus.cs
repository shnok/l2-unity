using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatus : Status {
    [SerializeField] private int _cp;
    [SerializeField] private long _pvpFlag;

    public int Cp { get => _cp; set => _cp = value; }
    public long PvpFlag { get => _pvpFlag; set => _pvpFlag = value; }

    public PlayerStatus() {}
}