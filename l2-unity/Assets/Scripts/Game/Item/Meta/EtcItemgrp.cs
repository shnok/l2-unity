using UnityEngine;

[System.Serializable]
public class EtcItemgrp : Abstractgrp {
    //item_begin	tag=2	object_id=1036	drop_type=0	drop_anim_type=0	drop_radius=2	drop_height=5	drop_texture={{[dropitems.drop_sack_m00];{[dropitemstex.drop_sack_t00]}}}	icon={[icon.etc_scroll_gray_i00];[None];[None];[None];[None]}	durability=-1	weight=0	material_type=steel	crystallizable=0	related_quest_id={161}	color=1	is_attribution=0	property_params=0	icon_panel=[None]	complete_item_dropsound_type=[None]	inventory_type=quest	mesh={[None]}	texture={[None]}	drop_sound=[None]	equip_sound=[None]	consume_type=consume_type_stackable	etcitem_type=none	crystal_type=none	item_end
    [SerializeField] private string _etcItemType;

    public string EtcItemType { get { return _etcItemType; } set { _etcItemType = value; } }
}
