
// Used by MONSTERS
public class NetworkMonsterEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    public override void OnStopMoving()
    {
        if (AnimationController.GetAnimationProperty((int)MonsterAnimationEvent.Atk01) == 0f)
        {
            AnimationController.SetAnimationProperty((int)MonsterAnimationEvent.Wait, 1f);
        }
    }

    public override void OnStartMoving(bool walking)
    {
        base.OnStartMoving(walking);
        AnimationController.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
    }
}
