using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : Entity
{
    [SerializeField]
    private NpcStatus status;
    public NpcStatus Status { get => status; set => status = value; }
}
