using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class ClientPacket : Packet {
    private List<byte> _buffer = new List<byte>();

    public ClientPacket(byte type) : base(type) {}
    public ClientPacket(byte[] data) : base(data) {
        BuildPacket();
    }

    public void WriteB(byte b) {
        _buffer.Add(b);
    }

    public void WriteS(String s) {
        Write(Encoding.GetEncoding("UTF-8").GetBytes(s)); 
    }

    public void WriteI(int i) {
        byte[] data = BitConverter.GetBytes(i);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }

    public void WriteF(float i) {
        byte[] data = BitConverter.GetBytes(i);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }

    private void Write(byte[] data) {
        _buffer.Add((byte)data.Length);
        _buffer.AddRange(data);
    }

    protected void BuildPacket() {
        _buffer.Insert(0, _packetType);
        _buffer.Insert(1, (byte)(_buffer.Count + 1));
        byte[] array = _buffer.ToArray();

        SetData(array);
    }
}