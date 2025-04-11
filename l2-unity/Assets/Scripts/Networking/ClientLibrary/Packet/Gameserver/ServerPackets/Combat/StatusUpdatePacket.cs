using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StatusUpdatePacket : ServerPacket
{
    public enum AttributeType : byte
    {
        LEVEL = 0x01,
        EXP = 0x02,
        STR = 0x03,
        DEX = 0x04,
        CON = 0x05,
        INT = 0x06,
        WIT = 0x07,
        MEN = 0x08,

        CUR_HP = 0x09,
        MAX_HP = 0x0a,
        CUR_MP = 0x0b,
        MAX_MP = 0x0c,

        SP = 0x0d,
        CUR_LOAD = 0x0e,
        MAX_LOAD = 0x0f,

        P_ATK = 0x11,
        ATK_SPD = 0x12,
        P_DEF = 0x13,
        P_EVASION = 0x14,
        P_ACCURACY = 0x15,
        P_CRITICAL = 0x16,
        M_ATK = 0x17,
        CAST_SPD = 0x18,
        M_DEF = 0x19,
        PVP_FLAG = 0x1a,
        KARMA = 0x1b,
        M_ACCURACY = 0x1C,
        M_EVASION = 0x1D,
        M_CRITICAL = 0x1E,

        CUR_CP = 0x21,
        MAX_CP = 0x22
    }

    public class Attribute
    {
        /**
         * id values 09 - current health 0a - max health 0b - current mana 0c - max mana
         */
        public int id;
        public int value;

        public Attribute(byte pId, int pValue)
        {
            id = pId;
            value = pValue;
        }
    }

    private int _objectId;
    private int _attributeCount;
    private List<Attribute> _attributes;

    public int ObjectId { get { return _objectId; } }
    public int AttributeCount { get { return _attributeCount; } }
    public List<Attribute> Attributes { get { return _attributes; } }

    public StatusUpdatePacket(byte[] d) : base(d)
    {
        _attributes = new List<Attribute>();
        Parse();
    }

    public override void Parse()
    {
        _objectId = ReadI();
        _attributeCount = ReadI();

        for (int i = 0; i < _attributeCount; i++)
        {
            byte attributeId = (byte)ReadI();
            int attributeValue = ReadI();

            _attributes.Add(new Attribute(attributeId, attributeValue));
        }

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"StatusUpdatePacket:");
        sb.AppendLine($"ObjectId: {_objectId}");
        sb.AppendLine($"AttributeCount: {_attributeCount}");

        sb.AppendLine("Attributes:");
        foreach (var attribute in _attributes)
        {
            var attributeName = (AttributeType)attribute.id;
            sb.AppendLine($"  - {attributeName} (0x{attribute.id:X2}): {attribute.value}");
        }

        return sb.ToString();
    }
}
