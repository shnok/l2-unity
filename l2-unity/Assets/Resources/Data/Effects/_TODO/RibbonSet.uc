//=============================================================================
// KFTATTRibbon.
//=============================================================================
class RibbonSet extends Emitter
	placeable;

defaultproperties
{
     Begin Object Class=RibbonEmitter Name=RibbonEmitter0
         SampleRate=0.002000
         NumPoints=80
         AccDrop=ADRP_BYTIME_DUAL
         PointsDropRate=5
         MinPoints=20
         GetPointAxisFrom=PAXIS_BoneNormal
         bUseInterpolation=True
         ScaleRatio=1.500000
         bDecayPointsWhenStopped=True
         bSyncDecayWhenKilled=True
         bLengthBasedTextureU=True
         bUseBones=True
         Opacity=0.350000
         FadeOutFactor=(W=2.000000,X=2.000000,Y=2.000000,Z=2.000000)
         FadeOut=True
         FadeInFactor=(W=0.500000,X=1.200000,Y=1.200000,Z=1.200000)
         FadeInEndTime=1.000000
         FadeIn=True
         RespawnDeadParticles=False
         Name="RibbonEmitter0"
         ZWrite=True
         InitialParticlesPerSecond=100.000000
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0082'
     End Object
     Emitters(0)=RibbonEmitter'RibbonEmitter0'
     bNoDelete=False
}
