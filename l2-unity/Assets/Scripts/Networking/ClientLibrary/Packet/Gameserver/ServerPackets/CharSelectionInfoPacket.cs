using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelectionInfoPacket : ServerPacket {
    private int _charCount;
    private int _maximumSlots;
    private List<CharSelectionInfoPackage> _characters;

    public int CharCount { get { return _charCount; } }
    public int MaximumSlots { get { return _maximumSlots; } }
    public List<CharSelectionInfoPackage> Characters { get { return _characters; } }

    public CharSelectionInfoPacket(byte[] d) : base(d) {
        _characters = new List<CharSelectionInfoPackage>();

        Parse();
    }

    public override void Parse() {

        _charCount = ReadB();
        _maximumSlots = ReadB();

        for (int i = 0; i < _charCount; i++) {
            CharSelectionInfoPackage character = new CharSelectionInfoPackage();
            PlayerAppearance appearance = new PlayerAppearance();
            PlayerStatus status = new PlayerStatus();

            character.Name = ReadS();
            character.Id = ReadI();
            character.Account = ReadS();
            character.ClanId = ReadI();

            appearance.Sex = ReadB();
            appearance.Race = ReadB();
            character.IsMage = ReadB() == 1;
            character.ClassId = ReadB();

            float x = ReadF();
            float y = ReadF();
            float z = ReadF();

            character.Position = new Vector3(x, y, z);

            status.Hp = ReadI();
            status.Mp = ReadI();
            
            character.Sp = ReadI();
            character.Exp = ReadI();
            character.ExpPercent = ReadF();

            status.Level = ReadI();
            
            character.Karma = ReadI();
            character.PkKills = ReadI();
            character.PvpKills = ReadI();

            appearance.LHand = ReadI();
            appearance.RHand = ReadI();
            appearance.Chest = ReadI();
            appearance.Legs = ReadI();
            appearance.Gloves = ReadI();
            appearance.Feet = ReadI();

            appearance.HairStyle = ReadB();
            appearance.HairColor = ReadB();
            appearance.Face = ReadB();

            status.MaxHp = ReadI();
            status.MaxMp = ReadI();

            character.DeleteTimer = ReadI();

            character.Selected = ReadB() == 1;

            character.CharacterRaceAnimation = CharacterRaceAnimationParser.ParseRace((CharacterRace)appearance.Race, appearance.Sex, character.IsMage);

            character.PlayerAppearance = appearance;
            character.PlayerStatus = status;

            _characters.Add(character);
        }
    }
}

