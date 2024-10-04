
// Used by NPCS and USERS
public class NetworkHumanoidCombat : NetworkCombat
{
    public HumanoidGear Gear { get { return (HumanoidGear)_referenceHolder.Gear; } }

    protected override void OnDeath()
    {
        base.OnDeath();
        AnimationController.SetAnimationProperty((int)HumanoidAnimationEvent.death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        AudioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            AnimationController.SetBool("atk01_" + Gear.WeaponAnim, true);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            AnimationController.SetBool("atk01_" + Gear.WeaponAnim, false);
            if (!NetworkCharacterControllerReceive.IsMoving() && !IsDead())
            {
                AnimationController.SetBool("atkwait_" + Gear.WeaponAnim, true);
            }
        }

        return true;
    }
}