// Upgrade NOTE: upgraded instancing buffer 'BOXOPHOBICTheVegetationEngineElementsMotionAdvanced' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Motion Advanced"
{
	Properties
	{
		[StyledMessage(Info, The Element Texture mode is setting the direction based on the Element Texture__ where RG is used as World XZ direction. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 20, 0, 0)]_ElementDirectionTextureMessage("Element Direction Message", Float) = 0
		[StyledMessage(Info, The Element Forward mode is setting the direction in the element transform forward axis. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 10, 0, 0)]_ElementDirectionForwardMessage("Element Direction Message", Float) = 0
		[StyledMessage(Info, The Particle Translate mode is setting the direction based on the particle gameobject transform movement direction. Use the Speed Treshold to control how fast the particle movement is transformend into interaction. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 30, 0, 0)]_ElementDirectionTranslateMessage("Element Direction Message", Float) = 0
		[StyledMessage(Info, The Particle Velocity mode is setting the direction based on the particles motion direction. This option requires the particle to have custom vertex streams for Velocity and Speed set after the UV stream under the particle Renderer module. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 40, 0, 0)]_ElementDirectionVelocityMessage("Element Direction Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsMotionElement("_IsMotionElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEMotionLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[HideInInspector]_ElementParams("Render Params", Vector) = (1,1,1,1)
		[Enum(Affect Material Interaction,13,Affect Interaction and Wind Power,15)]_ElementMotionMode("Render Effect", Float) = 15
		[Enum(Element Forward,10,Element Texture,20,Particle Translate,30,Particle Velocity,40)]_ElementDirectionMode("Render Direction", Float) = 20
		[StyledCategory(Element Settings)]_CategoryElement("[ Category Element ]", Float) = 0
		[NoScaleOffset][StyledTextureSingleLine]_MainTex("Element Texture", 2D) = "white" {}
		[StyledSpace(10)]_SpaceTexture("#SpaceTexture", Float) = 0
		[StyledRemapSlider(_MainTexColorMinValue, _MainTexColorMaxValue, 0, 1)]_MainTexColorRemap("Element Value", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexColorMinValue("Element Color Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexColorMaxValue("Element Color Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_MainTexAlphaMinValue, _MainTexAlphaMaxValue, 0, 1)]_MainTexAlphaRemap("Element Alpha", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexAlphaMinValue("Element Alpha Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexAlphaMaxValue("Element Alpha Max", Range( 0 , 1)) = 1
		[StyledRemapSlider(_MainTexFallofMinValue, _MainTexFallofMaxValue, 0, 1)]_MainTexFallofRemap("Element Falloff", Vector) = (0,0,0,0)
		[Space(10)][StyledRemapSlider(_SeasonMinValue, _SeasonMaxValue, 0, 1)]_SeasonRemap("Seasons Curve", Vector) = (0,0,0,0)
		[Space(10)][StyledTextureSingleLine]_NoiseTex("Noise Texture", 2D) = "gray" {}
		[Space(10)]_NoiseIntensityValue("Noise Intensity", Range( 0 , 1)) = 0
		[StyledRemapSlider(_NoiseMinValue, _NoiseMaxValue, 0, 1)]_NoiseRemap("Noise Remap", Vector) = (0,0,0,0)
		[HideInInspector]_NoiseMinValue("Noise Min", Range( 0 , 1)) = 0
		[HideInInspector]_NoiseMaxValue("Noise Max", Range( 0 , 1)) = 1
		_NoiseScaleValue("Noise Scale", Float) = 1
		_NoiseSpeedValue("Noise Speed", Float) = 1
		_NoisePhaseValue("Noise Phase", Float) = 0
		[Space(10)]_MotionPower("Wind Power", Range( 0 , 1)) = 0
		[Space(10)]_SpeedTresholdValue("Speed Treshold", Float) = 10
		[Space(10)][StyledToggle]_ElementInvertMode("Use Inverted Element Direction", Float) = 0
		[StyledCategory(Fading Settings)]_CategoryFade("[ Category Fade ]", Float) = 0
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Fade", Float) = 0
		[HDR][StyledToggle]_ElementRaycastMode("Enable Raycast Fade", Float) = 0
		[Space(10)][StyledLayers()]_RaycastLayerMask("Raycast Layer", Float) = 1
		_RaycastDistanceEndValue("Raycast Distance", Float) = 2
		[HideInInspector]_ElementLayerValue("Legacy Layer Value", Float) = -1
		[HideInInspector]_InvertX("Legacy Invert Mode", Float) = 0
		[HideInInspector]_ElementFadeSupport("Legacy Edge Fading", Float) = 0
		[HideInInspector]_element_direction_mode("_element_direction_mode", Vector) = (0,0,0,0)
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
			ColorMask [_render_colormask]
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _ElementDirectionForwardMessage;
			uniform half _ElementDirectionTextureMessage;
			uniform half _ElementDirectionTranslateMessage;
			uniform half _ElementDirectionVelocityMessage;
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
			uniform half _IsVersion;
			uniform half _IsElementShader;
			uniform half _IsMotionElement;
			uniform half _ElementLayerMask;
			uniform half4 _element_direction_mode;
			uniform sampler2D _MainTex;
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
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform float _ElementVolumeFadeMode;
			uniform half _ElementMotionMode;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsMotionAdvanced)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsMotionAdvanced
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsMotionAdvanced)
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

				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.color;
				o.ase_texcoord2 = v.ase_texcoord1;
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
				half2 Direction_Transform1406_g25297 = (( mul( unity_ObjectToWorld, float4( float3(0,0,1) , 0.0 ) ).xyz / ase_objectScale )).xz;
				half4 MainTex_RGBA587_g25297 = tex2D( _MainTex, ( 1.0 - i.ase_texcoord1.xy ) );
				float3 temp_cast_2 = (0.0001).xxx;
				float3 temp_cast_3 = (0.9999).xxx;
				float3 clampResult17_g25314 = clamp( (MainTex_RGBA587_g25297).rgb , temp_cast_2 , temp_cast_3 );
				float temp_output_7_0_g25315 = _MainTexColorMinValue;
				float3 temp_cast_4 = (temp_output_7_0_g25315).xxx;
				float temp_output_10_0_g25315 = ( _MainTexColorMaxValue - temp_output_7_0_g25315 );
				float3 temp_output_1765_0_g25297 = saturate( ( ( clampResult17_g25314 - temp_cast_4 ) / temp_output_10_0_g25315 ) );
				half Element_Remap_R73_g25297 = (temp_output_1765_0_g25297).x;
				half Element_Remap_G265_g25297 = (temp_output_1765_0_g25297).y;
				float3 appendResult274_g25297 = (float3((Element_Remap_R73_g25297*2.0 + -1.0) , 0.0 , (Element_Remap_G265_g25297*2.0 + -1.0)));
				float3 break281_g25297 = ( mul( unity_ObjectToWorld, float4( appendResult274_g25297 , 0.0 ) ).xyz / ase_objectScale );
				float2 appendResult1403_g25297 = (float2(break281_g25297.x , break281_g25297.z));
				half2 Direction_Texture284_g25297 = appendResult1403_g25297;
				float2 appendResult1404_g25297 = (float2(i.ase_color.r , i.ase_color.g));
				half2 Direction_VertexColor1150_g25297 = (appendResult1404_g25297*2.0 + -1.0);
				float2 appendResult1382_g25297 = (float2(i.ase_texcoord1.z , i.ase_texcoord2.x));
				half2 Direction_Velocity1394_g25297 = ( appendResult1382_g25297 / i.ase_texcoord2.y );
				float2 temp_output_1452_0_g25297 = ( ( _element_direction_mode.x * Direction_Transform1406_g25297 ) + ( _element_direction_mode.y * Direction_Texture284_g25297 ) + ( _element_direction_mode.z * Direction_VertexColor1150_g25297 ) + ( _element_direction_mode.w * Direction_Velocity1394_g25297 ) );
				half Element_InvertMode489_g25297 = _ElementInvertMode;
				float2 lerpResult1468_g25297 = lerp( temp_output_1452_0_g25297 , -temp_output_1452_0_g25297 , Element_InvertMode489_g25297);
				half2 Direction_Advanced1454_g25297 = lerpResult1468_g25297;
				half Noise_Scale892_g25297 = _NoiseScaleValue;
				half2 Noise_Coords1409_g25297 = ( -( (( WorldPosition - TVE_WorldOrigin )).xz * 0.1 ) * Noise_Scale892_g25297 );
				float2 temp_output_3_0_g25302 = Noise_Coords1409_g25297;
				float2 temp_output_21_0_g25302 = Direction_Advanced1454_g25297;
				float lerpResult128_g25301 = lerp( _Time.y , ( ( _Time.y * TVE_TimeParams.x ) + TVE_TimeParams.y ) , TVE_TimeParams.w);
				half Noise_Speed898_g25297 = _NoiseSpeedValue;
				half Noise_Phase1970_g25297 = _NoisePhaseValue;
				float temp_output_15_0_g25302 = (lerpResult128_g25301*Noise_Speed898_g25297 + Noise_Phase1970_g25297);
				float temp_output_23_0_g25302 = frac( temp_output_15_0_g25302 );
				float4 lerpResult39_g25302 = lerp( tex2D( _NoiseTex, ( temp_output_3_0_g25302 + ( temp_output_21_0_g25302 * temp_output_23_0_g25302 ) ) ) , tex2D( _NoiseTex, ( temp_output_3_0_g25302 + ( temp_output_21_0_g25302 * frac( ( temp_output_15_0_g25302 + 0.5 ) ) ) ) ) , ( abs( ( temp_output_23_0_g25302 - 0.5 ) ) / 0.5 ));
				half Noise_Min893_g25297 = _NoiseMinValue;
				float temp_output_7_0_g25304 = Noise_Min893_g25297;
				float4 temp_cast_7 = (temp_output_7_0_g25304).xxxx;
				half Noise_Max894_g25297 = _NoiseMaxValue;
				float temp_output_10_0_g25304 = ( Noise_Max894_g25297 - temp_output_7_0_g25304 );
				half2 Noise_Advanced1427_g25297 = (saturate( ( ( lerpResult39_g25302 - temp_cast_7 ) / temp_output_10_0_g25304 ) )).rg;
				half Noise_Intensity965_g25297 = _NoiseIntensityValue;
				float2 lerpResult1435_g25297 = lerp( (Direction_Advanced1454_g25297*0.5 + 0.5) , Noise_Advanced1427_g25297 , Noise_Intensity965_g25297);
				half Motion_Power1000_g25297 = _MotionPower;
				float3 appendResult1436_g25297 = (float3(saturate( lerpResult1435_g25297 ) , Motion_Power1000_g25297));
				half3 Final_MotionAdvanced_RGB1438_g25297 = appendResult1436_g25297;
				float clampResult17_g25311 = clamp( (MainTex_RGBA587_g25297).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g25312 = _MainTexAlphaMinValue;
				float temp_output_10_0_g25312 = ( _MainTexAlphaMaxValue - temp_output_7_0_g25312 );
				half Element_Remap_A74_g25297 = saturate( ( ( clampResult17_g25311 - temp_output_7_0_g25312 ) / ( temp_output_10_0_g25312 + 0.0001 ) ) );
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_A1737_g25297 = _ElementParams_Instance.w;
				half Element_Falloff1883_g25297 = 1.0;
				half4 Colors37_g25501 = TVE_ColorsCoords;
				half4 Extras37_g25501 = TVE_ExtrasCoords;
				half4 Motion37_g25501 = TVE_MotionCoords;
				half4 Vertex37_g25501 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g25501 = IS_ELEMENT( Colors37_g25501 , Extras37_g25501 , Motion37_g25501 , Vertex37_g25501 );
				float4 temp_output_35_0_g25503 = localIS_ELEMENT37_g25501;
				float temp_output_7_0_g25504 = TVE_ElementsFadeValue;
				float2 temp_cast_8 = (temp_output_7_0_g25504).xx;
				float temp_output_10_0_g25504 = ( 1.0 - temp_output_7_0_g25504 );
				float2 temp_output_16_0_g25500 = saturate( ( ( abs( (( (temp_output_35_0_g25503).zw + ( (temp_output_35_0_g25503).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_8 ) / temp_output_10_0_g25504 ) );
				float2 temp_output_12_0_g25500 = ( temp_output_16_0_g25500 * temp_output_16_0_g25500 );
				float lerpResult18_g25500 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g25500).x + (temp_output_12_0_g25500).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g25297 = ( _ElementIntensity * Element_Remap_A74_g25297 * Element_Params_A1737_g25297 * i.ase_color.a * Element_Falloff1883_g25297 * lerpResult18_g25500 );
				half Final_MotionAdvanced_A1439_g25297 = Element_Alpha56_g25297;
				float4 appendResult1463_g25297 = (float4(Final_MotionAdvanced_RGB1438_g25297 , Final_MotionAdvanced_A1439_g25297));
				half Element_EffectMotion946_g25297 = _ElementMotionMode;
				
				
				finalColor = ( appendResult1463_g25297 + ( Element_EffectMotion946_g25297 * 0.0 ) );
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _ElementDirectionForwardMessage;
			uniform half _ElementDirectionTextureMessage;
			uniform half _ElementDirectionTranslateMessage;
			uniform half _ElementDirectionVelocityMessage;
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
			uniform half _IsVersion;
			uniform half _IsElementShader;
			uniform half _IsMotionElement;
			uniform half _ElementLayerMask;
			uniform half4 _element_direction_mode;
			uniform sampler2D _MainTex;
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
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform float _ElementVolumeFadeMode;
			UNITY_INSTANCING_BUFFER_START(BOXOPHOBICTheVegetationEngineElementsMotionAdvanced)
				UNITY_DEFINE_INSTANCED_PROP(half4, _ElementParams)
#define _ElementParams_arr BOXOPHOBICTheVegetationEngineElementsMotionAdvanced
			UNITY_INSTANCING_BUFFER_END(BOXOPHOBICTheVegetationEngineElementsMotionAdvanced)
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

				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.color;
				o.ase_texcoord2 = v.ase_texcoord1;
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
				float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
				half2 Direction_Transform1406_g25297 = (( mul( unity_ObjectToWorld, float4( float3(0,0,1) , 0.0 ) ).xyz / ase_objectScale )).xz;
				half4 MainTex_RGBA587_g25297 = tex2D( _MainTex, ( 1.0 - i.ase_texcoord1.xy ) );
				float3 temp_cast_3 = (0.0001).xxx;
				float3 temp_cast_4 = (0.9999).xxx;
				float3 clampResult17_g25314 = clamp( (MainTex_RGBA587_g25297).rgb , temp_cast_3 , temp_cast_4 );
				float temp_output_7_0_g25315 = _MainTexColorMinValue;
				float3 temp_cast_5 = (temp_output_7_0_g25315).xxx;
				float temp_output_10_0_g25315 = ( _MainTexColorMaxValue - temp_output_7_0_g25315 );
				float3 temp_output_1765_0_g25297 = saturate( ( ( clampResult17_g25314 - temp_cast_5 ) / temp_output_10_0_g25315 ) );
				half Element_Remap_R73_g25297 = (temp_output_1765_0_g25297).x;
				half Element_Remap_G265_g25297 = (temp_output_1765_0_g25297).y;
				float3 appendResult274_g25297 = (float3((Element_Remap_R73_g25297*2.0 + -1.0) , 0.0 , (Element_Remap_G265_g25297*2.0 + -1.0)));
				float3 break281_g25297 = ( mul( unity_ObjectToWorld, float4( appendResult274_g25297 , 0.0 ) ).xyz / ase_objectScale );
				float2 appendResult1403_g25297 = (float2(break281_g25297.x , break281_g25297.z));
				half2 Direction_Texture284_g25297 = appendResult1403_g25297;
				float2 appendResult1404_g25297 = (float2(i.ase_color.r , i.ase_color.g));
				half2 Direction_VertexColor1150_g25297 = (appendResult1404_g25297*2.0 + -1.0);
				float2 appendResult1382_g25297 = (float2(i.ase_texcoord1.z , i.ase_texcoord2.x));
				half2 Direction_Velocity1394_g25297 = ( appendResult1382_g25297 / i.ase_texcoord2.y );
				float2 temp_output_1452_0_g25297 = ( ( _element_direction_mode.x * Direction_Transform1406_g25297 ) + ( _element_direction_mode.y * Direction_Texture284_g25297 ) + ( _element_direction_mode.z * Direction_VertexColor1150_g25297 ) + ( _element_direction_mode.w * Direction_Velocity1394_g25297 ) );
				half Element_InvertMode489_g25297 = _ElementInvertMode;
				float2 lerpResult1468_g25297 = lerp( temp_output_1452_0_g25297 , -temp_output_1452_0_g25297 , Element_InvertMode489_g25297);
				half2 Direction_Advanced1454_g25297 = lerpResult1468_g25297;
				half Noise_Scale892_g25297 = _NoiseScaleValue;
				half2 Noise_Coords1409_g25297 = ( -( (( WorldPosition - TVE_WorldOrigin )).xz * 0.1 ) * Noise_Scale892_g25297 );
				float2 temp_output_3_0_g25302 = Noise_Coords1409_g25297;
				float2 temp_output_21_0_g25302 = Direction_Advanced1454_g25297;
				float lerpResult128_g25301 = lerp( _Time.y , ( ( _Time.y * TVE_TimeParams.x ) + TVE_TimeParams.y ) , TVE_TimeParams.w);
				half Noise_Speed898_g25297 = _NoiseSpeedValue;
				half Noise_Phase1970_g25297 = _NoisePhaseValue;
				float temp_output_15_0_g25302 = (lerpResult128_g25301*Noise_Speed898_g25297 + Noise_Phase1970_g25297);
				float temp_output_23_0_g25302 = frac( temp_output_15_0_g25302 );
				float4 lerpResult39_g25302 = lerp( tex2D( _NoiseTex, ( temp_output_3_0_g25302 + ( temp_output_21_0_g25302 * temp_output_23_0_g25302 ) ) ) , tex2D( _NoiseTex, ( temp_output_3_0_g25302 + ( temp_output_21_0_g25302 * frac( ( temp_output_15_0_g25302 + 0.5 ) ) ) ) ) , ( abs( ( temp_output_23_0_g25302 - 0.5 ) ) / 0.5 ));
				half Noise_Min893_g25297 = _NoiseMinValue;
				float temp_output_7_0_g25304 = Noise_Min893_g25297;
				float4 temp_cast_8 = (temp_output_7_0_g25304).xxxx;
				half Noise_Max894_g25297 = _NoiseMaxValue;
				float temp_output_10_0_g25304 = ( Noise_Max894_g25297 - temp_output_7_0_g25304 );
				half2 Noise_Advanced1427_g25297 = (saturate( ( ( lerpResult39_g25302 - temp_cast_8 ) / temp_output_10_0_g25304 ) )).rg;
				half Noise_Intensity965_g25297 = _NoiseIntensityValue;
				float2 lerpResult1435_g25297 = lerp( (Direction_Advanced1454_g25297*0.5 + 0.5) , Noise_Advanced1427_g25297 , Noise_Intensity965_g25297);
				half Motion_Power1000_g25297 = _MotionPower;
				float3 appendResult1436_g25297 = (float3(saturate( lerpResult1435_g25297 ) , Motion_Power1000_g25297));
				half3 Final_MotionAdvanced_RGB1438_g25297 = appendResult1436_g25297;
				half3 Input_Color94_g25414 = Final_MotionAdvanced_RGB1438_g25297;
				half3 Element_Valid47_g25414 = ( Input_Color94_g25414 * Input_Color94_g25414 );
				half4 Colors37_g25426 = TVE_ColorsPosition;
				half4 Extras37_g25426 = TVE_ExtrasPosition;
				half4 Motion37_g25426 = TVE_MotionPosition;
				half4 Vertex37_g25426 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g25426 = IS_ELEMENT( Colors37_g25426 , Extras37_g25426 , Motion37_g25426 , Vertex37_g25426 );
				half4 Colors37_g25427 = TVE_ColorsScale;
				half4 Extras37_g25427 = TVE_ExtrasScale;
				half4 Motion37_g25427 = TVE_MotionScale;
				half4 Vertex37_g25427 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g25427 = IS_ELEMENT( Colors37_g25427 , Extras37_g25427 , Motion37_g25427 , Vertex37_g25427 );
				half Bounds42_g25414 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g25426).xyz ) ) - ( (localIS_ELEMENT37_g25427).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				float3 lerpResult50_g25414 = lerp( temp_cast_0 , Element_Valid47_g25414 , Bounds42_g25414);
				half CrossHatch44_g25414 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				float clampResult17_g25311 = clamp( (MainTex_RGBA587_g25297).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g25312 = _MainTexAlphaMinValue;
				float temp_output_10_0_g25312 = ( _MainTexAlphaMaxValue - temp_output_7_0_g25312 );
				half Element_Remap_A74_g25297 = saturate( ( ( clampResult17_g25311 - temp_output_7_0_g25312 ) / ( temp_output_10_0_g25312 + 0.0001 ) ) );
				half4 _ElementParams_Instance = UNITY_ACCESS_INSTANCED_PROP(_ElementParams_arr, _ElementParams);
				half Element_Params_A1737_g25297 = _ElementParams_Instance.w;
				half Element_Falloff1883_g25297 = 1.0;
				half4 Colors37_g25501 = TVE_ColorsCoords;
				half4 Extras37_g25501 = TVE_ExtrasCoords;
				half4 Motion37_g25501 = TVE_MotionCoords;
				half4 Vertex37_g25501 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g25501 = IS_ELEMENT( Colors37_g25501 , Extras37_g25501 , Motion37_g25501 , Vertex37_g25501 );
				float4 temp_output_35_0_g25503 = localIS_ELEMENT37_g25501;
				float temp_output_7_0_g25504 = TVE_ElementsFadeValue;
				float2 temp_cast_9 = (temp_output_7_0_g25504).xx;
				float temp_output_10_0_g25504 = ( 1.0 - temp_output_7_0_g25504 );
				float2 temp_output_16_0_g25500 = saturate( ( ( abs( (( (temp_output_35_0_g25503).zw + ( (temp_output_35_0_g25503).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_9 ) / temp_output_10_0_g25504 ) );
				float2 temp_output_12_0_g25500 = ( temp_output_16_0_g25500 * temp_output_16_0_g25500 );
				float lerpResult18_g25500 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g25500).x + (temp_output_12_0_g25500).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha56_g25297 = ( _ElementIntensity * Element_Remap_A74_g25297 * Element_Params_A1737_g25297 * i.ase_color.a * Element_Falloff1883_g25297 * lerpResult18_g25500 );
				half Final_MotionAdvanced_A1439_g25297 = Element_Alpha56_g25297;
				half Input_Alpha48_g25414 = Final_MotionAdvanced_A1439_g25297;
				float lerpResult57_g25414 = lerp( CrossHatch44_g25414 , Input_Alpha48_g25414 , Bounds42_g25414);
				float4 appendResult58_g25414 = (float4(lerpResult50_g25414 , lerpResult57_g25414));
				
				
				finalColor = appendResult58_g25414;
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
Node;AmplifyShaderEditor.FunctionNode;416;-64,-1280;Inherit;False;Compile All Elements;-1;;25296;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;440;-640,-1280;Half;False;Property;_ElementDirectionForwardMessage;Element Direction Message;1;0;Create;False;0;0;0;True;1;StyledMessage(Info, The Element Forward mode is setting the direction in the element transform forward axis. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 10, 0, 0);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;441;-640,-1216;Half;False;Property;_ElementDirectionTextureMessage;Element Direction Message;0;0;Create;False;0;0;0;True;1;StyledMessage(Info, The Element Texture mode is setting the direction based on the Element Texture__ where RG is used as World XZ direction. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 20, 0, 0);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;442;-640,-1152;Half;False;Property;_ElementDirectionTranslateMessage;Element Direction Message;2;0;Create;False;0;0;0;True;1;StyledMessage(Info, The Particle Translate mode is setting the direction based on the particle gameobject transform movement direction. Use the Speed Treshold to control how fast the particle movement is transformend into interaction. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 30, 0, 0);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;439;-640,-1088;Half;False;Property;_ElementDirectionVelocityMessage;Element Direction Message;3;0;Create;False;0;0;0;True;1;StyledMessage(Info, The Particle Velocity mode is setting the direction based on the particles motion direction. This option requires the particle to have custom vertex streams for Velocity and Speed set after the UV stream under the particle Renderer module. Element Texture A and Particle Color A are used as alpha masks., _ElementDirectionMode, 40, 0, 0);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;197;-640,-960;Half;False;Property;_render_colormask;_render_colormask;84;1;[HideInInspector];Create;True;0;0;0;True;0;False;15;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;447;-384,-768;Inherit;False;Element Shader;14;;25297;0e972c73cae2ee54ea51acc9738801d0;12,477,2,1778,2,478,0,1824,0,145,3,1814,3,1784,2,481,2,1935,1,1933,1,1904,0,1907,0;1;1974;FLOAT;1;False;2;FLOAT4;0;FLOAT4;1779
Node;AmplifyShaderEditor.FunctionNode;177;-640,-768;Inherit;False;Element Base Motion;4;;25508;6eebc31017d99e84e811285e6a5d199d;0;0;1;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;417;-64,-768;Float;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Motion Advanced;f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;2;5;False;;10;False;;4;1;False;;1;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;True;True;True;True;True;True;0;True;_render_colormask;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;418;-64,-640;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
WireConnection;447;1974;177;4
WireConnection;417;0;447;0
WireConnection;418;0;447;1779
ASEEND*/
//CHKSM=8B232982BC1A9C3BE867F2FB1FE5163F8E45455E