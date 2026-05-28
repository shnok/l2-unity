using UnityEngine;

[System.Serializable]
public class Weapongrp : Abstractgrp
{
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private ItemSlot _bodyPart;
    [SerializeField] private byte _soulshot;
    [SerializeField] private byte _spiritshot;
    [SerializeField] private int _mpConsume;
    [SerializeField] private string[] _models;
    [SerializeField] private string[] _textures;
    [SerializeField] private string[] _itemSounds;

    public ItemSlot BodyPart { get { return _bodyPart; } set { _bodyPart = value; } }
    public WeaponType WeaponType { get { return _weaponType; } set { _weaponType = value; } }
    public byte Soulshot { get { return _soulshot; } set { _soulshot = value; } }
    public byte Spiritshot { get { return _spiritshot; } set { _spiritshot = value; } }
    public int MpConsume { get { return _mpConsume; } set { _mpConsume = value; } }
    public string[] Models { get { return _models; } set { _models = value; } }
    public string[] Textures { get { return _textures; } set { _textures = value; } }
    public string[] ItemSounds { get { return _itemSounds; } set { _itemSounds = value; } }
}
