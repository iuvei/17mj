// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SeperableBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	#pragma glsl
	
	sampler2D	_MainTex;
	float		_SizeX;
	float		_SizeY;
	float4		_BlurDir;
	float		_BlurSpread;
	float4		_ChannelWeight;
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	//Common Vertex Shader
	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f IN) : COLOR
	{		
		half2 ScreenUV = IN.uv;
		
		float2 blurDir = _BlurDir.xy;
		float2 pixelSize = float2( 1.0 / _SizeX, 1.0 / _SizeY );
		
		float4 Scene = tex2D( _MainTex, ScreenUV ) * 0.1438749;
		
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * _BlurSpread ) ) * 0.1367508;
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * 2.0 * _BlurSpread ) ) * 0.1167897;
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * 3.0 * _BlurSpread ) ) * 0.08794503;
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * 4.0 * _BlurSpread ) ) * 0.05592986;
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * 5.0 * _BlurSpread ) ) * 0.02708518;
		Scene += tex2D( _MainTex, ScreenUV + ( blurDir * pixelSize * 6.0 * _BlurSpread ) ) * 0.007124048;
		
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * _BlurSpread ) ) * 0.1367508;
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * 2.0 * _BlurSpread ) ) * 0.1167897;
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * 3.0 * _BlurSpread ) ) * 0.08794503;
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * 4.0 * _BlurSpread ) ) * 0.05592986;
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * 5.0 * _BlurSpread ) ) * 0.02708518;
		Scene += tex2D( _MainTex, ScreenUV - ( blurDir * pixelSize * 6.0 * _BlurSpread ) ) * 0.007124048;

		Scene *= _ChannelWeight;
		float final = Scene.x + Scene.y + Scene.z + Scene.w;
		
		return float4( final,0,0,0 );
	}
	ENDCG
	
	 
	Subshader {

		ZTest Off
		Cull Off
		ZWrite Off
		Fog { Mode off }
		
		//Pass 0 Blur
		Pass 
		{
			Name "Blur"
		
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} 
	}
}
