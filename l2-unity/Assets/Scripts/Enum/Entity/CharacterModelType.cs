using System;
using System.Collections.Generic;

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

public class CharacterModelHelper
{
    private static readonly Dictionary<CharacterModelType, float> modelColHeights = new()
    {
        { CharacterModelType.FElf, 0.438f },
        { CharacterModelType.MElf, 0.457f },
        { CharacterModelType.MDarkElf, 0.457f },
        { CharacterModelType.FDarkElf, 0.447f },
        { CharacterModelType.FDwarf, 0.361f },
        { CharacterModelType.MDwarf, 0.342f },
        { CharacterModelType.MShaman, 0.523f },
        { CharacterModelType.FShaman, 0.485f },
        { CharacterModelType.MOrc, 0.533f },
        { CharacterModelType.FOrc, 0.514f },
        { CharacterModelType.FMagic, 0.428f },
        { CharacterModelType.MMagic, 0.434f },
        { CharacterModelType.FFighter, 0.447f },
        { CharacterModelType.MFighter, 0.438f }
    };

    public static float GetColHeight(CharacterModelType modelType)
    {
        return modelColHeights.TryGetValue(modelType, out float height) ? height : 0;
    }
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
