using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Armor : AbstractItem {
    public Armorgrp Armorgrp { get { return (Armorgrp) _itemgrp; } }

    public Armor(int id, ItemName itemName, ItemStatData itemStatData, Armorgrp armorgrp) : base(id, itemName, itemStatData, armorgrp, armorgrp.Icon) {
    }
}
