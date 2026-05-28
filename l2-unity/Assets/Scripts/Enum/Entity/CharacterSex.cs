public enum CharacterSex
{
    Male = 0,
    Female = 1
}

public static class CharacterSexParser
{
    public static CharacterSex ParseSex(CharacterModelType race)
    {
        switch (race)
        {
            case CharacterModelType.MDwarf:
            case CharacterModelType.MShaman:
            case CharacterModelType.MDarkElf:
            case CharacterModelType.MFighter:
            case CharacterModelType.MOrc:
            case CharacterModelType.MMagic:
            case CharacterModelType.MElf:
                return CharacterSex.Male;
            case CharacterModelType.FDwarf:
            case CharacterModelType.FShaman:
            case CharacterModelType.FOrc:
            case CharacterModelType.FFighter:
            case CharacterModelType.FElf:
            case CharacterModelType.FMagic:
            case CharacterModelType.FDarkElf:
                return CharacterSex.Female;
            default:
                return CharacterSex.Female;
        }
    }
}