using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class ServerPacket : Packet
{
    protected byte minimumLength;
    protected int _iterator;

    public ServerPacket(byte[] d) : base(d)
    {
        ReadB();
    }

    protected byte ReadB()
    {
        return _packetData[_iterator++];
    }

    protected byte[] ReadB(int length)
    {
        byte[] data = new byte[length];
        Array.Copy(_packetData, _iterator, data, 0, length);
        _iterator += length;

        return data;
    }

    protected virtual int ReadI()
    {
        byte[] data = new byte[4];
        Array.Copy(_packetData, _iterator, data, 0, 4);
        // Array.Reverse(data);
        int value = BitConverter.ToInt32(data, 0);
        _iterator += 4;
        return value;
    }

    protected virtual long ReadL()
    {
        byte[] data = new byte[8];
        Array.Copy(_packetData, _iterator, data, 0, 8);
        // Array.Reverse(data);
        long value = BitConverter.ToInt64(data, 0);
        _iterator += 8;
        return value;
    }

    protected virtual float ReadF()
    {
        byte[] data = new byte[4];
        Array.Copy(_packetData, _iterator, data, 0, 4);
        // Array.Reverse(data);
        float value = BitConverter.ToSingle(data, 0);
        _iterator += 4;
        return value;
    }

    protected virtual double ReadD()
    {
        byte[] data = new byte[8];
        Array.Copy(_packetData, _iterator, data, 0, 8);
        // Array.Reverse(data);
        double value = BitConverter.ToDouble(data, 0);
        _iterator += 8;
        return value;
    }

    protected virtual int ReadH()
    {
        byte[] data = new byte[2];
        Array.Copy(_packetData, _iterator, data, 0, 2);
        // Array.Reverse(data);
        double value = BitConverter.ToInt16(data, 0);
        _iterator += 2;
        return (int)value;
    }

    protected virtual string ReadS()
    {
        List<char> chars = new List<char>();

        // Read chars until we hit the null terminator
        while (true)
        {
            // Read 2 bytes as a char (matching Java's putChar)
            char c = BitConverter.ToChar(_packetData, _iterator);
            _iterator += 2; // Move iterator by 2 bytes

            // Check for null terminator
            if (c == '\0')
                break;

            chars.Add(c);
        }

        return new string(chars.ToArray());
    }

    public abstract void Parse();
}
