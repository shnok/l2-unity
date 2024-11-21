// Upgrade NOTE: upgraded instancing buffer 'HiddenBOXOPHOBICTheVegetationEngineElementsMotionTurbulence' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/BOXOPHOBIC/The Vegetation Engine/Elements/Motion Turbulence"
{
	Properties
	{
		[StyledBanner(Motion Turbulence Element)]_Banner("Banner", Float) = 0
		[StyledMessage(Info, Use the Motion Turbulence elements to add noise to the motion direction. Element Texture A is used as alpha mask. Particle Alpha is used as Element Intensity multiplier. The noise is animated in the element forward direction and it is updated with particles or in play mode only., 0,0)]_Message("Message", Float) = 0
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMask(TVELayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[Enum(Affect Material Interaction,13,Affect Interaction and Wind Power,15)]_ElementMotionMode("Render Effect", Float) = 15
		[Enum(Element Forward,10,Element Texture,20,Particle Translate,30,Particle Velocity,40)]_ElementDirectionMode("Render Direction", Float) = 20
		[StyledCategory(Element Settings)]_CategoryElement("[ Category Element ]", Float) = 0
		[NoScaleOffset][StyledTextureSingleLine]_MainTex("Element Texture", 2D) = "white" {}
		[StyledSpace(10)]_SpaceTexture("#SpaceTexture", Float) = 0
		[Enum(Main UVs,0,Planar UVs,1)]_ElementUVsMode("Element UVs", Float) = 0
		[StyledVector(9)]_MainUVs("Element UVs", Vector) = (1,1,0,0)
		[StyledRemapSlider(_MainTexColorMinValue, _MainTexColorMaxValue, 0, 1)]_MainTexColorRemap("Element Value", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexColorMinValue("Element Color Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexColorMaxValue("Element Color Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_MainTexAlphaMinValue, _MainTexAlphaMaxValue, 0, 1)]_MainTexAlphaRemap("Element Alpha", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexAlphaMinValue("Element Alpha Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexAlphaMaxValue("Element Alpha Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_MainTexFallofMinValue, _MainTexFallofMaxValue, 0, 1)]_MainTexFallofRemap("Element Falloff", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexFallofMinValue("Element Fallof Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexFallofMaxValue("Element Fallof Max", Range( 0 , 1)) = 0
		[Space(10)][StyledRemapSlider(_SeasonMinValue, _SeasonMaxValue, 0, 1)]_SeasonRemap("Seasons Curve", Vector) = (0,0,0,0)
		[Space(10)][StyledTextureSingleLine]_NoiseTex("Noise Texture", 2D) = "gray" {}
		[Space(10)]_NoiseIntensityValue("Noise Intensity", Range( 0 , 1)) = 0
		[StyledRemapSlider(_NoiseMinValue, _NoiseMaxValue, 0, 1)]_NoiseRemap("Noise Remap", Vector) = (0,0,0,0)
		[HideInInspector]_NoiseMinValue("Noise Min", Range( 0 , 1)) = 0
		[HideInInspector]_NoiseMaxValue("Noise Max", Range( 0 , 1)) = 1
		_NoiseScaleValue("Noise Scale", Float) = 1
		_NoisePhaseValue("Noise Phase", Float) = 0
		_NoiseSpeedValue("Noise Speed", Float) = 1
		[Space(10)]_MotionPower("Wind Power", Range( 0 , 1)) = 0
		[Space(10)]_SpeedTresholdValue("Speed Treshold", Float) = 10
		[Space(10)][StyledToggle]_ElementInvertMode("Use Inverted Element Direction", Float) = 0
		[StyledCategory(Fading Settings)]_CategoryFade("[ Category Fade ]", Float) = 0
		[HDR][StyledToggle]_ElementRaycastMode("Enable Raycast Fading", Float) = 0
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Edge Fading", Float) = 0
		[Space(10)][StyledLayers()]_RaycastLayerMask("Raycast Layer", Float) = 1
		_RaycastDistanceEndValue("Raycast Distance", Float) = 2
		[HideInInspector]_ElementLayerValue("Legacy Layer Value", Float) = -1
		[HideInInspector]_InvertX("Legacy Invert Mode", Float) = 0
		[HideInInspector]_ElementFadeSupport("Legacy Edge Fading", Float) = 0
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_element_direction_mode("_element_direction_mode", Vector) = (0,0,0,0)
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1100
		[HideInInspector]_IsMotionElement("_IsMotionElement", Float) = 1
		[HideInInspector]_render_colormask("_render_colormask", Float) = 15

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType"="Plane" "DisableBatching"="True" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha, One One
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define TVE_IS_MOTION_ELEMENT


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _render_colormask;
			uniform half _IsMotionElement;
			uniform half _Banner;
			uniform half _Message;
			uniform half _IsElementShader;
			uniform half _ElementLayerMask;
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half4 _NoiseRemap;
			uniform half4 _MainTexFallofRemap;
			uniform half4 _MainTexAlphaRemap;
			uniform half _CategoryRender;
			uniform half _CategoryElement;
			uniform half _CategoryFade;
			uniform half _SpaceTexture;
			uniform float _ElementFadeSupport;
			uniform half _ElementLayerValue;
			uniform float _InvertX;
			uniform half _IsVersion;
			uniform half4 _MainTexColorRemap;
			uniform half4 _SeasonRemap;
			uniform half _ElementRaycastMode;
			uniform half _RaycastLayerMask;
			uniform half _RaycastDistanceEndValue;
			uniform float _SpeedTresholdValue;
			uniform float _ElementDirectionMode;
			uniform half4 _element_direction_mode;
			uniform sampler2D _MainTex;
			uniform float _ElementUVsMode;
			uniform half4 _MainUVs;
			uniform half _MainTexColorMinValue;
			uniform half _MainTexColorMaxValue;
			uniform float _ElementInvertMode;
			uniform sampler2D _NoiseTex;
			uniform float3 TVE_WorldOrigin;
			uniform half _NoiseScaleValue;
			uniform half4 TVE_TimeParams;
			uniform half _NoiseSpeedValue;
			uniform half _NoisePhaseValue;
			uniform half _NoiseMinValue;
			uniform half _NoiseMaxValue;
			uniform half _NoiseIntensityValue;
			uniform half _MotionPower;
			uniform float _ElementIntensity;
			uniform half _MainTexAlphaMinValue;
			uniform half _MainTexAlphaMaxValue;
			uniform half _MainTexFallofMinValue;
			uniform half _MainTexFallofMaxValue;
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform float _ElementVolumeFadeMode;
			uniform half _ElementMotionMode;
			UNITY_INSTANCING_BUFFER_START(HiddenBOXOPHOBICTheVegetationEngineElementsMotionTurbulence)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr HiddenBOXOPHOBICTheVegetationEngineElementsMotionTurbulence
			UNITY_INSTANCING_BUFFER_END(HiddenBOXOPHOBICTheVegetationEngineElementsMotionTurbulence)
			half4 IS_ELEMENT( half4 Colors, half4 Extras, half4 Motion, half4 Vertex )
			{
				#if defined (TVE_IS_COLORS_ELEMENT)
				return Colors;
				#elif defined (TVE_IS_EXTRAS_ELEMENT)
				return Extras;
				#elif defined (TVE_IS_MOTION_ELEMENT)
				return Motion;
				#elif defined (TVE_IS_VERTEX_ELEMENT)
				return Vertex;
				#else
				return Colors;
				#endif
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				float2 appendResult1900_g20948 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 lerpResult1899_g20948 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g20948 , _ElementUVsMode);
				float2 vertexToFrag11_g22152 = ( ( lerpResult1899_g20948 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.xy = vertexToFrag11_g22152;
				
				o.ase_color = v.color;
				o.ase_texcoord2 = v.ase_texcoord;
				o.ase_texcoord3 = v.ase_texcoord1;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				half2 Direction_Transform1406_g20948 = (( mul( unity_ObjectToWorld, float4( float3(0,0,1) , 0.0 ) ).xyz / ase_objectScale )).xz;
				float2 vertexToFrag11_g22152 = i.ase_texcoord1.xy;
				half4 MainTex_RGBA587_g20948 = tex2D( _MainTex, vertexToFrag11_g22152 );
				float3 temp_cast_2 = (0.0001).xxx;
				float3 temp_cast_3 = (0.9999).xxx;
				float3 clampResult17_g22182 = clamp( (MainTex_RGBA587_g20948).rgb , temp_cast_2 , temp_cast_3 );
				float temp_output_7_0_g22183 = _MainTexColorMinValue;
				float3 temp_cast_4 = (temp_output_7_0_g22183).xxx;
				float temp_output_10_0_g22183 = ( _MainTexColorMaxValue - temp_output_7_0_g22183 );
				float3 temp_output_1765_0_g20948 = saturate( ( ( clampResult17_g22182 - temp_cast_4 ) / temp_output_10_0_g22183 ) );
				half Element_Remap_R73_g20948 = (temp_output_1765_0_g20948).x;
				half Element_Remap_G265_g20948 = (temp_output_1765_0_g20948).y;
				float3 appendResult274_g20948 = (float3((Element_Remap_R73_g20948*2.0 + -1.0) , 0.0 , (Element_Remap_G265_g20948*2.0 + -1.0)));
				float3 break281_g20948 = ( mul( unity_ObjectToWorld, float4( appendResult274_g20948 , 0.0 ) ).xyz / ase_objectScale );
				float2 appendResult1403_g20948 = (float2(break281_g20948.x , break281_g20948.z));
				half2 Direction_Texture284_g20948 = appendResult1403_g20948;
				float2 appendResult1404_g20948 = (float2(i.ase_color.r , i.ase_color.g));
				half2 Direction_VertexColor1150_g20948 = (appendResult1404_g20948*2.0 + -1.0);
				float2 appendResult1382_g20948 = (float2(i.ase_texcoord2.z , i.ase_texcoord3.x));
				half2 Direction_Velocity1394_g20948 = ( appendResult1382_g20948 / i.ase_texcoord3.y );
				float2 temp_output_1452_0_g20948 = ( ( _element_direction_mode.x * Direction_Transform1406_g20948 ) + ( _element_direction_mode.y * Direction_Texture284_g20948 ) + ( _element_direction_mode.z * Direction_VertexColor1150_g20948 ) + ( _element_direction_mode.w * Direction_Velocity1394_g20948 ) );
				half Element_InvertMode489_g20948 = _ElementInvertMode;
				float2 lerpResult1468_g20948 = lerp( temp_output_1452_0_g20948 , -temp_output_1452_0_g20948 , Element_InvertMode489_g20948);
				half2 Direction_Advanced1454_g20948 = lerpResult1468_g20948;
				half Noise_Scale892_g20948 = _NoiseScaleValue;
				half2 Noise_Coords1409_g20948 = ( -( (( WorldPosition - TVE_WorldOrigin )).xz * 0.1 ) * Noise_Scale892_g20948 );
				float2 temp_output_3_0_g21053 = Noise_Coords1409_g20948;
				float2 temp_output_21_0_g21053 = Direction_Advanced1454_g20948;
				float lerpResult128_g21052 = lerp( _Time.y , ( ( _Time.y * TVE_TimeParams.x ) + TVE_TimeParams.y ) , TVE_TimeParams.w);
				half Noise_Speed898_g20948 = _NoiseSpeedValue;
				half Noise_Phase1970_g20948 = _NoisePhaseValue;
				float temp_output_15_0_g21053 = (lerpResult128_g21052*Noise_Speed898_g20948 + Noise_Phase1970_g20948);
				float temp_output_23_0_g21053 = frac( temp_output_15_0_g21053 );
				float4 lerpResult39_g21053 = lerp( tex2D( _NoiseTex, ( temp_output_3_0_g21053 + ( temp_output_21_0_g21053 * temp_output_23_0_g21053 ) ) ) , tex2D( _NoiseTex, ( temp_output_3_0_g21053 + ( temp_output_21_0_g21053 * frac( ( temp_output_15_0_g21053 + 0.5 ) ) ) ) ) , ( abs( ( temp_output_23_0_g21053 - 0.5 ) ) / 0.5 ));
				half Noise_Min893_g20948 = _NoiseMinValue;
				float temp_output_7_0_g21070 = Noise_Min893_g20948;
				float4 temp_cast_7 = (temp_output_7_0_g21070).xxxx;
				half Noise_Max894_g20948 = _NoiseMaxValue;
				float temp_output_10_0_g21070 = ( Noise_Max894_g20948 - temp_output_7_0_g21070 );
				half2 Noise_Advanced1427_g20948 = (saturate( ( ( lerpResult39_g21053 - temp_cast_7 ) / temp_output_10_0_g21070 ) )).rg;
				half Noise_Intensity965_g20948 = _NoiseIntensityValue;
				float2 lerpResult1435_g20948 = lerp( (Direction_Advanced1454_g20948*0.5 + 0.5) , Noise_Advanced1427_g20948 , Noise_Intensity965_g20948);
				half Motion_Power1000_g20948 = _MotionPower;
				float3 appendResult1436_g20948 = (float3(lerpResult1435_g20948 , Motion_Power1000_g20948));
				half3 Final_MotionAdvanced_RGB1438_g20948 = appendResult1436_g20948;
				float clampResult17_g22148 = clamp( (MainTex_RGBA587_g20948).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g22149 = _MainTexAlphaMinValue;
				float temp_output_10_0_g22149 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22149 );
				half Element_Remap_A74_g20948 = saturate( ( ( clampResult17_g22148 - temp_output_7_0_g22149 ) / ( temp_output_10_0_g22149 + 0.0001 ) ) );
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_A1737_g20948 = _ElementParams_Instance.w;
				float clampResult17_g21978 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				float temp_output_7_0_g21979 = _MainTexFallofMinValue;
				float temp_output_10_0_g21979 = ( _MainTexFallofMaxValue - temp_output_7_0_g21979 );
				half Element_Fallof1883_g20948 = saturate( ( ( clampResult17_g21978 - temp_output_7_0_g21979 ) / ( temp_output_10_0_g21979 + 0.0001 ) ) );
				half4 Colors37_g21074 = TVE_ColorsCoords;
				half4 Extras37_g21074 = TVE_ExtrasCoords;
				half4 Motion37_g21074 = TVE_MotionCoords;
				half4 Vertex37_g21074 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g21074 = IS_ELEMENT( Colors37_g21074 , Extras37_g21074 , Motion37_g21074 , Vertex37_g21074 );
				float4 temp_output_35_0_g21078 = localIS_ELEMENT37_g21074;
				float temp_output_7_0_g21079 = TVE_ElementsFadeValue;
				float2 temp_cast_8 = (temp_output_7_0_g21079).xx;
				float temp_output_10_0_g21079 = ( 1.0 - temp_output_7_0_g21079 );
				float2 temp_output_16_0_g21072 = saturate( ( ( abs( (( (temp_output_35_0_g21078).zw + ( (temp_output_35_0_g21078).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_8 ) / temp_output_10_0_g21079 ) );
				float2 temp_output_12_0_g21072 = ( temp_output_16_0_g21072 * temp_output_16_0_g21072 );
				float lerpResult842_g20948 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g21072).x + (temp_output_12_0_g21072).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g20948 = ( _ElementIntensity * Element_Remap_A74_g20948 * Element_Params_A1737_g20948 * i.ase_color.a * Element_Fallof1883_g20948 * lerpResult842_g20948 );
				half Final_MotionAdvanced_A1439_g20948 = Element_Alpha56_g20948;
				float4 appendResult1463_g20948 = (float4(Final_MotionAdvanced_RGB1438_g20948 , Final_MotionAdvanced_A1439_g20948));
				half Element_EffectMotion946_g20948 = _ElementMotionMode;
				
				
				finalColor = ( appendResult1463_g20948 + ( Element_EffectMotion946_g20948 * 0.0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "TVEShaderElementGUI"
	
	Fallback Off
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.RangedFloatNode;197;-640,-1152;Half;False;Property;_render_colormask;_render_colormask;79;1;[HideInInspector];Create;True;0;0;0;True;0;False;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;177;-640,-1280;Inherit;False;Define Element Motion;77;;20879;6eebc31017d99e84e811285e6a5d199d;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-352,-1280;Half;False;Property;_Banner;Banner;0;0;Create;True;0;0;0;True;1;StyledBanner(Motion Turbulence Element);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-224,-1280;Half;False;Property;_Message;Message;1;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Motion Turbulence elements to add noise to the motion direction. Element Texture A is used as alpha mask. Particle Alpha is used as Element Intensity multiplier. The noise is animated in the element forward direction and it is updated with particles or in play mode only., 0,0);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-320,-1024;Float;False;True;-1;2;TVEShaderElementGUI;0;5;Hidden/BOXOPHOBIC/The Vegetation Engine/Elements/Motion Turbulence;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;4;1;False;;1;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;True;True;True;True;True;True;0;False;_render_colormask;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;0;False;;True;False;0;False;;0;False;;True;4;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;PreviewType=Plane;DisableBatching=True=DisableBatching;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.FunctionNode;281;-640,-1024;Inherit;False;Base Element;2;;20948;0e972c73cae2ee54ea51acc9738801d0;12,477,2,1778,2,478,0,1824,0,145,3,1814,3,1784,2,481,2,1935,1,1933,1,1904,1,1907,1;0;2;FLOAT4;0;FLOAT4;1779
WireConnection;0;0;281;0
ASEEND*/
//CHKSM=F3C51D7610E07D86A3122206B00DEC9C6AA4144E