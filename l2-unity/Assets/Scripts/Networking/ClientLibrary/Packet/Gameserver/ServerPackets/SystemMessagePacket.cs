using UnityEngine;
using System;
using static SMParam;

public class SystemMessagePacket : ServerPacket {
    private SMParam[] _params;
    private int _smId;

    public SMParam[] Params { get { return _params; } }
    public int Id { get { return _smId; } }

    public SystemMessagePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            _smId = ReadI();

            byte paramCount = ReadB();

            _params = new SMParam[paramCount];

            for (int i = 0; i < paramCount; i++) {

                byte paramType = ReadB();

                SMParam param = new SMParam((SMParamType) paramType);

                switch ((SMParamType)paramType) {
                    case SMParamType.TYPE_TEXT:
                    case SMParamType.TYPE_PLAYER_NAME:
                        param.SetValue(ReadS());
                        break;
                    case SMParamType.TYPE_LONG_NUMBER:
                        param.SetValue(ReadL());
                        break;
                    case SMParamType.TYPE_ITEM_NAME:
                    case SMParamType.TYPE_CASTLE_NAME:
                    case SMParamType.TYPE_INT_NUMBER:
                    case SMParamType.TYPE_NPC_NAME:
                    case SMParamType.TYPE_ELEMENT_NAME:
                    case SMParamType.TYPE_SYSTEM_STRING:
                    case SMParamType.TYPE_INSTANCE_NAME:
                    case SMParamType.TYPE_DOOR_NAME:
                        param.SetValue(ReadI());
                        break;
                    case SMParamType.TYPE_SKILL_NAME:
                        int[] array = new int[2];
                        array[0] = ReadI(); // SkillId
                        array[1] = ReadI(); ; // SkillLevel
                        param.SetValue(array);
                        break;
                    case SMParamType.TYPE_ZONE_NAME:
                        float[] array2 = new float[3];
                        array2[0] = ReadF(); // x
                        array2[1] = ReadF(); // y
                        array2[2] = ReadF(); // z
                        param.SetValue(array2);
                        break;
                }

                _params[i] = param;
            }
          
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}