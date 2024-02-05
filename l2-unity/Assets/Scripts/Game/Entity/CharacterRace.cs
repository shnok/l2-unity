using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterRace : int
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

public static class CharacterRaceParser {
    public static CharacterRace ParseRace(string type) {
        switch (type.ToLower()) {
            case "fhuman":
                return CharacterRace.FHFighter;
            case "mhuman":
                return CharacterRace.MHFighter;
            case "forc":
                return CharacterRace.FOFighter;
            case "morc":
                return CharacterRace.MOFighter;
            case "felf":
                return CharacterRace.FElf;
            case "melf":
                return CharacterRace.MElf;
            case "mdelf":
                return CharacterRace.MDElf;
            case "fdelf":
                return CharacterRace.FDElf;
            case "fdwarf":
                return CharacterRace.FDwarf;
            case "mdwarf":
                return CharacterRace.MDwarf;
            case "momagician":
                return CharacterRace.MOMagician;
            case "fomagician":
                return CharacterRace.FOMagician;
            case "fofighter":
                return CharacterRace.FOFighter;
            case "mofighter":
                return CharacterRace.MOFighter;
            case "fhmagician":
                return CharacterRace.FHMagician;
            case "mhmagician":
                return CharacterRace.MHMagician;
            case "fhfighter":
                return CharacterRace.FHFighter;
            case "mhfighter":
                return CharacterRace.MHFighter;
            default:
                return CharacterRace.Monster;
        }
    }
}
