// Made with Amplify Shader Editor v1.9.1.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/Default (Height Fog Support)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [StyledCategory(Fog Settings, false, _HeightFogStandalone, 10, 10)]_FogCat("[ Fog Cat]", Float) = 1
        [StyledCategory(Skybox Settings, false, _HeightFogStandalone, 10, 10)]_SkyboxCat("[ Skybox Cat ]", Float) = 1
        [StyledCategory(Directional Settings, false, _HeightFogStandalone, 10, 10)]_DirectionalCat("[ Directional Cat ]", Float) = 1
        [StyledCategory(Noise Settings, false, _HeightFogStandalone, 10, 10)]_NoiseCat("[ Noise Cat ]", Float) = 1
        [ASEEnd][StyledCategory(Advanced Settings, false, _HeightFogStandalone, 10, 10)]_AdvancedCat("[ Advanced Cat ]", Float) = 1
        [HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	CompFront [_StencilComp]
        	PassFront [_StencilOp]
        	FailFront Keep
        	ZFailFront Keep
        	CompBack Always
        	PassBack Keep
        	FailBack Keep
        	ZFailBack Keep
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"
            #define ASE_NEEDS_FRAG_COLOR
            //Atmospheric Height Fog Defines
            //#define AHF_DISABLE_NOISE3D
            //#define AHF_DISABLE_DIRECTIONAL
            //#define AHF_DISABLE_SKYBOXFOG
            //#define AHF_DISABLE_FALLOFF
            //#define AHF_DEBUG_WORLDPOS


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                float4 ase_texcoord3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform half _FogCat;
            uniform half _SkyboxCat;
            uniform half _AdvancedCat;
            uniform half _NoiseCat;
            uniform half _DirectionalCat;
            uniform half4 AHF_FogColorStart;
            uniform half4 AHF_FogColorEnd;
            uniform half AHF_FogDistanceStart;
            uniform half AHF_FogDistanceEnd;
            uniform half AHF_FogDistanceFalloff;
            uniform half AHF_FogColorDuo;
            uniform half4 AHF_DirectionalColor;
            uniform half3 AHF_DirectionalDir;
            uniform half AHF_DirectionalIntensity;
            uniform half AHF_DirectionalFalloff;
            uniform half3 AHF_FogAxisOption;
            uniform half AHF_FogHeightEnd;
            uniform half AHF_FarDistanceHeight;
            uniform float AHF_FarDistanceOffset;
            uniform half AHF_FogHeightStart;
            uniform half AHF_FogHeightFalloff;
            uniform half AHF_FogLayersMode;
            uniform half AHF_NoiseScale;
            uniform half3 AHF_NoiseSpeed;
            uniform half AHF_NoiseMin;
            uniform half AHF_NoiseMax;
            uniform half AHF_NoiseDistanceEnd;
            uniform half AHF_NoiseIntensity;
            uniform half AHF_FogIntensity;
            float4 mod289( float4 x )
            {
            	return x - floor(x * (1.0 / 289.0)) * 289.0;
            }
            
            float4 perm( float4 x )
            {
            	return mod289(((x * 34.0) + 1.0) * x);
            }
            
            float SimpleNoise3D( float3 p )
            {
            	    float3 a = floor(p);
            	    float3 d = p - a;
            	    d = d * d * (3.0 - 2.0 * d);
            	    float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
            	    float4 k1 = perm(b.xyxy);
            	    float4 k2 = perm(k1.xyxy + b.zzww);
            	    float4 c = k2 + a.zzzz;
            	    float4 k3 = perm(c);
            	    float4 k4 = perm(c + 1.0);
            	    float4 o1 = frac(k3 * (1.0 / 41.0));
            	    float4 o2 = frac(k4 * (1.0 / 41.0));
            	    float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
            	    float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
            	    return o4.y * d.y + o4.x * (1.0 - d.y);
            }
            

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
                OUT.ase_texcoord3.xyz = ase_worldPos;
                
                
                //setting value to unused interpolator channels and avoid initialization warnings
                OUT.ase_texcoord3.w = 0;

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 temp_output_4_0 = ( IN.color * ( tex2D( _MainTex, uv_MainTex ) + _TextureSampleAdd ) );
                float3 ase_worldPos = IN.ase_texcoord3.xyz;
                float3 WorldPosition2_g1022 = ase_worldPos;
                float temp_output_7_0_g1025 = AHF_FogDistanceStart;
                float temp_output_155_0_g1022 = saturate( ( ( distance( WorldPosition2_g1022 , _WorldSpaceCameraPos ) - temp_output_7_0_g1025 ) / ( AHF_FogDistanceEnd - temp_output_7_0_g1025 ) ) );
                #ifdef AHF_DISABLE_FALLOFF
                float staticSwitch467_g1022 = temp_output_155_0_g1022;
                #else
                float staticSwitch467_g1022 = ( 1.0 - pow( ( 1.0 - abs( temp_output_155_0_g1022 ) ) , AHF_FogDistanceFalloff ) );
                #endif
                half FogDistanceMask12_g1022 = staticSwitch467_g1022;
                float3 lerpResult258_g1022 = lerp( (AHF_FogColorStart).rgb , (AHF_FogColorEnd).rgb , ( ( FogDistanceMask12_g1022 * FogDistanceMask12_g1022 * FogDistanceMask12_g1022 ) * AHF_FogColorDuo ));
                float3 normalizeResult318_g1022 = normalize( ( WorldPosition2_g1022 - _WorldSpaceCameraPos ) );
                float dotResult145_g1022 = dot( normalizeResult318_g1022 , AHF_DirectionalDir );
                half Jitter502_g1022 = 0.0;
                float temp_output_140_0_g1022 = ( saturate( (( dotResult145_g1022 + Jitter502_g1022 )*0.5 + 0.5) ) * AHF_DirectionalIntensity );
                #ifdef AHF_DISABLE_FALLOFF
                float staticSwitch470_g1022 = temp_output_140_0_g1022;
                #else
                float staticSwitch470_g1022 = pow( abs( temp_output_140_0_g1022 ) , AHF_DirectionalFalloff );
                #endif
                float DirectionalMask30_g1022 = staticSwitch470_g1022;
                float3 lerpResult40_g1022 = lerp( lerpResult258_g1022 , (AHF_DirectionalColor).rgb , DirectionalMask30_g1022);
                #ifdef AHF_DISABLE_DIRECTIONAL
                float3 staticSwitch442_g1022 = lerpResult258_g1022;
                #else
                float3 staticSwitch442_g1022 = lerpResult40_g1022;
                #endif
                half3 Input_Color6_g1023 = staticSwitch442_g1022;
                #ifdef UNITY_COLORSPACE_GAMMA
                float3 staticSwitch1_g1023 = Input_Color6_g1023;
                #else
                float3 staticSwitch1_g1023 = ( Input_Color6_g1023 * ( ( Input_Color6_g1023 * ( ( Input_Color6_g1023 * 0.305306 ) + 0.6821711 ) ) + 0.01252288 ) );
                #endif
                half3 Final_Color462_g1022 = staticSwitch1_g1023;
                half3 AHF_FogAxisOption181_g1022 = AHF_FogAxisOption;
                float3 break159_g1022 = ( WorldPosition2_g1022 * AHF_FogAxisOption181_g1022 );
                float temp_output_7_0_g1026 = AHF_FogDistanceEnd;
                float temp_output_643_0_g1022 = saturate( ( ( distance( WorldPosition2_g1022 , _WorldSpaceCameraPos ) - temp_output_7_0_g1026 ) / ( ( AHF_FogDistanceEnd + AHF_FarDistanceOffset ) - temp_output_7_0_g1026 ) ) );
                half FogDistanceMaskFar645_g1022 = ( temp_output_643_0_g1022 * temp_output_643_0_g1022 );
                float lerpResult690_g1022 = lerp( AHF_FogHeightEnd , ( AHF_FogHeightEnd + AHF_FarDistanceHeight ) , FogDistanceMaskFar645_g1022);
                float temp_output_7_0_g1027 = lerpResult690_g1022;
                float temp_output_167_0_g1022 = saturate( ( ( ( break159_g1022.x + break159_g1022.y + break159_g1022.z ) - temp_output_7_0_g1027 ) / ( AHF_FogHeightStart - temp_output_7_0_g1027 ) ) );
                #ifdef AHF_DISABLE_FALLOFF
                float staticSwitch468_g1022 = temp_output_167_0_g1022;
                #else
                float staticSwitch468_g1022 = pow( abs( temp_output_167_0_g1022 ) , AHF_FogHeightFalloff );
                #endif
                half FogHeightMask16_g1022 = staticSwitch468_g1022;
                float lerpResult328_g1022 = lerp( ( FogDistanceMask12_g1022 * FogHeightMask16_g1022 ) , saturate( ( FogDistanceMask12_g1022 + FogHeightMask16_g1022 ) ) , AHF_FogLayersMode);
                float mulTime204_g1022 = _Time.y * 2.0;
                float3 temp_output_197_0_g1022 = ( ( WorldPosition2_g1022 * ( 1.0 / AHF_NoiseScale ) ) + ( -AHF_NoiseSpeed * mulTime204_g1022 ) );
                float3 p1_g1031 = temp_output_197_0_g1022;
                float localSimpleNoise3D1_g1031 = SimpleNoise3D( p1_g1031 );
                float temp_output_7_0_g1030 = AHF_NoiseMin;
                float temp_output_7_0_g1029 = AHF_NoiseDistanceEnd;
                half NoiseDistanceMask7_g1022 = saturate( ( ( distance( WorldPosition2_g1022 , _WorldSpaceCameraPos ) - temp_output_7_0_g1029 ) / ( 0.0 - temp_output_7_0_g1029 ) ) );
                float lerpResult198_g1022 = lerp( 1.0 , saturate( ( ( localSimpleNoise3D1_g1031 - temp_output_7_0_g1030 ) / ( AHF_NoiseMax - temp_output_7_0_g1030 ) ) ) , ( NoiseDistanceMask7_g1022 * AHF_NoiseIntensity ));
                half NoiseSimplex3D24_g1022 = lerpResult198_g1022;
                #ifdef AHF_DISABLE_NOISE3D
                float staticSwitch42_g1022 = lerpResult328_g1022;
                #else
                float staticSwitch42_g1022 = ( lerpResult328_g1022 * NoiseSimplex3D24_g1022 );
                #endif
                float temp_output_454_0_g1022 = ( staticSwitch42_g1022 * AHF_FogIntensity );
                half Final_Alpha463_g1022 = temp_output_454_0_g1022;
                float4 appendResult114_g1022 = (float4(Final_Color462_g1022 , Final_Alpha463_g1022));
                float4 appendResult457_g1022 = (float4(WorldPosition2_g1022 , 1.0));
                #ifdef AHF_DEBUG_WORLDPOS
                float4 staticSwitch456_g1022 = appendResult457_g1022;
                #else
                float4 staticSwitch456_g1022 = appendResult114_g1022;
                #endif
                float3 temp_output_96_86_g930 = (staticSwitch456_g1022).xyz;
                float temp_output_96_87_g930 = (staticSwitch456_g1022).w;
                float3 lerpResult82_g930 = lerp( (temp_output_4_0).rgb , temp_output_96_86_g930 , temp_output_96_87_g930);
                float4 appendResult9 = (float4(lerpResult82_g930 , (temp_output_4_0).a));
                

                half4 color = appendResult9;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    
	
	Fallback Off
}
/*ASEBEGIN
Version=19109
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;2;-512,0;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-320,0;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;11;-320,192;Inherit;False;0;0;_TextureSampleAdd;Pass;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;12;-512,-256;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;64,64;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;256,-256;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;6;448,-256;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;7;448,-160;Inherit;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;26;640,-256;Inherit;False;Apply Height Fog Unlit;0;;930;950890317d4f36a48a68d150cdab0168;0;1;81;FLOAT3;0,0,0;False;3;FLOAT3;85;FLOAT3;86;FLOAT;87
Node;AmplifyShaderEditor.DynamicAppendNode;9;896,-256;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1088,-256;Float;False;True;-1;2;;0;3;UI/Default (Height Fog Support);5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;3;0;2;0
WireConnection;10;0;3;0
WireConnection;10;1;11;0
WireConnection;4;0;12;0
WireConnection;4;1;10;0
WireConnection;6;0;4;0
WireConnection;7;0;4;0
WireConnection;26;81;6;0
WireConnection;9;0;26;85
WireConnection;9;3;7;0
WireConnection;1;0;9;0
ASEEND*/
//CHKSM=67A5E8F632ADB9659F380426B6515FB53F882AE0