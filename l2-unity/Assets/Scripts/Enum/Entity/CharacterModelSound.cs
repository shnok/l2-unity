public enum CharacterModelSound : byte
{
    Default,
    FElf,
    MElf,
    MDElf,
    FDElf,
    FDwarf,
    MDwarf,
    MOMagician,
    FOMagician,
    MOFighter,
    FOFighter,
    FHMagician,
    MHMagician,
    FHFighter,
    MHFighter,
    Monster
}

public static class CharacterModelSoundParser
{
    public static CharacterModelSound ParseRace(string type)
    {
        switch (type.ToLower())
        {
            case "fhuman":
                return CharacterModelSound.FHFighter;
            case "mhuman":
                return CharacterModelSound.MHFighter;
            case "forc":
                return CharacterModelSound.FOFighter;
            case "morc":
                return CharacterModelSound.MOFighter;
            case "felf":
                return CharacterModelSound.FElf;
            case "melf":
                return CharacterModelSound.MElf;
            case "mdelf":
                return CharacterModelSound.MDElf;
            case "fdelf":
                return CharacterModelSound.FDElf;
            case "fdwarf":
                return CharacterModelSound.FDwarf;
            case "mdwarf":
                return CharacterModelSound.MDwarf;
            case "momagician":
                return CharacterModelSound.MOMagician;
            case "fomagician":
                return CharacterModelSound.FOMagician;
            case "fofighter":
                return CharacterModelSound.FOFighter;
            case "mofighter":
                return CharacterModelSound.MOFighter;
            case "fhmagician":
                return CharacterModelSound.FHMagician;
            case "mhmagician":
                return CharacterModelSound.MHMagician;
            case "fhfighter":
                return CharacterModelSound.FHFighter;
            case "mhfighter":
                return CharacterModelSound.MHFighter;
            default:
                return CharacterModelSound.Monster;
        }
    }
}
