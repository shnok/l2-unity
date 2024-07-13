using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMParam {
    public enum SMParamType {
        TYPE_SYSTEM_STRING = 13,
        TYPE_PLAYER_NAME = 12,
        TYPE_DOOR_NAME = 11,
        TYPE_INSTANCE_NAME = 10,
        TYPE_ELEMENT_NAME = 9,
        // id 8 - same as 3
        TYPE_ZONE_NAME = 7,
        TYPE_LONG_NUMBER = 6,
        TYPE_CASTLE_NAME = 5,
        TYPE_SKILL_NAME = 4,
        TYPE_ITEM_NAME = 3,
        TYPE_NPC_NAME = 2,
        TYPE_INT_NUMBER = 1,
        TYPE_TEXT = 0
    }

    private SMParamType _type;
    private object _value;

    public SMParamType Type { get { return _type; } set { _type = value;} }

    public SMParam(SMParamType type, object value) {
        _type = type;
        _value = value;
    }

    public SMParam(SMParamType type) {
        _type = type;
    }

    public void SetValue(object value) {
        this._value = value;
    }

    public string GetStringValue() {
        return (string)_value;
    }

    public int GetIntValue() {
        return (int)_value;
    }

    public long GetLongValue() {
        return (long)_value;
    }

    public int[] GetIntArrayValue() {
        return (int[])_value;
    }

    public float[] GetFloatArrayValue() {
        return (float[])_value;
    }

}
