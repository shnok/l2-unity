public enum AttachMethod : int
{
    AM_NONE,                // don't attach
    AM_LOCATION,            // don't attach, there is no target, spawn with received location
    AM_RH,                  // attach to GetRHandBoneName()
    AM_LH,                  // attach to GetLHandBoneName()
    AM_RA,                  // attach to GetRArmBoneName()
    AM_LA,                  // attach to GetLArmBoneName()
    AM_WING,                // attach to GetWingBoneName()
    AM_BONESPECIFIED,       // attach to this.AttachBoneName
    AM_ALIASSPECIFIED,      // attach to TagAlias(AttachBoneName)
    AM_TRAIL,               // Source: don't attach, trail the targetactor( assume physics of the emitter is PHYS_Trailer )
                            // Shnok -> Basically attach the particle to the entity base transform (with an offset?)
    AM_BONELOCATION,        // don't attach, but spawn on AttachBoneName
    AM_ALIASLOCATION,       // don't attach, but spawn on TagAlias(AttachBoneName)
                            //branch
    AM_AGATHION,            // attach to AttachBoneName in Agathion
    AM_DEPENDONMAGICINFOCLIENT  // 
                                //end of branch
}