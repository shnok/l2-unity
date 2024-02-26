using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterRaceSound : byte
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

public static class CharacterRaceSoundParser {
    public static CharacterRaceSound ParseRace(string type) {
        switch (type.ToLower()) {
            case "fhuman":
                return CharacterRaceSound.FHFighter;
            case "mhuman":
                return CharacterRaceSound.MHFighter;
            case "forc":
                return CharacterRaceSound.FOFighter;
            case "morc":
                return CharacterRaceSound.MOFighter;
            case "felf":
                return CharacterRaceSound.FElf;
            case "melf":
                return CharacterRaceSound.MElf;
            case "mdelf":
                return CharacterRaceSound.MDElf;
            case "fdelf":
                return CharacterRaceSound.FDElf;
            case "fdwarf":
                return CharacterRaceSound.FDwarf;
            case "mdwarf":
                return CharacterRaceSound.MDwarf;
            case "momagician":
                return CharacterRaceSound.MOMagician;
            case "fomagician":
                return CharacterRaceSound.FOMagician;
            case "fofighter":
                return CharacterRaceSound.FOFighter;
            case "mofighter":
                return CharacterRaceSound.MOFighter;
            case "fhmagician":
                return CharacterRaceSound.FHMagician;
            case "mhmagician":
                return CharacterRaceSound.MHMagician;
            case "fhfighter":
                return CharacterRaceSound.FHFighter;
            case "mhfighter":
                return CharacterRaceSound.MHFighter;
            default:
                return CharacterRaceSound.Monster;
        }
    }
}
