// Made with Amplify Shader Editor v1.9.1.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Atmospherics/Height Fog Standalone"
{
	Properties
	{
		[StyledBanner(Height Fog Standalone)]_Banner("Banner", Float) = 0
		[StyledCategory(Fog Settings, false, _HeightFogStandalone, 10, 10)]_FogCat("[ Fog Cat]", Float) = 1
		_FogIntensity("Fog Intensity", Range( 0 , 1)) = 1
		[Enum(X Axis,0,Y Axis,1,Z Axis,2)][Space(10)]_FogAxisMode("Fog Axis Mode", Float) = 1
		[Enum(Multiply Distance and Height,0,Additive Distance and Height,1)]_FogLayersMode("Fog Layers Mode", Float) = 0
		[HDR][Space(10)]_FogColorStart("Fog Color Start", Color) = (0.4411765,0.722515,1,0)
		[HDR]_FogColorEnd("Fog Color End", Color) = (0.4411765,0.722515,1,0)
		_FogColorDuo("Fog Color Duo", Range( 0 , 1)) = 1
		[Space(10)]_FogDistanceStart("Fog Distance Start", Float) = 0
		_FogDistanceEnd("Fog Distance End", Float) = 100
		_FogDistanceFalloff("Fog Distance Falloff", Range( 1 , 8)) = 2
		[Space(10)]_FogHeightStart("Fog Height Start", Float) = 0
		_FogHeightEnd("Fog Height End", Float) = 100
		_FogHeightFalloff("Fog Height Falloff", Range( 1 , 8)) = 2
		[Space(10)]_FarDistanceHeight("Far Distance Height", Float) = 0
		_FarDistanceOffset("Far Distance Offset", Float) = 0
		[StyledCategory(Skybox Settings, false, _HeightFogStandalone, 10, 10)]_SkyboxCat("[ Skybox Cat ]", Float) = 1
		_SkyboxFogIntensity("Skybox Fog Intensity", Range( 0 , 1)) = 0
		_SkyboxFogHeight("Skybox Fog Height", Range( 0 , 8)) = 1
		_SkyboxFogFalloff("Skybox Fog Falloff", Range( 1 , 8)) = 2
		_SkyboxFogOffset("Skybox Fog Offset", Range( -1 , 1)) = 0
		_SkyboxFogBottom("Skybox Fog Bottom", Range( 0 , 1)) = 0
		_SkyboxFogFill("Skybox Fog Fill", Range( 0 , 1)) = 0
		[StyledCategory(Directional Settings, false, _HeightFogStandalone, 10, 10)]_DirectionalCat("[ Directional Cat ]", Float) = 1
		[HDR]_DirectionalColor("Directional Color", Color) = (1,0.8280286,0.6084906,0)
		_DirectionalIntensity("Directional Intensity", Range( 0 , 1)) = 1
		_DirectionalFalloff("Directional Falloff", Range( 1 , 8)) = 2
		[StyledVector(18)]_DirectionalDir("Directional Dir", Vector) = (1,1,1,0)
		[StyledCategory(Noise Settings, false, _HeightFogStandalone, 10, 10)]_NoiseCat("[ Noise Cat ]", Float) = 1
		_NoiseIntensity("Noise Intensity", Range( 0 , 1)) = 1
		_NoiseMin("Noise Min", Range( 0 , 1)) = 0
		_NoiseMax("Noise Max", Range( 0 , 1)) = 1
		_NoiseScale("Noise Scale", Float) = 30
		[StyledVector(18)]_NoiseSpeed("Noise Speed", Vector) = (0.5,0.5,0,0)
		[Space(10)]_NoiseDistanceEnd("Noise Distance End", Float) = 200
		[StyledCategory(Advanced Settings, false, _HeightFogStandalone, 10, 10)]_AdvancedCat("[ Advanced Cat ]", Float) = 1
		[ASEEnd]_JitterIntensity("Jitter Intensity", Float) = 0
		[HideInInspector]_FogAxisOption("_FogAxisOption", Vector) = (0,0,0,0)
		[HideInInspector]_HeightFogStandalone("_HeightFogStandalone", Float) = 1
		[HideInInspector]_IsHeightFogShader("_IsHeightFogShader", Float) = 1

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Overlay" "Queue"="Overlay" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Front
		ColorMask RGBA
		ZWrite Off
		ZTest Always
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			//Atmospheric Height Fog Defines
			//#define AHF_DISABLE_NOISE3D
			//#define AHF_DISABLE_DIRECTIONAL
			//#define AHF_DISABLE_SKYBOXFOG
			//#define AHF_DISABLE_FALLOFF
			//#define AHF_DEBUG_WORLDPOS


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _Banner;
			uniform half _IsHeightFogShader;
			uniform half _HeightFogStandalone;
			uniform half _FogCat;
			uniform half _SkyboxCat;
			uniform half _AdvancedCat;
			uniform half _NoiseCat;
			uniform half _DirectionalCat;
			uniform half4 _FogColorStart;
			uniform half4 _FogColorEnd;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform half _FogDistanceStart;
			uniform half _FogDistanceEnd;
			uniform half _FogDistanceFalloff;
			uniform half _FogColorDuo;
			uniform half4 _DirectionalColor;
			uniform half3 _DirectionalDir;
			uniform half _JitterIntensity;
			uniform half _DirectionalIntensity;
			uniform half _DirectionalFalloff;
			uniform half3 _FogAxisOption;
			uniform half _FogAxisMode;
			uniform half _FogHeightEnd;
			uniform half _FarDistanceHeight;
			uniform float _FarDistanceOffset;
			uniform half _FogHeightStart;
			uniform half _FogHeightFalloff;
			uniform half _FogLayersMode;
			uniform half _NoiseScale;
			uniform half3 _NoiseSpeed;
			uniform half _NoiseMin;
			uniform half _NoiseMax;
			uniform half _NoiseDistanceEnd;
			uniform half _NoiseIntensity;
			uniform half _FogIntensity;
			uniform half _SkyboxFogOffset;
			uniform half _SkyboxFogHeight;
			uniform half _SkyboxFogFalloff;
			uniform half _SkyboxFogBottom;
			uniform half _SkyboxFogFill;
			uniform half _SkyboxFogIntensity;
			float4 mod289( float4 x )
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}
			
			float4 perm( float4 x )
			{
				return mod289(((x * 34.0) + 1.0) * x);
			}
			
			float SimpleNoise3D( float3 p )
			{
				    float3 a = floor(p);
				    float3 d = p - a;
				    d = d * d * (3.0 - 2.0 * d);
				    float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
				    float4 k1 = perm(b.xyxy);
				    float4 k2 = perm(k1.xyxy + b.zzww);
				    float4 c = k2 + a.zzzz;
				    float4 k3 = perm(c);
				    float4 k4 = perm(c + 1.0);
				    float4 o1 = frac(k3 * (1.0 / 41.0));
				    float4 o2 = frac(k4 * (1.0 / 41.0));
				    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
				    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
				    return o4.y * d.y + o4.x * (1.0 - d.y);
			}
			
			float2 UnStereo( float2 UV )
			{
				#if UNITY_SINGLE_PASS_STEREO
				float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex];
				UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
				#endif
				return UV;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 UV235_g1043 = ase_screenPosNorm.xy;
				float2 localUnStereo235_g1043 = UnStereo( UV235_g1043 );
				float2 break248_g1043 = localUnStereo235_g1043;
				float clampDepth227_g1043 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch250_g1043 = ( 1.0 - clampDepth227_g1043 );
				#else
				float staticSwitch250_g1043 = clampDepth227_g1043;
				#endif
				float3 appendResult244_g1043 = (float3(break248_g1043.x , break248_g1043.y , staticSwitch250_g1043));
				float4 appendResult220_g1043 = (float4((appendResult244_g1043*2.0 + -1.0) , 1.0));
				float4 break229_g1043 = mul( unity_CameraInvProjection, appendResult220_g1043 );
				float3 appendResult237_g1043 = (float3(break229_g1043.x , break229_g1043.y , break229_g1043.z));
				float4 appendResult233_g1043 = (float4(( ( appendResult237_g1043 / break229_g1043.w ) * half3(1,1,-1) ) , 1.0));
				float4 break245_g1043 = mul( unity_CameraToWorld, appendResult233_g1043 );
				float3 appendResult239_g1043 = (float3(break245_g1043.x , break245_g1043.y , break245_g1043.z));
				half3 WorldPosFromDepth_Birp566_g1043 = appendResult239_g1043;
				half3 WorldPosFromDepth253_g1043 = WorldPosFromDepth_Birp566_g1043;
				float3 WorldPosition2_g1043 = WorldPosFromDepth253_g1043;
				float temp_output_7_0_g1046 = _FogDistanceStart;
				float temp_output_155_0_g1043 = saturate( ( ( distance( WorldPosition2_g1043 , _WorldSpaceCameraPos ) - temp_output_7_0_g1046 ) / ( _FogDistanceEnd - temp_output_7_0_g1046 ) ) );
				#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch467_g1043 = temp_output_155_0_g1043;
				#else
				float staticSwitch467_g1043 = ( 1.0 - pow( ( 1.0 - abs( temp_output_155_0_g1043 ) ) , _FogDistanceFalloff ) );
				#endif
				half FogDistanceMask12_g1043 = staticSwitch467_g1043;
				float3 lerpResult258_g1043 = lerp( (_FogColorStart).rgb , (_FogColorEnd).rgb , ( ( FogDistanceMask12_g1043 * FogDistanceMask12_g1043 * FogDistanceMask12_g1043 ) * _FogColorDuo ));
				float3 normalizeResult318_g1043 = normalize( ( WorldPosition2_g1043 - _WorldSpaceCameraPos ) );
				float dotResult145_g1043 = dot( normalizeResult318_g1043 , _DirectionalDir );
				float4 ScreenPos3_g1045 = screenPos;
				float2 UV13_g1045 = ( ( (ScreenPos3_g1045).xy / (ScreenPos3_g1045).z ) * (_ScreenParams).xy );
				float3 Magic14_g1045 = float3(0.06711056,0.00583715,52.98292);
				float dotResult16_g1045 = dot( UV13_g1045 , (Magic14_g1045).xy );
				float lerpResult494_g1043 = lerp( 0.0 , frac( ( frac( dotResult16_g1045 ) * (Magic14_g1045).z ) ) , ( _JitterIntensity * 0.1 ));
				half Jitter502_g1043 = lerpResult494_g1043;
				float temp_output_140_0_g1043 = ( saturate( (( dotResult145_g1043 + Jitter502_g1043 )*0.5 + 0.5) ) * _DirectionalIntensity );
				#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch470_g1043 = temp_output_140_0_g1043;
				#else
				float staticSwitch470_g1043 = pow( abs( temp_output_140_0_g1043 ) , _DirectionalFalloff );
				#endif
				float DirectionalMask30_g1043 = staticSwitch470_g1043;
				float3 lerpResult40_g1043 = lerp( lerpResult258_g1043 , (_DirectionalColor).rgb , DirectionalMask30_g1043);
				#ifdef AHF_DISABLE_DIRECTIONAL
				float3 staticSwitch442_g1043 = lerpResult258_g1043;
				#else
				float3 staticSwitch442_g1043 = lerpResult40_g1043;
				#endif
				half3 Input_Color6_g1044 = staticSwitch442_g1043;
				#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch1_g1044 = Input_Color6_g1044;
				#else
				float3 staticSwitch1_g1044 = ( Input_Color6_g1044 * ( ( Input_Color6_g1044 * ( ( Input_Color6_g1044 * 0.305306 ) + 0.6821711 ) ) + 0.01252288 ) );
				#endif
				half3 Final_Color462_g1043 = staticSwitch1_g1044;
				half3 AHF_FogAxisOption181_g1043 = ( _FogAxisOption + ( _FogAxisMode * 0.0 ) );
				float3 break159_g1043 = ( WorldPosition2_g1043 * AHF_FogAxisOption181_g1043 );
				float temp_output_7_0_g1047 = _FogDistanceEnd;
				float temp_output_643_0_g1043 = saturate( ( ( distance( WorldPosition2_g1043 , _WorldSpaceCameraPos ) - temp_output_7_0_g1047 ) / ( ( _FogDistanceEnd + _FarDistanceOffset ) - temp_output_7_0_g1047 ) ) );
				half FogDistanceMaskFar645_g1043 = ( temp_output_643_0_g1043 * temp_output_643_0_g1043 );
				float lerpResult614_g1043 = lerp( _FogHeightEnd , ( _FogHeightEnd + _FarDistanceHeight ) , FogDistanceMaskFar645_g1043);
				float temp_output_7_0_g1048 = lerpResult614_g1043;
				float temp_output_167_0_g1043 = saturate( ( ( ( break159_g1043.x + break159_g1043.y + break159_g1043.z ) - temp_output_7_0_g1048 ) / ( _FogHeightStart - temp_output_7_0_g1048 ) ) );
				#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch468_g1043 = temp_output_167_0_g1043;
				#else
				float staticSwitch468_g1043 = pow( abs( temp_output_167_0_g1043 ) , _FogHeightFalloff );
				#endif
				half FogHeightMask16_g1043 = staticSwitch468_g1043;
				float lerpResult328_g1043 = lerp( ( FogDistanceMask12_g1043 * FogHeightMask16_g1043 ) , saturate( ( FogDistanceMask12_g1043 + FogHeightMask16_g1043 ) ) , _FogLayersMode);
				float mulTime204_g1043 = _Time.y * 2.0;
				float3 temp_output_197_0_g1043 = ( ( WorldPosition2_g1043 * ( 1.0 / _NoiseScale ) ) + ( -_NoiseSpeed * mulTime204_g1043 ) );
				float3 p1_g1052 = temp_output_197_0_g1043;
				float localSimpleNoise3D1_g1052 = SimpleNoise3D( p1_g1052 );
				float temp_output_7_0_g1051 = _NoiseMin;
				float temp_output_7_0_g1050 = _NoiseDistanceEnd;
				half NoiseDistanceMask7_g1043 = saturate( ( ( distance( WorldPosition2_g1043 , _WorldSpaceCameraPos ) - temp_output_7_0_g1050 ) / ( 0.0 - temp_output_7_0_g1050 ) ) );
				float lerpResult198_g1043 = lerp( 1.0 , saturate( ( ( localSimpleNoise3D1_g1052 - temp_output_7_0_g1051 ) / ( _NoiseMax - temp_output_7_0_g1051 ) ) ) , ( NoiseDistanceMask7_g1043 * _NoiseIntensity ));
				half NoiseSimplex3D24_g1043 = lerpResult198_g1043;
				#ifdef AHF_DISABLE_NOISE3D
				float staticSwitch42_g1043 = lerpResult328_g1043;
				#else
				float staticSwitch42_g1043 = ( lerpResult328_g1043 * NoiseSimplex3D24_g1043 );
				#endif
				float temp_output_454_0_g1043 = ( staticSwitch42_g1043 * _FogIntensity );
				float3 normalizeResult169_g1043 = normalize( ( WorldPosition2_g1043 - _WorldSpaceCameraPos ) );
				float3 break170_g1043 = ( normalizeResult169_g1043 * AHF_FogAxisOption181_g1043 );
				float temp_output_449_0_g1043 = ( ( break170_g1043.x + break170_g1043.y + break170_g1043.z ) + -_SkyboxFogOffset );
				float temp_output_7_0_g1049 = _SkyboxFogHeight;
				float temp_output_176_0_g1043 = saturate( ( ( abs( temp_output_449_0_g1043 ) - temp_output_7_0_g1049 ) / ( 0.0 - temp_output_7_0_g1049 ) ) );
				float saferPower309_g1043 = abs( temp_output_176_0_g1043 );
				#ifdef AHF_DISABLE_FALLOFF
				float staticSwitch469_g1043 = temp_output_176_0_g1043;
				#else
				float staticSwitch469_g1043 = pow( saferPower309_g1043 , _SkyboxFogFalloff );
				#endif
				float lerpResult179_g1043 = lerp( saturate( ( staticSwitch469_g1043 + ( _SkyboxFogBottom * step( temp_output_449_0_g1043 , 0.0 ) ) ) ) , 1.0 , _SkyboxFogFill);
				half SkyboxFogHeightMask108_g1043 = ( lerpResult179_g1043 * _SkyboxFogIntensity );
				float clampDepth118_g1043 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy );
				#ifdef UNITY_REVERSED_Z
				float staticSwitch123_g1043 = clampDepth118_g1043;
				#else
				float staticSwitch123_g1043 = ( 1.0 - clampDepth118_g1043 );
				#endif
				half SkyboxFogMask95_g1043 = ( 1.0 - ceil( staticSwitch123_g1043 ) );
				float lerpResult112_g1043 = lerp( temp_output_454_0_g1043 , SkyboxFogHeightMask108_g1043 , SkyboxFogMask95_g1043);
				#ifdef AHF_DISABLE_SKYBOXFOG
				float staticSwitch455_g1043 = temp_output_454_0_g1043;
				#else
				float staticSwitch455_g1043 = lerpResult112_g1043;
				#endif
				half Final_Alpha463_g1043 = staticSwitch455_g1043;
				float4 appendResult114_g1043 = (float4(Final_Color462_g1043 , Final_Alpha463_g1043));
				float4 appendResult457_g1043 = (float4(WorldPosition2_g1043 , 1.0));
				#ifdef AHF_DEBUG_WORLDPOS
				float4 staticSwitch456_g1043 = appendResult457_g1043;
				#else
				float4 staticSwitch456_g1043 = appendResult114_g1043;
				#endif
				
				
				finalColor = staticSwitch456_g1043;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "HeightFogShaderGUI"
	
	Fallback Off
}
/*ASEBEGIN
Version=19109
Node;AmplifyShaderEditor.RangedFloatNode;1093;-3328,-4736;Inherit;False;Property;_Banner;Banner;0;0;Create;True;0;0;0;True;1;StyledBanner(Height Fog Standalone);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1106;-2880,-4736;Half;False;Property;_IsHeightFogShader;_IsHeightFogShader;44;1;[HideInInspector];Create;False;0;0;0;True;0;False;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;383;-3072,-4608;Float;False;True;-1;2;HeightFogShaderGUI;0;5;BOXOPHOBIC/Atmospherics/Height Fog Standalone;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;2;5;False;;10;False;;0;5;False;;10;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;1;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;222;False;;255;False;;255;False;;6;False;;2;False;;0;False;;0;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;7;False;;True;False;0;False;;1000;False;;True;2;RenderType=Overlay=RenderType;Queue=Overlay=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.CommentaryNode;1105;-3328,-4864;Inherit;False;919.8825;100;Drawers;0;;1,0.475862,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1107;-3136,-4736;Half;False;Property;_HeightFogStandalone;_HeightFogStandalone;43;1;[HideInInspector];Create;False;0;0;0;True;0;False;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1154;-3328,-4608;Inherit;False;Base;1;;1043;13c50910e5b86de4097e1181ba121e0e;36,360,1,376,1,380,1,372,1,384,1,476,1,450,1,382,1,370,1,378,1,386,1,555,1,557,1,388,1,550,1,374,1,347,1,351,1,685,1,339,1,392,1,355,1,116,1,364,1,361,1,366,1,597,1,343,1,354,1,99,1,500,1,603,1,681,1,345,1,368,1,349,1;0;3;FLOAT4;113;FLOAT3;86;FLOAT;87
WireConnection;383;0;1154;113
ASEEND*/
//CHKSM=2AA2D03ED06F1BAEA546B5984F84E4BF6861D99F