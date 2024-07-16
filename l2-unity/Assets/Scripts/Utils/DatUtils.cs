using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatUtils {

    public static string CleanupString(string name) {
        return name.Replace("[", string.Empty).Replace("]", string.Empty);
    }

    public static string[] ParseArray(string value) {
        return SplitJSON(value);
    }

    public static string ParseIcon(string value) {
        return SplitJSON(value)[0];
    }

    public static string[] SplitJSON(string value) {
        return value.Replace("{", string.Empty).Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Split(";");
    }

    public static bool ParseBaseAbstractItemGrpDat(Abstractgrp abstractgrp, string key, string value) {

        switch (key) {
            case "object_id":
                abstractgrp.ObjectId = int.Parse(value);
                break;
            case "drop_texture": //{{[dropitems.drop_mfighter_m001_t02_u_m00];{[MFighter.MFighter_m001_t02_u]}}}
                string[] modTex = DatUtils.ParseArray(value);
                abstractgrp.DropTexture = modTex[1];
                abstractgrp.DropModel = modTex[0];
                break;
            case "icon": // {[icon.armor_t02_u_i00];[None];[None];[None];[None]}
                abstractgrp.Icon = DatUtils.ParseIcon(value);
                break;
            case "weight":
                abstractgrp.Weight = int.Parse(value);
                break;
            case "material_type":
                abstractgrp.Material = ItemMaterialParser.Parse(value);
                break;
            case "crystal_type": //crystal_type=d
                abstractgrp.Grade = ItemGradeParser.Parse(value);
                break;
            case "drop_sound": // [ItemSound.itemdrop_dagger]
                abstractgrp.DropSound = DatUtils.CleanupString(value);
                break;
            case "crystallizable":
                abstractgrp.Crystallizable = value == "1";
                break;
            case "equip_sound":
                abstractgrp.EquipSound = DatUtils.CleanupString(value);
                break;
            case "inventory_type":
                abstractgrp.InventoryType = value;
                break;
            default:
                return false;
        }


        return true;
    }
}
