using UnityEngine;

public class HumanoidGear : Gear
{
    public override void Initialize(int ownderId, CharacterModelType raceId)
    {
        base.Initialize(ownderId, raceId);
        UpdateWeaponAnim("hand");
    }


    protected override void UpdateWeaponType(WeaponType weaponType)
    {
        UpdateWeaponAnim(WeaponTypeParser.GetWeaponAnim(weaponType));
    }

    public override void UpdateWeaponAnim(string weaponAnim)
    {
        _weaponAnim = weaponAnim;
        NotifyAnimator(_weaponAnim);
    }

    protected virtual void NotifyAnimator(string newWeaponAnim)
    {
        if (_animationController != null)
        {
            _animationController.WeaponAnimChanged(_weaponAnim);
        }
    }
}