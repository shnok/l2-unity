class shot_N_atk extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter324
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=0.800000,Color=(B=146,G=71,R=73,A=255))
         ColorScale(2)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorScaleRepeats=12.000000
         ColorMultiplierRange=(Z=(Min=0.600000,Max=0.600000))
         FadeOutStartTime=0.660000
         FadeOut=True
         MaxActiveDistance=1500
         MaxParticles=6
         RespawnDeadParticles=False
         Name="Particle"
         StartLocationOffset=(X=2.000000)
         StartLocationRange=(X=(Min=-1.000000,Max=1.000000),Y=(Min=-5.000000,Max=5.000000),Z=(Min=-5.000000,Max=5.000000))
         StartLocationShape=PTLS_Polar
         StartLocationPolarRange=(X=(Min=90.000000,Max=90.000000),Y=(Min=-140.000000,Max=140.000000),Z=(Min=10.000000,Max=20.000000))
         SpinParticles=True
         SpinCCWorCW=(X=1.000000)
         SpinsPerSecondRange=(X=(Min=0.200000,Max=0.200000))
         StartSpinRange=(X=(Min=0.100000,Max=0.100000))
         UseSizeScale=True
         UseRegularSizeScale=False
         UniformSize=True
         SizeScale(0)=(RelativeTime=0.200000,RelativeSize=1.000000)
         SizeScale(1)=(RelativeTime=1.000000,RelativeSize=0.200000)
         StartSizeRange=(X=(Min=1.000000,Max=4.000000),Y=(Min=1.000000,Max=4.000000),Z=(Min=1.000000,Max=4.000000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0084'
         TextureUSubdivisions=4
         TextureVSubdivisions=8
         SubdivisionStart=31
         SubdivisionEnd=32
         LifetimeRange=(Min=0.800000,Max=2.000000)
         StartVelocityRange=(X=(Min=300.000000,Max=300.000000),Y=(Min=300.000000,Max=300.000000),Z=(Min=300.000000,Max=300.000000))
         VelocityLossRange=(X=(Min=6.000000,Max=6.000000),Y=(Min=6.000000,Max=6.000000),Z=(Min=6.000000,Max=6.000000))
         GetVelocityDirectionFrom=PTVD_OwnerAndStartPosition
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter324'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter325
         Acceleration=(X=20.000000,Z=30.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.716000,Max=0.716000),Y=(Min=0.830000,Max=0.830000),Z=(Min=0.784000,Max=0.784000))
         Opacity=0.600000
         FadeOutStartTime=0.320000
         FadeOut=True
         FadeInEndTime=0.120000
         FadeIn=True
         MaxActiveDistance=1500
         MaxParticles=6
         RespawnDeadParticles=False
         Name="smog"
         StartLocationPolarRange=(X=(Max=360.000000),Y=(Min=90.000000,Max=90.000000),Z=(Min=5.000000,Max=5.000000))
         SpinParticles=True
         SpinsPerSecondRange=(X=(Min=0.050000,Max=0.100000))
         StartSpinRange=(X=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         UniformSize=True
         SizeScale(0)=(RelativeTime=0.070000,RelativeSize=1.600000)
         SizeScale(1)=(RelativeTime=0.240000,RelativeSize=2.000000)
         SizeScale(2)=(RelativeTime=0.530000,RelativeSize=2.300000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=2.500000)
         StartSizeRange=(X=(Min=10.000000,Max=16.000000),Y=(Min=10.000000,Max=16.000000),Z=(Min=10.000000,Max=16.000000))
         InitialParticlesPerSecond=100.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0067'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         UseRandomSubdivision=True
         SubdivisionStart=2
         SubdivisionEnd=4
         LifetimeRange=(Min=0.486000,Max=1.300000)
         StartVelocityRange=(X=(Min=50.000000,Max=50.000000),Y=(Min=-80.000000,Max=80.000000),Z=(Min=-80.000000,Max=80.000000))
         VelocityLossRange=(X=(Min=8.000000,Max=8.000000),Y=(Min=8.000000,Max=8.000000),Z=(Min=8.000000,Max=8.000000))
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter325'
     Begin Object Class=MeshEmitter Name=MeshEmitter225
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.etc.spirit_gun00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=0.500000,Color=(B=179,G=179,R=179,A=255))
         ColorScale(2)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorScaleRepeats=7.000000
         ColorMultiplierRange=(X=(Min=0.600000,Max=0.600000),Y=(Min=0.600000,Max=0.600000),Z=(Min=0.600000,Max=0.600000))
         Opacity=0.300000
         FadeOutStartTime=0.410000
         FadeOut=True
         FadeInEndTime=0.090000
         FadeIn=True
         MaxActiveDistance=1500
         MaxParticles=6
         RespawnDeadParticles=False
         Name="Spirit"
         StartLocationOffset=(X=-5.000000)
         SpinParticles=True
         SpinCCWorCW=(Z=0.000000)
         SpinsPerSecondRange=(X=(Min=0.200000,Max=0.200000),Y=(Min=0.200000,Max=0.200000),Z=(Min=1.000000,Max=2.000000))
         StartSpinRange=(X=(Min=-0.100000,Max=0.100000),Y=(Min=-0.100000,Max=0.100000),Z=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.150000,RelativeSize=2.000000)
         SizeScale(1)=(RelativeTime=0.320000,RelativeSize=2.500000)
         SizeScale(2)=(RelativeTime=0.540000,RelativeSize=3.000000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=3.500000)
         StartSizeRange=(X=(Min=0.015000,Max=0.015000),Y=(Min=0.100000,Max=0.100000),Z=(Min=0.100000,Max=0.100000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=1.000000,Max=1.500000)
         StartVelocityRange=(X=(Min=10.000000,Max=10.000000),Y=(Min=-20.000000,Max=20.000000),Z=(Min=-20.000000,Max=20.000000))
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter225'
     Begin Object Class=MeshEmitter Name=MeshEmitter226
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.800000,Max=0.800000),Y=(Min=0.800000,Max=0.800000),Z=(Min=0.700000,Max=0.700000))
         Opacity=0.600000
         FadeOutStartTime=0.037500
         FadeOut=True
         MaxActiveDistance=1500
         MaxParticles=2
         RespawnDeadParticles=False
         Name="ShockWave"
         SpinParticles=True
         StartSpinRange=(Z=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.090000,RelativeSize=2.000000)
         SizeScale(1)=(RelativeTime=0.230000,RelativeSize=2.500000)
         SizeScale(2)=(RelativeTime=0.520000,RelativeSize=3.000000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=3.500000)
         StartSizeRange=(X=(Min=0.100000,Max=0.100000),Y=(Min=0.140000,Max=0.140000),Z=(Min=0.140000,Max=0.140000))
         InitialParticlesPerSecond=15.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         LifetimeRange=(Min=0.200000,Max=0.200000)
         StartVelocityRange=(X=(Min=4.000000,Max=4.000000))
         WarmupTicksPerSecond=20.000000
         RelativeWarmupTime=0.050000
     End Object
     Emitters(3)=MeshEmitter'MeshEmitter226'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter326
         UseDirectionAs=PTDU_Up
         ProjectionNormal=(X=1.000000,Z=0.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.900000,Max=0.900000),Y=(Min=0.900000,Max=0.900000))
         Opacity=0.600000
         FadeOutStartTime=0.084000
         FadeOut=True
         MaxActiveDistance=1500
         MaxParticles=8
         RespawnDeadParticles=False
         Name="Needle"
         StartLocationOffset=(X=2.000000)
         StartLocationShape=PTLS_Polar
         SphereRadiusRange=(Min=15.000000,Max=25.000000)
         StartLocationPolarRange=(X=(Min=90.000000,Max=90.000000),Y=(Max=360.000000),Z=(Min=10.000000,Max=15.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.460000,RelativeSize=1.500000)
         SizeScale(1)=(RelativeTime=1.000000,RelativeSize=1.000000)
         StartSizeRange=(X=(Min=0.150000,Max=0.700000),Y=(Min=18.000000,Max=25.000000),Z=(Min=0.150000,Max=0.700000))
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=2
         TextureVSubdivisions=2
         SubdivisionStart=2
         SubdivisionEnd=3
         LifetimeRange=(Min=0.300000,Max=0.300000)
         StartVelocityRange=(X=(Min=150.000000,Max=180.000000),Y=(Min=150.000000,Max=180.000000),Z=(Min=150.000000,Max=180.000000))
         VelocityLossRange=(X=(Min=0.500000,Max=0.500000),Y=(Min=0.500000,Max=0.500000),Z=(Min=0.500000,Max=0.500000))
         GetVelocityDirectionFrom=PTVD_OwnerAndStartPosition
     End Object
     Emitters(4)=SpriteEmitter'SpriteEmitter326'
     bLightChanged=True
     bNoDelete=False
     bSunAffect=True
     DrawScale=0.200000
}
