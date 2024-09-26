public class HumanoidGear : Gear
{
    public override void Initialize(int ownderId, CharacterRaceAnimation raceId)
    {
        base.Initialize(ownderId, raceId);
        UpdateWeaponAnim("hand");
    }

    protected override void UpdateWeaponType(WeaponType weaponType)
    {
        UpdateWeaponAnim(WeaponTypeParser.GetWeaponAnim(weaponType));
        //UpdateIdleAnimation();
    }

    public override void UpdateWeaponAnim(string weaponAnim)
    {
        _weaponAnim = weaponAnim;
        NotifyAnimator(_weaponAnim);
    }

    protected virtual void NotifyAnimator(string newWeaponAnim)
    {
        if (_networkAnimationReceive != null)
        {
            _networkAnimationReceive.WeaponAnimChanged(_weaponAnim);
        }
    }
}