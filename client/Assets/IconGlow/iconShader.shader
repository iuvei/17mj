// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/iconShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		_GlowTex ("Glow (R)", 2D) = "black" {}
		_GlowColor ("Glow Color", Color) = (1,1,1,1)
		_GlowAlpha ("Glow Alpha", Range(0.0,5.0) ) = 1.0
	}
	SubShader {
		Tags {
			"Queue"="Overlay"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		LOD 200
		
		Pass {
			LOD 200
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _GlowTex;

			fixed4 _Color;
			fixed4 _GlowColor;
			float _GlowAlpha;
			
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert ( appdata_full v ) {
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv =  v.texcoord.xy;
				return o;
			}

			half4 frag ( v2f IN ) : COLOR {

				half4 icon = tex2D (_MainTex, IN.uv) * _Color;
				half glow = tex2D (_GlowTex, IN.uv).x;
				glow = saturate( glow * _GlowAlpha );
				
				icon.xyz = lerp( _GlowColor.xyz, icon.xyz, icon.w );
				icon.w = saturate( icon.w + glow * _GlowColor.w );
				
				return icon;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
