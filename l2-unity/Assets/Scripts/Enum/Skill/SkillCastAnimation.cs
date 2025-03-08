public enum SkillCastAnimation : int
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
    None = 0,
    CastShort = 53,
    CastMid = 54,
    CastLong = 55,
    NoCast = 56, //battle heal (type j)
    WarriorBuff01 = 60,
    SpAtk01 = 100,
    SpAtk02 = 101,
    Spatk03 = 102,
    SpAtk04 = 103,
}