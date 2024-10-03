using UnityEngine;

public class HumanoidGear : Gear
{
    [SerializeField] protected string _weaponAnim;

    public string WeaponAnim { get { return _weaponAnim; } }

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
        if (AnimationController != null)
        {
            AnimationController.WeaponAnimChanged(_weaponAnim);
        }
    }
}