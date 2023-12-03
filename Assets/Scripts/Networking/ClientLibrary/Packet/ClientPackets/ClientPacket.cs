using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class ClientPacket : Packet {
    private List<byte> buffer = new List<byte>();

    public ClientPacket(byte type) : base(type) {}
    public ClientPacket(byte[] data) : base(data) {
        BuildPacket();
    }

    public void WriteB(byte b) {
        buffer.Add(b);
    }

    public void WriteS(String s) {
        Write(Encoding.GetEncoding("UTF-8").GetBytes(s)); 
    }

    public void WriteI(int i) {
        byte[] data = BitConverter.GetBytes(i);
        Array.Reverse(data);
        buffer.AddRange(data);
    }

    public void WriteF(float i) {
        byte[] data = BitConverter.GetBytes(i);
        Array.Reverse(data);
        buffer.AddRange(data);
    }

    private void Write(byte[] data) {
        buffer.Add((byte)data.Length);
        buffer.AddRange(data);
    }

    protected void BuildPacket() {
        buffer.Insert(0, _packetType);
        buffer.Insert(1, (byte)(buffer.Count + 1));
        byte[] array = buffer.ToArray();

        SetData(array);
    }
}