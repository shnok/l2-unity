public enum WeaponType : byte {
    none, //shield
    hand,
    sword,
    bigword,
    blunt,
    bigblunt,
    bow,
    dagger,
    fist,
    dual,
    dualfist,
    pole
}

public class WeaponTypeParser {
    public static string GetWeaponAnim(WeaponType weaponType) {
        switch (weaponType) {
            case WeaponType.none:
                return "shield";
            case WeaponType.fist:
            case WeaponType.hand:
                return "hand";
            case WeaponType.sword:
            case WeaponType.blunt:
            case WeaponType.dagger:
                return "1HS";
            case WeaponType.bigword:
            case WeaponType.bigblunt:
                return "2HS";
            case WeaponType.dual:
            case WeaponType.dualfist:
                return "dual";
            case WeaponType.pole:
                return "pole";
            case WeaponType.bow:
                return "bow";
            default:
                return "hand";
        }
    }
}
