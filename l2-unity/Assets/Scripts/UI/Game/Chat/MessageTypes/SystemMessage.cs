using System;
using static SMParam;

public class SystemMessage {

    private SMParam[] _params;
    private SystemMessageDat _messageData;

    public SystemMessage(SMParam[] smParams, SystemMessageDat messageData) {
        _params = smParams;
        _messageData = messageData;
    }

    public override string ToString() {
        string value = String.Copy(_messageData.Message);

        for (int i = 1; i <= _params.Length; i++ ){
            SMParam param = _params[i-1];
           // Debug.LogWarning($"{i}: {param.Type}");

            switch (param.Type) {
                case SMParamType.TYPE_TEXT:
                    value = value.Replace($"$s{i}", param.GetStringValue());
                    value = value.Replace($"$c{i}", param.GetStringValue());
                    break;
                case SMParamType.TYPE_PLAYER_NAME:
                    value = value.Replace($"$c{i}", param.GetStringValue());
                    break;
                case SMParamType.TYPE_LONG_NUMBER:
                    value = value.Replace($"$s{i}", param.GetLongValue().ToString());
                    break;
                case SMParamType.TYPE_ITEM_NAME:
                    AbstractItem item = ItemTable.Instance.GetItem(param.GetIntValue());
                    string itemName = "Unknown";
                    if(item != null) {
                        itemName = item.ItemName.Name;
                    }
                    value = value.Replace($"$s{i}", itemName);
                    break;
                case SMParamType.TYPE_CASTLE_NAME:
                case SMParamType.TYPE_INT_NUMBER:
                case SMParamType.TYPE_NPC_NAME:
                case SMParamType.TYPE_ELEMENT_NAME:
                case SMParamType.TYPE_SYSTEM_STRING:
                case SMParamType.TYPE_INSTANCE_NAME:
                case SMParamType.TYPE_DOOR_NAME:
                    value = value.Replace($"$s{i}", param.GetIntValue().ToString());
                    break;
                case SMParamType.TYPE_SKILL_NAME:
                    int[] array = param.GetIntArrayValue();
                    //array[0] = ReadI(); // SkillId
                    //array[1] = ReadI(); ; // SkillLevel
                    break;
                case SMParamType.TYPE_ZONE_NAME:
                    float[] array2 = param.GetFloatArrayValue();
                    //array2[0] = ReadF(); // x
                    //array2[1] = ReadF(); // y
                    //array2[2] = ReadF(); // z
                    break;
            }
        }

        return $"<color=#{_messageData.Color}>{value}</color>";
    }
}