using UnityEngine;

[RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]
public class NetworkHumanoidEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    public override void UpdateWaitType(ChangeWaitTypePacket.WaitType moveType)
    {
        base.UpdateWaitType(moveType);

        if (moveType == ChangeWaitTypePacket.WaitType.WT_SITTING)
        {
            AnimationController.SetBool("sit", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STANDING)
        {
            AnimationController.SetBool("stand", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_START_FAKEDEATH)
        {
            AnimationController.SetBool("death", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STOP_FAKEDEATH)
        {
        }
    }
}