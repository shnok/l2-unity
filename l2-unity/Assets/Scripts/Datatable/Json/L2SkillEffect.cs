using System.Collections.Generic;
/*
Mortal Blow 16
Power Shot 56
Power Strike 3
Self Heal 1216 or 6847
Wind Strike 1177
Cure Poison 1012
Heal 1011
Curse: Poison 1168
Might 4345
Shield 1040
Group Heal 1027 --------> Cant find ?
Battle Heal 1015 --------> Cant find ?
Vampiric Touch 1147 --------> Cant find ?
Curse: Weakness 1164 --------> Cant find ?
Defense Aura 91 --------> Cant find ?
Attack Aura 77 --------> Cant find ?
Drain Health 70

ORC: 
Iron Punch 29
Life Drain 1090
Chill Flame 1100 --------> Cant find ?
Dreaming Spirit 1097 --------> Cant find ?
Soul Shield 1010
Venom 1095 --------> Cant find ?


Soulshots (cast):
2039 NG
2150 D
2151 C
2152 B
2153 A
2154 S

*/
public class L2SkillEffect
{
    public int SkillId { get; set; }
    public List<L2SkillEffectEmitter> CastingActions { get; set; }
    public List<L2SkillEffectEmitter> ShotActions { get; set; }
}
