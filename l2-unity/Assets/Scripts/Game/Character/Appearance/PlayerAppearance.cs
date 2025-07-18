using UnityEngine;

[System.Serializable]
public class PlayerAppearance : Appearance
{
    [SerializeField] private byte _race;
    [SerializeField] private byte _sex;
    [SerializeField] private byte _face;
    [SerializeField] private byte _hairStyle;
    [SerializeField] private byte _hairColor;
    [SerializeField] private int _chest;
    [SerializeField] private int _legs;
    [SerializeField] private int _gloves;
    [SerializeField] private int _feet;

    public byte Race { get { return _race; } set { _race = value; } }
    public byte Sex { get { return _sex; } set { _sex = value; } }
    public byte Face { get { return _face; } set { _face = value; } }
    public byte HairStyle { get { return _hairStyle; } set { _hairStyle = value; } }
    public byte HairColor { get { return _hairColor; } set { _hairColor = value; } }
    public int Chest { get { return _chest; } set { _chest = value; } }
    public int Legs { get { return _legs; } set { _legs = value; } }
    public int Gloves { get { return _gloves; } set { _gloves = value; } }
    public int Feet { get { return _feet; } set { _feet = value; } }

    public PlayerAppearance() { }

    public void UpdateAppearance(PlayerAppearance appearance)
    {
        base.UpdateAppearance(appearance);

        _race = appearance.Race;
        _sex = appearance.Sex;
        _face = appearance.Face;
        _hairStyle = appearance.HairStyle;
        _hairColor = appearance.HairColor;
        _chest = appearance.Chest;
        _legs = appearance.Legs;
        _gloves = appearance.Gloves;
        _feet = appearance.Feet;
    }

    public bool ShouldUpdateFace(PlayerAppearance newAppearance)
    {
        return Face != newAppearance.Face;
    }

    public bool ShouldUpdateHair(PlayerAppearance newAppearance)
    {
        return HairColor != newAppearance.HairColor || HairStyle != newAppearance.HairStyle;
    }

    public bool ShouldUpdateArmors(PlayerAppearance newAppearance)
    {
        return Legs != newAppearance.Legs || Chest != newAppearance.Chest || Gloves != newAppearance.Gloves || Feet != newAppearance.Feet;
    }

    public override string ToString()
    {
        return $"PlayerAppearance: \n" +
               $"  Race: {_race}, Sex: {_sex}, Face: {_face}, HairStyle: {_hairStyle}, HairColor: {_hairColor}\n" +
               $"  Chest: {_chest}, Legs: {_legs}, Gloves: {_gloves}, Feet: {_feet}\n" +
               $"  CollisionHeight: {CollisionHeight}, CollisionRadius: {CollisionRadius}\n" +
               $"  LHand: {LHand}, RHand: {RHand}, ServerNameColor: {ServerNameColor}, ServerTitleColor: {ServerTitleColor}";
    }
}
