using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtcItem : AbstractItem {
    public EtcItemgrp EtcItemgrp { get { return (EtcItemgrp) _itemgrp; } }

    public EtcItem(int id, ItemName itemName, ItemStatData itemStatData, EtcItemgrp etcItemgrp) : base(id, itemName, itemStatData, etcItemgrp, etcItemgrp.Icon) {
    }
}
