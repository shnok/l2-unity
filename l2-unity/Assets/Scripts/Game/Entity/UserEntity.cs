using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserEntity : Entity {
    [SerializeField]
    private PlayerStatus _status;
    public PlayerStatus Status { get => _status; set => _status = value; }
}
