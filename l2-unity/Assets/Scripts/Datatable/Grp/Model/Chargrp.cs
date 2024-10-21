using System.Collections.Generic;
using FMODUnity;

public class Chargrp
{
    public string RaceAnim { get; set; }
    public string FaceMesh { get; set; }
    public string[] FaceTextures { get; set; }
    public string AttackEffect { get; set; }
    public string[] AttackSounds { get; set; }
    public List<EventReference> AttackSoundsEvents { get; set; }
    public string[] DefenseSounds { get; set; }
    public List<EventReference> DefenseSoundsEvents { get; set; }
    public string[] DamageSounds { get; set; }
    public List<EventReference> DamageSoundsEvents { get; set; }
    // public string[,] VoiceSoundWeapon { get; set; }
    public string ClassName { get; set; }
    public int Race { get; set; }
    public int ClassId { get; set; }
    public int ClassType { get; set; }
    public HairStyle[] HairStyles { get; set; }
}