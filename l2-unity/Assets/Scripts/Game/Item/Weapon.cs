using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Weapon : AbstractItem {
    public Weapongrp Weapongrp { get { return (Weapongrp) _itemgrp; } }

    public Weapon(int id, ItemName itemName, ItemStatData itemStatData, Weapongrp weapongrp) : base(id, itemName, itemStatData, weapongrp, weapongrp.Icon) {
    }
}
