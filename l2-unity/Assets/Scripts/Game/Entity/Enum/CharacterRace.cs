public enum CharacterRace : byte
{
    Human = 0,
    Elf = 1,
    DarkElf = 2,
    Orc = 3,
    Dwarf = 4,
    Monster = 5
}

public static class CharacterRaceParser
{
    public static CharacterRace ParseRace(CharacterModelType race)
    {
        switch (race)
        {
            case CharacterModelType.MDwarf:
            case CharacterModelType.FDwarf:
                return CharacterRace.Dwarf;
            case CharacterModelType.FDarkElf:
            case CharacterModelType.MDarkElf:
                return CharacterRace.DarkElf;
            case CharacterModelType.MElf:
            case CharacterModelType.FElf:
                return CharacterRace.Elf;
            case CharacterModelType.MShaman:
            case CharacterModelType.FShaman:
            case CharacterModelType.MOrc:
            case CharacterModelType.FOrc:
                return CharacterRace.Orc;
            case CharacterModelType.MFighter:
            case CharacterModelType.FFighter:
            case CharacterModelType.MMagic:
            case CharacterModelType.FMagic:
                return CharacterRace.Human;
            default:
                return CharacterRace.Dwarf;
        }
    }
}
