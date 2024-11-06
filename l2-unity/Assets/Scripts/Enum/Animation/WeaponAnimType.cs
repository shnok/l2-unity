public enum WeaponAnimType : int
{
    hand,
    _1HS,
    _2HS,
    pole,
    dual,
    bow,
    shield
}

public class WeaponAnimParser
{
    public static WeaponAnimType GetWeaponAnim(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.none:
                return WeaponAnimType.shield;
            case WeaponType.fist:
            case WeaponType.hand:
                return WeaponAnimType.hand;
            case WeaponType.sword:
            case WeaponType.blunt:
            case WeaponType.dagger:
                return WeaponAnimType._1HS;
            case WeaponType.bigword:
            case WeaponType.bigblunt:
                return WeaponAnimType._2HS;
            case WeaponType.dual:
            case WeaponType.dualfist:
                return WeaponAnimType.dual;
            case WeaponType.pole:
                return WeaponAnimType.pole;
            case WeaponType.bow:
                return WeaponAnimType.bow;
            default:
                return WeaponAnimType.hand;
        }
    }
}
