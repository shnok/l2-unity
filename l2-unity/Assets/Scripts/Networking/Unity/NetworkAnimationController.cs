using System;

public class NetworkAnimationController : HumanoidAnimationController
{
    public override void Initialize()
    {
        if (World.Instance.OfflineMode)
        {
            this.enabled = false;
            return;
        }

        base.Initialize();
    }
}
