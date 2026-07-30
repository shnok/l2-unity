class el_wind_strike_pr extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter8
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Wind.windblowin00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.900000,Max=0.900000),Y=(Min=0.950000,Max=0.950000))
         FadeOutFactor=(X=0.300000,Y=0.300000,Z=0.300000)
         FadeOutStartTime=0.200000
         FadeOut=True
         FadeInFactor=(X=0.300000,Y=0.300000,Z=0.300000)
         FadeInEndTime=0.100000
         FadeIn=True
         MaxParticles=100
         ResetAfterChange=True
         RespawnDeadParticles=False
         Name="Wind"
         StartLocationRange=(Z=(Min=-3.000000,Max=3.000000))
         SpinParticles=True
         SpinCCWorCW=(X=0.000000)
         SpinsPerSecondRange=(X=(Min=1.800000,Max=1.800000))
         StartSpinRange=(X=(Min=1.000000,Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=0.600000)
         SizeScale(1)=(RelativeTime=0.200000,RelativeSize=0.500000)
         SizeScale(2)=(RelativeTime=1.000000,RelativeSize=0.100000)
         StartSizeRange=(X=(Min=0.200000,Max=0.200000),Y=(Min=0.200000,Max=0.200000),Z=(Min=0.100000,Max=0.200000))
         InitialParticlesPerSecond=20.000000
         AutomaticInitialSpawning=False
         DrawStyle=PTDS_AlphaBlend
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         LifetimeRange=(Min=0.500000,Max=0.500000)
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter8'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter10
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.700000,Max=0.700000),Y=(Min=0.700000,Max=0.700000),Z=(Min=0.700000,Max=0.700000))
         FadeOut=True
         MaxParticles=200
         RespawnDeadParticles=False
         Name="Core"
         SpinParticles=True
         StartSpinRange=(X=(Max=360.000000))
         UniformSize=True
         StartSizeRange=(X=(Min=7.000000,Max=7.000000))
         InitialParticlesPerSecond=40.000000
         AutomaticInitialSpawning=False
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0000'
         TextureUSubdivisions=4
         TextureVSubdivisions=4
         UseRandomSubdivision=True
         SubdivisionStart=7
         SubdivisionEnd=8
         LifetimeRange=(Min=0.100000,Max=0.100000)
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter10'
     bNoDelete=False
     Rotation=(Yaw=32892)
     DrawScale=0.100000
}
