
using UnityEngine;

public class MagicSkillUsePacket : ServerPacket
{
    public int ObjectId { get; private set; }
    public int TargetId { get; private set; }
    public int SkillId { get; private set; }
    public int SkillLevel { get; private set; }
    public int HitTime { get; private set; }
    public int ReuseDelay { get; private set; }
    public bool Success { get; private set; }
    public Vector3 ObjectPosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    private Vector3 _tmpPos = new Vector3();


    public MagicSkillUsePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();
        TargetId = ReadI();
        SkillId = ReadI();
        SkillLevel = ReadI();
        HitTime = ReadI();
        ReuseDelay = ReadI();

        _tmpPos = new Vector3();
        _tmpPos.z = ReadI() / 52.5f;
        _tmpPos.x = ReadI() / 52.5f;
        _tmpPos.y = ReadI() / 52.5f;
        ObjectPosition = _tmpPos;

        Success = ReadI() == 1;
        if (Success)
        {
            ReadB();
            ReadB();
        }

        _tmpPos = new Vector3();
        _tmpPos.z = ReadI() / 52.5f;
        _tmpPos.x = ReadI() / 52.5f;
        _tmpPos.y = ReadI() / 52.5f;
        TargetPosition = _tmpPos;

        Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        return $"MagicSkillUsePacket:\n" +
               $"  ObjectId: {ObjectId}\n" +
               $"  TargetId: {TargetId}\n" +
               $"  SkillId: {SkillId}\n" +
               $"  SkillLevel: {SkillLevel}\n" +
               $"  HitTime: {HitTime}\n" +
               $"  ReuseDelay: {ReuseDelay}\n" +
               $"  Success: {Success}\n" +
               $"  ObjectPosition: {ObjectPosition}\n" +
               $"  TargetPosition: {TargetPosition}";
    }
}
