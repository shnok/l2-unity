public enum CharacterRaceAnimation : byte {
    FElf = 0,
    MElf = 1,
    MDarkElf = 2,
    FDarkElf = 3,
    FDwarf = 4,
    MDwarf = 5,
    MShaman = 6,
    FShaman = 7,
    MOrc = 8,
    FOrc = 9,
    FMagic = 10,
    MMagic = 11,
    FFighter = 12,
    MFighter = 13
}

public static class CharacterRaceAnimationParser {
    public static CharacterRaceAnimation ParseRace(CharacterRace race, byte sex, bool isMage) {
        switch (race) {
            case CharacterRace.Elf:
                return sex == 0 ? CharacterRaceAnimation.MElf : CharacterRaceAnimation.FElf;
            case CharacterRace.DarkElf:
                return sex == 0 ? CharacterRaceAnimation.MDarkElf : CharacterRaceAnimation.FDarkElf;
            case CharacterRace.Dwarf:
                return sex == 0 ? CharacterRaceAnimation.MDwarf : CharacterRaceAnimation.FDwarf;
            case CharacterRace.Orc:
                if (isMage) {
                    return sex == 0 ? CharacterRaceAnimation.MShaman : CharacterRaceAnimation.FShaman;
                } else {
                    return sex == 0 ? CharacterRaceAnimation.MOrc : CharacterRaceAnimation.FOrc;
                }
            case CharacterRace.Human:
                if (isMage) {
                    return sex == 0 ? CharacterRaceAnimation.MMagic : CharacterRaceAnimation.FMagic;
                } else {
                    return sex == 0 ? CharacterRaceAnimation.MFighter : CharacterRaceAnimation.FFighter;
                }
            default:
                return CharacterRaceAnimation.FDwarf;
        }
    }
}
