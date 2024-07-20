using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Weapon : AbstractItem {
    [SerializeField] private Weapongrp _weapongrp;

    public Weapongrp Weapongrp { get { return _weapongrp; } set { _weapongrp = value; } }

    public Weapon(int id, ItemName itemName, ItemStatData itemStatData, Weapongrp weapongrp) : base(id, itemName, itemStatData, weapongrp.Icon) {
        _weapongrp = weapongrp;
    }
}
