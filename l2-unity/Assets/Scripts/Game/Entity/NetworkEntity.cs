using UnityEngine;

public abstract class NetworkEntity : Entity
{

    public override void Initialize()
    {
        base.Initialize();
    }

    public override float UpdateMAtkSpeed(int mAtkSpd)
    {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        AnimationController.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd)
    {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        AnimationController.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateRunSpeed(int speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        AnimationController.SetRunSpeed(converted);
        return converted;
    }
    public override float UpdateWalkSpeed(int speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        AnimationController.SetWalkSpeed(converted);
        return converted;
    }
}