using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterRace : byte { 
    Human = 0,
    Elf = 1,
    DarkElf = 2,
    Orc = 3,
    Dwarf = 4,
    Monster = 5
}

public static class CharacterRaceParser {
    public static CharacterRace ParseRace(CharacterRaceAnimation race) {
        switch (race) {
            case CharacterRaceAnimation.MDwarf:
            case CharacterRaceAnimation.FDwarf:
                return CharacterRace.Dwarf;
            case CharacterRaceAnimation.FDarkElf:
            case CharacterRaceAnimation.MDarkElf:
                return CharacterRace.DarkElf;
            case CharacterRaceAnimation.MElf:
            case CharacterRaceAnimation.FElf:
                return CharacterRace.Elf;
            case CharacterRaceAnimation.MShaman:
            case CharacterRaceAnimation.FShaman:
            case CharacterRaceAnimation.MOrc:
            case CharacterRaceAnimation.FOrc:
                return CharacterRace.Orc;
            case CharacterRaceAnimation.MFighter:
            case CharacterRaceAnimation.FFighter:
            case CharacterRaceAnimation.MMagic:
            case CharacterRaceAnimation.FMagic:
                return CharacterRace.Human;
            default:
                return CharacterRace.Dwarf;
        }
    }
}
