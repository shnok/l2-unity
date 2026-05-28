// 移除了深度贴图读取功能，简化主要运算逻辑
Shader "Zhanyou/Mobile/Water" {
    Properties {
		[NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Color", color) = (1, 1, 1, 0.5)
		_MainTiling ("Tiling", float) = 1
		[NoScaleOffset]_BumpTex ("Normal Map", 2D) = "bump" {}
		_BumpTiling ("Bump Tiling", float) = 1

		_Refraction ("Refraction", Range(0, 1)) = 0
		_Specular ("Specular", Range(0, 2))  = 0.5
		_Gloss ("Gloss", Range(0, 3)) = 2
		_Distortion ("Distortion", Range(0, 1)) = 0.1
		// 水深到完全不透明的深度值
		_DepthMax("Depth Max", float) = 10
		_Speed("Speed (XY Main; ZW Bump)", Vector) = (0.5, 0.5, 0.5, 0.5)
    }
    SubShader
	{
        Tags {
            "IgnoreProjector"="True"
            "Queue"="AlphaTest+55"
            "RenderType"="Transparent"
			"ForceNoShadowCasting"="True"
		}
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Lighting Off
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ WATER_DEPTH_OFF
			#include "UnityCG.cginc"
			#include "SimLightSpecular.cginc"
			sampler2D _MainTex, _BumpTex;
#if !WATER_DEPTH_OFF
				sampler2D _CameraDepthTexture;
				float _DepthMax;
#endif
			fixed4 _Color;
			half4 _Speed;
			half _Gloss, _Specular, _Refraction, _MainTiling, _BumpTiling, _Distortion;
			GLOBAL_LIGHT_INFORMATION;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};

			#if !WATER_DEPTH_OFF
				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 bumpUv0 : TEXCOORD1;
					float2 bumpUv1 : TEXCOORD2;
                    float4 tSpace0 : TEXCOORD3;
                    float4 tSpace1 : TEXCOORD4;
                    float4 tSpace2 : TEXCOORD5;
					float4 scrPos : TEXCOORD6;
					UNITY_FOG_COORDS(7)
				};
			#else
				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 bumpUv0 : TEXCOORD1;
					float2 bumpUv1 : TEXCOORD2;
                    float4 tSpace0 : TEXCOORD3;
                    float4 tSpace1 : TEXCOORD4;
                    float4 tSpace2 : TEXCOORD5;
					UNITY_FOG_COORDS(6)
				};
			#endif
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
		        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		        fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		        fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		        fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		        fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		        o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		        o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		        o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
				o.uv = (v.texcoord + _Speed.xy * _Time.r) * _MainTiling;
				float2 bumpUv = v.texcoord * _BumpTiling;
				float2 _rotator = bumpUv - float2(0.5, 0.5);
				_rotator.x = -_rotator.x;
				o.bumpUv0 = _rotator + _Speed.zw * _Time.r * _BumpTiling;
				o.bumpUv1 = bumpUv + _Speed.zw * _Time.r * _BumpTiling;
#if !WATER_DEPTH_OFF
					o.scrPos = ComputeScreenPos(o.pos);
					COMPUTE_EYEDEPTH(o.scrPos.z);
#endif
				UNITY_TRANSFER_FOG(o,o.pos); 
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half3 normal_0 = UnpackNormal(tex2D(_BumpTex, i.bumpUv0));
				half3 normal_1 = UnpackNormal(tex2D(_BumpTex, i.bumpUv1));
				half3 normal = normalize(normal_0 + normal_1);
				normal = lerp(normal, half3(0, 0, 1), _Refraction);
				float3 worldPos = float3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				// 世界法线
				fixed3 worldNormal;
				worldNormal.x = dot(i.tSpace0.xyz, normal);
				worldNormal.y = dot(i.tSpace1.xyz, normal);
				worldNormal.z = dot(i.tSpace2.xyz, normal);
				worldNormal = normalize(worldNormal);
				// 扰动主要贴图
				fixed4 col = tex2D(_MainTex, i.uv - normal.xy * _Distortion) * _Color;
				// 漫反射效果
				fixed3 lightColor = SIM_LIGHT_COLOR;
				half NtoL = max(0.0, dot(worldNormal, _DirectionalLight.xyz));
				fixed3 diffuse = col.rgb * lightColor * NtoL;
				// 高光反射效果
				half3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				half3 halfDir = normalize(_DirectionalLight.xyz + viewDir);
				half NtoHalf = max(0.0, dot(worldNormal, halfDir));
				fixed3 specular = pow(NtoHalf, _Specular * 128.0) * _DirectionalLightColor * _Gloss;
				col.rgb = diffuse + specular;
#if WATER_DEPTH_OFF 
					// 略微平均穿透数值以做出水面效果
					col.a *= 0.7;
#else
					// 计算深度穿透
					float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
					depth -= i.scrPos.z;
					col.a *= 0.2 + 0.8 * saturate(depth / _DepthMax);
#endif
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
    FallBack "Mobile/Particles/Alpha Blended"
}