Shader "EGA/Particles/FireSphere"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Emission("Emission", Float) = 2
		_StartFrequency("Start Frequency", Float) = 4
		_Frequency("Frequency", Float) = 10
		_Amplitude("Amplitude", Float) = 1
		[Toggle]_Usedepth("Use depth?", Float) = 0
		_Depthpower("Depth power", Float) = 1
		[Toggle]_Useblack("Use black", Float) = 0
		_Opacity("Opacity", Float) = 1
		[HideInInspector] _tex3coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  "PreviewType"="Plane" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float3 uv_tex3coord;
			float4 screenPos;
		};

		uniform float _Useblack;
		uniform float _Emission;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float _StartFrequency;
		uniform float _Amplitude;
		uniform float _Frequency;
		uniform float _Usedepth;
		uniform float _Opacity;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depthpower;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 temp_output_121_0 = ( _Emission * _Color * i.vertexColor );
			float2 temp_output_8_0 = ( ( ( float2( 0.2,0 ) * _Time.y ) + (i.uv_tex3coord).xy + i.uv_tex3coord.z ) * _StartFrequency );
			float2 break18 = floor( temp_output_8_0 );
			float temp_output_21_0 = ( break18.x + ( break18.y * 57.0 ) );
			float2 temp_output_10_0 = frac( temp_output_8_0 );
			float2 temp_cast_1 = (3.0).xx;
			float2 break17 = ( temp_output_10_0 * temp_output_10_0 * ( temp_cast_1 - ( temp_output_10_0 * 2.0 ) ) );
			float lerpResult39 = lerp( frac( ( 473.5 * sin( temp_output_21_0 ) ) ) , frac( ( 473.5 * sin( ( 1.0 + temp_output_21_0 ) ) ) ) , break17.x);
			float lerpResult38 = lerp( frac( ( 473.5 * sin( ( 57.0 + temp_output_21_0 ) ) ) ) , frac( ( 473.5 * sin( ( 58.0 + temp_output_21_0 ) ) ) ) , break17.x);
			float lerpResult40 = lerp( lerpResult39 , lerpResult38 , break17.y);
			float3 temp_output_51_0 = ( ( float3( ( float2( 0.5,0.5 ) * _Time.y ) ,  0.0 ) + ( i.uv_tex3coord * ( lerpResult40 * _Amplitude ) ) + i.uv_tex3coord.z ) * _Frequency );
			float3 break87 = floor( temp_output_51_0 );
			float temp_output_90_0 = ( break87.x + ( break87.y * 57.0 ) );
			float3 temp_output_52_0 = frac( temp_output_51_0 );
			float3 temp_cast_3 = (3.0).xxx;
			float3 break110 = ( temp_output_52_0 * temp_output_52_0 * ( temp_cast_3 - ( temp_output_52_0 * 2.0 ) ) );
			float lerpResult109 = lerp( frac( ( 473.5 * sin( temp_output_90_0 ) ) ) , frac( ( 473.5 * sin( ( 1.0 + temp_output_90_0 ) ) ) ) , break110.x);
			float lerpResult105 = lerp( frac( ( 473.5 * sin( ( 57.0 + temp_output_90_0 ) ) ) ) , frac( ( 473.5 * sin( ( 58.0 + temp_output_90_0 ) ) ) ) , break110.x);
			float lerpResult106 = lerp( lerpResult109 , lerpResult105 , break110.y);
			float Amp114 = _Amplitude;
			float4 tex2DNode117 = tex2D( _MainTex, ( i.uv_tex3coord + ( 0.2 * ( lerpResult106 * Amp114 ) ) ).xy );
			o.Emission = lerp(temp_output_121_0,( temp_output_121_0 * tex2DNode117 ),_Useblack).rgb;
			float4 clampResult132 = clamp( ( i.vertexColor.a * tex2DNode117 * _Opacity ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth137 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth137 = abs( ( screenDepth137 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depthpower ) );
			float clampResult136 = clamp( distanceDepth137 , 0.0 , 1.0 );
			o.Alpha = lerp(clampResult132,( clampResult132 * clampResult136 ),_Usedepth).r;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=17000
165;194;1326;839;7535.3;-75.36414;2.204268;True;False
Node;AmplifyShaderEditor.Vector2Node;2;-8339.338,985.6799;Float;False;Constant;_Vector0;Vector 0;0;0;Create;True;0;0;False;0;0.2,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-8473.372,1250.548;Float;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;3;-8341.885,1115.568;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;143;-8218.255,1183.854;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-8151.904,1094.41;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;144;-8009.953,1231.439;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-8309.805,1408.944;Float;False;Property;_StartFrequency;Start Frequency;3;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-7973.625,1096.957;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-7813.932,1241.061;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FloorOpNode;9;-7547.65,1082.107;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;18;-7344.967,1083.629;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-7062.681,1160.891;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;57;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-7529.174,1580.68;Float;False;Constant;_Float1;Float 1;0;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-6806.613,1076.975;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;10;-7553.76,1363.48;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-7318.913,1456.596;Float;False;Constant;_Float2;Float 2;0;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-6584.13,372.1529;Float;False;2;2;0;FLOAT;58;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-6558.124,854.1409;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-7324.859,1541.259;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-6558.122,586.1391;Float;False;2;2;0;FLOAT;57;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-6356.774,280.2781;Float;False;Constant;_Float3;Float 3;0;0;Create;True;0;0;False;0;473.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;13;-7137.232,1516.901;Float;False;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;28;-6342.376,1074.423;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;27;-6346.015,841.5241;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;25;-6352.599,368.8428;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;26;-6347.835,586.7913;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-6081.798,583.8928;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-6090.109,1070.09;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-6086.764,368.9619;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-6085.954,843.6132;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-6956.632,1375.047;Float;True;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;35;-5814.168,580.2521;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;34;-5822.274,367.3298;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;17;-5831.321,1378.868;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FractNode;36;-5805.767,838.3047;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;37;-5797.52,1070.841;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;38;-5476.058,512.5654;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;41;-5269.879,1134.923;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;39;-5483.018,912.7736;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;-5117.311,718.1027;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-5037.042,955.2929;Float;False;Property;_Amplitude;Amplitude;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-4818.819,717.6736;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;47;-4804.05,458.4331;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;46;-4812.312,329.1589;Float;False;Constant;_Vector1;Vector 1;0;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-4916.955,572.0232;Float;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-4640.396,695.9497;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-4593.8,375.2411;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-4319.869,637.7733;Float;False;Property;_Frequency;Frequency;4;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-4351.151,377.1398;Float;True;3;3;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-4085.566,374.4995;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FloorOpNode;54;-3872.618,372.1716;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;87;-3682.724,363.05;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-3400.439,440.3125;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;57;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;52;-3877.763,629.3062;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-3144.371,356.3967;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-3868.931,860.0997;Float;False;Constant;_Float6;Float 6;0;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-3656.671,736.0167;Float;False;Constant;_Float4;Float 4;0;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-2895.881,-134.4393;Float;False;2;2;0;FLOAT;57;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-3662.617,820.6796;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-2921.888,-348.4255;Float;False;2;2;0;FLOAT;58;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;-2895.883,133.5624;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;95;-2685.592,-133.787;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;94;-2690.357,-351.7355;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;-3474.99,796.3214;Float;False;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-2694.532,-440.2999;Float;False;Constant;_Float5;Float 5;0;0;Create;True;0;0;False;0;473.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;97;-2680.133,353.8448;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;96;-2683.773,120.9456;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-2424.521,-351.6166;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-3294.389,654.4678;Float;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2423.712,123.0346;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-2427.867,349.5115;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-2419.556,-136.6857;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;107;-2143.523,117.7261;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;110;-2169.078,658.2891;Float;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FractNode;108;-2135.276,350.2626;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;103;-2160.031,-353.2485;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;104;-2151.925,-140.3264;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;105;-1813.815,-208.0128;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;109;-1820.775,192.1951;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;-4804.202,960.5021;Float;False;Amp;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;106;-1455.067,-2.476338;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-1404.034,234.1648;Float;False;114;Amp;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;113;-1181.266,-4.527974;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-1118.897,235.3207;Float;False;Constant;_Float7;Float 7;3;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-860.5004,-13.68515;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;-1202.528,-154.2663;Float;False;0;-1;3;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;125;-572.5908,-152.6133;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;117;140.8239,117.1614;Float;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;False;0;5228a04ef529d2641937cab585cc1a02;5228a04ef529d2641937cab585cc1a02;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;138;332.3323,563.8065;Float;False;Property;_Depthpower;Depth power;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;120;220.0708,-58.2219;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;131;282.9455,341.1415;Float;False;Property;_Opacity;Opacity;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;137;523.3648,521.5419;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;259.26,-305.5326;Float;False;Property;_Emission;Emission;2;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;530.514,154.4088;Float;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;119;199.4791,-221.3163;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;136;790.9812,444.1675;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;132;775.0079,151.4682;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;469.5908,-155.6279;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;973.0854,335.2226;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;640.5754,-58.20366;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;145;1134.603,147.4463;Float;False;Property;_Usedepth;Use depth?;6;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;129;244.175,-421.8842;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;146;800.4197,-159.0233;Float;False;Property;_Useblack;Use black;8;0;Create;True;0;0;False;0;0;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;1;1393.489,-33.16033;Float;False;True;2;Float;;0;0;Unlit;EGA/Particles/FireSphere;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;1;PreviewType=Plane;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;143;0;6;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;144;0;6;3
WireConnection;5;0;4;0
WireConnection;5;1;143;0
WireConnection;5;2;144;0
WireConnection;8;0;5;0
WireConnection;8;1;7;0
WireConnection;9;0;8;0
WireConnection;18;0;9;0
WireConnection;20;0;18;1
WireConnection;21;0;18;0
WireConnection;21;1;20;0
WireConnection;10;0;8;0
WireConnection;24;1;21;0
WireConnection;22;1;21;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;23;1;21;0
WireConnection;13;0;14;0
WireConnection;13;1;11;0
WireConnection;28;0;21;0
WireConnection;27;0;22;0
WireConnection;25;0;24;0
WireConnection;26;0;23;0
WireConnection;31;0;30;0
WireConnection;31;1;26;0
WireConnection;33;0;30;0
WireConnection;33;1;28;0
WireConnection;29;0;30;0
WireConnection;29;1;25;0
WireConnection;32;0;30;0
WireConnection;32;1;27;0
WireConnection;15;0;10;0
WireConnection;15;1;10;0
WireConnection;15;2;13;0
WireConnection;35;0;31;0
WireConnection;34;0;29;0
WireConnection;17;0;15;0
WireConnection;36;0;32;0
WireConnection;37;0;33;0
WireConnection;38;0;35;0
WireConnection;38;1;34;0
WireConnection;38;2;17;0
WireConnection;41;0;17;1
WireConnection;39;0;37;0
WireConnection;39;1;36;0
WireConnection;39;2;17;0
WireConnection;40;0;39;0
WireConnection;40;1;38;0
WireConnection;40;2;41;0
WireConnection;42;0;40;0
WireConnection;42;1;43;0
WireConnection;44;0;45;0
WireConnection;44;1;42;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;49;0;48;0
WireConnection;49;1;44;0
WireConnection;49;2;45;3
WireConnection;51;0;49;0
WireConnection;51;1;50;0
WireConnection;54;0;51;0
WireConnection;87;0;54;0
WireConnection;89;0;87;1
WireConnection;52;0;51;0
WireConnection;90;0;87;0
WireConnection;90;1;89;0
WireConnection;92;1;90;0
WireConnection;84;0;52;0
WireConnection;84;1;112;0
WireConnection;91;1;90;0
WireConnection;93;1;90;0
WireConnection;95;0;92;0
WireConnection;94;0;91;0
WireConnection;86;0;85;0
WireConnection;86;1;84;0
WireConnection;97;0;90;0
WireConnection;96;0;93;0
WireConnection;99;0;98;0
WireConnection;99;1;94;0
WireConnection;88;0;52;0
WireConnection;88;1;52;0
WireConnection;88;2;86;0
WireConnection;102;0;98;0
WireConnection;102;1;96;0
WireConnection;101;0;98;0
WireConnection;101;1;97;0
WireConnection;100;0;98;0
WireConnection;100;1;95;0
WireConnection;107;0;102;0
WireConnection;110;0;88;0
WireConnection;108;0;101;0
WireConnection;103;0;99;0
WireConnection;104;0;100;0
WireConnection;105;0;104;0
WireConnection;105;1;103;0
WireConnection;105;2;110;0
WireConnection;109;0;108;0
WireConnection;109;1;107;0
WireConnection;109;2;110;0
WireConnection;114;0;43;0
WireConnection;106;0;109;0
WireConnection;106;1;105;0
WireConnection;106;2;110;1
WireConnection;113;0;106;0
WireConnection;113;1;115;0
WireConnection;118;0;123;0
WireConnection;118;1;113;0
WireConnection;125;0;116;0
WireConnection;125;1;118;0
WireConnection;117;1;125;0
WireConnection;137;0;138;0
WireConnection;126;0;120;4
WireConnection;126;1;117;0
WireConnection;126;2;131;0
WireConnection;136;0;137;0
WireConnection;132;0;126;0
WireConnection;121;0;122;0
WireConnection;121;1;119;0
WireConnection;121;2;120;0
WireConnection;139;0;132;0
WireConnection;139;1;136;0
WireConnection;130;0;121;0
WireConnection;130;1;117;0
WireConnection;145;0;132;0
WireConnection;145;1;139;0
WireConnection;146;0;121;0
WireConnection;146;1;130;0
WireConnection;1;2;146;0
WireConnection;1;9;145;0
ASEEND*/
//CHKSM=BA134D5A112370580FAC7DC7683741E7C1BC9DF0