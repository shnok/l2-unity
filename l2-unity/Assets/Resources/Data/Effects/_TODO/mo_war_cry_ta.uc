class mo_war_cry_ta extends Emitter;

defaultproperties
{
     Begin Object Class=MeshEmitter Name=MeshEmitter1
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.etc.etcpotion01'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(Y=(Min=0.900000,Max=0.900000),Z=(Min=0.600000,Max=0.600000))
         Opacity=0.770000
         FadeOutStartTime=0.087000
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         Name="MeshEmitter1"
         StartLocationOffset=(Z=10.000000)
         SpinParticles=True
         SpinCCWorCW=(X=0.000000)
         SpinsPerSecondRange=(X=(Min=-3.000000,Max=-3.000000))
         StartSpinRange=(X=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=1.000000,RelativeSize=2.000000)
         StartSizeRange=(X=(Min=0.300000,Max=0.300000),Y=(Min=0.300000,Max=0.300000),Z=(Min=0.050000,Max=0.050000))
         InitialParticlesPerSecond=20.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.300000,Max=0.300000)
         StartVelocityRange=(Z=(Min=60.000000,Max=60.000000))
     End Object
     Emitters(0)=MeshEmitter'MeshEmitter1'
     Begin Object Class=MeshEmitter Name=MeshEmitter4
         StaticMesh=StaticMesh'LineageEffectsStaticmeshes.Skill_Power.skill_charge00'
         UseMeshBlendMode=False
         RenderTwoSided=True
         ColorScale(0)=(Color=(B=255,G=255,R=255,A=255))
         ColorScale(1)=(RelativeTime=1.000000,Color=(B=255,G=255,R=255,A=255))
         ColorMultiplierRange=(Y=(Min=0.713000,Max=0.713000),Z=(Min=0.645000,Max=0.645000))
         FadeOut=True
         MaxParticles=1
         RespawnDeadParticles=False
         Name="MeshEmitter4"
         StartLocationOffset=(Z=8.000000)
         SpinParticles=True
         StartSpinRange=(X=(Max=1.000000))
         UseSizeScale=True
         UseRegularSizeScale=False
         SizeScale(0)=(RelativeTime=0.070000,RelativeSize=1.600000)
         SizeScale(1)=(RelativeTime=0.200000,RelativeSize=2.200000)
         SizeScale(2)=(RelativeTime=0.430000,RelativeSize=2.800000)
         SizeScale(3)=(RelativeTime=1.000000,RelativeSize=3.400000)
         StartSizeRange=(X=(Min=0.170000,Max=0.170000),Y=(Min=0.170000,Max=0.170000),Z=(Min=0.020000,Max=0.020000))
         InitialParticlesPerSecond=15.000000
         AutomaticInitialSpawning=False
         LifetimeRange=(Min=0.200000,Max=0.200000)
     End Object
     Emitters(1)=MeshEmitter'MeshEmitter4'
     bNoDelete=False
     DrawScale=0.050000
}
