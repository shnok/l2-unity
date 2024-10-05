
// Used by MONSTERS
using UnityEngine;

public class NetworkMonsterEntity : NetworkEntity
{
    public MonsterAnimationController MonsterAnimationController { get { return (MonsterAnimationController)_referenceHolder.AnimationController; } }

    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    public override void OnStopMoving()
    {
        // if (MonsterAnimationController.GetBool(MonsterAnimationEvent.atk01) == false)
        // {
        //     Debug.LogWarning("On stop moving valid monster");
        //     MonsterAnimationController.SetBool(MonsterAnimationEvent.wait, true);
        // }
        Debug.LogWarning("On stop moving monster");
    }

    public override void OnStartMoving(bool walking)
    {
        base.OnStartMoving(walking);
        // MonsterAnimationController.SetBool(walking ? MonsterAnimationEvent.walk : MonsterAnimationEvent.run, true);
    }
}
