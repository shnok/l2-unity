public enum SkillAnimation : int
{
    /*
    j = castend + magic no target

    A = castshort + castend + magic no target
    B = castshort + castend + magic shot
    C = castshort + castend + magic throw

    D = castmid + castend + magic no target
    E = castmid + castend + magic shot
    f = castmid + castend + magic throw

    G = castlong + castend + magic no target

    S = spatk01
    t = spatk02
    V = spatk03

    X = spatk06_hand / spatk07_hand
    */
    None,
    CastShort_NoTarget,
    CastShort_Shot,
    CastShort_Throw,
    CastMid_NoTarget,
    CastMid_Shot,
    CastMid_Throw,
    CastLong_NoTarget,
    NoCast_NoTarget, //battle heal (type j)
    SpAtk01,
    SpAtk02,
    spatk03,
    SpAtk04,
    WarriorBuff01,
}