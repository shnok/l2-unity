using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CharSelectionInfoPacket : ServerPacket
{
    private int _charCount;
    private int _maximumSlots;
    private List<CharSelectionInfoPackage> _characters;
    private int _selectedSlotId;
    private int _sessionId;

    public int CharCount { get { return _charCount; } }
    public int MaximumSlots { get { return _maximumSlots; } }
    public List<CharSelectionInfoPackage> Characters { get { return _characters; } }
    public int SelectedSlotId { get { return _selectedSlotId; } }
    public int SessionId { get { return _sessionId; } }

    public CharSelectionInfoPacket(byte[] d) : base(d)
    {
        _characters = new List<CharSelectionInfoPackage>();

        Parse();
    }

    public override void Parse()
    {
        _charCount = ReadI();
        _maximumSlots = 9;

        for (int i = 0; i < _charCount; i++)
        {
            CharSelectionInfoPackage character = new CharSelectionInfoPackage();
            PlayerAppearance appearance = new PlayerAppearance();
            PlayerStatus status = new PlayerStatus();
            PlayerStats stats = new PlayerStats();

            character.Slot = i;
            character.Name = ReadS();
            character.Id = ReadI();
            character.Id = i; // CharId shared by the server is always the same
            character.Account = ReadS();
            _sessionId = ReadI();
            character.ClanId = ReadI();

            ReadI();

            appearance.Sex = (byte)ReadI();
            appearance.Race = (byte)ReadI();
            character.BaseClassId = (byte)ReadI();

            ReadI();

            float z = ReadI() / 52.5f;
            float x = ReadI() / 52.5f;
            float y = ReadI() / 52.5f;

            character.Position = new Vector3(x, y, z);

            status.Hp = (int)ReadD();
            status.Mp = (int)ReadD();

            character.Sp = ReadI();
            character.Exp = (int)ReadL();
            character.ExpPercent = (float)ReadD();
            stats.Level = ReadI();
            // character.ExpPercent = ReadF();

            character.Karma = ReadI();
            character.PkKills = ReadI();
            character.PvpKills = ReadI();

            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();

            ReadI(); //HairAll?
            ReadI(); //Rear
            ReadI(); //Lear
            ReadI(); //Neck
            ReadI(); //Rfinger
            ReadI(); //Lfinger
            ReadI(); //Head
            appearance.RHand = ReadI();
            appearance.LHand = ReadI();
            appearance.Gloves = ReadI();
            appearance.Chest = ReadI();
            appearance.Legs = ReadI();
            appearance.Feet = ReadI();
            ReadI(); //Cloak
            appearance.RHand = ReadI();
            ReadI(); //Hair
            ReadI(); //Face

            ReadI(); //HairAll?
            ReadI(); //Rear
            ReadI(); //Lear
            ReadI(); //Neck
            ReadI(); //Rfinger
            ReadI(); //Lfinger
            ReadI(); //Head
            appearance.RHand = ReadI();
            appearance.LHand = ReadI();
            appearance.Gloves = ReadI();
            appearance.Chest = ReadI();
            appearance.Legs = ReadI();
            appearance.Feet = ReadI();
            ReadI(); //Cloak
            appearance.RHand = ReadI();
            ReadI(); //Hair
            ReadI(); //Face

            appearance.HairStyle = (byte)ReadI();
            appearance.HairColor = (byte)ReadI();
            appearance.Face = (byte)ReadI();

            stats.MaxHp = (int)ReadD();
            stats.MaxMp = (int)ReadD();

            character.DeleteTimer = ReadI();
            character.ClassId = (byte)ReadI();
            character.Selected = ReadI() == 1;

            ReadB(); // enchant effect
            ReadI(); // augmentation id

            if (character.Selected)
            {
                _selectedSlotId = i;
            }

            character.IsMage = CharacterClassParser.IsMage((CharacterClass)character.ClassId);
            character.CharacterRaceAnimation = CharacterModelTypeParser.ParseRace((CharacterRace)appearance.Race, appearance.Sex, character.IsMage);
            appearance.CollisionHeight = CharacterModelHelper.GetColHeight(character.CharacterRaceAnimation);

            character.PlayerAppearance = appearance;
            character.PlayerStatus = status;
            character.PlayerStats = stats;

            _characters.Add(character);
        }

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("CharSelectionInfoPacket:");
        sb.AppendLine($"  Session ID: {_sessionId}");
        sb.AppendLine($"  Char Count: {_charCount}");
        sb.AppendLine($"  Maximum Slots: {_maximumSlots}");
        sb.AppendLine($"  Selected Slot ID: {_selectedSlotId}");

        for (int i = 0; i < _characters.Count; i++)
        {
            var character = _characters[i];
            sb.AppendLine($"  Character {i + 1}:");
            sb.AppendLine($"    Name: {character.Name}");
            sb.AppendLine($"    Slot: {character.Slot}");
            sb.AppendLine($"    ID: {character.Id}");
            sb.AppendLine($"    Account: {character.Account}");
            sb.AppendLine($"    Clan ID: {character.ClanId}");
            sb.AppendLine($"    Position: {character.Position}");
            sb.AppendLine($"    Base Class ID: {character.BaseClassId}");
            sb.AppendLine($"    Is Mage: {character.IsMage}");
            sb.AppendLine($"    Race Animation: {character.CharacterRaceAnimation}");
            sb.AppendLine($"    Stats:");
            sb.AppendLine($"      Level: {character.PlayerStats.Level}");
            sb.AppendLine($"      EXP %: {character.PlayerStats.ExpPercent}");
            sb.AppendLine($"      Max HP: {character.PlayerStats.MaxHp}");
            sb.AppendLine($"      Max MP: {character.PlayerStats.MaxMp}");
            sb.AppendLine($"    Status:");
            sb.AppendLine($"      HP: {character.PlayerStatus.Hp}");
            sb.AppendLine($"      MP: {character.PlayerStatus.Mp}");
            sb.AppendLine($"    Experience: {character.Exp}");
            sb.AppendLine($"    SP: {character.Sp}");
            sb.AppendLine($"    Karma: {character.Karma}");
            sb.AppendLine($"    PK Kills: {character.PkKills}");
            sb.AppendLine($"    PvP Kills: {character.PvpKills}");
            sb.AppendLine($"    Delete Timer: {character.DeleteTimer}");
            sb.AppendLine($"    Selected: {character.Selected}");
            sb.AppendLine($"    Appearance:");
            sb.AppendLine($"      Race: {(CharacterRace)character.PlayerAppearance.Race}");
            sb.AppendLine($"      Sex: {character.PlayerAppearance.Sex}");
            sb.AppendLine($"      Hair Style: {character.PlayerAppearance.HairStyle}");
            sb.AppendLine($"      Hair Color: {character.PlayerAppearance.HairColor}");
            sb.AppendLine($"      Col Height: {character.PlayerAppearance.CollisionHeight}");
            sb.AppendLine($"      Face: {character.PlayerAppearance.Face}");
            sb.AppendLine($"      Equipment: RHand={character.PlayerAppearance.RHand}, LHand={character.PlayerAppearance.LHand}, Gloves={character.PlayerAppearance.Gloves}, Chest={character.PlayerAppearance.Chest}, Legs={character.PlayerAppearance.Legs}, Feet={character.PlayerAppearance.Feet}");
        }

        return sb.ToString();
    }
}

