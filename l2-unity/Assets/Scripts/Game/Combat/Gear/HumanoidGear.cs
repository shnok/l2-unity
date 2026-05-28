using UnityEngine;

public class HumanoidGear : Gear
{
    [SerializeField] protected WeaponAnimType _weaponAnim;

    public WeaponAnimType WeaponAnim { get { return _weaponAnim; } }

    public override void Initialize(int ownderId)
    {
        base.Initialize(ownderId);
    }

    protected override void UpdateWeaponType(WeaponType weaponType)
    {
        UpdateWeaponAnim(WeaponAnimParser.GetWeaponAnim(weaponType));
    }

    public override void UpdateWeaponAnim(WeaponAnimType weaponAnim)
    {
        // Debug.LogWarning("UpdateWeaponAnim: " + weaponAnim);
        _weaponAnim = weaponAnim;
        NotifyAnimator(_weaponAnim);
    }

    protected virtual void NotifyAnimator(WeaponAnimType newWeaponAnim)
    {
        if (AnimationController != null)
        {
            AnimationController.WeaponAnimChanged(_weaponAnim);
        }
    }
}