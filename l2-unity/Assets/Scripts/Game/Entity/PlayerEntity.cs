using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity {
    [SerializeField]
    private PlayerStatus _status;
    public PlayerStatus Status { get => _status; set => _status = value; }

    private static PlayerEntity _instance;
    public static PlayerEntity Instance { get => _instance; }

    void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }
}