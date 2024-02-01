using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : Entity
{
    [SerializeField]
    private NpcStatus _status;
    public NpcStatus Status { get => _status; set => _status = value; }
}
