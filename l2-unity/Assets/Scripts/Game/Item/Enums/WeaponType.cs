using UnityEngine;

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

// hands: 244 Elven fighter fists


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

    public static WeaponType Parse(string type) {
        switch (type) {
            case "sword": // For 2H sword check at handness
                return WeaponType.sword;
            case "twohandsword":
                return WeaponType.blunt;
            case "blunt":
                return WeaponType.pole;
            case "buster":
                return WeaponType.dagger;
            case "dagger":
                return WeaponType.dual;
            case "staff":
                return WeaponType.bigblunt;
            case "fist": // shield or hand check at handness
                return WeaponType.hand;
            case "twohandblunt":
                return WeaponType.fist;
            case "bow":
                return WeaponType.bow;
            default:
                return WeaponType.hand;
        }
    }

    public static WeaponType ParseByHandness(int handness) {
        switch (handness) {
            case 0:
                return WeaponType.none;
            case 1:
                return WeaponType.sword;
            case 2:
                return WeaponType.bigword;
            case 3:
                return WeaponType.hand;
            case 4:
                return WeaponType.pole;
            case 5:
                return WeaponType.bow;
            case 6: // special
                return WeaponType.hand;
            case 7:
                return WeaponType.fist;
            case 8: // arbalet
                return WeaponType.hand;
            case 9: // rapier
                return WeaponType.hand;
            case 10: // dualdagger
                return WeaponType.hand;
            case 12: 
                return WeaponType.dagger;
            case 14:
                return WeaponType.bigblunt;
            default:
                return WeaponType.hand;

        }
    }
}
