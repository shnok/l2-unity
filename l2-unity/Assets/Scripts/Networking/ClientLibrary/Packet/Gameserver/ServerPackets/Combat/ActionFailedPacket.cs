using System;
using UnityEngine;

public class ActionFailedPacket : ServerPacket
{
    public ActionFailedPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
