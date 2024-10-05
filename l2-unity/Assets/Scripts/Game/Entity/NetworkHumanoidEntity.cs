using UnityEngine;

[RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]
// Used by NPCS and USERS
public class NetworkHumanoidEntity : NetworkEntity
{
    public HumanoidAnimationController HumanoidAnimationController { get { return (HumanoidAnimationController)_referenceHolder.AnimationController; } }
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
            HumanoidAnimationController.SetBool(HumanoidAnimType.sit, true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STANDING)
        {
            HumanoidAnimationController.SetBool(HumanoidAnimType.stand, true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_START_FAKEDEATH)
        {
            HumanoidAnimationController.SetBool(HumanoidAnimType.death, true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STOP_FAKEDEATH)
        {
        }
    }
}