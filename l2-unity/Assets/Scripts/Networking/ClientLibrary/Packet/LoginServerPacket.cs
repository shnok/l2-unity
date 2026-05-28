using System;
using System.Text;
using UnityEngine;

public abstract class LoginServerPacket : ServerPacket
{
    public LoginServerPacket(byte[] d) : base(d)
    {
    }

    protected override string ReadS()
    {
        int start = _iterator;

        // Find the null terminator in UTF-16LE encoding.
        int end = start;
        while (end < _packetData.Length - 1 && (_packetData[end] != 0 || _packetData[end + 1] != 0))
            end += 2;

        // Move the offset past the string and the null terminator.
        int strLen = end - start;

        _iterator += strLen + 2;

        // Create a string from the bytes between start and end.
        byte[] data = new byte[strLen];
        Array.Copy(_packetData, _iterator, data, 0, strLen);

        string s = Encoding.GetEncoding("UTF-8").GetString(data);

        Debug.Log("Read string: " + s + " : [" + strLen + "]: " + StringUtils.ByteArrayToString(data));
        return s;
    }

    protected override int ReadI()
    {
        byte[] data = new byte[4];
        Array.Copy(_packetData, _iterator, data, 0, 4);
        // Array.Reverse(data);
        int value = BitConverter.ToInt32(data, 0);
        _iterator += 4;
        return value;
    }

    protected override long ReadL()
    {
        byte[] data = new byte[8];
        Array.Copy(_packetData, _iterator, data, 0, 8);
        // Array.Reverse(data);
        long value = BitConverter.ToInt64(data, 0);
        _iterator += 8;
        return value;
    }

    protected override float ReadF()
    {
        byte[] data = new byte[4];
        Array.Copy(_packetData, _iterator, data, 0, 4);
        // Array.Reverse(data);
        float value = BitConverter.ToSingle(data, 0);
        _iterator += 4;
        return value;
    }
}
