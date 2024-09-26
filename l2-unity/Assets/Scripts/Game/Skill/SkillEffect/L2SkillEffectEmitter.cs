using System.Numerics;

public class L2SkillEffectEmitter
{
    public string AttachOn { get; set; }
    public bool SpawnOnTarget { get; set; }
    public bool RelativeToCylinder { get; set; }
    public string EffectClass { get; set; }
    public float ScaleSize { get; set; }
    public bool OnMultiTarget { get; set; }
    public Vector3 Offset { get; set; }
    public string EtcEffect { get; set; }
    public string EtcEffectInfo { get; set; }
}
