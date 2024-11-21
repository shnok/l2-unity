// Upgrade NOTE: upgraded instancing buffer 'BOXOPHOBICTheVegetationEngineElementsColorsMap' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Colors Map"
{
	Properties
	{
		[StyledMessage(Info, Use the Colors Map elements to add color tinting to the vegetation assets. Element Texture RGB is used as tint color and Texture A is used as alpha mask., 0,0)]_ElementMessage("Element Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsColorsElement("_IsColorsElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEColorsLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
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
		[Space(10)][StyledRemapSlider(_SeasonMinValue, _SeasonMaxValue, 0, 1)]_SeasonRemap("Seasons Curve", Vector) = (0,0,0,0)
		[HideInInspector]_SeasonMinValue("Seasons Min", Range( 0 , 1)) = 0
		[HideInInspector]_SeasonMaxValue("Seasons Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_NoiseMinValue, _NoiseMaxValue, 0, 1)]_NoiseRemap("Noise Remap", Vector) = (0,0,0,0)
		[Space(10)]_SpeedTresholdValue("Speed Treshold", Float) = 10
		[Space(10)]_InfluenceValue1("Winter Influence", Range( 0 , 1)) = 1
		_InfluenceValue2("Spring Influence", Range( 0 , 1)) = 1
		_InfluenceValue3("Summer Influence", Range( 0 , 1)) = 1
		_InfluenceValue4("Autumn Influence", Range( 0 , 1)) = 1
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
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _render_colormask;
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
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsColorsElement;
			uniform half _IsElementShader;
			uniform half _IsVersion;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half4 _MainColor;
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
			uniform half4 TVE_SeasonOptions;
			uniform half _InfluenceValue1;
			uniform half _InfluenceValue2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half _InfluenceValue3;
			uniform half _InfluenceValue4;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsColorsMap)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsColorsMap
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsColorsMap)
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
				half2 appendResult1900_g22175 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g22175 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22175 , _ElementUVsMode);
				half2 vertexToFrag11_g22191 = ( ( lerpResult1899_g22175 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.xy = vertexToFrag11_g22191;
				half TVE_SeasonOptions_X50_g22175 = TVE_SeasonOptions.x;
				half Influence_Winter808_g22175 = _InfluenceValue1;
				half Influence_Spring814_g22175 = _InfluenceValue2;
				half temp_output_7_0_g22376 = _SeasonMinValue;
				half temp_output_10_0_g22376 = ( _SeasonMaxValue - temp_output_7_0_g22376 );
				half TVE_SeasonLerp54_g22175 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22376 ) / temp_output_10_0_g22376 ) ) );
				half lerpResult829_g22175 = lerp( Influence_Winter808_g22175 , Influence_Spring814_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_Y51_g22175 = TVE_SeasonOptions.y;
				half Influence_Summer815_g22175 = _InfluenceValue3;
				half lerpResult833_g22175 = lerp( Influence_Spring814_g22175 , Influence_Summer815_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_Z52_g22175 = TVE_SeasonOptions.z;
				half Influence_Autumn810_g22175 = _InfluenceValue4;
				half lerpResult816_g22175 = lerp( Influence_Summer815_g22175 , Influence_Autumn810_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_W53_g22175 = TVE_SeasonOptions.w;
				half lerpResult817_g22175 = lerp( Influence_Autumn810_g22175 , Influence_Winter808_g22175 , TVE_SeasonLerp54_g22175);
				half vertexToFrag11_g22178 = ( ( TVE_SeasonOptions_X50_g22175 * lerpResult829_g22175 ) + ( TVE_SeasonOptions_Y51_g22175 * lerpResult833_g22175 ) + ( TVE_SeasonOptions_Z52_g22175 * lerpResult816_g22175 ) + ( TVE_SeasonOptions_W53_g22175 * lerpResult817_g22175 ) );
				o.ase_texcoord2.x = vertexToFrag11_g22178;
				
				o.ase_color = v.color;
				o.ase_texcoord1.zw = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.yzw = 0;
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
				half4 Color_Main_RGBA49_g22175 = _MainColor;
				half2 vertexToFrag11_g22191 = i.ase_texcoord1.xy;
				half4 MainTex_RGBA587_g22175 = tex2D( _MainTex, vertexToFrag11_g22191 );
				half3 temp_cast_0 = (0.0001).xxx;
				half3 temp_cast_1 = (0.9999).xxx;
				half3 clampResult17_g22192 = clamp( (MainTex_RGBA587_g22175).rgb , temp_cast_0 , temp_cast_1 );
				half temp_output_7_0_g22193 = _MainTexColorMinValue;
				half3 temp_cast_2 = (temp_output_7_0_g22193).xxx;
				half temp_output_10_0_g22193 = ( _MainTexColorMaxValue - temp_output_7_0_g22193 );
				half3 temp_output_1765_0_g22175 = saturate( ( ( clampResult17_g22192 - temp_cast_2 ) / temp_output_10_0_g22193 ) );
				half3 Element_Remap_RGB1555_g22175 = temp_output_1765_0_g22175;
				half3 Final_ColorsMap_RGB598_g22175 = ( (Color_Main_RGBA49_g22175).rgb * Element_Remap_RGB1555_g22175 );
				half clampResult17_g22189 = clamp( (MainTex_RGBA587_g22175).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g22190 = _MainTexAlphaMinValue;
				half temp_output_10_0_g22190 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22190 );
				half Element_Remap_A74_g22175 = saturate( ( ( clampResult17_g22189 - temp_output_7_0_g22190 ) / ( temp_output_10_0_g22190 + 0.0001 ) ) );
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_A1737_g22175 = _ElementParams_Instance.w;
				half clampResult17_g22187 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord1.zw*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g22188 = _MainTexFallofMinValue;
				half temp_output_10_0_g22188 = ( _MainTexFallofMaxValue - temp_output_7_0_g22188 );
				half Element_Falloff1883_g22175 = saturate( ( ( clampResult17_g22187 - temp_output_7_0_g22188 ) / ( temp_output_10_0_g22188 + 0.0001 ) ) );
				half4 Colors37_g22379 = TVE_ColorsCoords;
				half4 Extras37_g22379 = TVE_ExtrasCoords;
				half4 Motion37_g22379 = TVE_MotionCoords;
				half4 Vertex37_g22379 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22379 = IS_ELEMENT( Colors37_g22379 , Extras37_g22379 , Motion37_g22379 , Vertex37_g22379 );
				half4 temp_output_35_0_g22381 = localIS_ELEMENT37_g22379;
				half temp_output_7_0_g22382 = TVE_ElementsFadeValue;
				half2 temp_cast_3 = (temp_output_7_0_g22382).xx;
				half temp_output_10_0_g22382 = ( 1.0 - temp_output_7_0_g22382 );
				half2 temp_output_16_0_g22378 = saturate( ( ( abs( (( (temp_output_35_0_g22381).zw + ( (temp_output_35_0_g22381).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_3 ) / temp_output_10_0_g22382 ) );
				half2 temp_output_12_0_g22378 = ( temp_output_16_0_g22378 * temp_output_16_0_g22378 );
				half lerpResult18_g22378 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22378).x + (temp_output_12_0_g22378).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22175 = ( _ElementIntensity * Element_Remap_A74_g22175 * Element_Params_A1737_g22175 * i.ase_color.a * Element_Falloff1883_g22175 * lerpResult18_g22378 );
				half vertexToFrag11_g22178 = i.ase_texcoord2.x;
				half Element_Seasons834_g22175 = vertexToFrag11_g22178;
				half Final_ColorsMap_A603_g22175 = ( Element_Alpha56_g22175 * Element_Seasons834_g22175 );
				half4 appendResult622_g22175 = (half4(Final_ColorsMap_RGB598_g22175 , Final_ColorsMap_A603_g22175));
				
				
				finalColor = appendResult622_g22175;
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
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _render_colormask;
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
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsColorsElement;
			uniform half _IsElementShader;
			uniform half _IsVersion;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half4 _MainColor;
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
			uniform half4 TVE_SeasonOptions;
			uniform half _InfluenceValue1;
			uniform half _InfluenceValue2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half _InfluenceValue3;
			uniform half _InfluenceValue4;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsColorsMap)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsColorsMap
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsColorsMap)
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
				half2 appendResult1900_g22175 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g22175 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22175 , _ElementUVsMode);
				half2 vertexToFrag11_g22191 = ( ( lerpResult1899_g22175 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.xy = vertexToFrag11_g22191;
				half TVE_SeasonOptions_X50_g22175 = TVE_SeasonOptions.x;
				half Influence_Winter808_g22175 = _InfluenceValue1;
				half Influence_Spring814_g22175 = _InfluenceValue2;
				half temp_output_7_0_g22376 = _SeasonMinValue;
				half temp_output_10_0_g22376 = ( _SeasonMaxValue - temp_output_7_0_g22376 );
				half TVE_SeasonLerp54_g22175 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22376 ) / temp_output_10_0_g22376 ) ) );
				half lerpResult829_g22175 = lerp( Influence_Winter808_g22175 , Influence_Spring814_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_Y51_g22175 = TVE_SeasonOptions.y;
				half Influence_Summer815_g22175 = _InfluenceValue3;
				half lerpResult833_g22175 = lerp( Influence_Spring814_g22175 , Influence_Summer815_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_Z52_g22175 = TVE_SeasonOptions.z;
				half Influence_Autumn810_g22175 = _InfluenceValue4;
				half lerpResult816_g22175 = lerp( Influence_Summer815_g22175 , Influence_Autumn810_g22175 , TVE_SeasonLerp54_g22175);
				half TVE_SeasonOptions_W53_g22175 = TVE_SeasonOptions.w;
				half lerpResult817_g22175 = lerp( Influence_Autumn810_g22175 , Influence_Winter808_g22175 , TVE_SeasonLerp54_g22175);
				half vertexToFrag11_g22178 = ( ( TVE_SeasonOptions_X50_g22175 * lerpResult829_g22175 ) + ( TVE_SeasonOptions_Y51_g22175 * lerpResult833_g22175 ) + ( TVE_SeasonOptions_Z52_g22175 * lerpResult816_g22175 ) + ( TVE_SeasonOptions_W53_g22175 * lerpResult817_g22175 ) );
				o.ase_texcoord2.x = vertexToFrag11_g22178;
				
				o.ase_color = v.color;
				o.ase_texcoord1.zw = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.yzw = 0;
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
				half4 Color_Main_RGBA49_g22175 = _MainColor;
				half2 vertexToFrag11_g22191 = i.ase_texcoord1.xy;
				half4 MainTex_RGBA587_g22175 = tex2D( _MainTex, vertexToFrag11_g22191 );
				half3 temp_cast_1 = (0.0001).xxx;
				half3 temp_cast_2 = (0.9999).xxx;
				half3 clampResult17_g22192 = clamp( (MainTex_RGBA587_g22175).rgb , temp_cast_1 , temp_cast_2 );
				half temp_output_7_0_g22193 = _MainTexColorMinValue;
				half3 temp_cast_3 = (temp_output_7_0_g22193).xxx;
				half temp_output_10_0_g22193 = ( _MainTexColorMaxValue - temp_output_7_0_g22193 );
				half3 temp_output_1765_0_g22175 = saturate( ( ( clampResult17_g22192 - temp_cast_3 ) / temp_output_10_0_g22193 ) );
				half3 Element_Remap_RGB1555_g22175 = temp_output_1765_0_g22175;
				half3 Final_ColorsMap_RGB598_g22175 = ( (Color_Main_RGBA49_g22175).rgb * Element_Remap_RGB1555_g22175 );
				half3 Input_Color94_g22208 = Final_ColorsMap_RGB598_g22175;
				half3 Element_Valid47_g22208 = Input_Color94_g22208;
				half4 Colors37_g22220 = TVE_ColorsPosition;
				half4 Extras37_g22220 = TVE_ExtrasPosition;
				half4 Motion37_g22220 = TVE_MotionPosition;
				half4 Vertex37_g22220 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g22220 = IS_ELEMENT( Colors37_g22220 , Extras37_g22220 , Motion37_g22220 , Vertex37_g22220 );
				half4 Colors37_g22221 = TVE_ColorsScale;
				half4 Extras37_g22221 = TVE_ExtrasScale;
				half4 Motion37_g22221 = TVE_MotionScale;
				half4 Vertex37_g22221 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g22221 = IS_ELEMENT( Colors37_g22221 , Extras37_g22221 , Motion37_g22221 , Vertex37_g22221 );
				half Bounds42_g22208 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g22220).xyz ) ) - ( (localIS_ELEMENT37_g22221).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				half3 lerpResult50_g22208 = lerp( temp_cast_0 , Element_Valid47_g22208 , Bounds42_g22208);
				half CrossHatch44_g22208 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				half clampResult17_g22189 = clamp( (MainTex_RGBA587_g22175).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g22190 = _MainTexAlphaMinValue;
				half temp_output_10_0_g22190 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22190 );
				half Element_Remap_A74_g22175 = saturate( ( ( clampResult17_g22189 - temp_output_7_0_g22190 ) / ( temp_output_10_0_g22190 + 0.0001 ) ) );
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_A1737_g22175 = _ElementParams_Instance.w;
				half clampResult17_g22187 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord1.zw*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g22188 = _MainTexFallofMinValue;
				half temp_output_10_0_g22188 = ( _MainTexFallofMaxValue - temp_output_7_0_g22188 );
				half Element_Falloff1883_g22175 = saturate( ( ( clampResult17_g22187 - temp_output_7_0_g22188 ) / ( temp_output_10_0_g22188 + 0.0001 ) ) );
				half4 Colors37_g22379 = TVE_ColorsCoords;
				half4 Extras37_g22379 = TVE_ExtrasCoords;
				half4 Motion37_g22379 = TVE_MotionCoords;
				half4 Vertex37_g22379 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22379 = IS_ELEMENT( Colors37_g22379 , Extras37_g22379 , Motion37_g22379 , Vertex37_g22379 );
				half4 temp_output_35_0_g22381 = localIS_ELEMENT37_g22379;
				half temp_output_7_0_g22382 = TVE_ElementsFadeValue;
				half2 temp_cast_4 = (temp_output_7_0_g22382).xx;
				half temp_output_10_0_g22382 = ( 1.0 - temp_output_7_0_g22382 );
				half2 temp_output_16_0_g22378 = saturate( ( ( abs( (( (temp_output_35_0_g22381).zw + ( (temp_output_35_0_g22381).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_4 ) / temp_output_10_0_g22382 ) );
				half2 temp_output_12_0_g22378 = ( temp_output_16_0_g22378 * temp_output_16_0_g22378 );
				half lerpResult18_g22378 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22378).x + (temp_output_12_0_g22378).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22175 = ( _ElementIntensity * Element_Remap_A74_g22175 * Element_Params_A1737_g22175 * i.ase_color.a * Element_Falloff1883_g22175 * lerpResult18_g22378 );
				half vertexToFrag11_g22178 = i.ase_texcoord2.x;
				half Element_Seasons834_g22175 = vertexToFrag11_g22178;
				half Final_ColorsMap_A603_g22175 = ( Element_Alpha56_g22175 * Element_Seasons834_g22175 );
				half Input_Alpha48_g22208 = Final_ColorsMap_A603_g22175;
				half lerpResult57_g22208 = lerp( CrossHatch44_g22208 , Input_Alpha48_g22208 , Bounds42_g22208);
				half4 appendResult58_g22208 = (half4(lerpResult50_g22208 , lerpResult57_g22208));
				
				
				finalColor = appendResult58_g22208;
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
Node;AmplifyShaderEditor.RangedFloatNode;139;-640,-640;Half;False;Property;_render_colormask;_render_colormask;81;1;[HideInInspector];Create;True;0;0;0;True;0;False;15;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;158;-384,-384;Inherit;False;Element Shader;11;;22175;0e972c73cae2ee54ea51acc9738801d0;12,477,0,1778,0,478,1,1824,1,145,0,1814,0,1784,0,481,0,1935,1,1933,1,1904,1,1907,1;1;1974;FLOAT;1;False;2;FLOAT4;0;FLOAT4;1779
Node;AmplifyShaderEditor.FunctionNode;108;-640,-384;Inherit;False;Element Base Colors;1;;22386;378049ebac362e14aae08c2daa8ed737;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;153;-64,-384;Half;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Colors Map;f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;154;-64,-256;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.FunctionNode;152;-64,-768;Inherit;False;Compile All Elements;-1;;22388;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-640,-768;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Colors Map elements to add color tinting to the vegetation assets. Element Texture RGB is used as tint color and Texture A is used as alpha mask., 0,0);False;0;0;1;1;0;1;FLOAT;0
WireConnection;158;1974;108;0
WireConnection;153;0;158;0
WireConnection;154;0;158;1779
ASEEND*/
//CHKSM=43A7549D9C482B2E20CDAD42C1BD9048E73965B7