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

    public void WriteB(byte[] b) {
        foreach (byte b2 in b) {
            _buffer.Add(b2);
        }
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

        // Padding for checksum
        WriteI(0);

        PadBuffer();

        byte[] array = _buffer.ToArray();

        SetData(array);
    }

    private void PadBuffer() {
        byte paddingLength = (byte)(_buffer.Count % 8);
        if (paddingLength > 0) {

            paddingLength = (byte)(8 - paddingLength);

            //Debug.Log($"Packet needs a padding of {paddingLength} bytes.");

            for (int i = 0; i < paddingLength; i++) {
                _buffer.Add((byte)0);
            }
        }
    }
}