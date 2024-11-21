// Upgrade NOTE: upgraded instancing buffer 'BOXOPHOBICTheVegetationEngineElementsExtrasEmissive' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Extras Emissive"
{
	Properties
	{
		[StyledMessage(Info, Use the Emissive elements to control the emissive effect on vegetation or static props. Element Texture R and Particle Color R are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0)]_ElementMessage("Element Message", Float) = 0
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
			ColorMask R
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
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsExtrasEmissive)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsExtrasEmissive
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsExtrasEmissive)
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

				half TVE_SeasonOptions_X50_g22783 = TVE_SeasonOptions.x;
				half Value_Winter158_g22783 = _AdditionalValue1;
				half Value_Spring159_g22783 = _AdditionalValue2;
				float temp_output_7_0_g22984 = _SeasonMinValue;
				float temp_output_10_0_g22984 = ( _SeasonMaxValue - temp_output_7_0_g22984 );
				half TVE_SeasonLerp54_g22783 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22984 ) / temp_output_10_0_g22984 ) ) );
				float lerpResult168_g22783 = lerp( Value_Winter158_g22783 , Value_Spring159_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_Y51_g22783 = TVE_SeasonOptions.y;
				half Value_Summer160_g22783 = _AdditionalValue3;
				float lerpResult167_g22783 = lerp( Value_Spring159_g22783 , Value_Summer160_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_Z52_g22783 = TVE_SeasonOptions.z;
				half Value_Autumn161_g22783 = _AdditionalValue4;
				float lerpResult166_g22783 = lerp( Value_Summer160_g22783 , Value_Autumn161_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_W53_g22783 = TVE_SeasonOptions.w;
				float lerpResult165_g22783 = lerp( Value_Autumn161_g22783 , Value_Winter158_g22783 , TVE_SeasonLerp54_g22783);
				float vertexToFrag11_g22785 = ( ( ( TVE_SeasonOptions_X50_g22783 * lerpResult168_g22783 ) + ( TVE_SeasonOptions_Y51_g22783 * lerpResult167_g22783 ) ) + ( ( TVE_SeasonOptions_Z52_g22783 * lerpResult166_g22783 ) + ( TVE_SeasonOptions_W53_g22783 * lerpResult165_g22783 ) ) );
				o.ase_texcoord1.x = vertexToFrag11_g22785;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				float2 appendResult1900_g22783 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 lerpResult1899_g22783 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22783 , _ElementUVsMode);
				float2 vertexToFrag11_g22799 = ( ( lerpResult1899_g22783 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.yz = vertexToFrag11_g22799;
				
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
				half Value_Main157_g22783 = _MainValue;
				float vertexToFrag11_g22785 = i.ase_texcoord1.x;
				half Value_Seasons1721_g22783 = vertexToFrag11_g22785;
				half Element_Mode55_g22783 = _ElementMode;
				float lerpResult181_g22783 = lerp( Value_Main157_g22783 , Value_Seasons1721_g22783 , Element_Mode55_g22783);
				float2 vertexToFrag11_g22799 = i.ase_texcoord1.yz;
				half4 MainTex_RGBA587_g22783 = tex2D( _MainTex, vertexToFrag11_g22799 );
				float3 temp_cast_0 = (0.0001).xxx;
				float3 temp_cast_1 = (0.9999).xxx;
				float3 clampResult17_g22800 = clamp( (MainTex_RGBA587_g22783).rgb , temp_cast_0 , temp_cast_1 );
				float temp_output_7_0_g22801 = _MainTexColorMinValue;
				float3 temp_cast_2 = (temp_output_7_0_g22801).xxx;
				float temp_output_10_0_g22801 = ( _MainTexColorMaxValue - temp_output_7_0_g22801 );
				float3 temp_output_1765_0_g22783 = saturate( ( ( clampResult17_g22800 - temp_cast_2 ) / temp_output_10_0_g22801 ) );
				half Element_Remap_R73_g22783 = (temp_output_1765_0_g22783).x;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_R1738_g22783 = _ElementParams_Instance.x;
				half Element_Value1744_g22783 = ( Element_Remap_R73_g22783 * Element_Params_R1738_g22783 * i.ase_color.r );
				half Final_Extras_RGB213_g22783 = ( lerpResult181_g22783 * Element_Value1744_g22783 );
				float clampResult17_g22797 = clamp( (MainTex_RGBA587_g22783).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g22798 = _MainTexAlphaMinValue;
				float temp_output_10_0_g22798 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22798 );
				half Element_Remap_A74_g22783 = saturate( ( ( clampResult17_g22797 - temp_output_7_0_g22798 ) / ( temp_output_10_0_g22798 + 0.0001 ) ) );
				half Element_Params_A1737_g22783 = _ElementParams_Instance.w;
				float clampResult17_g22795 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				float temp_output_7_0_g22796 = _MainTexFallofMinValue;
				float temp_output_10_0_g22796 = ( _MainTexFallofMaxValue - temp_output_7_0_g22796 );
				half Element_Falloff1883_g22783 = saturate( ( ( clampResult17_g22795 - temp_output_7_0_g22796 ) / ( temp_output_10_0_g22796 + 0.0001 ) ) );
				half4 Colors37_g22987 = TVE_ColorsCoords;
				half4 Extras37_g22987 = TVE_ExtrasCoords;
				half4 Motion37_g22987 = TVE_MotionCoords;
				half4 Vertex37_g22987 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22987 = IS_ELEMENT( Colors37_g22987 , Extras37_g22987 , Motion37_g22987 , Vertex37_g22987 );
				float4 temp_output_35_0_g22989 = localIS_ELEMENT37_g22987;
				float temp_output_7_0_g22990 = TVE_ElementsFadeValue;
				float2 temp_cast_3 = (temp_output_7_0_g22990).xx;
				float temp_output_10_0_g22990 = ( 1.0 - temp_output_7_0_g22990 );
				float2 temp_output_16_0_g22986 = saturate( ( ( abs( (( (temp_output_35_0_g22989).zw + ( (temp_output_35_0_g22989).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_3 ) / temp_output_10_0_g22990 ) );
				float2 temp_output_12_0_g22986 = ( temp_output_16_0_g22986 * temp_output_16_0_g22986 );
				float lerpResult18_g22986 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22986).x + (temp_output_12_0_g22986).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22783 = ( _ElementIntensity * Element_Remap_A74_g22783 * Element_Params_A1737_g22783 * i.ase_color.a * Element_Falloff1883_g22783 * lerpResult18_g22986 );
				half Final_Extras_A241_g22783 = Element_Alpha56_g22783;
				float4 appendResult882_g22783 = (float4(Final_Extras_RGB213_g22783 , 0.0 , 0.0 , Final_Extras_A241_g22783));
				
				
				finalColor = appendResult882_g22783;
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
			uniform float _ElementMode;
			uniform sampler2D _MainTex;
			uniform float _ElementUVsMode;
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
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsExtrasEmissive)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsExtrasEmissive
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsExtrasEmissive)
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

				half TVE_SeasonOptions_X50_g22783 = TVE_SeasonOptions.x;
				half Value_Winter158_g22783 = _AdditionalValue1;
				half Value_Spring159_g22783 = _AdditionalValue2;
				float temp_output_7_0_g22984 = _SeasonMinValue;
				float temp_output_10_0_g22984 = ( _SeasonMaxValue - temp_output_7_0_g22984 );
				half TVE_SeasonLerp54_g22783 = saturate( saturate( ( ( TVE_SeasonLerp - temp_output_7_0_g22984 ) / temp_output_10_0_g22984 ) ) );
				float lerpResult168_g22783 = lerp( Value_Winter158_g22783 , Value_Spring159_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_Y51_g22783 = TVE_SeasonOptions.y;
				half Value_Summer160_g22783 = _AdditionalValue3;
				float lerpResult167_g22783 = lerp( Value_Spring159_g22783 , Value_Summer160_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_Z52_g22783 = TVE_SeasonOptions.z;
				half Value_Autumn161_g22783 = _AdditionalValue4;
				float lerpResult166_g22783 = lerp( Value_Summer160_g22783 , Value_Autumn161_g22783 , TVE_SeasonLerp54_g22783);
				half TVE_SeasonOptions_W53_g22783 = TVE_SeasonOptions.w;
				float lerpResult165_g22783 = lerp( Value_Autumn161_g22783 , Value_Winter158_g22783 , TVE_SeasonLerp54_g22783);
				float vertexToFrag11_g22785 = ( ( ( TVE_SeasonOptions_X50_g22783 * lerpResult168_g22783 ) + ( TVE_SeasonOptions_Y51_g22783 * lerpResult167_g22783 ) ) + ( ( TVE_SeasonOptions_Z52_g22783 * lerpResult166_g22783 ) + ( TVE_SeasonOptions_W53_g22783 * lerpResult165_g22783 ) ) );
				o.ase_texcoord1.x = vertexToFrag11_g22785;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				float2 appendResult1900_g22783 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 lerpResult1899_g22783 = lerp( ( 1.0 - v.ase_texcoord.xy ) , appendResult1900_g22783 , _ElementUVsMode);
				float2 vertexToFrag11_g22799 = ( ( lerpResult1899_g22783 * (_MainUVs).xy ) + (_MainUVs).zw );
				o.ase_texcoord1.yz = vertexToFrag11_g22799;
				
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
				float3 temp_cast_0 = (0.0).xxx;
				half Value_Main157_g22783 = _MainValue;
				float vertexToFrag11_g22785 = i.ase_texcoord1.x;
				half Value_Seasons1721_g22783 = vertexToFrag11_g22785;
				half Element_Mode55_g22783 = _ElementMode;
				float lerpResult181_g22783 = lerp( Value_Main157_g22783 , Value_Seasons1721_g22783 , Element_Mode55_g22783);
				float2 vertexToFrag11_g22799 = i.ase_texcoord1.yz;
				half4 MainTex_RGBA587_g22783 = tex2D( _MainTex, vertexToFrag11_g22799 );
				float3 temp_cast_1 = (0.0001).xxx;
				float3 temp_cast_2 = (0.9999).xxx;
				float3 clampResult17_g22800 = clamp( (MainTex_RGBA587_g22783).rgb , temp_cast_1 , temp_cast_2 );
				float temp_output_7_0_g22801 = _MainTexColorMinValue;
				float3 temp_cast_3 = (temp_output_7_0_g22801).xxx;
				float temp_output_10_0_g22801 = ( _MainTexColorMaxValue - temp_output_7_0_g22801 );
				float3 temp_output_1765_0_g22783 = saturate( ( ( clampResult17_g22800 - temp_cast_3 ) / temp_output_10_0_g22801 ) );
				half Element_Remap_R73_g22783 = (temp_output_1765_0_g22783).x;
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_R1738_g22783 = _ElementParams_Instance.x;
				half Element_Value1744_g22783 = ( Element_Remap_R73_g22783 * Element_Params_R1738_g22783 * i.ase_color.r );
				half Final_Extras_RGB213_g22783 = ( lerpResult181_g22783 * Element_Value1744_g22783 );
				float3 temp_cast_4 = (Final_Extras_RGB213_g22783).xxx;
				half3 Input_Color94_g22942 = temp_cast_4;
				float3 break68_g22942 = Input_Color94_g22942;
				float clampResult80_g22942 = clamp( max( max( break68_g22942.x , break68_g22942.y ) , break68_g22942.z ) , 0.1 , 10000.0 );
				float4 color1872_g22783 = IsGammaSpace() ? float4(1,0.6,0,0) : float4(1,0.3185468,0,0);
				half3 Input_Tint76_g22942 = (color1872_g22783).rgb;
				half3 Element_Valid47_g22942 = ( clampResult80_g22942 * Input_Tint76_g22942 );
				half4 Colors37_g22954 = TVE_ColorsPosition;
				half4 Extras37_g22954 = TVE_ExtrasPosition;
				half4 Motion37_g22954 = TVE_MotionPosition;
				half4 Vertex37_g22954 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g22954 = IS_ELEMENT( Colors37_g22954 , Extras37_g22954 , Motion37_g22954 , Vertex37_g22954 );
				half4 Colors37_g22955 = TVE_ColorsScale;
				half4 Extras37_g22955 = TVE_ExtrasScale;
				half4 Motion37_g22955 = TVE_MotionScale;
				half4 Vertex37_g22955 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g22955 = IS_ELEMENT( Colors37_g22955 , Extras37_g22955 , Motion37_g22955 , Vertex37_g22955 );
				half Bounds42_g22942 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g22954).xyz ) ) - ( (localIS_ELEMENT37_g22955).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				float3 lerpResult50_g22942 = lerp( temp_cast_0 , Element_Valid47_g22942 , Bounds42_g22942);
				half CrossHatch44_g22942 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				float clampResult17_g22797 = clamp( (MainTex_RGBA587_g22783).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g22798 = _MainTexAlphaMinValue;
				float temp_output_10_0_g22798 = ( _MainTexAlphaMaxValue - temp_output_7_0_g22798 );
				half Element_Remap_A74_g22783 = saturate( ( ( clampResult17_g22797 - temp_output_7_0_g22798 ) / ( temp_output_10_0_g22798 + 0.0001 ) ) );
				half Element_Params_A1737_g22783 = _ElementParams_Instance.w;
				float clampResult17_g22795 = clamp( saturate( ( 1.0 - distance( (i.ase_texcoord2.xy*2.0 + -1.0) , float2( 0,0 ) ) ) ) , 0.0001 , 0.9999 );
				float temp_output_7_0_g22796 = _MainTexFallofMinValue;
				float temp_output_10_0_g22796 = ( _MainTexFallofMaxValue - temp_output_7_0_g22796 );
				half Element_Falloff1883_g22783 = saturate( ( ( clampResult17_g22795 - temp_output_7_0_g22796 ) / ( temp_output_10_0_g22796 + 0.0001 ) ) );
				half4 Colors37_g22987 = TVE_ColorsCoords;
				half4 Extras37_g22987 = TVE_ExtrasCoords;
				half4 Motion37_g22987 = TVE_MotionCoords;
				half4 Vertex37_g22987 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g22987 = IS_ELEMENT( Colors37_g22987 , Extras37_g22987 , Motion37_g22987 , Vertex37_g22987 );
				float4 temp_output_35_0_g22989 = localIS_ELEMENT37_g22987;
				float temp_output_7_0_g22990 = TVE_ElementsFadeValue;
				float2 temp_cast_5 = (temp_output_7_0_g22990).xx;
				float temp_output_10_0_g22990 = ( 1.0 - temp_output_7_0_g22990 );
				float2 temp_output_16_0_g22986 = saturate( ( ( abs( (( (temp_output_35_0_g22989).zw + ( (temp_output_35_0_g22989).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_5 ) / temp_output_10_0_g22990 ) );
				float2 temp_output_12_0_g22986 = ( temp_output_16_0_g22986 * temp_output_16_0_g22986 );
				float lerpResult18_g22986 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g22986).x + (temp_output_12_0_g22986).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g22783 = ( _ElementIntensity * Element_Remap_A74_g22783 * Element_Params_A1737_g22783 * i.ase_color.a * Element_Falloff1883_g22783 * lerpResult18_g22986 );
				half Final_Extras_A241_g22783 = Element_Alpha56_g22783;
				half Input_Alpha48_g22942 = Final_Extras_A241_g22783;
				float lerpResult57_g22942 = lerp( CrossHatch44_g22942 , Input_Alpha48_g22942 , Bounds42_g22942);
				float4 appendResult58_g22942 = (float4(lerpResult50_g22942 , lerpResult57_g22942));
				
				
				finalColor = appendResult58_g22942;
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
Node;AmplifyShaderEditor.FunctionNode;142;-384,-1152;Inherit;False;Element Shader;11;;22783;0e972c73cae2ee54ea51acc9738801d0;12,477,1,1778,1,478,0,1824,0,145,0,1814,0,1784,0,481,0,1935,1,1933,1,1904,1,1907,1;1;1974;FLOAT;1;False;2;FLOAT4;0;FLOAT4;1779
Node;AmplifyShaderEditor.FunctionNode;113;-640,-1152;Inherit;False;Element Base Extras;1;;22994;adca672cb6779794dba5f669b4c5f8e3;0;0;1;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;131;-64,-1408;Inherit;False;Compile All Elements;-1;;22996;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;132;-64,-1152;Float;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Extras Emissive;f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;True;True;True;False;False;False;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;133;-64,-1024;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-640,-1408;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Emissive elements to control the emissive effect on vegetation or static props. Element Texture R and Particle Color R are used as value multipliers. Element Texture A and Particle Color A are used as alpha masks., 0,0);False;0;0;1;1;0;1;FLOAT;0
WireConnection;142;1974;113;4
WireConnection;132;0;142;0
WireConnection;133;0;142;1779
ASEEND*/
//CHKSM=FF7236DA037FE159DCDFB07D61579C3442252459