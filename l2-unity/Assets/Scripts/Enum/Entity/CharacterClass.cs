public enum CharacterClass : byte
{
    Fighter = 0x00,
    Warrior = 0x01,
    Gladiator = 0x02,
    Warlord = 0x03,
    Knight = 0x04,
    Paladin = 0x05,
    DarkAvenger = 0x06,
    Rogue = 0x07,
    TreasureHunter = 0x08,
    Hawkeye = 0x09,

    Mage = 0x0A,
    Wizard = 0x0B,
    Sorceror = 0x0C,
    Necromancer = 0x0D,
    Warlock = 0x0E,
    Cleric = 0x0F,
    Bishop = 0x10,
    Prophet = 0x11,

    ElvenFighter = 0x12,
    ElvenKnight = 0x13,
    TempleKnight = 0x14,
    SwordSinger = 0x15,
    ElvenScout = 0x16,
    PlainsWalker = 0x17,
    SilverRanger = 0x18,

    ElvenMage = 0x19,
    ElvenWizard = 0x1A,
    Spellsinger = 0x1B,
    ElementalSummoner = 0x1C,
    Oracle = 0x1D,
    Elder = 0x1E,

    DarkFighter = 0x1F,
    PalusKnight = 0x20,
    ShillienKnight = 0x21,
    Bladedancer = 0x22,
    Assassin = 0x23,
    AbyssWalker = 0x24,
    PhantomRanger = 0x25,

    DarkMage = 0x26,
    DarkWizard = 0x27,
    Spellhowler = 0x28,
    PhantomSummoner = 0x29,
    ShillienOracle = 0x2A,
    ShillenElder = 0x2B,

    OrcFighter = 0x2C,
    OrcRaider = 0x2D,
    Destroyer = 0x2E,
    OrcMonk = 0x2F,
    Tyrant = 0x30,

    OrcMage = 0x31,
    OrcShaman = 0x32,
    Overlord = 0x33,
    Warcryer = 0x34,

    DwarvenFighter = 0x35,
    Scavenger = 0x36,
    BountyHunter = 0x37,
    Artisan = 0x38,
    Warsmith = 0x39
}

public class CharacterClassParser
{
    public static bool IsMage(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Mage:
            case CharacterClass.Wizard:
            case CharacterClass.Sorceror:
            case CharacterClass.Necromancer:
            case CharacterClass.Warlock:
            case CharacterClass.Cleric:
            case CharacterClass.Bishop:
            case CharacterClass.Prophet:
            case CharacterClass.ElvenMage:
            case CharacterClass.ElvenWizard:
            case CharacterClass.Spellsinger:
            case CharacterClass.ElementalSummoner:
            case CharacterClass.Oracle:
            case CharacterClass.Elder:
            case CharacterClass.DarkMage:
            case CharacterClass.DarkWizard:
            case CharacterClass.Spellhowler:
            case CharacterClass.PhantomSummoner:
            case CharacterClass.ShillienOracle:
            case CharacterClass.ShillenElder:
            case CharacterClass.OrcMage:
            case CharacterClass.OrcShaman:
            case CharacterClass.Overlord:
            case CharacterClass.Warcryer:
                return true;
            default:
                return false;
        }
    }
}
