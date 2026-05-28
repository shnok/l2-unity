using UnityEngine;

[RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]
// Used by NPCS and USERS
public class NetworkHumanoidEntity : NetworkEntity
{
    public NewHumanoidAnimationController HumanoidAnimationController { get { return (NewHumanoidAnimationController)_referenceHolder.NewAnimationController; } }
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
            HumanoidAnimationController.Sit();
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STANDING)
        {
            HumanoidAnimationController.Stand();
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_START_FAKEDEATH)
        {
            HumanoidAnimationController.Die();
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STOP_FAKEDEATH)
        {
            HumanoidAnimationController.Resurrect();
        }
    }
}