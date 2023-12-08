using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public abstract class Packet {
    protected byte[] _packetData;
    protected byte _packetLength;
    protected byte _packetType;

    public Packet() {}
    public Packet(byte type) {
        _packetType = type;
    }

    public Packet(byte[] d) {
        _packetData = d;

        //Debug.Log("Received: [" + string.Join(",", d) + "]");
    }

    public void SetData(byte[] data) {
        _packetType = data[0];
        _packetLength = data[1];
        _packetData = data;

       // Debug.Log("Sent: [" + string.Join(",", _packetData) + "]");
    }

    public byte[] GetData() {
        return _packetData;
    }
 
    public byte GetPacketType() {
        return _packetType;
    }

    public byte GetLength() {
        return _packetLength;
    }
}