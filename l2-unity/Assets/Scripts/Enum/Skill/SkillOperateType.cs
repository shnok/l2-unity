public enum SkillOperateType : int
{
    // 0 = DamageHeal, 1 = buff/debuff, 2 = clan buffs, 3 = transf pain/activables, 4 = special event consumable buffs, 
    Damage,
    BuffDebuff,
    ClanBuff,
    Toggleable,
    Special,
    Heal = 64
}