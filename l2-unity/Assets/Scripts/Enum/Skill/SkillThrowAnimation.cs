public enum SkillThrowAnimation : int
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
    MagicNoTarget = 15,
    MagicShot,
    MagicThrow
}