using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserEntity : Entity {
    [SerializeField]
    private PlayerStatus status;
    public PlayerStatus Status { get => status; set => status = value; }
}
