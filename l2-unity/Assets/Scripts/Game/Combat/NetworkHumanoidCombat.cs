
// Used by NPCS and USERS
public class NetworkHumanoidCombat : NetworkCombat
{
    public HumanoidAnimationController HumanoidAnimationController { get { return (HumanoidAnimationController)_referenceHolder.AnimationController; } }
    public HumanoidGear Gear { get { return (HumanoidGear)_referenceHolder.Gear; } }

    protected override void OnDeath()
    {
        base.OnDeath();
        HumanoidAnimationController.SetBool(HumanoidAnimType.death, true);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
        //AudioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, true);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, false);
            if (!NetworkCharacterControllerReceive.IsMoving() && !IsDead())
            {
                HumanoidAnimationController.SetBool(HumanoidAnimType.atkwait, true);
            }
        }

        return true;
    }
}