using UnityEngine;

public class NetworkEntityReferenceHolder : EntityReferenceHolder
{
    [Header("Network")]
    [SerializeField] protected NetworkTransformReceive _networkTransformReceive;
    [SerializeField] protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;

    public NetworkTransformReceive NetworkTransformReceive { get { return _networkTransformReceive; } }
    public NetworkCharacterControllerReceive NetworkCharacterControllerReceive { get { return _networkCharacterControllerReceive; } }
}