// Upgrade NOTE: upgraded instancing buffer 'BOXOPHOBICTheVegetationEngineElementsColorsTint' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Colors Tint"
{
	Properties
	{
		[StyledMessage(Info, Use the Colors elements to add color tinting to the vegetation assets. Element Texture RGB and Particle Color RGB are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0)]_ElementMessage("Element Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsColorsElement("_IsColorsElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEColorsLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
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
		[HDR][Gamma][Space(10)]_MainColor("Element Value", Color) = (0.5019608,0.5019608,0.5019608,1)
		[HDR][Gamma][Space(10)]_AdditionalColor1("Winter Value", Color) = (0.5019608,0.5019608,0.5019608,1)
		[HDR][Gamma]_AdditionalColor2("Spring Value", Color) = (0.5019608,0.5019608,0.5019608,1)
		[HDR][Gamma]_AdditionalColor3("Summer Value", Color) = (0.5019608,0.5019608,0.5019608,1)
		[HDR][Gamma]_AdditionalColor4("Autumn Value", Color) = (0.5019608,0.5019608,0.5019608,1)
		[Space(10)][StyledRemapSlider(_SeasonMinValue, _SeasonMaxValue, 0, 1)]_SeasonRemap("Seasons Curve", Vector) = (0,0,0,0)
		[HideInInspector]_SeasonMinValue("Seasons Min", Range( 0 , 1)) = 0
		[HideInInspector]_SeasonMaxValue("Seasons Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_NoiseMinValue, _NoiseMaxValue, 0, 1)]_NoiseRemap("Noise Remap", Vector) = (0,0,0,0)
		[Space(10)]_SpeedTresholdValue("Speed Treshold", Float) = 10
		[StyledCategory(Fading Settings)]_CategoryFade("[ Category Fade ]", Float) = 0
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Fade", Float) = 0
		[HDR][StyledToggle]_ElementRaycastMode("Enable Raycast Fade", Float) = 0
		[Space(10)][StyledLayers()]_RaycastLayerMask("Raycast Layer", Float) = 1
		_RaycastDistanceEndValue("Raycast Distance", Float) = 2
		[HideInInspector]_ElementLayerValue("Legacy Layer Value", Float) = -1
		[HideInInspector]_InvertX("Legacy Invert Mode", Float) = 0
		[HideInInspector]_ElementFadeSupport("Legacy Edge Fading", Float) = 0
		[HideInInspector]_render_colormask("_render_colormask", Float) = 15

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True" }
	LOD 100

		
		Pass
		{
			
			Name "VolumePass"
			
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha, One One
			AlphaToMask Off
			Cull Back
			ColorMask RGBA
			ZWrite Off
			ZTest LEqual
			
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
			#define TVE_IS_COLORS_ELEMENT


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
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _render_colormask;
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsColorsElement;
			uniform half _IsElementShader;
			uniform half _IsVersion;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half4 _NoiseRemap;
			uniform half4 _MainTexFallofRemap;
			uniform half4 _MainTexAlphaRemap;
			uniform half4 _MainTexColorRemap;
			uniform half4 _SeasonRemap;
			uniform float _SpeedTresholdValue;
			uniform float _ElementDirectionMode;
			uniform float _ElementFadeSupport;
			uniform half _ElementLayerValue;
			uniform float _InvertX;
			uniform half _CategoryElement;
			uniform half _CategoryFade;
			uniform half _SpaceTexture;
			uniform half _RaycastLayerMask;
			uniform half _RaycastDistanceEndValue;
			uniform half _ElementRaycastMode;
			uniform half4 _MainColor;
			uniform half4 TVE_SeasonOptions;
			uniform half4 _AdditionalColor1;
			uniform half4 _AdditionalColor2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half4 _AdditionalColor3;
			uniform half4 _AdditionalColor4;
			uniform half _ElementMode;
			uniform sampler2D _MainTex;
			uniform half _ElementUVsMode;
			uniform half4 _MainUVs;
			uniform half _MainTexColorMinValue;
			uniform half _MainTexColorMaxValue;
			uniform half _ElementIntensity;
			uniform half _MainTexAlphaMinValue;
			uniform half _MainTexAlphaMaxValue;
			uniform half _MainTexFallofMinValue;
			uniform half _MainTexFallofMaxValue;
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform half _ElementVolumeFadeMode;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsColorsTint)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsColorsTint
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsColorsTint)
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

				half TVE_SeasonOptions_X50_g23654 = TVE_SeasonOptions.x;
				half4 Color_Winter_RGBA58_g23654 = _AdditionalColor1;
				half4 Color_Spring_RGBA59_g23654 = _AdditionalColor2;
				half temp_output_7_0_g23855 = _SeasonMinValue;
				half temp_output_10_0_g23855 = ( _SeasonMaxValue - temp_output_7_0_g23855 );
				half TVE_SeasonLerp54_g23654 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g23855 ) / temp_output_10_0_g23855 ) ) );
				half4 lerpResult13_g23654 = lerp( Color_Winter_RGBA58_g23654 , Color_Spring_RGBA59_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_Y51_g23654 = TVE_SeasonOptions.y;
				half4 Color_Summer_RGBA60_g23654 = _AdditionalColor3;
				half4 lerpResult14_g23654 = lerp( Color_Spring_RGBA59_g23654 , Color_Summer_RGBA60_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_Z52_g23654 = TVE_SeasonOptions.z;
				half4 Color_Autumn_RGBA61_g23654 = _AdditionalColor4;
				half4 lerpResult15_g23654 = lerp( Color_Summer_RGBA60_g23654 , Color_Autumn_RGBA61_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_W53_g23654 = TVE_SeasonOptions.w;
				half4 lerpResult12_g23654 = lerp( Color_Autumn_RGBA61_g23654 , Color_Winter_RGBA58_g23654 , TVE_SeasonLerp54_g23654);
				half4 vertexToFrag11_g23655 = ( ( TVE_SeasonOptions_X50_g23654 * lerpResult13_g23654 ) + ( TVE_SeasonOptions_Y51_g23654 * lerpResult14_g23654 ) + ( TVE_SeasonOptions_Z52_g23654 * lerpResult15_g23654 ) + ( TVE_SeasonOptions_W53_g23654 * lerpResult12_g23654 ) );
				o.ase_texcoord1 = vertexToFrag11_g23655;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				half2 appendResult1900_g23654 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g23654 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g23654 , _ElementUVsMode);
				half2 vertexToFrag11_g23670 = ( ( lerpResult1899_g23654 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord2.xy = vertexToFrag11_g23670;
				
				o.ase_color = v.color;
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
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
				half4 Color_Main_RGBA49_g23654 = _MainColor;
				half4 vertexToFrag11_g23655 = i.ase_texcoord1;
				half4 Color_Seasons1715_g23654 = vertexToFrag11_g23655;
				half Element_Mode55_g23654 = _ElementMode;
				half4 lerpResult30_g23654 = lerp( Color_Main_RGBA49_g23654 , Color_Seasons1715_g23654 , Element_Mode55_g23654);
				half2 vertexToFrag11_g23670 = i.ase_texcoord2.xy;
				half4 MainTex_RGBA587_g23654 = tex2D( _MainTex, vertexToFrag11_g23670 );
				half3 temp_cast_0 = (0.0001).xxx;
				half3 temp_cast_1 = (0.9999).xxx;
				half3 clampResult17_g23671 = clamp( (MainTex_RGBA587_g23654).rgb , temp_cast_0 , temp_cast_1 );
				half temp_output_7_0_g23672 = _MainTexColorMinValue;
				half3 temp_cast_2 = (temp_output_7_0_g23672).xxx;
				half temp_output_10_0_g23672 = ( _MainTexColorMaxValue - temp_output_7_0_g23672 );
				half3 temp_output_1765_0_g23654 = saturate( ( ( clampResult17_g23671 - temp_cast_2 ) / temp_output_10_0_g23672 ) );
				half3 Element_Remap_RGB1555_g23654 = temp_output_1765_0_g23654;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half3 Element_Params_RGB1735_g23654 = (_ElementParams_Instance).xyz;
				half3 Element_Color1756_g23654 = ( Element_Remap_RGB1555_g23654 * Element_Params_RGB1735_g23654 * (i.ase_color).rgb );
				half3 Final_Colors_RGB142_g23654 = ( (lerpResult30_g23654).rgb * Element_Color1756_g23654 );
				half clampResult17_g23668 = clamp( (MainTex_RGBA587_g23654).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g23669 = _MainTexAlphaMinValue;
				half temp_output_10_0_g23669 = ( _MainTexAlphaMaxValue - temp_output_7_0_g23669 );
				half Element_Remap_A74_g23654 = saturate( ( ( clampResult17_g23668 - temp_output_7_0_g23669 ) / ( temp_output_10_0_g23669 + 0.0001 ) ) );
				half Element_Params_A1737_g23654 = _ElementParams_Instance.w;
				half clampResult17_g23666 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.zw*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g23667 = _MainTexFallofMinValue;
				half temp_output_10_0_g23667 = ( _MainTexFallofMaxValue - temp_output_7_0_g23667 );
				half Element_Falloff1883_g23654 = saturate( ( ( clampResult17_g23666 - temp_output_7_0_g23667 ) / ( temp_output_10_0_g23667 + 0.0001 ) ) );
				half4 Colors37_g23858 = TVE_ColorsCoords;
				half4 Extras37_g23858 = TVE_ExtrasCoords;
				half4 Motion37_g23858 = TVE_MotionCoords;
				half4 Vertex37_g23858 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g23858 = IS_ELEMENT( Colors37_g23858 , Extras37_g23858 , Motion37_g23858 , Vertex37_g23858 );
				half4 temp_output_35_0_g23860 = localIS_ELEMENT37_g23858;
				half temp_output_7_0_g23861 = TVE_ElementsFadeValue;
				half2 temp_cast_3 = (temp_output_7_0_g23861).xx;
				half temp_output_10_0_g23861 = ( 1.0 - temp_output_7_0_g23861 );
				half2 temp_output_16_0_g23857 = saturate( ( ( abs( (( (temp_output_35_0_g23860).zw + ( (temp_output_35_0_g23860).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_3 ) / temp_output_10_0_g23861 ) );
				half2 temp_output_12_0_g23857 = ( temp_output_16_0_g23857 * temp_output_16_0_g23857 );
				half lerpResult18_g23857 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g23857).x + (temp_output_12_0_g23857).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g23654 = ( _ElementIntensity * Element_Remap_A74_g23654 * Element_Params_A1737_g23654 * i.ase_color.a * Element_Falloff1883_g23654 * lerpResult18_g23857 );
				half Final_Colors_A144_g23654 = ( (lerpResult30_g23654).a * Element_Alpha56_g23654 );
				half4 appendResult470_g23654 = (half4(Final_Colors_RGB142_g23654 , Final_Colors_A144_g23654));
				
				
				finalColor = appendResult470_g23654;
				return finalColor;
			}
			ENDCG
		}
		
		
		Pass
		{
			
			Name "VisualPass"
			
			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Back
			ColorMask RGBA
			ZWrite Off
			ZTest LEqual
			
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_COLOR
			#define TVE_IS_COLORS_ELEMENT


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
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _render_colormask;
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsColorsElement;
			uniform half _IsElementShader;
			uniform half _IsVersion;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half4 _NoiseRemap;
			uniform half4 _MainTexFallofRemap;
			uniform half4 _MainTexAlphaRemap;
			uniform half4 _MainTexColorRemap;
			uniform half4 _SeasonRemap;
			uniform half _SpeedTresholdValue;
			uniform half _ElementDirectionMode;
			uniform half _ElementFadeSupport;
			uniform half _ElementLayerValue;
			uniform half _InvertX;
			uniform half _CategoryElement;
			uniform half _CategoryFade;
			uniform half _SpaceTexture;
			uniform half _RaycastLayerMask;
			uniform half _RaycastDistanceEndValue;
			uniform half _ElementRaycastMode;
			uniform half4 _MainColor;
			uniform half4 TVE_SeasonOptions;
			uniform half4 _AdditionalColor1;
			uniform half4 _AdditionalColor2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half4 _AdditionalColor3;
			uniform half4 _AdditionalColor4;
			uniform half _ElementMode;
			uniform sampler2D _MainTex;
			uniform half _ElementUVsMode;
			uniform half4 _MainUVs;
			uniform half _MainTexColorMinValue;
			uniform half _MainTexColorMaxValue;
			uniform half4 TVE_ColorsPosition;
			uniform half4 TVE_ExtrasPosition;
			uniform half4 TVE_MotionPosition;
			uniform half4 TVE_VertexPosition;
			uniform half4 TVE_ColorsScale;
			uniform half4 TVE_ExtrasScale;
			uniform half4 TVE_MotionScale;
			uniform half4 TVE_VertexScale;
			uniform half _ElementIntensity;
			uniform half _MainTexAlphaMinValue;
			uniform half _MainTexAlphaMaxValue;
			uniform half _MainTexFallofMinValue;
			uniform half _MainTexFallofMaxValue;
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform half _ElementVolumeFadeMode;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsColorsTint)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsColorsTint
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsColorsTint)
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

				half TVE_SeasonOptions_X50_g23654 = TVE_SeasonOptions.x;
				half4 Color_Winter_RGBA58_g23654 = _AdditionalColor1;
				half4 Color_Spring_RGBA59_g23654 = _AdditionalColor2;
				half temp_output_7_0_g23855 = _SeasonMinValue;
				half temp_output_10_0_g23855 = ( _SeasonMaxValue - temp_output_7_0_g23855 );
				half TVE_SeasonLerp54_g23654 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g23855 ) / temp_output_10_0_g23855 ) ) );
				half4 lerpResult13_g23654 = lerp( Color_Winter_RGBA58_g23654 , Color_Spring_RGBA59_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_Y51_g23654 = TVE_SeasonOptions.y;
				half4 Color_Summer_RGBA60_g23654 = _AdditionalColor3;
				half4 lerpResult14_g23654 = lerp( Color_Spring_RGBA59_g23654 , Color_Summer_RGBA60_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_Z52_g23654 = TVE_SeasonOptions.z;
				half4 Color_Autumn_RGBA61_g23654 = _AdditionalColor4;
				half4 lerpResult15_g23654 = lerp( Color_Summer_RGBA60_g23654 , Color_Autumn_RGBA61_g23654 , TVE_SeasonLerp54_g23654);
				half TVE_SeasonOptions_W53_g23654 = TVE_SeasonOptions.w;
				half4 lerpResult12_g23654 = lerp( Color_Autumn_RGBA61_g23654 , Color_Winter_RGBA58_g23654 , TVE_SeasonLerp54_g23654);
				half4 vertexToFrag11_g23655 = ( ( TVE_SeasonOptions_X50_g23654 * lerpResult13_g23654 ) + ( TVE_SeasonOptions_Y51_g23654 * lerpResult14_g23654 ) + ( TVE_SeasonOptions_Z52_g23654 * lerpResult15_g23654 ) + ( TVE_SeasonOptions_W53_g23654 * lerpResult12_g23654 ) );
				o.ase_texcoord1 = vertexToFrag11_g23655;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				half2 appendResult1900_g23654 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g23654 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g23654 , _ElementUVsMode);
				half2 vertexToFrag11_g23670 = ( ( lerpResult1899_g23654 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord2.xy = vertexToFrag11_g23670;
				
				o.ase_color = v.color;
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
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
				half3 temp_cast_0 = (0.0).xxx;
				half4 Color_Main_RGBA49_g23654 = _MainColor;
				half4 vertexToFrag11_g23655 = i.ase_texcoord1;
				half4 Color_Seasons1715_g23654 = vertexToFrag11_g23655;
				half Element_Mode55_g23654 = _ElementMode;
				half4 lerpResult30_g23654 = lerp( Color_Main_RGBA49_g23654 , Color_Seasons1715_g23654 , Element_Mode55_g23654);
				half2 vertexToFrag11_g23670 = i.ase_texcoord2.xy;
				half4 MainTex_RGBA587_g23654 = tex2D( _MainTex, vertexToFrag11_g23670 );
				half3 temp_cast_1 = (0.0001).xxx;
				half3 temp_cast_2 = (0.9999).xxx;
				half3 clampResult17_g23671 = clamp( (MainTex_RGBA587_g23654).rgb , temp_cast_1 , temp_cast_2 );
				half temp_output_7_0_g23672 = _MainTexColorMinValue;
				half3 temp_cast_3 = (temp_output_7_0_g23672).xxx;
				half temp_output_10_0_g23672 = ( _MainTexColorMaxValue - temp_output_7_0_g23672 );
				half3 temp_output_1765_0_g23654 = saturate( ( ( clampResult17_g23671 - temp_cast_3 ) / temp_output_10_0_g23672 ) );
				half3 Element_Remap_RGB1555_g23654 = temp_output_1765_0_g23654;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half3 Element_Params_RGB1735_g23654 = (_ElementParams_Instance).xyz;
				half3 Element_Color1756_g23654 = ( Element_Remap_RGB1555_g23654 * Element_Params_RGB1735_g23654 * (i.ase_color).rgb );
				half3 Final_Colors_RGB142_g23654 = ( (lerpResult30_g23654).rgb * Element_Color1756_g23654 );
				half3 Input_Color94_g23673 = Final_Colors_RGB142_g23654;
				half3 Element_Valid47_g23673 = Input_Color94_g23673;
				half4 Colors37_g23685 = TVE_ColorsPosition;
				half4 Extras37_g23685 = TVE_ExtrasPosition;
				half4 Motion37_g23685 = TVE_MotionPosition;
				half4 Vertex37_g23685 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g23685 = IS_ELEMENT( Colors37_g23685 , Extras37_g23685 , Motion37_g23685 , Vertex37_g23685 );
				half4 Colors37_g23686 = TVE_ColorsScale;
				half4 Extras37_g23686 = TVE_ExtrasScale;
				half4 Motion37_g23686 = TVE_MotionScale;
				half4 Vertex37_g23686 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g23686 = IS_ELEMENT( Colors37_g23686 , Extras37_g23686 , Motion37_g23686 , Vertex37_g23686 );
				half Bounds42_g23673 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g23685).xyz ) ) - ( (localIS_ELEMENT37_g23686).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				half3 lerpResult50_g23673 = lerp( temp_cast_0 , Element_Valid47_g23673 , Bounds42_g23673);
				half CrossHatch44_g23673 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				half clampResult17_g23668 = clamp( (MainTex_RGBA587_g23654).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g23669 = _MainTexAlphaMinValue;
				half temp_output_10_0_g23669 = ( _MainTexAlphaMaxValue - temp_output_7_0_g23669 );
				half Element_Remap_A74_g23654 = saturate( ( ( clampResult17_g23668 - temp_output_7_0_g23669 ) / ( temp_output_10_0_g23669 + 0.0001 ) ) );
				half Element_Params_A1737_g23654 = _ElementParams_Instance.w;
				half clampResult17_g23666 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.zw*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g23667 = _MainTexFallofMinValue;
				half temp_output_10_0_g23667 = ( _MainTexFallofMaxValue - temp_output_7_0_g23667 );
				half Element_Falloff1883_g23654 = saturate( ( ( clampResult17_g23666 - temp_output_7_0_g23667 ) / ( temp_output_10_0_g23667 + 0.0001 ) ) );
				half4 Colors37_g23858 = TVE_ColorsCoords;
				half4 Extras37_g23858 = TVE_ExtrasCoords;
				half4 Motion37_g23858 = TVE_MotionCoords;
				half4 Vertex37_g23858 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g23858 = IS_ELEMENT( Colors37_g23858 , Extras37_g23858 , Motion37_g23858 , Vertex37_g23858 );
				half4 temp_output_35_0_g23860 = localIS_ELEMENT37_g23858;
				half temp_output_7_0_g23861 = TVE_ElementsFadeValue;
				half2 temp_cast_4 = (temp_output_7_0_g23861).xx;
				half temp_output_10_0_g23861 = ( 1.0 - temp_output_7_0_g23861 );
				half2 temp_output_16_0_g23857 = saturate( ( ( abs( (( (temp_output_35_0_g23860).zw + ( (temp_output_35_0_g23860).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_4 ) / temp_output_10_0_g23861 ) );
				half2 temp_output_12_0_g23857 = ( temp_output_16_0_g23857 * temp_output_16_0_g23857 );
				half lerpResult18_g23857 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g23857).x + (temp_output_12_0_g23857).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g23654 = ( _ElementIntensity * Element_Remap_A74_g23654 * Element_Params_A1737_g23654 * i.ase_color.a * Element_Falloff1883_g23654 * lerpResult18_g23857 );
				half Final_Colors_A144_g23654 = ( (lerpResult30_g23654).a * Element_Alpha56_g23654 );
				half Input_Alpha48_g23673 = Final_Colors_A144_g23654;
				half lerpResult57_g23673 = lerp( CrossHatch44_g23673 , Input_Alpha48_g23673 , Bounds42_g23673);
				half4 appendResult58_g23673 = (half4(lerpResult50_g23673 , lerpResult57_g23673));
				
				
				finalColor = appendResult58_g23673;
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
Node;AmplifyShaderEditor.RangedFloatNode;178;-640,-640;Half;False;Property;_render_colormask;_render_colormask;81;1;[HideInInspector];Create;True;0;0;0;True;0;False;15;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;186;-640,-384;Inherit;False;Element Base Colors;1;;22385;378049ebac362e14aae08c2daa8ed737;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;248;-64,-768;Inherit;False;Compile All Elements;-1;;22387;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;272;-64,-384;Half;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Colors Tint;f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;True;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;2;5;False;;10;False;;4;1;False;;1;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;273;-64,-256;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-640,-768;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Colors elements to add color tinting to the vegetation assets. Element Texture RGB and Particle Color RGB are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;393;-384,-384;Inherit;False;Element Shader;11;;23654;0e972c73cae2ee54ea51acc9738801d0;12,477,0,1778,0,478,0,1824,0,145,0,1814,0,1784,0,481,0,1935,1,1933,1,1904,1,1907,1;1;1974;FLOAT;0;False;2;FLOAT4;0;FLOAT4;1779
WireConnection;272;0;393;0
WireConnection;273;0;393;1779
WireConnection;393;1974;186;0
ASEEND*/
//CHKSM=5D559FED76B67A1006A6B948EBF7D18871261081