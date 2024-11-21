// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Vertex Height (Offset)"
{
	Properties
	{
		[StyledMessage(Info, Use the Height Offset elements to add noise variation to the height elements., 0,0)]_ElementMessage("Element Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsVertexElement("_IsVertexElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEVertexLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[StyledCategory(Element Settings)]_CategoryElement("[ Category Element ]", Float) = 0
		[NoScaleOffset][StyledTextureSingleLine]_MainTex("Element Texture", 2D) = "white" {}
		[Space(10)][StyledVector(9)]_MainUVs("Element UVs", Vector) = (1,1,0,0)
		[StyledRemapSlider(_MainTexAlphaMinValue, _MainTexAlphaMaxValue, 0, 1)]_MainTexAlphaRemap("Element Alpha", Vector) = (0,0,0,0)
		[HideInInspector]_MainTexAlphaMinValue("Element Alpha Min", Range( 0 , 1)) = 0
		[HideInInspector]_MainTexAlphaMaxValue("Element Alpha Max", Range( 0 , 1)) = 1
		[Space(10)]_HeightOffsetValue("Height Offset", Float) = 0
		[StyledCategory(Fading Settings)]_CategoryFade("[ Category Fade ]", Float) = 0
		[StyledToggle]_ElementVolumeFadeMode("Enable Volume Fade", Float) = 0

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
			Blend One One
			AlphaToMask Off
			Cull Back
			ColorMask B
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
			#define TVE_IS_VERTEX_ELEMENT


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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half4 _MainTexAlphaRemap;
			uniform half _ElementMessage;
			uniform half _CategoryElement;
			uniform half _CategoryFade;
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsVersion;
			uniform half _IsElementShader;
			uniform half _IsVertexElement;
			uniform half _ElementLayerMask;
			uniform float _HeightOffsetValue;
			uniform sampler2D _MainTex;
			uniform half4 _MainUVs;
			uniform half _MainTexAlphaMinValue;
			uniform half _MainTexAlphaMaxValue;
			uniform float _ElementIntensity;
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform float _ElementVolumeFadeMode;
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

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
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
				half Element_Offset212 = _HeightOffsetValue;
				half4 MainTex_RGBA199 = tex2D( _MainTex, ( ( ( 1.0 - i.ase_texcoord1.xy ) * (_MainUVs).xy ) + (_MainUVs).zw ) );
				float clampResult17_g21500 = clamp( (MainTex_RGBA199).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g21501 = _MainTexAlphaMinValue;
				float temp_output_10_0_g21501 = ( _MainTexAlphaMaxValue - temp_output_7_0_g21501 );
				half Element_Remap_A209 = saturate( ( ( clampResult17_g21500 - temp_output_7_0_g21501 ) / ( temp_output_10_0_g21501 + 0.0001 ) ) );
				half4 Colors37_g21526 = TVE_ColorsCoords;
				half4 Extras37_g21526 = TVE_ExtrasCoords;
				half4 Motion37_g21526 = TVE_MotionCoords;
				half4 Vertex37_g21526 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g21526 = IS_ELEMENT( Colors37_g21526 , Extras37_g21526 , Motion37_g21526 , Vertex37_g21526 );
				float4 temp_output_35_0_g21528 = localIS_ELEMENT37_g21526;
				float temp_output_7_0_g21529 = TVE_ElementsFadeValue;
				float2 temp_cast_0 = (temp_output_7_0_g21529).xx;
				float temp_output_10_0_g21529 = ( 1.0 - temp_output_7_0_g21529 );
				float2 temp_output_16_0_g21525 = saturate( ( ( abs( (( (temp_output_35_0_g21528).zw + ( (temp_output_35_0_g21528).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_0 ) / temp_output_10_0_g21529 ) );
				float2 temp_output_12_0_g21525 = ( temp_output_16_0_g21525 * temp_output_16_0_g21525 );
				float lerpResult18_g21525 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g21525).x + (temp_output_12_0_g21525).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha182 = ( _ElementIntensity * lerpResult18_g21525 );
				float4 appendResult169 = (float4(0.0 , 0.0 , ( Element_Offset212 * Element_Remap_A209 * Element_Alpha182 ) , 0.0));
				
				
				finalColor = appendResult169;
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
			#define TVE_IS_VERTEX_ELEMENT


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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half4 _MainTexAlphaRemap;
			uniform half _ElementMessage;
			uniform half _CategoryElement;
			uniform half _CategoryFade;
			uniform half _ElementLayerMessage;
			uniform half _ElementLayerWarning;
			uniform half _CategoryRender;
			uniform half _IsVersion;
			uniform half _IsElementShader;
			uniform half _IsVertexElement;
			uniform half _ElementLayerMask;
			uniform float _HeightOffsetValue;
			uniform half4 TVE_ColorsPosition;
			uniform half4 TVE_ExtrasPosition;
			uniform half4 TVE_MotionPosition;
			uniform half4 TVE_VertexPosition;
			uniform half4 TVE_ColorsScale;
			uniform half4 TVE_ExtrasScale;
			uniform half4 TVE_MotionScale;
			uniform half4 TVE_VertexScale;
			uniform sampler2D _MainTex;
			uniform half4 _MainUVs;
			uniform half _MainTexAlphaMinValue;
			uniform half _MainTexAlphaMaxValue;
			uniform float _ElementIntensity;
			uniform half4 TVE_ColorsCoords;
			uniform half4 TVE_ExtrasCoords;
			uniform half4 TVE_MotionCoords;
			uniform half4 TVE_VertexCoords;
			uniform half TVE_ElementsFadeValue;
			uniform float _ElementVolumeFadeMode;
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

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
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
				float3 temp_cast_0 = (0.0).xxx;
				half Element_Offset212 = _HeightOffsetValue;
				float3 temp_cast_1 = (saturate( Element_Offset212 )).xxx;
				half3 Input_Color94_g21510 = temp_cast_1;
				float3 break68_g21510 = Input_Color94_g21510;
				float clampResult80_g21510 = clamp( max( max( break68_g21510.x , break68_g21510.y ) , break68_g21510.z ) , 0.1 , 10000.0 );
				float4 color185 = IsGammaSpace() ? float4(0,0.2,1,0) : float4(0,0.03310476,1,0);
				half3 Input_Tint76_g21510 = (color185).rgb;
				half3 Element_Valid47_g21510 = ( clampResult80_g21510 * Input_Tint76_g21510 );
				half4 Colors37_g21522 = TVE_ColorsPosition;
				half4 Extras37_g21522 = TVE_ExtrasPosition;
				half4 Motion37_g21522 = TVE_MotionPosition;
				half4 Vertex37_g21522 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g21522 = IS_ELEMENT( Colors37_g21522 , Extras37_g21522 , Motion37_g21522 , Vertex37_g21522 );
				half4 Colors37_g21523 = TVE_ColorsScale;
				half4 Extras37_g21523 = TVE_ExtrasScale;
				half4 Motion37_g21523 = TVE_MotionScale;
				half4 Vertex37_g21523 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g21523 = IS_ELEMENT( Colors37_g21523 , Extras37_g21523 , Motion37_g21523 , Vertex37_g21523 );
				half Bounds42_g21510 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g21522).xyz ) ) - ( (localIS_ELEMENT37_g21523).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				float3 lerpResult50_g21510 = lerp( temp_cast_0 , Element_Valid47_g21510 , Bounds42_g21510);
				half CrossHatch44_g21510 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				half4 MainTex_RGBA199 = tex2D( _MainTex, ( ( ( 1.0 - i.ase_texcoord1.xy ) * (_MainUVs).xy ) + (_MainUVs).zw ) );
				float clampResult17_g21500 = clamp( (MainTex_RGBA199).a , 0.0001 , 0.9999 );
				float temp_output_7_0_g21501 = _MainTexAlphaMinValue;
				float temp_output_10_0_g21501 = ( _MainTexAlphaMaxValue - temp_output_7_0_g21501 );
				half Element_Remap_A209 = saturate( ( ( clampResult17_g21500 - temp_output_7_0_g21501 ) / ( temp_output_10_0_g21501 + 0.0001 ) ) );
				half4 Colors37_g21526 = TVE_ColorsCoords;
				half4 Extras37_g21526 = TVE_ExtrasCoords;
				half4 Motion37_g21526 = TVE_MotionCoords;
				half4 Vertex37_g21526 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g21526 = IS_ELEMENT( Colors37_g21526 , Extras37_g21526 , Motion37_g21526 , Vertex37_g21526 );
				float4 temp_output_35_0_g21528 = localIS_ELEMENT37_g21526;
				float temp_output_7_0_g21529 = TVE_ElementsFadeValue;
				float2 temp_cast_2 = (temp_output_7_0_g21529).xx;
				float temp_output_10_0_g21529 = ( 1.0 - temp_output_7_0_g21529 );
				float2 temp_output_16_0_g21525 = saturate( ( ( abs( (( (temp_output_35_0_g21528).zw + ( (temp_output_35_0_g21528).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_2 ) / temp_output_10_0_g21529 ) );
				float2 temp_output_12_0_g21525 = ( temp_output_16_0_g21525 * temp_output_16_0_g21525 );
				float lerpResult18_g21525 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g21525).x + (temp_output_12_0_g21525).y ) ) ) , _ElementVolumeFadeMode);
				half Element_Alpha182 = ( _ElementIntensity * lerpResult18_g21525 );
				half Input_Alpha48_g21510 = ( Element_Remap_A209 * Element_Alpha182 );
				float lerpResult57_g21510 = lerp( CrossHatch44_g21510 , Input_Alpha48_g21510 , Bounds42_g21510);
				float4 appendResult58_g21510 = (float4(lerpResult50_g21510 , lerpResult57_g21510));
				
				
				finalColor = appendResult58_g21510;
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
Node;AmplifyShaderEditor.TexCoordVertexDataNode;195;-2048,-896;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;201;-1856,-688;Inherit;False;FLOAT2;2;3;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;202;-1856,-768;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;203;-2048,-768;Half;False;Property;_MainUVs;Element UVs;13;0;Create;False;0;0;0;False;2;Space(10);StyledVector(9);False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;196;-1856,-896;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;-1664,-896;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;198;-1472,-896;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-1792,-192;Half;False;Property;_MainTexAlphaMaxValue;Element Alpha Max;16;1;[HideInInspector];Create;False;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-2048,-384;Inherit;False;199;MainTex_RGBA;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-1792,-256;Half;False;Property;_MainTexAlphaMinValue;Element Alpha Min;15;1;[HideInInspector];Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;207;-2048,-256;Half;False;Property;_MainTexAlphaRemap;Element Alpha;14;0;Create;False;0;0;0;True;1;StyledRemapSlider(_MainTexAlphaMinValue, _MainTexAlphaMaxValue, 0, 1);False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;208;-1792,-384;Inherit;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;210;-1536,-384;Inherit;False;Math Clamp;-1;;21500;be0e6188e535d474483310546a0d9e78;0;1;6;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;211;-1280,-384;Inherit;False;Math Remap;-1;;21501;5eda8a2bb94ebef4ab0e43d19291cd8b;1,14,0;3;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;170;-1408,128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-944,128;Half;False;Element_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;200;-1280,-896;Inherit;True;Property;_MainTex;Element Texture;12;1;[NoScaleOffset];Create;False;0;0;0;False;1;StyledTextureSingleLine;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-944,-896;Half;False;MainTex_RGBA;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;209;-960,-384;Half;False;Element_Remap_A;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;180;640,-512;Inherit;False;Element Visuals;-1;;21510;78cf0f00cfd72824e84597f2db54a76e;2,108,1,64,1;3;59;FLOAT3;0,0,0;False;117;FLOAT;0;False;77;COLOR;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;169;640,-896;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-128,-896;Inherit;False;212;Element_Offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;187;128,-512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;213;-128,-512;Inherit;False;212;Element_Offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;-128,-384;Inherit;False;182;Element_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;217;-128,-448;Inherit;False;209;Element_Remap_A;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;218;128,-448;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;185;-128,-320;Inherit;False;Constant;_Color1;Color 1;15;0;Create;True;0;0;0;False;0;False;0,0.2,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;128,-896;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;214;-128,-832;Inherit;False;209;Element_Remap_A;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-128,-768;Inherit;False;182;Element_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-2048,512;Inherit;False;Property;_HeightOffsetValue;Height Offset;17;0;Create;False;0;0;0;False;1;Space(10);False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2048,-1280;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Height Offset elements to add noise variation to the height elements., 0,0);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-1840,-1280;Half;False;Property;_CategoryElement;[ Category Element ];11;0;Create;True;0;0;0;True;1;StyledCategory(Element Settings);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-1632,-1280;Half;False;Property;_CategoryFade;[ Category Fade ];18;0;Create;True;0;0;0;True;1;StyledCategory(Fading Settings);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;167;-1344,-1280;Inherit;False;Compile All Elements;-1;;21524;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;174;-2048,192;Inherit;False;Element Fade Volume;19;;21525;4935729172cdadd45b9b14c3fa9c1db4;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;136;-2048,128;Inherit;False;Element Base Vertex;1;;21533;9f8670cd8fdc98444822270656343d82;0;0;1;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;212;-944,512;Half;False;Element_Offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;178;1088,-512;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;177;1088,-896;Float;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Vertex Height (Offset);f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;4;1;False;;1;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;True;True;False;False;True;False;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
WireConnection;201;0;203;0
WireConnection;202;0;203;0
WireConnection;196;0;195;0
WireConnection;197;0;196;0
WireConnection;197;1;202;0
WireConnection;198;0;197;0
WireConnection;198;1;201;0
WireConnection;208;0;205;0
WireConnection;210;6;208;0
WireConnection;211;6;210;0
WireConnection;211;7;206;0
WireConnection;211;8;204;0
WireConnection;170;0;136;4
WireConnection;170;1;174;0
WireConnection;182;0;170;0
WireConnection;200;1;198;0
WireConnection;199;0;200;0
WireConnection;209;0;211;0
WireConnection;180;59;187;0
WireConnection;180;117;218;0
WireConnection;180;77;185;0
WireConnection;169;2;215;0
WireConnection;187;0;213;0
WireConnection;218;0;217;0
WireConnection;218;1;216;0
WireConnection;215;0;176;0
WireConnection;215;1;214;0
WireConnection;215;2;183;0
WireConnection;212;0;194;0
WireConnection;178;0;180;0
WireConnection;177;0;169;0
ASEEND*/
//CHKSM=B81E5D06AB5A972588804856B7FCB09B534926F8