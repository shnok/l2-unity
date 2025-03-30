class m_u025_b extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter10
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_power01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         UseColorScale=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=0.125000,Color=(B=255,G=255,R=255,A=255))
         ColorScale(2)=(RelativeTime=0.425000,Color=(B=128,G=128,R=128,A=255))
         ColorScale(3)=(RelativeTime=1.000000,Color=(A=255))
         ColorMultiplierRange=(Y=(Min=0.714000,Max=0.714000))
         Opacity=0.600000
         FadeOutStartTime=0.300000
         MaxParticles=5
         RespawnDeadParticles=False
         Name="Needle"
         SpinParticles=True
         StartSpinRange=(X=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeSize=1.000000)
         SizeScale(1)=(RelativeTime=1.000000,RelativeSize=6.000000)
         StartSizeRange=(X=(Min=0.150000,Max=0.150000),Y=(Min=0.150000,Max=0.150000),Z=(Min=0.060000,Max=0.060000))
         InitialParticlesPerSecond=30.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.500000,Max=0.500000)
         VelocityLossRange=(Z=(Min=4.000000,Max=4.000000))
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter10'
     Begin Object Class=SpriteEmitter Name=SpriteEmitter9
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(X=(Min=0.767000),Y=(Min=0.767000),Z=(Min=0.767000))
         Opacity=0.800000
         FadeOutStartTime=0.040000
         FadeOut=True
         MaxParticles=5
         RespawnDeadParticles=False
         Name="cloud"
         SpinParticles=True
         StartSpinRange=(X=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         UniformSize=True
         SizeScale(0)=(RelativeTime=0.080000,RelativeSize=2.000000)
         SizeScale(1)=(RelativeTime=0.270000,RelativeSize=4.000000)
         SizeScale(2)=(RelativeTime=0.660000,RelativeSize=6.000000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=7.000000)
         StartSizeRange=(X=(Min=6.000000,Max=6.000000),Y=(Min=6.000000,Max=6.000000),Z=(Min=6.000000,Max=6.000000))
         InitialParticlesPerSecond=30.000000
         AutomaticInitialSpawning=False
         Texture=Texture'LineageEffectsTextures.Particles.fx_m_t0067'
         TextureUSubdivisions=2
         TextureVSubdivisions=4
         UseRandomSubdivision=True
         SubdivisionStart=2
         SubdivisionEnd=4
         LifetimeRange=(Min=0.500000,Max=0.500000)
     End Object
     Emitters(1)=SpriteEmitter'SpriteEmitter9'
     AutoReplay=True
     bNoDelete=False
     DrawScale=0.100000
}
