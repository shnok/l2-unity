using System;
using System.Text;

public abstract class ServerPacket : Packet
{
    protected byte minimumLength;
    private int _iterator;

    public ServerPacket(byte[] d) : base(d) {
        ReadB();
    }

    protected byte ReadB() {
        return _packetData[_iterator++];
    }

    protected byte[] ReadB(int length) {
        byte[] data = new byte[length];
        Array.Copy(_packetData, _iterator, data, 0, length);
        _iterator += length;

        return data;
    }

    protected int ReadI() {
        byte[] data = new byte[4];     
        Array.Copy(_packetData, _iterator, data, 0, 4);
        Array.Reverse(data);
        int value = BitConverter.ToInt32(data, 0);
        _iterator += 4;
        return value;
    }

    protected long ReadL() {
        byte[] data = new byte[8];
        Array.Copy(_packetData, _iterator, data, 0, 8);
        Array.Reverse(data);
        long value = BitConverter.ToInt64(data, 0);
        _iterator += 8;
        return value;
    }

    protected float ReadF() {
        byte[] data = new byte[4];     
        Array.Copy(_packetData, _iterator, data, 0, 4);
        Array.Reverse(data);
        float value = BitConverter.ToSingle(data, 0);
        _iterator += 4;
        return value; 
    }

    protected string ReadS() {
        byte strLen = ReadB();
        byte[] data = new byte[strLen];
        Array.Copy(_packetData, _iterator, data, 0, strLen);
        _iterator += strLen;

        return Encoding.GetEncoding("UTF-8").GetString(data);
    }

    public abstract void Parse();
}
