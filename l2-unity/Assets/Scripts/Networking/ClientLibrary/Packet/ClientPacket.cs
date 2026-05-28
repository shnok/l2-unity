using System;
using System.Collections.Generic;
using System.Text;

public abstract class ClientPacket : Packet
{
    protected List<byte> _buffer = new List<byte>();

    public ClientPacket(byte type) : base(type) { }
    public ClientPacket(byte[] data) : base(data)
    {
        BuildPacket();
    }

    public void WriteB(byte b)
    {
        _buffer.Add(b);
    }

    public void WriteB(bool b)
    {
        _buffer.Add(b ? (byte)1 : (byte)0);
    }

    public void WriteB(byte[] b)
    {
        foreach (byte b2 in b)
        {
            _buffer.Add(b2);
        }
    }

    // public void WriteS(String s) {
    //     Write(Encoding.GetEncoding("UTF-8LE").GetBytes(s)); 
    // }

    protected void WriteS(String s)
    {
        if (s != null)
        {
            byte[] d = Encoding.GetEncoding("UTF-16LE").GetBytes(s);
            _buffer.AddRange(d);
            WriteB((byte)0);
            WriteB((byte)0);
        }
    }

    public void WriteI(int i)
    {
        byte[] data = BitConverter.GetBytes(i);
        // Array.Reverse(data);
        _buffer.AddRange(data);
    }

    public void WriteF(float i)
    {
        byte[] data = BitConverter.GetBytes(i);
        // Array.Reverse(data);
        _buffer.AddRange(data);
    }
    public void WriteL(long i)
    {
        byte[] data = BitConverter.GetBytes(i);
        _buffer.AddRange(data);
    }
    private void Write(byte[] data)
    {
        _buffer.Add((byte)data.Length);
        _buffer.AddRange(data);
    }

    public void WriteD(double d)
    {
        byte[] data = BitConverter.GetBytes(d);
        _buffer.AddRange(data);
    }

    protected virtual void BuildPacket()
    {
        _buffer.Insert(0, _packetType);

        // Padding for checksum
        WriteI(0);

        PadBuffer();

        byte[] array = _buffer.ToArray();

        SetData(array);
    }

    protected virtual void PadBuffer()
    {

    }
}