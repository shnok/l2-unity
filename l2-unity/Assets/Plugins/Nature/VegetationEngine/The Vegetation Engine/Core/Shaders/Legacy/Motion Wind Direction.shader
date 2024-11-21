// Upgrade NOTE: upgraded instancing buffer 'HiddenBOXOPHOBICTheVegetationEngineElementsMotionWindDirection' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/BOXOPHOBIC/The Vegetation Engine/Elements/Motion Wind Direction"
{
	Properties
	{
		[StyledBanner(Motion Direction Element)]_Banner("Banner", Float) = 0
		[StyledMessage(Info, Use the Motion Direction elements to change the wind direction based on the element forward direction. Element Texture A is used as alpha mask. Particle Color A is used as Element Intensity multiplier. , 0,0)]_Message("Message", Float) = 0
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMask(TVELayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[Enum(Constant,0,Seasons,1)]_ElementMode("Render Mode", Float) = 0
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
		[Space(10)]_MainValue("Element Value", Range( 0 , 1)) = 1
		[Space(10)]_AdditionalValue1("Winter Value", Range( 0 , 1)) = 1
		_AdditionalValue2("Spring Value", Range( 0 , 1)) = 1
		_AdditionalValue3("Summer Value", Range( 0 , 1)) = 1
		_AdditionalValue4("Autumn Value", Range( 0 , 1)) = 1
		[Space(10)][StyledRemapSlider(_SeasonMinValue, _SeasonMaxValue, 0, 1)]_SeasonRemap("Seasons Curve", Vector) = (0,0,0,0)
		[HideInInspector]_SeasonMinValue("Seasons Min", Range( 0 , 1)) = 0
		[HideInInspector]_SeasonMaxValue("Seasons Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_NoiseMinValue, _NoiseMaxValue, 0, 1)]_NoiseRemap("Noise Remap", Vector) = (0,0,0,0)
		[Space(10)]_SpeedTresholdValue("Speed Treshold", Float) = 10
		[StyledCategory(Fading Settings)]_CategoryFade("[ Category Fade ]", Float) = 0
		[HDR][StyledToggle]_ElementRaycastMode("Enable Raycast Fading", Float) = 0
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Edge Fading", Float) = 0
		[Space(10)][StyledLayers()]_RaycastLayerMask("Raycast Layer", Float) = 1
		_RaycastDistanceEndValue("Raycast Distance", Float) = 2
		[HideInInspector]_ElementLayerValue("Legacy Layer Value", Float) = -1
		[HideInInspector]_InvertX("Legacy Invert Mode", Float) = 0
		[HideInInspector]_ElementFadeSupport("Legacy Edge Fading", Float) = 0
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1100
		[HideInInspector]_IsMotionElement("_IsMotionElement", Float) = 1

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType"="Plane" "DisableBatching"="True" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RG
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
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define TVE_IS_MOTION_ELEMENT


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _Banner;
			uniform half _IsMotionElement;
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
			uniform half _MainValue;
			uniform half4 TVE_SeasonOptions;
			uniform half _AdditionalValue1;
			uniform half _AdditionalValue2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half _AdditionalValue3;
			uniform half _AdditionalValue4;
			uniform float _ElementMode;
			uniform sampler2D _MainTex;
			uniform float _ElementUVsMode;
			uniform half4 _MainUVs;
			uniform half _MainTexColorMinValue;
			uniform half _MainTexColorMaxValue;
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
			UNITY_INSTANCING_BUFFER_START(HiddenBOXOPHOBICTheVegetationEngineElementsMotionWindDirection)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr HiddenBOXOPHOBICTheVegetationEngineElementsMotionWindDirection
			UNITY_INSTANCING_BUFFER_END(HiddenBOXOPHOBICTheVegetationEngineElementsMotionWindDirection)
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

				half TVE_SeasonOptions_X50_g53336 = TVE_SeasonOptions.x;
				half Value_Winter158_g53336 = _AdditionalValue1;
				half Value_Spring159_g53336 = _AdditionalValue2;
				float temp_output_7_0_g53546 = _SeasonMinValue;
				float temp_output_10_0_g53546 = ( _SeasonMaxValue - temp_output_7_0_g53546 );
				half TVE_SeasonLerp54_g53336 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g53546 ) / temp_output_10_0_g53546 ) ) );
				float lerpResult168_g53336 = lerp( Value_Winter158_g53336 , Value_Spring159_g53336 , TVE_SeasonLerp54_g53336);
				half TVE_SeasonOptions_Y51_g53336 = TVE_SeasonOptions.y;
				half Value_Summer160_g53336 = _AdditionalValue3;
				float lerpResult167_g53336 = lerp( Value_Spring159_g53336 , Value_Summer160_g53336 , TVE_SeasonLerp54_g53336);
				half TVE_SeasonOptions_Z52_g53336 = TVE_SeasonOptions.z;
				half Value_Autumn161_g53336 = _AdditionalValue4;
				float lerpResult166_g53336 = lerp( Value_Summer160_g53336 , Value_Autumn161_g53336 , TVE_SeasonLerp54_g53336);
				half TVE_SeasonOptions_W53_g53336 = TVE_SeasonOptions.w;
				float lerpResult165_g53336 = lerp( Value_Autumn161_g53336 , Value_Winter158_g53336 , TVE_SeasonLerp54_g53336);
				float vertexToFrag11_g53338 = ( ( ( TVE_SeasonOptions_X50_g53336 * lerpResult168_g53336 ) + ( TVE_SeasonOptions_Y51_g53336 * lerpResult167_g53336 ) ) + ( ( TVE_SeasonOptions_Z52_g53336 * lerpResult166_g53336 ) + ( TVE_SeasonOptions_W53_g53336 * lerpResult165_g53336 ) ) );
				o.ase_texcoord1.x = vertexToFrag11_g53338;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				float2 appendResult1900_g53336 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 lerpResult1899_g53336 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g53336 , _ElementUVsMode);
				float2 vertexToFrag11_g53360 = ( ( lerpResult1899_g53336 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.yz = vertexToFrag11_g53360;
				
				o.ase_color = v.color;
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.zw = 0;
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
				half Value_Main157_g53336 = _MainValue;
				float vertexToFrag11_g53338 = i.ase_texcoord1.x;
				half Value_Seasons1721_g53336 = vertexToFrag11_g53338;
				half Element_Mode55_g53336 = _ElementMode;
				float lerpResult181_g53336 = lerp( Value_Main157_g53336 , Value_Seasons1721_g53336 , Element_Mode55_g53336);
				float2 vertexToFrag11_g53360 = i.ase_texcoord1.yz;
				half4 MainTex_RGBA587_g53336 = tex2D( _MainTex, vertexToFrag11_g53360 );
				float3 temp_cast_0 = (0.0001).xxx;
				float3 temp_cast_1 = (0.9999).xxx;
				float3 clampResult17_g53362 = clamp( (MainTex_RGBA587_g53336).rgb , temp_cast_0 , temp_cast_1 );
				float temp_output_7_0_g53363 = _MainTexColorMinValue;
				float3 temp_cast_2 = (temp_output_7_0_g53363).xxx;
				float temp_output_10_0_g53363 = ( _MainTexColorMaxValue - temp_output_7_0_g53363 );
				float3 temp_output_1765_0_g53336 = saturate( ( ( clampResult17_g53362 - temp_cast_2 ) / temp_output_10_0_g53363 ) );
				half Element_Remap_R73_g53336 = (temp_output_1765_0_g53336).x;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_R1738_g53336 = _ElementParams_Instance.x;
				half Element_Value1744_g53336 = ( Element_Remap_R73_g53336 * Element_Params_R1738_g53336 * i.ase_color.r );
				half Final_Extras_RGB213_g53336 = ( lerpResult181_g53336 * Element_Value1744_g53336 );
				float clampResult17_g53358 = clamp( (MainTex_RGBA587_g53336).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g53359 = _MainTexAlphaMinValue;
				float temp_output_10_0_g53359 = ( _MainTexAlphaMaxValue - temp_output_7_0_g53359 );
				half Element_Remap_A74_g53336 = saturate( ( ( clampResult17_g53358 - temp_output_7_0_g53359 ) / ( temp_output_10_0_g53359 + 0.0001 ) ) );
				half Element_Params_A1737_g53336 = _ElementParams_Instance.w;
				float clampResult17_g53356 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				float temp_output_7_0_g53357 = _MainTexFallofMinValue;
				float temp_output_10_0_g53357 = ( _MainTexFallofMaxValue - temp_output_7_0_g53357 );
				half Element_Fallof1883_g53336 = saturate( ( ( clampResult17_g53356 - temp_output_7_0_g53357 ) / ( temp_output_10_0_g53357 + 0.0001 ) ) );
				half4 Colors37_g53346 = TVE_ColorsCoords;
				half4 Extras37_g53346 = TVE_ExtrasCoords;
				half4 Motion37_g53346 = TVE_MotionCoords;
				half4 Vertex37_g53346 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g53346 = IS_ELEMENT( Colors37_g53346 , Extras37_g53346 , Motion37_g53346 , Vertex37_g53346 );
				float4 temp_output_35_0_g53350 = localIS_ELEMENT37_g53346;
				float temp_output_7_0_g53351 = TVE_ElementsFadeValue;
				float2 temp_cast_3 = (temp_output_7_0_g53351).xx;
				float temp_output_10_0_g53351 = ( 1.0 - temp_output_7_0_g53351 );
				float2 temp_output_16_0_g53344 = saturate( ( ( abs( (( (temp_output_35_0_g53350).zw + ( (temp_output_35_0_g53350).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_3 ) / temp_output_10_0_g53351 ) );
				float2 temp_output_12_0_g53344 = ( temp_output_16_0_g53344 * temp_output_16_0_g53344 );
				float lerpResult842_g53336 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g53344).x + (temp_output_12_0_g53344).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g53336 = ( _ElementIntensity * Element_Remap_A74_g53336 * Element_Params_A1737_g53336 * i.ase_color.a * Element_Fallof1883_g53336 * lerpResult842_g53336 );
				half Final_Extras_A241_g53336 = Element_Alpha56_g53336;
				float4 appendResult1732_g53336 = (float4(0.0 , 0.0 , Final_Extras_RGB213_g53336 , Final_Extras_A241_g53336));
				
				
				finalColor = appendResult1732_g53336;
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
Node;AmplifyShaderEditor.RangedFloatNode;95;-384,-1280;Half;False;Property;_Banner;Banner;0;0;Create;True;0;0;0;True;1;StyledBanner(Motion Direction Element);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;117;-640,-1280;Inherit;False;Define Element Motion;77;;19292;6eebc31017d99e84e811285e6a5d199d;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-256,-1280;Half;False;Property;_Message;Message;1;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Motion Direction elements to change the wind direction based on the element forward direction. Element Texture A is used as alpha mask. Particle Color A is used as Element Intensity multiplier. , 0,0);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-304,-1024;Float;False;True;-1;2;TVEShaderElementGUI;0;5;Hidden/BOXOPHOBIC/The Vegetation Engine/Elements/Motion Wind Direction;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;True;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;True;False;255;False;;255;False;;255;False;;7;False;;1;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;0;False;;True;False;0;False;;0;False;;True;4;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;PreviewType=Plane;DisableBatching=True=DisableBatching;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.FunctionNode;199;-640,-1024;Inherit;False;Base Element;2;;53336;0e972c73cae2ee54ea51acc9738801d0;12,477,2,1778,2,478,0,1824,0,145,3,1814,3,1784,0,481,0,1935,1,1933,1,1904,1,1907,1;0;2;FLOAT4;0;FLOAT4;1779
WireConnection;0;0;199;0
ASEEND*/
//CHKSM=43229FAA6BEE57564F5DFA2DCCB4842857223CFE