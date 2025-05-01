using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AbnormalStatusUpdatePacket : ServerPacket
{
    public BuffEffect[] Effects;
    public AbnormalStatusUpdatePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int effectsTogglesSize = ReadH();
        Effects = new BuffEffect[effectsTogglesSize];
        for (int i = 0; i < effectsTogglesSize; i++)
        {
            int skillId = ReadI();
            int skillLvl = ReadH();
            int duration = ReadI();
            Effects[i] = new BuffEffect(skillId, skillLvl, duration);
            // Debug.LogError($"Received buff id: {skillId}, duration: {duration}");
        }
    }
}
