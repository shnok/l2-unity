// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/BOXOPHOBIC/The Vegetation Engine/Helpers/Packer"
{
	Properties
	{
		[HideInInspector]_MainTex("Dummy Texture", 2D) = "white" {}
		[NoScaleOffset]_Packer_TexR("Packer_TexR", 2D) = "white" {}
		[NoScaleOffset]_Packer_TexG("Packer_TexG", 2D) = "white" {}
		[NoScaleOffset]_Packer_TexB("Packer_TexB", 2D) = "white" {}
		[NoScaleOffset]_Packer_TexA("Packer_TexA", 2D) = "white" {}
		[Space(10)]_Packer_FloatR("Packer_FloatR", Range( 0 , 1)) = 0
		_Packer_FloatG("Packer_FloatG", Range( 0 , 1)) = 0
		_Packer_FloatB("Packer_FloatB", Range( 0 , 1)) = 0
		_Packer_FloatA("Packer_FloatA", Range( 0 , 1)) = 0
		[Space(10)]_Packer_Action0B("Packer_Action0B", Float) = 0
		[Space(10)]_Packer_Action1B("Packer_Action1B", Float) = 0
		[Space(10)]_Packer_Action0G("Packer_Action0G", Float) = 0
		[Space(10)]_Packer_Action2B("Packer_Action2B", Float) = 0
		[Space(10)]_Packer_Action2R("Packer_Action2R", Float) = 0
		[Space(10)]_Packer_Action1R("Packer_Action1R", Float) = 0
		[Space(10)]_Packer_Action0A("Packer_Action0A", Float) = 0
		[Space(10)]_Packer_Action2G("Packer_Action2G", Float) = 0
		[Space(10)]_Packer_Action1G("Packer_Action1G", Float) = 0
		[Space(10)]_Packer_Action2A("Packer_Action2A", Float) = 0
		[Space(10)]_Packer_Action0R("Packer_Action0R", Float) = 0
		[Space(10)]_Packer_TransformSpace("Packer_TransformSpace", Float) = 0
		[Space(10)]_Packer_Action1A("Packer_Action1A", Float) = 0
		[IntRange]_Packer_CoordR("Packer_CoordR", Range( 0 , 4)) = 0
		[IntRange]_Packer_CoordB("Packer_CoordB", Range( 0 , 4)) = 0
		[IntRange]_Packer_CoordG("Packer_CoordG", Range( 0 , 4)) = 0
		[IntRange]_Packer_CoordA("Packer_CoordA", Range( 0 , 4)) = 0
		[IntRange][Space(10)]_Packer_ChannelR("Packer_ChannelR", Range( 0 , 4)) = 0
		[IntRange]_Packer_ChannelG("Packer_ChannelG", Range( 0 , 4)) = 0
		[IntRange]_Packer_ChannelB("Packer_ChannelB", Range( 0 , 4)) = 0
		[ASEEnd][IntRange]_Packer_ChannelA("Packer_ChannelA", Range( 0 , 4)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" "PreviewType"="Plane" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			#define ASE_ABSOLUTE_VERTEX_POS 1


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform float _Packer_TransformSpace;
			uniform float _Packer_Action2R;
			uniform float _Packer_Action1R;
			uniform float _Packer_Action0R;
			uniform float _Packer_ChannelR;
			uniform float _Packer_FloatR;
			uniform sampler2D _Packer_TexR;
			uniform float _Packer_CoordR;
			uniform float _Packer_Action2G;
			uniform float _Packer_Action1G;
			uniform float _Packer_Action0G;
			uniform float _Packer_ChannelG;
			uniform float _Packer_FloatG;
			uniform sampler2D _Packer_TexG;
			uniform float _Packer_CoordG;
			uniform float _Packer_Action2B;
			uniform float _Packer_Action1B;
			uniform float _Packer_Action0B;
			uniform float _Packer_ChannelB;
			uniform float _Packer_FloatB;
			uniform sampler2D _Packer_TexB;
			uniform float _Packer_CoordB;
			uniform float _Packer_Action2A;
			uniform float _Packer_Action1A;
			uniform float _Packer_Action0A;
			uniform float _Packer_ChannelA;
			uniform float _Packer_FloatA;
			uniform sampler2D _Packer_TexA;
			uniform float _Packer_CoordA;
			inline float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max( 0.001f , dot( inVec , inVec ) );
				return inVec* rsqrt( dp3);
			}
			
			float3x3 Inverse3x3(float3x3 input)
			{
				float3 a = input._11_21_31;
				float3 b = input._12_22_32;
				float3 c = input._13_23_33;
				return float3x3(cross(b,c), cross(c,a), cross(a,b)) * (1.0 / dot(a,cross(b,c)));
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 appendResult410 = (float3(v.ase_texcoord.xy , 0.0));
				
				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord2.xyz = ase_worldTangent;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord1.zw = v.ase_texcoord1.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = appendResult410;
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
				float Packer_Transform362 = _Packer_TransformSpace;
				int Action2189_g65 = (int)_Packer_Action2R;
				int Action1187_g65 = (int)_Packer_Action1R;
				int Action0173_g65 = (int)_Packer_Action0R;
				int Channel31_g65 = (int)_Packer_ChannelR;
				float ifLocalVar24_g65 = 0;
				if( Channel31_g65 == 0 )
				ifLocalVar24_g65 = _Packer_FloatR;
				int Index42_g18 = (int)_Packer_CoordR;
				float2 ifLocalVar40_g18 = 0;
				if( Index42_g18 == 0.0 )
				ifLocalVar40_g18 = i.ase_texcoord1.xy;
				float2 ifLocalVar47_g18 = 0;
				if( Index42_g18 == 1.0 )
				ifLocalVar47_g18 = i.ase_texcoord1.zw;
				float2 ifLocalVar50_g18 = 0;
				if( Index42_g18 == 2.0 )
				ifLocalVar50_g18 = i.ase_texcoord1.zw;
				float2 ifLocalVar54_g18 = 0;
				if( Index42_g18 == 3.0 )
				ifLocalVar54_g18 = i.ase_texcoord1.zw;
				float4 temp_output_19_0_g65 = tex2Dlod( _Packer_TexR, float4( ( ifLocalVar40_g18 + ifLocalVar47_g18 + ifLocalVar50_g18 + ifLocalVar54_g18 ), 0, 0.0) );
				float4 break23_g65 = temp_output_19_0_g65;
				float R39_g65 = break23_g65.r;
				float ifLocalVar13_g65 = 0;
				if( Channel31_g65 == 1 )
				ifLocalVar13_g65 = R39_g65;
				float G40_g65 = break23_g65.g;
				float ifLocalVar12_g65 = 0;
				if( Channel31_g65 == 2 )
				ifLocalVar12_g65 = G40_g65;
				float B41_g65 = break23_g65.b;
				float ifLocalVar11_g65 = 0;
				if( Channel31_g65 == 3 )
				ifLocalVar11_g65 = B41_g65;
				float A42_g65 = break23_g65.a;
				float ifLocalVar17_g65 = 0;
				if( Channel31_g65 == 4 )
				ifLocalVar17_g65 = A42_g65;
				float Compute_Max265_g65 = max( max( R39_g65 , G40_g65 ) , B41_g65 );
				float ifLocalVar62_g65 = 0;
				if( Channel31_g65 == 111 )
				ifLocalVar62_g65 = Compute_Max265_g65;
				float3 RGB258_g65 = (temp_output_19_0_g65).rgb;
				float3 temp_output_3_0_g66 = RGB258_g65;
				float3 break22_g66 = temp_output_3_0_g66;
				float Compute_Gray135_g65 = ( ( break22_g66.x + break22_g66.y + break22_g66.z ) / 3.0 );
				float ifLocalVar271_g65 = 0;
				if( Channel31_g65 == 555 )
				ifLocalVar271_g65 = Compute_Gray135_g65;
				float Packed_Raw182_g65 = ( ifLocalVar24_g65 + ( ifLocalVar13_g65 + ifLocalVar12_g65 + ifLocalVar11_g65 + ifLocalVar17_g65 ) + ( ifLocalVar62_g65 + ifLocalVar271_g65 ) );
				float ifLocalVar180_g65 = 0;
				if( Action0173_g65 == 0 )
				ifLocalVar180_g65 = Packed_Raw182_g65;
				float ifLocalVar171_g65 = 0;
				if( Action0173_g65 == 1 )
				ifLocalVar171_g65 = ( 1.0 - Packed_Raw182_g65 );
				float Packed_Action0185_g65 = saturate( ( ifLocalVar180_g65 + ifLocalVar171_g65 ) );
				float ifLocalVar193_g65 = 0;
				if( Action1187_g65 == 0 )
				ifLocalVar193_g65 = Packed_Action0185_g65;
				float ifLocalVar197_g65 = 0;
				if( Action1187_g65 == 1 )
				ifLocalVar197_g65 = ( Packed_Action0185_g65 * 0.0 );
				float ifLocalVar207_g65 = 0;
				if( Action1187_g65 == 2 )
				ifLocalVar207_g65 = ( Packed_Action0185_g65 * 2.0 );
				float ifLocalVar248_g65 = 0;
				if( Action1187_g65 == 3 )
				ifLocalVar248_g65 = ( Packed_Action0185_g65 * 3.0 );
				float ifLocalVar211_g65 = 0;
				if( Action1187_g65 == 5 )
				ifLocalVar211_g65 = ( Packed_Action0185_g65 * 5.0 );
				float Packed_Action1202_g65 = saturate( ( ifLocalVar193_g65 + ifLocalVar197_g65 + ifLocalVar207_g65 + ifLocalVar248_g65 + ifLocalVar211_g65 ) );
				float ifLocalVar220_g65 = 0;
				if( Action2189_g65 == 0 )
				ifLocalVar220_g65 = Packed_Action1202_g65;
				float ifLocalVar254_g65 = 0;
				if( Action2189_g65 == 1 )
				ifLocalVar254_g65 = pow( Packed_Action1202_g65 , 0.0 );
				float ifLocalVar225_g65 = 0;
				if( Action2189_g65 == 2 )
				ifLocalVar225_g65 = pow( Packed_Action1202_g65 , 2.0 );
				float ifLocalVar229_g65 = 0;
				if( Action2189_g65 == 3 )
				ifLocalVar229_g65 = pow( Packed_Action1202_g65 , 3.0 );
				float ifLocalVar234_g65 = 0;
				if( Action2189_g65 == 4 )
				ifLocalVar234_g65 = pow( Packed_Action1202_g65 , 4.0 );
				float Packed_Action2237_g65 = saturate( ( ifLocalVar220_g65 + ifLocalVar254_g65 + ifLocalVar225_g65 + ifLocalVar229_g65 + ifLocalVar234_g65 ) );
				float R74 = Packed_Action2237_g65;
				int Action2189_g63 = (int)_Packer_Action2G;
				int Action1187_g63 = (int)_Packer_Action1G;
				int Action0173_g63 = (int)_Packer_Action0G;
				int Channel31_g63 = (int)_Packer_ChannelG;
				float ifLocalVar24_g63 = 0;
				if( Channel31_g63 == 0 )
				ifLocalVar24_g63 = _Packer_FloatG;
				int Index42_g19 = (int)_Packer_CoordG;
				float2 ifLocalVar40_g19 = 0;
				if( Index42_g19 == 0.0 )
				ifLocalVar40_g19 = i.ase_texcoord1.xy;
				float2 ifLocalVar47_g19 = 0;
				if( Index42_g19 == 1.0 )
				ifLocalVar47_g19 = i.ase_texcoord1.zw;
				float2 ifLocalVar50_g19 = 0;
				if( Index42_g19 == 2.0 )
				ifLocalVar50_g19 = i.ase_texcoord1.zw;
				float2 ifLocalVar54_g19 = 0;
				if( Index42_g19 == 3.0 )
				ifLocalVar54_g19 = i.ase_texcoord1.zw;
				float4 temp_output_19_0_g63 = tex2Dlod( _Packer_TexG, float4( ( ifLocalVar40_g19 + ifLocalVar47_g19 + ifLocalVar50_g19 + ifLocalVar54_g19 ), 0, 0.0) );
				float4 break23_g63 = temp_output_19_0_g63;
				float R39_g63 = break23_g63.r;
				float ifLocalVar13_g63 = 0;
				if( Channel31_g63 == 1 )
				ifLocalVar13_g63 = R39_g63;
				float G40_g63 = break23_g63.g;
				float ifLocalVar12_g63 = 0;
				if( Channel31_g63 == 2 )
				ifLocalVar12_g63 = G40_g63;
				float B41_g63 = break23_g63.b;
				float ifLocalVar11_g63 = 0;
				if( Channel31_g63 == 3 )
				ifLocalVar11_g63 = B41_g63;
				float A42_g63 = break23_g63.a;
				float ifLocalVar17_g63 = 0;
				if( Channel31_g63 == 4 )
				ifLocalVar17_g63 = A42_g63;
				float Compute_Max265_g63 = max( max( R39_g63 , G40_g63 ) , B41_g63 );
				float ifLocalVar62_g63 = 0;
				if( Channel31_g63 == 111 )
				ifLocalVar62_g63 = Compute_Max265_g63;
				float3 RGB258_g63 = (temp_output_19_0_g63).rgb;
				float3 temp_output_3_0_g64 = RGB258_g63;
				float3 break22_g64 = temp_output_3_0_g64;
				float Compute_Gray135_g63 = ( ( break22_g64.x + break22_g64.y + break22_g64.z ) / 3.0 );
				float ifLocalVar271_g63 = 0;
				if( Channel31_g63 == 555 )
				ifLocalVar271_g63 = Compute_Gray135_g63;
				float Packed_Raw182_g63 = ( ifLocalVar24_g63 + ( ifLocalVar13_g63 + ifLocalVar12_g63 + ifLocalVar11_g63 + ifLocalVar17_g63 ) + ( ifLocalVar62_g63 + ifLocalVar271_g63 ) );
				float ifLocalVar180_g63 = 0;
				if( Action0173_g63 == 0 )
				ifLocalVar180_g63 = Packed_Raw182_g63;
				float ifLocalVar171_g63 = 0;
				if( Action0173_g63 == 1 )
				ifLocalVar171_g63 = ( 1.0 - Packed_Raw182_g63 );
				float Packed_Action0185_g63 = saturate( ( ifLocalVar180_g63 + ifLocalVar171_g63 ) );
				float ifLocalVar193_g63 = 0;
				if( Action1187_g63 == 0 )
				ifLocalVar193_g63 = Packed_Action0185_g63;
				float ifLocalVar197_g63 = 0;
				if( Action1187_g63 == 1 )
				ifLocalVar197_g63 = ( Packed_Action0185_g63 * 0.0 );
				float ifLocalVar207_g63 = 0;
				if( Action1187_g63 == 2 )
				ifLocalVar207_g63 = ( Packed_Action0185_g63 * 2.0 );
				float ifLocalVar248_g63 = 0;
				if( Action1187_g63 == 3 )
				ifLocalVar248_g63 = ( Packed_Action0185_g63 * 3.0 );
				float ifLocalVar211_g63 = 0;
				if( Action1187_g63 == 5 )
				ifLocalVar211_g63 = ( Packed_Action0185_g63 * 5.0 );
				float Packed_Action1202_g63 = saturate( ( ifLocalVar193_g63 + ifLocalVar197_g63 + ifLocalVar207_g63 + ifLocalVar248_g63 + ifLocalVar211_g63 ) );
				float ifLocalVar220_g63 = 0;
				if( Action2189_g63 == 0 )
				ifLocalVar220_g63 = Packed_Action1202_g63;
				float ifLocalVar254_g63 = 0;
				if( Action2189_g63 == 1 )
				ifLocalVar254_g63 = pow( Packed_Action1202_g63 , 0.0 );
				float ifLocalVar225_g63 = 0;
				if( Action2189_g63 == 2 )
				ifLocalVar225_g63 = pow( Packed_Action1202_g63 , 2.0 );
				float ifLocalVar229_g63 = 0;
				if( Action2189_g63 == 3 )
				ifLocalVar229_g63 = pow( Packed_Action1202_g63 , 3.0 );
				float ifLocalVar234_g63 = 0;
				if( Action2189_g63 == 4 )
				ifLocalVar234_g63 = pow( Packed_Action1202_g63 , 4.0 );
				float Packed_Action2237_g63 = saturate( ( ifLocalVar220_g63 + ifLocalVar254_g63 + ifLocalVar225_g63 + ifLocalVar229_g63 + ifLocalVar234_g63 ) );
				float G78 = Packed_Action2237_g63;
				int Action2189_g59 = (int)_Packer_Action2B;
				int Action1187_g59 = (int)_Packer_Action1B;
				int Action0173_g59 = (int)_Packer_Action0B;
				int Channel31_g59 = (int)_Packer_ChannelB;
				float ifLocalVar24_g59 = 0;
				if( Channel31_g59 == 0 )
				ifLocalVar24_g59 = _Packer_FloatB;
				int Index42_g17 = (int)_Packer_CoordB;
				float2 ifLocalVar40_g17 = 0;
				if( Index42_g17 == 0.0 )
				ifLocalVar40_g17 = i.ase_texcoord1.xy;
				float2 ifLocalVar47_g17 = 0;
				if( Index42_g17 == 1.0 )
				ifLocalVar47_g17 = i.ase_texcoord1.zw;
				float2 ifLocalVar50_g17 = 0;
				if( Index42_g17 == 2.0 )
				ifLocalVar50_g17 = i.ase_texcoord1.zw;
				float2 ifLocalVar54_g17 = 0;
				if( Index42_g17 == 3.0 )
				ifLocalVar54_g17 = i.ase_texcoord1.zw;
				float4 temp_output_19_0_g59 = tex2Dlod( _Packer_TexB, float4( ( ifLocalVar40_g17 + ifLocalVar47_g17 + ifLocalVar50_g17 + ifLocalVar54_g17 ), 0, 0.0) );
				float4 break23_g59 = temp_output_19_0_g59;
				float R39_g59 = break23_g59.r;
				float ifLocalVar13_g59 = 0;
				if( Channel31_g59 == 1 )
				ifLocalVar13_g59 = R39_g59;
				float G40_g59 = break23_g59.g;
				float ifLocalVar12_g59 = 0;
				if( Channel31_g59 == 2 )
				ifLocalVar12_g59 = G40_g59;
				float B41_g59 = break23_g59.b;
				float ifLocalVar11_g59 = 0;
				if( Channel31_g59 == 3 )
				ifLocalVar11_g59 = B41_g59;
				float A42_g59 = break23_g59.a;
				float ifLocalVar17_g59 = 0;
				if( Channel31_g59 == 4 )
				ifLocalVar17_g59 = A42_g59;
				float Compute_Max265_g59 = max( max( R39_g59 , G40_g59 ) , B41_g59 );
				float ifLocalVar62_g59 = 0;
				if( Channel31_g59 == 111 )
				ifLocalVar62_g59 = Compute_Max265_g59;
				float3 RGB258_g59 = (temp_output_19_0_g59).rgb;
				float3 temp_output_3_0_g60 = RGB258_g59;
				float3 break22_g60 = temp_output_3_0_g60;
				float Compute_Gray135_g59 = ( ( break22_g60.x + break22_g60.y + break22_g60.z ) / 3.0 );
				float ifLocalVar271_g59 = 0;
				if( Channel31_g59 == 555 )
				ifLocalVar271_g59 = Compute_Gray135_g59;
				float Packed_Raw182_g59 = ( ifLocalVar24_g59 + ( ifLocalVar13_g59 + ifLocalVar12_g59 + ifLocalVar11_g59 + ifLocalVar17_g59 ) + ( ifLocalVar62_g59 + ifLocalVar271_g59 ) );
				float ifLocalVar180_g59 = 0;
				if( Action0173_g59 == 0 )
				ifLocalVar180_g59 = Packed_Raw182_g59;
				float ifLocalVar171_g59 = 0;
				if( Action0173_g59 == 1 )
				ifLocalVar171_g59 = ( 1.0 - Packed_Raw182_g59 );
				float Packed_Action0185_g59 = saturate( ( ifLocalVar180_g59 + ifLocalVar171_g59 ) );
				float ifLocalVar193_g59 = 0;
				if( Action1187_g59 == 0 )
				ifLocalVar193_g59 = Packed_Action0185_g59;
				float ifLocalVar197_g59 = 0;
				if( Action1187_g59 == 1 )
				ifLocalVar197_g59 = ( Packed_Action0185_g59 * 0.0 );
				float ifLocalVar207_g59 = 0;
				if( Action1187_g59 == 2 )
				ifLocalVar207_g59 = ( Packed_Action0185_g59 * 2.0 );
				float ifLocalVar248_g59 = 0;
				if( Action1187_g59 == 3 )
				ifLocalVar248_g59 = ( Packed_Action0185_g59 * 3.0 );
				float ifLocalVar211_g59 = 0;
				if( Action1187_g59 == 5 )
				ifLocalVar211_g59 = ( Packed_Action0185_g59 * 5.0 );
				float Packed_Action1202_g59 = saturate( ( ifLocalVar193_g59 + ifLocalVar197_g59 + ifLocalVar207_g59 + ifLocalVar248_g59 + ifLocalVar211_g59 ) );
				float ifLocalVar220_g59 = 0;
				if( Action2189_g59 == 0 )
				ifLocalVar220_g59 = Packed_Action1202_g59;
				float ifLocalVar254_g59 = 0;
				if( Action2189_g59 == 1 )
				ifLocalVar254_g59 = pow( Packed_Action1202_g59 , 0.0 );
				float ifLocalVar225_g59 = 0;
				if( Action2189_g59 == 2 )
				ifLocalVar225_g59 = pow( Packed_Action1202_g59 , 2.0 );
				float ifLocalVar229_g59 = 0;
				if( Action2189_g59 == 3 )
				ifLocalVar229_g59 = pow( Packed_Action1202_g59 , 3.0 );
				float ifLocalVar234_g59 = 0;
				if( Action2189_g59 == 4 )
				ifLocalVar234_g59 = pow( Packed_Action1202_g59 , 4.0 );
				float Packed_Action2237_g59 = saturate( ( ifLocalVar220_g59 + ifLocalVar254_g59 + ifLocalVar225_g59 + ifLocalVar229_g59 + ifLocalVar234_g59 ) );
				float B79 = Packed_Action2237_g59;
				int Action2189_g61 = (int)_Packer_Action2A;
				int Action1187_g61 = (int)_Packer_Action1A;
				int Action0173_g61 = (int)_Packer_Action0A;
				int Channel31_g61 = (int)_Packer_ChannelA;
				float ifLocalVar24_g61 = 0;
				if( Channel31_g61 == 0 )
				ifLocalVar24_g61 = _Packer_FloatA;
				int Index42_g16 = (int)_Packer_CoordA;
				float2 ifLocalVar40_g16 = 0;
				if( Index42_g16 == 0.0 )
				ifLocalVar40_g16 = i.ase_texcoord1.xy;
				float2 ifLocalVar47_g16 = 0;
				if( Index42_g16 == 1.0 )
				ifLocalVar47_g16 = i.ase_texcoord1.zw;
				float2 ifLocalVar50_g16 = 0;
				if( Index42_g16 == 2.0 )
				ifLocalVar50_g16 = i.ase_texcoord1.zw;
				float2 ifLocalVar54_g16 = 0;
				if( Index42_g16 == 3.0 )
				ifLocalVar54_g16 = i.ase_texcoord1.zw;
				float4 temp_output_19_0_g61 = tex2Dlod( _Packer_TexA, float4( ( ifLocalVar40_g16 + ifLocalVar47_g16 + ifLocalVar50_g16 + ifLocalVar54_g16 ), 0, 0.0) );
				float4 break23_g61 = temp_output_19_0_g61;
				float R39_g61 = break23_g61.r;
				float ifLocalVar13_g61 = 0;
				if( Channel31_g61 == 1 )
				ifLocalVar13_g61 = R39_g61;
				float G40_g61 = break23_g61.g;
				float ifLocalVar12_g61 = 0;
				if( Channel31_g61 == 2 )
				ifLocalVar12_g61 = G40_g61;
				float B41_g61 = break23_g61.b;
				float ifLocalVar11_g61 = 0;
				if( Channel31_g61 == 3 )
				ifLocalVar11_g61 = B41_g61;
				float A42_g61 = break23_g61.a;
				float ifLocalVar17_g61 = 0;
				if( Channel31_g61 == 4 )
				ifLocalVar17_g61 = A42_g61;
				float Compute_Max265_g61 = max( max( R39_g61 , G40_g61 ) , B41_g61 );
				float ifLocalVar62_g61 = 0;
				if( Channel31_g61 == 111 )
				ifLocalVar62_g61 = Compute_Max265_g61;
				float3 RGB258_g61 = (temp_output_19_0_g61).rgb;
				float3 temp_output_3_0_g62 = RGB258_g61;
				float3 break22_g62 = temp_output_3_0_g62;
				float Compute_Gray135_g61 = ( ( break22_g62.x + break22_g62.y + break22_g62.z ) / 3.0 );
				float ifLocalVar271_g61 = 0;
				if( Channel31_g61 == 555 )
				ifLocalVar271_g61 = Compute_Gray135_g61;
				float Packed_Raw182_g61 = ( ifLocalVar24_g61 + ( ifLocalVar13_g61 + ifLocalVar12_g61 + ifLocalVar11_g61 + ifLocalVar17_g61 ) + ( ifLocalVar62_g61 + ifLocalVar271_g61 ) );
				float ifLocalVar180_g61 = 0;
				if( Action0173_g61 == 0 )
				ifLocalVar180_g61 = Packed_Raw182_g61;
				float ifLocalVar171_g61 = 0;
				if( Action0173_g61 == 1 )
				ifLocalVar171_g61 = ( 1.0 - Packed_Raw182_g61 );
				float Packed_Action0185_g61 = saturate( ( ifLocalVar180_g61 + ifLocalVar171_g61 ) );
				float ifLocalVar193_g61 = 0;
				if( Action1187_g61 == 0 )
				ifLocalVar193_g61 = Packed_Action0185_g61;
				float ifLocalVar197_g61 = 0;
				if( Action1187_g61 == 1 )
				ifLocalVar197_g61 = ( Packed_Action0185_g61 * 0.0 );
				float ifLocalVar207_g61 = 0;
				if( Action1187_g61 == 2 )
				ifLocalVar207_g61 = ( Packed_Action0185_g61 * 2.0 );
				float ifLocalVar248_g61 = 0;
				if( Action1187_g61 == 3 )
				ifLocalVar248_g61 = ( Packed_Action0185_g61 * 3.0 );
				float ifLocalVar211_g61 = 0;
				if( Action1187_g61 == 5 )
				ifLocalVar211_g61 = ( Packed_Action0185_g61 * 5.0 );
				float Packed_Action1202_g61 = saturate( ( ifLocalVar193_g61 + ifLocalVar197_g61 + ifLocalVar207_g61 + ifLocalVar248_g61 + ifLocalVar211_g61 ) );
				float ifLocalVar220_g61 = 0;
				if( Action2189_g61 == 0 )
				ifLocalVar220_g61 = Packed_Action1202_g61;
				float ifLocalVar254_g61 = 0;
				if( Action2189_g61 == 1 )
				ifLocalVar254_g61 = pow( Packed_Action1202_g61 , 0.0 );
				float ifLocalVar225_g61 = 0;
				if( Action2189_g61 == 2 )
				ifLocalVar225_g61 = pow( Packed_Action1202_g61 , 2.0 );
				float ifLocalVar229_g61 = 0;
				if( Action2189_g61 == 3 )
				ifLocalVar229_g61 = pow( Packed_Action1202_g61 , 3.0 );
				float ifLocalVar234_g61 = 0;
				if( Action2189_g61 == 4 )
				ifLocalVar234_g61 = pow( Packed_Action1202_g61 , 4.0 );
				float Packed_Action2237_g61 = saturate( ( ifLocalVar220_g61 + ifLocalVar254_g61 + ifLocalVar225_g61 + ifLocalVar229_g61 + ifLocalVar234_g61 ) );
				float A80 = Packed_Action2237_g61;
				float4 appendResult48 = (float4(R74 , G78 , B79 , A80));
				float4 Final_Color343 = appendResult48;
				float4 ifLocalVar360 = 0;
				if( Packer_Transform362 == 0.0 )
				ifLocalVar360 = Final_Color343;
				float3 appendResult379 = (float3(R74 , G78 , B79));
				half3 linearToGamma389 = appendResult379;
				linearToGamma389 = half3( LinearToGammaSpaceExact(linearToGamma389.r), LinearToGammaSpaceExact(linearToGamma389.g), LinearToGammaSpaceExact(linearToGamma389.b) );
				float4 appendResult380 = (float4(linearToGamma389 , A80));
				float4 Final_LinearToGamma388 = appendResult380;
				float4 ifLocalVar364 = 0;
				if( Packer_Transform362 == 1.0 )
				ifLocalVar364 = Final_LinearToGamma388;
				float3 appendResult384 = (float3(R74 , G78 , B79));
				float3 gammaToLinear390 = GammaToLinearSpace( appendResult384 );
				float4 appendResult385 = (float4(gammaToLinear390 , A80));
				float4 Final_GammaToLinear387 = appendResult385;
				float4 ifLocalVar369 = 0;
				if( Packer_Transform362 == 2.0 )
				ifLocalVar369 = Final_GammaToLinear387;
				float3 appendResult345 = (float3(R74 , G78 , B79));
				float3 ase_worldTangent = i.ase_texcoord2.xyz;
				float3 ase_worldNormal = i.ase_texcoord3.xyz;
				float3 ase_worldBitangent = i.ase_texcoord4.xyz;
				float3x3 ase_worldToTangent = float3x3(ase_worldTangent,ase_worldBitangent,ase_worldNormal);
				float3 objectToTangentDir349 = ASESafeNormalize( mul( ase_worldToTangent, mul( unity_ObjectToWorld, float4( (appendResult345*2.0 + -1.0), 0 ) ).xyz) );
				float4 appendResult350 = (float4((objectToTangentDir349*0.5 + 0.5) , A80));
				float4 Final_ObjectToTangent348 = appendResult350;
				float4 ifLocalVar392 = 0;
				if( Packer_Transform362 == 3.0 )
				ifLocalVar392 = Final_ObjectToTangent348;
				float3 appendResult358 = (float3(R74 , G78 , B79));
				float3x3 ase_tangentToWorldPrecise = Inverse3x3( ase_worldToTangent );
				float3 tangentTobjectDir356 = ASESafeNormalize( mul( unity_WorldToObject, float4( mul( ase_tangentToWorldPrecise, (appendResult358*2.0 + -1.0) ), 0 ) ).xyz );
				float4 appendResult359 = (float4((tangentTobjectDir356*0.5 + 0.5) , A80));
				float4 Final_TangentToObject357 = appendResult359;
				float4 ifLocalVar395 = 0;
				if( Packer_Transform362 == 4.0 )
				ifLocalVar395 = Final_TangentToObject357;
				
				
				finalColor = ( ifLocalVar360 + ifLocalVar364 + ifLocalVar369 + ifLocalVar392 + ifLocalVar395 );
				return finalColor;
			}
			ENDCG
		}
	}
	
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.RangedFloatNode;323;-768,2688;Float;False;Property;_Packer_CoordA;Packer_CoordA;29;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-768,1792;Float;False;Property;_Packer_CoordB;Packer_CoordB;25;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;317;-768,896;Float;False;Property;_Packer_CoordG;Packer_CoordG;27;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;309;-768,0;Float;False;Property;_Packer_CoordR;Packer_CoordR;22;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;322;-384,2688;Inherit;False;Tool Coords;-1;;16;da63d4e23383c5c4298f6ecca9db7d6f;0;1;34;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;319;-384,1792;Inherit;False;Tool Coords;-1;;17;da63d4e23383c5c4298f6ecca9db7d6f;0;1;34;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;314;-384,0;Inherit;False;Tool Coords;-1;;18;da63d4e23383c5c4298f6ecca9db7d6f;0;1;34;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;316;-384,896;Inherit;False;Tool Coords;-1;;19;da63d4e23383c5c4298f6ecca9db7d6f;0;1;34;INT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;283;-128,512;Float;False;Property;_Packer_Action2R;Packer_Action2R;13;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;289;-128,2304;Float;False;Property;_Packer_Action2B;Packer_Action2B;12;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;287;-128,2176;Float;False;Property;_Packer_Action0B;Packer_Action0B;9;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;60;-128,2688;Inherit;True;Property;_Packer_TexA;Packer_TexA;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;285;-128,1344;Float;False;Property;_Packer_Action1G;Packer_Action1G;17;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-128,1984;Float;False;Property;_Packer_FloatB;Packer_FloatB;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-128,1168;Float;False;Property;_Packer_ChannelG;Packer_ChannelG;31;1;[IntRange];Create;True;0;0;0;False;0;False;0;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;282;-128,448;Float;False;Property;_Packer_Action1R;Packer_Action1R;14;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-128,192;Float;False;Property;_Packer_FloatR;Packer_FloatR;5;0;Create;True;0;0;0;False;1;Space(10);False;0;0.519;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-128,2064;Float;False;Property;_Packer_ChannelB;Packer_ChannelB;32;1;[IntRange];Create;True;0;0;0;False;0;False;0;3;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-128,272;Float;False;Property;_Packer_ChannelR;Packer_ChannelR;30;1;[IntRange];Create;True;0;0;0;False;1;Space(10);False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-128,2960;Float;False;Property;_Packer_ChannelA;Packer_ChannelA;33;1;[IntRange];Create;True;0;0;0;False;0;False;0;4;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-128,2880;Float;False;Property;_Packer_FloatA;Packer_FloatA;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;-128,2240;Float;False;Property;_Packer_Action1B;Packer_Action1B;10;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;290;-128,3072;Float;False;Property;_Packer_Action0A;Packer_Action0A;15;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;284;-128,1280;Float;False;Property;_Packer_Action0G;Packer_Action0G;11;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;291;-128,3136;Float;False;Property;_Packer_Action1A;Packer_Action1A;21;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;57;-128,1792;Inherit;True;Property;_Packer_TexB;Packer_TexB;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;-128,1088;Float;False;Property;_Packer_FloatG;Packer_FloatG;6;0;Create;True;0;0;0;False;0;False;0;0.356;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;286;-128,1408;Float;False;Property;_Packer_Action2G;Packer_Action2G;16;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;292;-128,3200;Float;False;Property;_Packer_Action2A;Packer_Action2A;18;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;318;-768,960;Float;False;Property;_Packer_LayerG;Packer_LayerG;23;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;324;-768,2752;Float;False;Property;_Packer_LayerA;Packer_LayerA;28;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;321;-768,1856;Float;False;Property;_Packer_LayerB;Packer_LayerB;26;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;315;-768,64;Float;False;Property;_Packer_LayerR;Packer_LayerR;24;1;[IntRange];Create;True;0;0;0;False;0;False;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;720,0;Float;False;R;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;720,896;Float;False;G;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;720,1792;Float;False;B;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;720,2672;Float;False;A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;143;1664,0;Inherit;False;74;R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;1920,0;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;1664,64;Inherit;False;78;G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;146;1664,128;Inherit;False;79;B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;257;-128,384;Float;False;Property;_Packer_Action0R;Packer_Action0R;19;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;155;-128,-896;Inherit;True;Property;_MainTex;Dummy Texture;0;1;[HideInInspector];Create;False;0;0;0;True;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;362;128,-640;Inherit;False;Packer_Transform;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-128,0;Inherit;True;Property;_Packer_TexR;Packer_TexR;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;371;384,1792;Inherit;False;Tool Packer;-1;;59;7c427933118005a479c95a1271b737a6;0;7;19;COLOR;0,0,0,0;False;25;FLOAT;0;False;10;INT;0;False;172;INT;0;False;241;INT;0;False;242;INT;0;False;243;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;372;384,2688;Inherit;False;Tool Packer;-1;;61;7c427933118005a479c95a1271b737a6;0;7;19;COLOR;0,0,0,0;False;25;FLOAT;0;False;10;INT;0;False;172;INT;0;False;241;INT;0;False;242;INT;0;False;243;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;373;384,896;Inherit;False;Tool Packer;-1;;63;7c427933118005a479c95a1271b737a6;0;7;19;COLOR;0,0,0,0;False;25;FLOAT;0;False;10;INT;0;False;172;INT;0;False;241;INT;0;False;242;INT;0;False;243;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;374;384,0;Inherit;False;Tool Packer;-1;;65;7c427933118005a479c95a1271b737a6;0;7;19;COLOR;0,0,0,0;False;25;FLOAT;0;False;10;INT;0;False;172;INT;0;False;241;INT;0;False;242;INT;0;False;243;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;375;1664,384;Inherit;False;74;R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;376;1664,448;Inherit;False;78;G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;377;1664,512;Inherit;False;79;B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;379;1920,384;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;381;1664,768;Inherit;False;74;R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;382;1664,832;Inherit;False;78;G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;383;1664,896;Inherit;False;79;B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;384;1920,768;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;344;1664,1152;Inherit;False;74;R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;346;1664,1216;Inherit;False;78;G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;347;1664,1280;Inherit;False;79;B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;353;1664,1536;Inherit;False;74;R;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;354;1664,1600;Inherit;False;78;G;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;355;1664,1664;Inherit;False;79;B;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;358;1920,1536;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;1664,192;Inherit;False;80;A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;343;3152,0;Inherit;False;Final_Color;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;351;3840,64;Inherit;False;343;Final_Color;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;380;2944,384;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;385;2944,768;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;359;2944,1536;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;365;3840,256;Inherit;False;362;Packer_Transform;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;366;3840,320;Inherit;False;388;Final_LinearToGamma;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;367;3840,512;Inherit;False;362;Packer_Transform;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;368;3840,576;Inherit;False;387;Final_GammaToLinear;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;394;3840,1024;Inherit;False;362;Packer_Transform;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;396;3840,1088;Inherit;False;357;Final_TangentToObject;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;388;3120,384;Inherit;False;Final_LinearToGamma;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;387;3120,768;Inherit;False;Final_GammaToLinear;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;3104,1536;Inherit;False;Final_TangentToObject;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ConditionalIfNode;360;4224,0;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ConditionalIfNode;364;4224,256;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ConditionalIfNode;369;4224,512;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ConditionalIfNode;392;4224,768;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ConditionalIfNode;395;4224,1024;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;370;4736,0;Inherit;False;5;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;399;2560,1344;Inherit;False;80;A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;403;2112,1536;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;400;2560,1728;Inherit;False;80;A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;398;2560,896;Inherit;False;80;A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;397;2560,512;Inherit;False;80;A;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;402;2560,1152;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;404;2560,1536;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;3840,768;Inherit;False;362;Packer_Transform;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-128,896;Inherit;True;Property;_Packer_TexG;Packer_TexG;2;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToTexture2D;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LinearToGammaNode;389;2304,384;Inherit;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GammaToLinearNode;390;2304,768;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;350;2944,1152;Inherit;False;FLOAT4;4;0;FLOAT3;0.5,0,0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;363;3840,0;Inherit;False;362;Packer_Transform;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;356;2304,1536;Inherit;False;Tangent;Object;True;Precise;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleAndOffsetNode;401;2112,1152;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;348;3120,1152;Inherit;False;Final_ObjectToTangent;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;393;3840,832;Inherit;False;348;Final_ObjectToTangent;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;345;1920,1152;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformDirectionNode;349;2304,1152;Inherit;False;Object;Tangent;True;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;407;2130.034,1023.65;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;408;2144.385,839.9318;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;361;-128,-640;Float;False;Property;_Packer_TransformSpace;Packer_TransformSpace;20;0;Create;True;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;5504,0;Float;False;True;-1;2;;0;5;Hidden/BOXOPHOBIC/The Vegetation Engine/Helpers/Packer;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Opaque=RenderType;PreviewType=Plane;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;0;638156234381087878;0;1;True;False;;False;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;409;4736,256;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;410;4992,256;Inherit;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
WireConnection;322;34;323;0
WireConnection;319;34;320;0
WireConnection;314;34;309;0
WireConnection;316;34;317;0
WireConnection;60;1;322;0
WireConnection;57;1;319;0
WireConnection;74;0;374;0
WireConnection;78;0;373;0
WireConnection;79;0;371;0
WireConnection;80;0;372;0
WireConnection;48;0;143;0
WireConnection;48;1;144;0
WireConnection;48;2;146;0
WireConnection;48;3;145;0
WireConnection;362;0;361;0
WireConnection;26;1;314;0
WireConnection;371;19;57;0
WireConnection;371;25;59;0
WireConnection;371;10;67;0
WireConnection;371;172;287;0
WireConnection;371;241;288;0
WireConnection;371;242;289;0
WireConnection;372;19;60;0
WireConnection;372;25;64;0
WireConnection;372;10;68;0
WireConnection;372;172;290;0
WireConnection;372;241;291;0
WireConnection;372;242;292;0
WireConnection;373;19;51;0
WireConnection;373;25;50;0
WireConnection;373;10;66;0
WireConnection;373;172;284;0
WireConnection;373;241;285;0
WireConnection;373;242;286;0
WireConnection;374;19;26;0
WireConnection;374;25;47;0
WireConnection;374;10;65;0
WireConnection;374;172;257;0
WireConnection;374;241;282;0
WireConnection;374;242;283;0
WireConnection;379;0;375;0
WireConnection;379;1;376;0
WireConnection;379;2;377;0
WireConnection;384;0;381;0
WireConnection;384;1;382;0
WireConnection;384;2;383;0
WireConnection;358;0;353;0
WireConnection;358;1;354;0
WireConnection;358;2;355;0
WireConnection;343;0;48;0
WireConnection;380;0;389;0
WireConnection;380;3;397;0
WireConnection;385;0;390;0
WireConnection;385;3;398;0
WireConnection;359;0;404;0
WireConnection;359;3;400;0
WireConnection;388;0;380;0
WireConnection;387;0;385;0
WireConnection;357;0;359;0
WireConnection;360;0;363;0
WireConnection;360;3;351;0
WireConnection;364;0;365;0
WireConnection;364;3;366;0
WireConnection;369;0;367;0
WireConnection;369;3;368;0
WireConnection;392;0;391;0
WireConnection;392;3;393;0
WireConnection;395;0;394;0
WireConnection;395;3;396;0
WireConnection;370;0;360;0
WireConnection;370;1;364;0
WireConnection;370;2;369;0
WireConnection;370;3;392;0
WireConnection;370;4;395;0
WireConnection;403;0;358;0
WireConnection;402;0;349;0
WireConnection;404;0;356;0
WireConnection;51;1;316;0
WireConnection;389;0;379;0
WireConnection;390;0;384;0
WireConnection;350;0;402;0
WireConnection;350;3;399;0
WireConnection;356;0;403;0
WireConnection;401;0;345;0
WireConnection;348;0;350;0
WireConnection;345;0;344;0
WireConnection;345;1;346;0
WireConnection;345;2;347;0
WireConnection;349;0;401;0
WireConnection;0;0;370;0
WireConnection;0;1;410;0
WireConnection;410;0;409;0
ASEEND*/
//CHKSM=EDC168C3FD11D45E35B2361695D23FEF82285C0A
