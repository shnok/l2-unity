class NAttackDemageEffect extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorMultiplierRange=(X=(Min=0.800000,Max=0.900000),Y=(Min=0.800000,Max=0.900000),Z=(Min=0.800000,Max=0.900000))
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         AutoDestroy=True
         Name="MeshEmitter1"
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.200000)
         SizeScale(1)=(RelativeTime=0.150000,RelativeSize=2.600000)
         SizeScale(2)=(RelativeTime=0.500000,RelativeSize=3.600000)
         StartSizeRange=(X=(Min=0.200000,Max=0.200000),Y=(Min=0.160000,Max=0.160000),Z=(Min=0.160000,Max=0.160000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.360000,Max=0.360000)
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter1
         UseDirectionAs=PTDU_Normal
         ProjectionNormal=(X=1.000000,Z=0.000000)
         UseColorScale=True
         ColorMultiplierRange=(X=(Min=20.000000,Max=20.000000),Y=(Min=20.000000,Max=20.000000))
         FadeOutStartTime=0.040000
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         AutoDestroy=True
         Name="SpriteEmitter1"
         StartMassRange=(Min=0.000000,Max=0.000000)
         StartSpinRange=(X=(Max=360.000000),Y=(Max=360.000000),Z=(Max=360.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         UniformSize=True
         SizeScale(0)=(RelativeSize=1.000000)
         SizeScale(1)=(RelativeTime=0.200000,RelativeSize=8.000000)
         StartSizeRange=(X=(Min=2.200000,Max=2.200000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0006'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionEnd=1
         LifetimeRange=(Min=0.200000,Max=0.200000)
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter1'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         UseDirectionAs=PTDU_Up
         Acceleration=(Z=-300.000000)
         FadeOut=True
         RespawnDeadParticles=False
         AutoDestroy=True
         Name="SpriteEmitter2"
         StartMassRange=(Min=0.000000,Max=0.000000)
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.500000)
         StartSizeRange=(X=(Min=0.600000,Max=0.600000),Y=(Min=3.000000,Max=3.000000))
         InitialParticlesPerSecond=3000.000000
         AutomaticInitialSpawning=False
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=8
         TextureVSubdivisions=8
         SubdivisionStart=11
         SubdivisionEnd=12
         LifetimeRange=(Min=0.300000,Max=0.500000)
         StartVelocityRange=(X=(Min=-100.000000,Max=-100.000000),Y=(Min=-100.000000,Max=100.000000),Z=(Min=-100.000000,Max=100.000000))
     End Object
     Emitters(2)=SpriteEmitter'SpriteEmitter2'
     bUseDynamicLights=False
     bNoDelete=False
     bAcceptsProjectors=False
     LifeSpan=100.000000
}
