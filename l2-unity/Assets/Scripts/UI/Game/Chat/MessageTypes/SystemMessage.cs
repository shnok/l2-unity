using System;
using UnityEngine;
using static SMParam;

public class SystemMessage
{

    private SMParam[] _params;
    public SystemMessageDat MessageData;

    public SystemMessage(SMParam[] smParams, SystemMessageDat messageData)
    {
        _params = smParams;
        MessageData = messageData;
    }

    public override string ToString()
    {
        return PrintMessage(true);
    }

    public string PrintMessage(bool colored)
    {
        string value = String.Copy(MessageData.Message);

        if (_params != null && _params.Length > 0)
        {
            for (int i = 1; i <= _params.Length; i++)
            {
                SMParam param = _params[i - 1];

                // Debug.LogWarning($"{i}: {param.Type}");

                switch (param.Type)
                {
                    case SMParamType.TYPE_TEXT:
                        value = value.Replace($"$s{i}", param.GetStringValue());
                        value = value.Replace($"$c{i}", param.GetStringValue());
                        break;
                    case SMParamType.TYPE_PLAYER_NAME:
                        value = value.Replace($"$c{i}", param.GetStringValue());
                        break;
                    case SMParamType.TYPE_ITEM_NAME:
                        AbstractItem item = ItemTable.Instance.GetItem(param.GetIntValue());
                        string itemName = "Unknown";
                        if (item != null)
                        {
                            itemName = item.ItemName.Name;
                        }
                        value = value.Replace($"$s{i}", itemName);
                        break;
                    case SMParamType.TYPE_ITEM_NUMBER:
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
                        if (array.Length > 0)
                        {
                            int skillId = array[0];
                            SkillNameData skillNameData = SkillTable.Instance.GetSkill(skillId)?.SkillNameDatas?[0];
                            string skillName = $"[{skillId}]";
                            if (skillNameData != null)
                            {
                                skillName = skillNameData.Name;
                            }
                            value = value.Replace($"$s{i}", skillName);
                        }
                        else
                        {
                            Debug.LogWarning("SPParam TYPE_SKILL_NAME is missing values.");
                        }
                        break;
                    case SMParamType.TYPE_ZONE_NAME:
                        float[] array2 = param.GetFloatArrayValue();
                        if (array2.Length >= 3)
                        {
                            value = $"X={array2[1]} Y={array2[3]} Z={array2[0]}";
                        }
                        else
                        {
                            Debug.LogWarning("SPParam TYPE_ZONE_NAME is missing values.");
                        }
                        break;
                }
            }
        }

        if (colored)
            return $"<color=#{MessageData.Color}>{value}</color>";
        else
            return value;
    }
}