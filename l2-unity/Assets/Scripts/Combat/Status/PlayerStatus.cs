using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatus : Status {
    [SerializeField]
    private int cp;
    [SerializeField]
    private int maxCp;

    [SerializeField]
    private int mp;
    [SerializeField]
    private int maxMp;

    public int Mp { get => mp; set => mp = value; }
    public int MaxMp { get => maxMp; set => maxMp = value; }
    public int Cp { get => cp; set => cp = value; }
    public int MaxCp { get => maxCp; set => maxCp = value; }

    public PlayerStatus() {}
}