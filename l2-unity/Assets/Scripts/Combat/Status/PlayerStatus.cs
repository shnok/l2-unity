using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatus : Status {
    [SerializeField] private int _cp;

    public int Cp { get => _cp; set => _cp = value; }

    public PlayerStatus() {}
}