using System;

public enum CharacterModelType : byte
{
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

public static class CharacterModelTypeParser
{
    public static CharacterModelType ParseRace(CharacterRace race, byte sex, bool isMage)
    {
        switch (race)
        {
            case CharacterRace.Elf:
                return sex == 0 ? CharacterModelType.MElf : CharacterModelType.FElf;
            case CharacterRace.DarkElf:
                return sex == 0 ? CharacterModelType.MDarkElf : CharacterModelType.FDarkElf;
            case CharacterRace.Dwarf:
                return sex == 0 ? CharacterModelType.MDwarf : CharacterModelType.FDwarf;
            case CharacterRace.Orc:
                if (isMage)
                {
                    return sex == 0 ? CharacterModelType.MShaman : CharacterModelType.FShaman;
                }
                else
                {
                    return sex == 0 ? CharacterModelType.MOrc : CharacterModelType.FOrc;
                }
            case CharacterRace.Human:
                if (isMage)
                {
                    return sex == 0 ? CharacterModelType.MMagic : CharacterModelType.FMagic;
                }
                else
                {
                    return sex == 0 ? CharacterModelType.MFighter : CharacterModelType.FFighter;
                }
            default:
                return CharacterModelType.FDwarf;
        }
    }

    public static CharacterModelType ParseRace(string value)
    {
        return (CharacterModelType)Enum.Parse(typeof(CharacterModelType), value, ignoreCase: true);
    }
}
