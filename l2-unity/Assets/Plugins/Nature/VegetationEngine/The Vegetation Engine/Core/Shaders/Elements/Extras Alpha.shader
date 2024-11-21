// Upgrade NOTE: upgraded instancing buffer 'BOXOPHOBICTheVegetationEngineElementsExtrasAlpha' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Extras Alpha"
{
	Properties
	{
		[StyledMessage(Info, Use the Alpha elements to reduce the leaves amount. Useful to create winter sceneries or dead forests and dissolve effects. Element Texture R and Particle Color R are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0)]_ElementMessage("Element Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsExtrasElement("_IsExtrasElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEExtrasLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
		[Enum(Constant,0,Seasons,1)]_ElementMode("Render Mode", Float) = 0
		[Enum(Multiply,0,Additive,1)]_ElementBlendA("Render Blend", Float) = 0
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
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Fade", Float) = 0
		[HDR][StyledToggle]_ElementRaycastMode("Enable Raycast Fade", Float) = 0
		[Space(10)][StyledLayers()]_RaycastLayerMask("Raycast Layer", Float) = 1
		_RaycastDistanceEndValue("Raycast Distance", Float) = 2
		[HideInInspector]_ElementLayerValue("Legacy Layer Value", Float) = -1
		[HideInInspector]_InvertX("Legacy Invert Mode", Float) = 0
		[HideInInspector]_ElementFadeSupport("Legacy Edge Fading", Float) = 0
		[HideInInspector]_render_src("_render_src", Float) = 2
		[HideInInspector]_render_dst("_render_dst", Float) = 0

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
			Blend One Zero, [_render_src] [_render_dst]
			AlphaToMask Off
			Cull Back
			ColorMask A
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
			#define TVE_IS_EXTRAS_ELEMENT


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

			uniform half _render_src;
			uniform half _render_dst;
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
			uniform half _IsVersion;
			uniform half _IsExtrasElement;
			uniform half _IsElementShader;
			uniform half _CategoryRender;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half _MainValue;
			uniform half4 TVE_SeasonOptions;
			uniform half _AdditionalValue1;
			uniform half _AdditionalValue2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half _AdditionalValue3;
			uniform half _AdditionalValue4;
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
			uniform half _ElementBlendA;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsExtrasAlpha)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsExtrasAlpha
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsExtrasAlpha)
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

				half TVE_SeasonOptions_X50_g22097 = TVE_SeasonOptions.x;
				half Value_Winter158_g22097 = _AdditionalValue1;
				half Value_Spring159_g22097 = _AdditionalValue2;
				half temp_output_7_0_g22367 = _SeasonMinValue;
				half temp_output_10_0_g22367 = ( _SeasonMaxValue - temp_output_7_0_g22367 );
				half TVE_SeasonLerp54_g22097 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22367 ) / temp_output_10_0_g22367 ) ) );
				half lerpResult168_g22097 = lerp( Value_Winter158_g22097 , Value_Spring159_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_Y51_g22097 = TVE_SeasonOptions.y;
				half Value_Summer160_g22097 = _AdditionalValue3;
				half lerpResult167_g22097 = lerp( Value_Spring159_g22097 , Value_Summer160_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_Z52_g22097 = TVE_SeasonOptions.z;
				half Value_Autumn161_g22097 = _AdditionalValue4;
				half lerpResult166_g22097 = lerp( Value_Summer160_g22097 , Value_Autumn161_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_W53_g22097 = TVE_SeasonOptions.w;
				half lerpResult165_g22097 = lerp( Value_Autumn161_g22097 , Value_Winter158_g22097 , TVE_SeasonLerp54_g22097);
				half vertexToFrag11_g22099 = ( ( ( TVE_SeasonOptions_X50_g22097 * lerpResult168_g22097 ) + ( TVE_SeasonOptions_Y51_g22097 * lerpResult167_g22097 ) ) + ( ( TVE_SeasonOptions_Z52_g22097 * lerpResult166_g22097 ) + ( TVE_SeasonOptions_W53_g22097 * lerpResult165_g22097 ) ) );
				o.ase_texcoord1.x = vertexToFrag11_g22099;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				half2 appendResult1900_g22097 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g22097 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22097 , _ElementUVsMode);
				half2 vertexToFrag11_g22152 = ( ( lerpResult1899_g22097 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.yz = vertexToFrag11_g22152;
				
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
				half Value_Main157_g22097 = _MainValue;
				half vertexToFrag11_g22099 = i.ase_texcoord1.x;
				half Value_Seasons1721_g22097 = vertexToFrag11_g22099;
				half Element_Mode55_g22097 = _ElementMode;
				half lerpResult181_g22097 = lerp( Value_Main157_g22097 , Value_Seasons1721_g22097 , Element_Mode55_g22097);
				half2 vertexToFrag11_g22152 = i.ase_texcoord1.yz;
				half4 MainTex_RGBA587_g22097 = tex2D( _MainTex, vertexToFrag11_g22152 );
				half3 temp_cast_0 = (0.0001).xxx;
				half3 temp_cast_1 = (0.9999).xxx;
				half3 clampResult17_g22182 = clamp( (MainTex_RGBA587_g22097).rgb , temp_cast_0 , temp_cast_1 );
				half temp_output_7_0_g22183 = _MainTexColorMinValue;
				half3 temp_cast_2 = (temp_output_7_0_g22183).xxx;
				half temp_output_10_0_g22183 = ( _MainTexColorMaxValue - temp_output_7_0_g22183 );
				half3 temp_output_1765_0_g22097 = saturate( ( ( clampResult17_g22182 - temp_cast_2 ) / temp_output_10_0_g22183 ) );
				half Element_Remap_R73_g22097 = (temp_output_1765_0_g22097).x;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_R1738_g22097 = _ElementParams_Instance.x;
				half Element_Value1744_g22097 = ( Element_Remap_R73_g22097 * Element_Params_R1738_g22097 * i.ase_color.r );
				half Final_Extras_RGB213_g22097 = ( lerpResult181_g22097 * Element_Value1744_g22097 );
				half clampResult17_g22148 = clamp( (MainTex_RGBA587_g22097).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g22149 = _MainTexAlphaMinValue;
				half temp_output_10_0_g22149 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22149 );
				half Element_Remap_A74_g22097 = saturate( ( ( clampResult17_g22148 - temp_output_7_0_g22149 ) / ( temp_output_10_0_g22149 + 0.0001 ) ) );
				half Element_Params_A1737_g22097 = _ElementParams_Instance.w;
				half clampResult17_g22109 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g22110 = _MainTexFallofMinValue;
				half temp_output_10_0_g22110 = ( _MainTexFallofMaxValue - temp_output_7_0_g22110 );
				half Element_Falloff1883_g22097 = saturate( ( ( clampResult17_g22109 - temp_output_7_0_g22110 ) / ( temp_output_10_0_g22110 + 0.0001 ) ) );
				half4 Colors37_g22378 = TVE_ColorsCoords;
				half4 Extras37_g22378 = TVE_ExtrasCoords;
				half4 Motion37_g22378 = TVE_MotionCoords;
				half4 Vertex37_g22378 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22378 = IS_ELEMENT( Colors37_g22378 , Extras37_g22378 , Motion37_g22378 , Vertex37_g22378 );
				half4 temp_output_35_0_g22380 = localIS_ELEMENT37_g22378;
				half temp_output_7_0_g22381 = TVE_ElementsFadeValue;
				half2 temp_cast_3 = (temp_output_7_0_g22381).xx;
				half temp_output_10_0_g22381 = ( 1.0 - temp_output_7_0_g22381 );
				half2 temp_output_16_0_g22377 = saturate( ( ( abs( (( (temp_output_35_0_g22380).zw + ( (temp_output_35_0_g22380).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_3 ) / temp_output_10_0_g22381 ) );
				half2 temp_output_12_0_g22377 = ( temp_output_16_0_g22377 * temp_output_16_0_g22377 );
				half lerpResult18_g22377 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22377).x + (temp_output_12_0_g22377).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22097 = ( _ElementIntensity * Element_Remap_A74_g22097 * Element_Params_A1737_g22097 * i.ase_color.a * Element_Falloff1883_g22097 * lerpResult18_g22377 );
				half lerpResult1634_g22097 = lerp( 1.0 , Final_Extras_RGB213_g22097 , Element_Alpha56_g22097);
				half Element_BlendA918_g22097 = _ElementBlendA;
				half lerpResult933_g22097 = lerp( lerpResult1634_g22097 , ( Final_Extras_RGB213_g22097 * Element_Alpha56_g22097 ) , Element_BlendA918_g22097);
				half Final_Extras_Blend211_g22097 = lerpResult933_g22097;
				half4 appendResult472_g22097 = (half4(Final_Extras_RGB213_g22097 , 0.0 , 0.0 , Final_Extras_Blend211_g22097));
				
				
				finalColor = appendResult472_g22097;
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
			#define TVE_IS_EXTRAS_ELEMENT


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

			uniform half _render_src;
			uniform half _render_dst;
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
			uniform half _IsVersion;
			uniform half _IsExtrasElement;
			uniform half _IsElementShader;
			uniform half _CategoryRender;
			uniform half _ElementLayerMask;
			uniform half _ElementMessage;
			uniform half _MainValue;
			uniform half4 TVE_SeasonOptions;
			uniform half _AdditionalValue1;
			uniform half _AdditionalValue2;
			uniform half TVE_SeasonLerp;
			uniform half _SeasonMinValue;
			uniform half _SeasonMaxValue;
			uniform half _AdditionalValue3;
			uniform half _AdditionalValue4;
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
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsExtrasAlpha)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsExtrasAlpha
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsExtrasAlpha)
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

				half TVE_SeasonOptions_X50_g22097 = TVE_SeasonOptions.x;
				half Value_Winter158_g22097 = _AdditionalValue1;
				half Value_Spring159_g22097 = _AdditionalValue2;
				half temp_output_7_0_g22367 = _SeasonMinValue;
				half temp_output_10_0_g22367 = ( _SeasonMaxValue - temp_output_7_0_g22367 );
				half TVE_SeasonLerp54_g22097 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22367 ) / temp_output_10_0_g22367 ) ) );
				half lerpResult168_g22097 = lerp( Value_Winter158_g22097 , Value_Spring159_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_Y51_g22097 = TVE_SeasonOptions.y;
				half Value_Summer160_g22097 = _AdditionalValue3;
				half lerpResult167_g22097 = lerp( Value_Spring159_g22097 , Value_Summer160_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_Z52_g22097 = TVE_SeasonOptions.z;
				half Value_Autumn161_g22097 = _AdditionalValue4;
				half lerpResult166_g22097 = lerp( Value_Summer160_g22097 , Value_Autumn161_g22097 , TVE_SeasonLerp54_g22097);
				half TVE_SeasonOptions_W53_g22097 = TVE_SeasonOptions.w;
				half lerpResult165_g22097 = lerp( Value_Autumn161_g22097 , Value_Winter158_g22097 , TVE_SeasonLerp54_g22097);
				half vertexToFrag11_g22099 = ( ( ( TVE_SeasonOptions_X50_g22097 * lerpResult168_g22097 ) + ( TVE_SeasonOptions_Y51_g22097 * lerpResult167_g22097 ) ) + ( ( TVE_SeasonOptions_Z52_g22097 * lerpResult166_g22097 ) + ( TVE_SeasonOptions_W53_g22097 * lerpResult165_g22097 ) ) );
				o.ase_texcoord1.x = vertexToFrag11_g22099;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				half2 appendResult1900_g22097 = (half2(ase_worldPos.x , ase_worldPos.z));
				half2 lerpResult1899_g22097 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22097 , _ElementUVsMode);
				half2 vertexToFrag11_g22152 = ( ( lerpResult1899_g22097 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.yz = vertexToFrag11_g22152;
				
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
				half3 temp_cast_0 = (0.0).xxx;
				half Value_Main157_g22097 = _MainValue;
				half vertexToFrag11_g22099 = i.ase_texcoord1.x;
				half Value_Seasons1721_g22097 = vertexToFrag11_g22099;
				half Element_Mode55_g22097 = _ElementMode;
				half lerpResult181_g22097 = lerp( Value_Main157_g22097 , Value_Seasons1721_g22097 , Element_Mode55_g22097);
				half2 vertexToFrag11_g22152 = i.ase_texcoord1.yz;
				half4 MainTex_RGBA587_g22097 = tex2D( _MainTex, vertexToFrag11_g22152 );
				half3 temp_cast_1 = (0.0001).xxx;
				half3 temp_cast_2 = (0.9999).xxx;
				half3 clampResult17_g22182 = clamp( (MainTex_RGBA587_g22097).rgb , temp_cast_1 , temp_cast_2 );
				half temp_output_7_0_g22183 = _MainTexColorMinValue;
				half3 temp_cast_3 = (temp_output_7_0_g22183).xxx;
				half temp_output_10_0_g22183 = ( _MainTexColorMaxValue - temp_output_7_0_g22183 );
				half3 temp_output_1765_0_g22097 = saturate( ( ( clampResult17_g22182 - temp_cast_3 ) / temp_output_10_0_g22183 ) );
				half Element_Remap_R73_g22097 = (temp_output_1765_0_g22097).x;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_R1738_g22097 = _ElementParams_Instance.x;
				half Element_Value1744_g22097 = ( Element_Remap_R73_g22097 * Element_Params_R1738_g22097 * i.ase_color.r );
				half Final_Extras_RGB213_g22097 = ( lerpResult181_g22097 * Element_Value1744_g22097 );
				half3 temp_cast_4 = (Final_Extras_RGB213_g22097).xxx;
				half3 Input_Color94_g22311 = temp_cast_4;
				half3 break68_g22311 = Input_Color94_g22311;
				half clampResult80_g22311 = clamp( max( max( break68_g22311.x , break68_g22311.y ) , break68_g22311.z ) , 0.1 , 10000.0 );
				half4 color1875_g22097 = IsGammaSpace() ? half4(0.5,0.5,0.5,0) : half4(0.2140411,0.2140411,0.2140411,0);
				half3 Input_Tint76_g22311 = (color1875_g22097).rgb;
				half3 Element_Valid47_g22311 = ( clampResult80_g22311 * Input_Tint76_g22311 );
				half4 Colors37_g22323 = TVE_ColorsPosition;
				half4 Extras37_g22323 = TVE_ExtrasPosition;
				half4 Motion37_g22323 = TVE_MotionPosition;
				half4 Vertex37_g22323 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g22323 = IS_ELEMENT( Colors37_g22323 , Extras37_g22323 , Motion37_g22323 , Vertex37_g22323 );
				half4 Colors37_g22324 = TVE_ColorsScale;
				half4 Extras37_g22324 = TVE_ExtrasScale;
				half4 Motion37_g22324 = TVE_MotionScale;
				half4 Vertex37_g22324 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g22324 = IS_ELEMENT( Colors37_g22324 , Extras37_g22324 , Motion37_g22324 , Vertex37_g22324 );
				half Bounds42_g22311 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g22323).xyz ) ) - ( (localIS_ELEMENT37_g22324).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				half3 lerpResult50_g22311 = lerp( temp_cast_0 , Element_Valid47_g22311 , Bounds42_g22311);
				half CrossHatch44_g22311 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				half clampResult17_g22148 = clamp( (MainTex_RGBA587_g22097).a , 0.0001 , 0.9999 );
				half temp_output_7_0_g22149 = _MainTexAlphaMinValue;
				half temp_output_10_0_g22149 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22149 );
				half Element_Remap_A74_g22097 = saturate( ( ( clampResult17_g22148 - temp_output_7_0_g22149 ) / ( temp_output_10_0_g22149 + 0.0001 ) ) );
				half Element_Params_A1737_g22097 = _ElementParams_Instance.w;
				half clampResult17_g22109 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				half temp_output_7_0_g22110 = _MainTexFallofMinValue;
				half temp_output_10_0_g22110 = ( _MainTexFallofMaxValue - temp_output_7_0_g22110 );
				half Element_Falloff1883_g22097 = saturate( ( ( clampResult17_g22109 - temp_output_7_0_g22110 ) / ( temp_output_10_0_g22110 + 0.0001 ) ) );
				half4 Colors37_g22378 = TVE_ColorsCoords;
				half4 Extras37_g22378 = TVE_ExtrasCoords;
				half4 Motion37_g22378 = TVE_MotionCoords;
				half4 Vertex37_g22378 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22378 = IS_ELEMENT( Colors37_g22378 , Extras37_g22378 , Motion37_g22378 , Vertex37_g22378 );
				half4 temp_output_35_0_g22380 = localIS_ELEMENT37_g22378;
				half temp_output_7_0_g22381 = TVE_ElementsFadeValue;
				half2 temp_cast_5 = (temp_output_7_0_g22381).xx;
				half temp_output_10_0_g22381 = ( 1.0 - temp_output_7_0_g22381 );
				half2 temp_output_16_0_g22377 = saturate( ( ( abs( (( (temp_output_35_0_g22380).zw + ( (temp_output_35_0_g22380).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_5 ) / temp_output_10_0_g22381 ) );
				half2 temp_output_12_0_g22377 = ( temp_output_16_0_g22377 * temp_output_16_0_g22377 );
				half lerpResult18_g22377 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22377).x + (temp_output_12_0_g22377).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22097 = ( _ElementIntensity * Element_Remap_A74_g22097 * Element_Params_A1737_g22097 * i.ase_color.a * Element_Falloff1883_g22097 * lerpResult18_g22377 );
				half Final_Extras_A241_g22097 = Element_Alpha56_g22097;
				half Input_Alpha48_g22311 = Final_Extras_A241_g22097;
				half lerpResult57_g22311 = lerp( CrossHatch44_g22311 , Input_Alpha48_g22311 , Bounds42_g22311);
				half4 appendResult58_g22311 = (half4(lerpResult50_g22311 , lerpResult57_g22311));
				
				
				finalColor = appendResult58_g22311;
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
Node;AmplifyShaderEditor.RangedFloatNode;112;-640,-1280;Inherit;False;Property;_render_src;_render_src;81;1;[HideInInspector];Create;True;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-480,-1280;Inherit;False;Property;_render_dst;_render_dst;82;1;[HideInInspector];Create;True;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;142;-384,-1024;Inherit;False;Element Shader;11;;22097;0e972c73cae2ee54ea51acc9738801d0;12,477,1,1778,1,478,0,1824,0,145,3,1814,3,1784,0,481,0,1935,1,1933,1,1904,1,1907,1;1;1974;FLOAT;1;False;2;FLOAT4;0;FLOAT4;1779
Node;AmplifyShaderEditor.FunctionNode;106;-640,-1024;Inherit;False;Element Base Extras;1;;22385;adca672cb6779794dba5f669b4c5f8e3;0;0;1;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;138;-64,-1408;Inherit;False;Compile All Elements;-1;;22387;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;139;-64,-1024;Half;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Extras Alpha;f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;1;1;False;;0;False;;1;0;True;_render_src;0;True;_render_dst;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;True;True;False;False;False;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;140;-64,-896;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;104;-640,-1408;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Alpha elements to reduce the leaves amount. Useful to create winter sceneries or dead forests and dissolve effects. Element Texture R and Particle Color R are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0);False;0;0;1;1;0;1;FLOAT;0
WireConnection;142;1974;106;4
WireConnection;139;0;142;0
WireConnection;140;0;142;1779
ASEEND*/
//CHKSM=4AC45C03AC925C729C11B70FA0293A44D8690CAF