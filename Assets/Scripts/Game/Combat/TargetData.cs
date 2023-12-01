using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
public class TargetData
{
    public Status status;
    public NetworkIdentity identity;
    public ObjectData data;
    public float distance;

    public TargetData(ObjectData target) {
        data = target;
        identity = data.objectTransform.GetComponent<Entity>().Identity;

        if(identity.EntityType == EntityType.Player) {
            status = data.objectTransform.GetComponent<PlayerEntity>().Status;
        }
        if(identity.EntityType == EntityType.User) {
            status = data.objectTransform.GetComponent<UserEntity>().Status;
        }
        if(identity.EntityType == EntityType.NPC) {
            status = data.objectTransform.GetComponent<NpcEntity>().Status;
        }
    }
}
