using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Armor : AbstractItem {
    [SerializeField] private Armorgrp _armorgrp;

    public Armorgrp Armorgrp { get { return _armorgrp; } }

    public Armor(int id, ItemName itemName, ItemStatData itemStatData, Armorgrp armorgrp) : base(id, itemName, itemStatData) {
        _armorgrp = armorgrp;
    }
}
