using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public abstract class ServerPacket : Packet
{
    protected byte minimumLength;
    private int iterator;

    public ServerPacket() {}
    public ServerPacket(byte[] d) : base(d) {
        ReadB();
        ReadB();
    }

    protected byte ReadB() {
        return _packetData[iterator++];
    }

    protected int ReadI() {
        byte[] data = new byte[4];     
        Array.Copy(_packetData, iterator, data, 0, 4);
        Array.Reverse(data);
        int value = BitConverter.ToInt32(data, 0);
        iterator += 4;
        return value;
    }

    protected float ReadF() {
        byte[] data = new byte[4];     
        Array.Copy(_packetData, iterator, data, 0, 4);
        Array.Reverse(data);
        float value = BitConverter.ToSingle(data, 0);
        iterator += 4;
        return value; 
    }

    protected string ReadS() {
        byte strLen = ReadB();
        byte[] data = new byte[strLen];
        Array.Copy(_packetData, iterator, data, 0, strLen);
        iterator += strLen;

        return Encoding.GetEncoding("UTF-8").GetString(data);
    }

    public abstract void Parse();
}
