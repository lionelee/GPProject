Shader "Custom/OutLighting"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (0, 0, 0, 1)
		_AtmoColor("Atmosphere Color", Color) = (0, 0, 0, 1)
		_Size("Size", Range(0.1,1)) = 0.1
		_Falloff("Falloff",Range(0,12)) = 5
		_Tranparency("Transparency", Range(0.1,16)) = 15
	}
	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "Always" }
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color;
			uniform float4 _AtmoColor;
			uniform float _Size;

			struct vertexOutput
			{
				float4 pos:SV_POSITION;
				float3 normal:TEXCOORD0;
				float3 worldvertpos:TEXCOORD1;
				float2 texcoord:TEXCOORD2;
			};

			vertexOutput vert(appdata_base v)
			{
				vertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			float4 frag(vertexOutput i) :COLOR
			{
				float4 color = tex2D(_MainTex, i.texcoord);
				return color * _Color;

			}
			ENDCG
		}
		
		//generate externel flare
		Pass
		{
			Tags{ "LightMode" = "Always" }
			Cull Front
			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform float4 _AtmoColor;
			uniform float _Size;
			uniform float _Falloff;
			uniform float _Tranparency;

			struct vertexOutput
			{
				float4 pos:SV_POSITION;
				float3 normal:TEXCOORD0;
				float3 worldvertpos:TEXCOORD1;
			};

			vertexOutput vert(appdata_base v)
			{
				vertexOutput o;
				v.vertex.xyz += v.normal*_Size;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float4 frag(vertexOutput i) :COLOR
			{
				i.normal = normalize(i.normal);
				float3 viewdir = normalize(i.worldvertpos.xyz - _WorldSpaceCameraPos.xyz);
				float4 color = _AtmoColor;
				color.a = pow(saturate(dot(viewdir, i.normal)), _Falloff);
				color.a *= _Tranparency * dot(viewdir, i.normal);
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
