// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/The Vegetation Engine/Elements/Vertex Orientation (Terrain)"
{
	Properties
	{
		[StyledMessage(Info, Use the Orientation Terrain elements to align the prefabs on terrain surfaces. Please note__ the Terrain Mask option need to be set to Normal on the element script for the element to get the terrain texture. The Normal texture is only available when the terrain is instanced., 0,0)]_ElementMessage("Element Message", Float) = 0
		[HideInInspector]_IsVersion("_IsVersion", Float) = 1230
		[HideInInspector]_IsElementShader("_IsElementShader", Float) = 1
		[HideInInspector]_IsVertexElement("_IsVertexElement", Float) = 1
		[StyledCategory(Render Settings)]_CategoryRender("[ Category Render ]", Float) = 0
		_ElementIntensity("Render Intensity", Range( 0 , 1)) = 1
		[StyledMessage(Info, When using a higher Layer number the Global Volume will create more render textures to render the elements. Try using fewer layers when possible., _ElementLayerMessage, 1, 10, 10)]_ElementLayerMessage("Render Layer Message", Float) = 0
		[StyledMessage(Warning, When using all layers the Global Volume will create one render texture for each layer to render the elements. Try using fewer layers when possible., _ElementLayerWarning, 1, 10, 10)]_ElementLayerWarning("Render Layer Warning", Float) = 0
		[StyledMask(TVEVertexLayers, Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8, 0, 0)]_ElementLayerMask("Render Layer", Float) = 1
		[StyledCategory(Element Settings)]_CategoryElement("[ Category Element ]", Float) = 0
		[StyledTextureSingleLine]_MainTex("Element Texture", 2D) = "linearGrey" {}
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
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaToMask Off
			Cull Back
			ColorMask RG
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
			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;
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
				float2 appendResult207 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float4 tex2DNode146 = tex2D( _MainTex, ( ( 1.0 - i.ase_texcoord1.xy ) + ( appendResult207 * 0.5 ) ) );
				float2 appendResult167 = (float2(tex2DNode146.r , tex2DNode146.b));
				float2 Terrain_Normal195 = appendResult167;
				half4 Colors37_g21530 = TVE_ColorsCoords;
				half4 Extras37_g21530 = TVE_ExtrasCoords;
				half4 Motion37_g21530 = TVE_MotionCoords;
				half4 Vertex37_g21530 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g21530 = IS_ELEMENT( Colors37_g21530 , Extras37_g21530 , Motion37_g21530 , Vertex37_g21530 );
				float4 temp_output_35_0_g21532 = localIS_ELEMENT37_g21530;
				float temp_output_7_0_g21533 = TVE_ElementsFadeValue;
				float2 temp_cast_0 = (temp_output_7_0_g21533).xx;
				float temp_output_10_0_g21533 = ( 1.0 - temp_output_7_0_g21533 );
				float2 temp_output_16_0_g21529 = saturate( ( ( abs( (( (temp_output_35_0_g21532).zw + ( (temp_output_35_0_g21532).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_0 ) / temp_output_10_0_g21533 ) );
				float2 temp_output_12_0_g21529 = ( temp_output_16_0_g21529 * temp_output_16_0_g21529 );
				float lerpResult18_g21529 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g21529).x + (temp_output_12_0_g21529).y ) ) ) , _ElementVolumeFadeMode);
				float Element_Alpha196 = ( _ElementIntensity * lerpResult18_g21529 );
				float4 appendResult188 = (float4(Terrain_Normal195 , 0.0 , Element_Alpha196));
				
				
				finalColor = appendResult188;
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
			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			uniform half4 TVE_ColorsPosition;
			uniform half4 TVE_ExtrasPosition;
			uniform half4 TVE_MotionPosition;
			uniform half4 TVE_VertexPosition;
			uniform half4 TVE_ColorsScale;
			uniform half4 TVE_ExtrasScale;
			uniform half4 TVE_MotionScale;
			uniform half4 TVE_VertexScale;
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
				float2 appendResult207 = (float2(_MainTex_TexelSize.x , _MainTex_TexelSize.y));
				float4 tex2DNode146 = tex2D( _MainTex, ( ( 1.0 - i.ase_texcoord1.xy ) + ( appendResult207 * 0.5 ) ) );
				float2 appendResult167 = (float2(tex2DNode146.r , tex2DNode146.b));
				float2 Terrain_Normal195 = appendResult167;
				half3 Input_Color94_g21501 = float3( Terrain_Normal195 ,  0.0 );
				half3 Element_Valid47_g21501 = ( Input_Color94_g21501 * Input_Color94_g21501 );
				half4 Colors37_g21513 = TVE_ColorsPosition;
				half4 Extras37_g21513 = TVE_ExtrasPosition;
				half4 Motion37_g21513 = TVE_MotionPosition;
				half4 Vertex37_g21513 = TVE_VertexPosition;
				half4 localIS_ELEMENT37_g21513 = IS_ELEMENT( Colors37_g21513 , Extras37_g21513 , Motion37_g21513 , Vertex37_g21513 );
				half4 Colors37_g21514 = TVE_ColorsScale;
				half4 Extras37_g21514 = TVE_ExtrasScale;
				half4 Motion37_g21514 = TVE_MotionScale;
				half4 Vertex37_g21514 = TVE_VertexScale;
				half4 localIS_ELEMENT37_g21514 = IS_ELEMENT( Colors37_g21514 , Extras37_g21514 , Motion37_g21514 , Vertex37_g21514 );
				half Bounds42_g21501 = ( 1.0 - saturate( ( distance( max( ( abs( ( WorldPosition - (localIS_ELEMENT37_g21513).xyz ) ) - ( (localIS_ELEMENT37_g21514).xyz * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / 0.001 ) ) );
				float3 lerpResult50_g21501 = lerp( temp_cast_0 , Element_Valid47_g21501 , Bounds42_g21501);
				half CrossHatch44_g21501 = ( saturate( ( ( abs( sin( ( ( WorldPosition.x + WorldPosition.z ) * 3.0 ) ) ) - 0.7 ) * 100.0 ) ) * 0.25 );
				half4 Colors37_g21530 = TVE_ColorsCoords;
				half4 Extras37_g21530 = TVE_ExtrasCoords;
				half4 Motion37_g21530 = TVE_MotionCoords;
				half4 Vertex37_g21530 = TVE_VertexCoords;
				half4 localIS_ELEMENT37_g21530 = IS_ELEMENT( Colors37_g21530 , Extras37_g21530 , Motion37_g21530 , Vertex37_g21530 );
				float4 temp_output_35_0_g21532 = localIS_ELEMENT37_g21530;
				float temp_output_7_0_g21533 = TVE_ElementsFadeValue;
				float2 temp_cast_2 = (temp_output_7_0_g21533).xx;
				float temp_output_10_0_g21533 = ( 1.0 - temp_output_7_0_g21533 );
				float2 temp_output_16_0_g21529 = saturate( ( ( abs( (( (temp_output_35_0_g21532).zw + ( (temp_output_35_0_g21532).xy * (WorldPosition).xz ) )*2.002 + -1.001) ) - temp_cast_2 ) / temp_output_10_0_g21533 ) );
				float2 temp_output_12_0_g21529 = ( temp_output_16_0_g21529 * temp_output_16_0_g21529 );
				float lerpResult18_g21529 = lerp( 1.0 , ( 1.0 - saturate( ( (temp_output_12_0_g21529).x + (temp_output_12_0_g21529).y ) ) ) , _ElementVolumeFadeMode);
				float Element_Alpha196 = ( _ElementIntensity * lerpResult18_g21529 );
				half Input_Alpha48_g21501 = Element_Alpha196;
				float lerpResult57_g21501 = lerp( CrossHatch44_g21501 , Input_Alpha48_g21501 , Bounds42_g21501);
				float4 appendResult58_g21501 = (float4(lerpResult50_g21501 , lerpResult57_g21501));
				
				
				finalColor = appendResult58_g21501;
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
Node;AmplifyShaderEditor.DynamicAppendNode;167;-384,-640;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;188;1022.824,-639.9997;Inherit;False;FLOAT4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;766.8244,-639.9997;Inherit;False;195;Terrain_Normal;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;766.8244,-575.9997;Inherit;False;196;Element_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;201;768,-384;Inherit;False;195;Terrain_Normal;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;202;768,-320;Inherit;False;196;Element_Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;203;1024,-384;Inherit;False;Element Visuals;-1;;21501;78cf0f00cfd72824e84597f2db54a76e;2,108,2,64,2;3;59;FLOAT3;0,0,0;False;117;FLOAT;0;False;77;COLOR;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;204;-1536,-640;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;205;-1280,-640;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexelSizeNode;206;-1536,-512;Inherit;False;146;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;207;-1280,-512;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;208;-960,-640;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-1088,-512;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-1280,-416;Half;False;Constant;_Float3;Float 3;14;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;146;-768,-640;Inherit;True;Property;_MainTex;Element Texture;12;0;Create;False;0;0;0;False;1;StyledTextureSingleLine;False;-1;None;None;True;0;False;linearGrey;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;112;-1536,-1152;Half;False;Property;_ElementMessage;Element Message;0;0;Create;True;0;0;0;True;1;StyledMessage(Info, Use the Orientation Terrain elements to align the prefabs on terrain surfaces. Please note__ the Terrain Mask option need to be set to Normal on the element script for the element to get the terrain texture. The Normal texture is only available when the terrain is instanced., 0,0);False;0;0;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-1328,-1152;Half;False;Property;_CategoryElement;[ Category Element ];11;0;Create;True;0;0;0;True;1;StyledCategory(Element Settings);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-1120,-1152;Half;False;Property;_CategoryFade;[ Category Fade ];13;0;Create;True;0;0;0;True;1;StyledCategory(Fading Settings);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;176;-832,-1152;Inherit;False;Compile All Elements;-1;;21526;5302407fc6d65554791e558e67d59358;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;136;-1536,-128;Inherit;False;Element Base Vertex;1;;21527;9f8670cd8fdc98444822270656343d82;0;0;1;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;194;-1536,-64;Inherit;False;Element Fade Volume;14;;21529;4935729172cdadd45b9b14c3fa9c1db4;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;-1280,-128;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-192,-128;Inherit;False;Element_Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-192,-640;Inherit;False;Terrain_Normal;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;199;1440,-640;Float;False;True;-1;2;TVEShaderElementGUI;100;16;BOXOPHOBIC/The Vegetation Engine/Elements/Vertex Orientation (Terrain);f4f273c3bb6262d4396be458405e60f9;True;VolumePass;0;0;VolumePass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;DisableBatching=True=DisableBatching;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;True;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;2;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;200;1472,-384;Float;False;False;-1;2;ASEMaterialInspector;100;16;New Amplify Shader;f4f273c3bb6262d4396be458405e60f9;True;VisualPass;0;1;VisualPass;2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;False;False;0;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;0;False;;True;False;0;False;;0;False;;False;True;2;False;0;;0;0;Standard;0;False;0
WireConnection;167;0;146;1
WireConnection;167;1;146;3
WireConnection;188;0;197;0
WireConnection;188;3;198;0
WireConnection;203;59;201;0
WireConnection;203;117;202;0
WireConnection;205;0;204;0
WireConnection;207;0;206;1
WireConnection;207;1;206;2
WireConnection;208;0;205;0
WireConnection;208;1;209;0
WireConnection;209;0;207;0
WireConnection;209;1;210;0
WireConnection;146;1;208;0
WireConnection;192;0;136;4
WireConnection;192;1;194;0
WireConnection;196;0;192;0
WireConnection;195;0;167;0
WireConnection;199;0;188;0
WireConnection;200;0;203;0
ASEEND*/
//CHKSM=D7792CA6299ADFC31FF0463BC6BC35362682626D