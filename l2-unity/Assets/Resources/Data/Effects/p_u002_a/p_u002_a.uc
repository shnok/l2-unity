class p_u002_a extends Emitter;

defaultproperties
{
     Begin Object Class=SpriteEmitter Name=SpriteEmitter2
         UseDirectionAs=PTDU_Up
         Acceleration=(Z=-200.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         FadeOutStartTime=0.115000
         FadeOut=True
         MaxParticles=15
         RespawnDeadParticles=False
         Name="Spark"
         StartLocationShape=PTLS_Polar
         StartLocationPolarRange=(X=(Min=-45.000000,Max=45.000000),Y=(Min=60.000000,Max=120.000000),Z=(Min=4.000000,Max=4.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.500000)
         SizeScale(1)=(RelativeTime=0.370000,RelativeSize=2.000000)
         SizeScale(2)=(RelativeTime=1.000000,RelativeSize=0.300000)
         StartSizeRange=(X=(Min=0.200000,Max=0.400000),Y=(Min=1.800000,Max=3.500000))
         InitialParticlesPerSecond=10000.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_Brighten
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0061'
         TextureUSubdivisions=8
         TextureVSubdivisions=8
         UseRandomSubdivision=True
         SubdivisionStart=17
         SubdivisionEnd=20
         LifetimeRange=(Min=0.600000,Max=0.600000)
         StartVelocityRange=(X=(Min=50.000000,Max=120.000000),Y=(Min=-80.000000,Max=80.000000),Z=(Min=-80.000000,Max=80.000000))
     End Object
     Emitters(0)=SpriteEmitter'SpriteEmitter2'
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.sparkredcone00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         Acceleration=(X=-10.000000)
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.700000,Max=0.800000),Y=(Min=0.700000,Max=0.800000),Z=(Min=0.750000,Max=0.850000))
         FadeOutStartTime=0.035200
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         AutoDestroy=True
         Name="SparkCone"
         UseRotationFrom=PTRS_Actor
         SpinParticles=True
         StartSpinRange=(Z=(Max=1.000000))
         RotationNormal=(X=1.000000)
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.150000,RelativeSize=2.200000)
         SizeScale(1)=(RelativeTime=0.310000,RelativeSize=3.000000)
         SizeScale(2)=(RelativeTime=0.590000,RelativeSize=3.500000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=4.000000)
         StartSizeRange=(X=(Min=0.300000,Max=0.300000),Y=(Min=0.180000,Max=0.180000),Z=(Min=0.180000,Max=0.180000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.400000,Max=0.400000)
         StartVelocityRange=(X=(Min=4.000000,Max=4.000000))
         VelocityLossRange=(X=(Min=2.000000,Max=2.000000))
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter0
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Impact.shockwave00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         Opacity=0.490000
         FadeOutStartTime=0.037500
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         Name="Wave2"
         StartLocationRange=(X=(Min=-3.000000,Max=-3.000000))
         SpinParticles=True
         StartSpinRange=(Z=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.090000,RelativeSize=1.450000)
         SizeScale(1)=(RelativeTime=0.230000,RelativeSize=1.800000)
         SizeScale(2)=(RelativeTime=0.620000,RelativeSize=2.000000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=2.200000)
         StartSizeRange=(X=(Min=0.150000,Max=0.150000),Y=(Min=0.150000,Max=0.150000),Z=(Min=0.150000,Max=0.150000))
         InitialParticlesPerSecond=1000.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.250000,Max=0.250000)
         StartVelocityRange=(X=(Min=3.000000,Max=3.000000))
     End Object
     Emitters(2)=MeshEmitter'MeshEmitter0'
     AutoReplay=True
     bUseDynamicLights=False
     bLightChanged=True
     bNoDelete=False
     bAcceptsProjectors=False
     DrawScale=0.020000
}