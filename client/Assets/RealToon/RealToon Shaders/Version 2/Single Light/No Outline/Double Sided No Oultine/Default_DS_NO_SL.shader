// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//RealToon V2.0.0 Shader
//Made By MJQStudioWorks
//Made Using Shader Forge
//2017

Shader "RealToon/Version 2/Single Light/No Outline/Double Sided/Default" {
    Properties {
        [Header((RealToon V2.0.0))]
    	[Header((Single Light x No Outline x Double Sided))]
    	[Header((Default))]

        [Space(20)][Header((Texture Color))]_Texture ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.8014706,0.8014706,0.8014706,1)
        [MaterialToggle] _Transparent ("Transparent", Float ) = 0

        [Space(20)][Header((NormalMap))][Normal]_NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Float ) = 1

        [Space(20)][Header((Color Adjustment))]_Saturation ("Saturation", Range(0, 2)) = 1
        _ReduceWhite ("Reduce White", Range(0, 1)) = 0

        [Space(20)][Header((Self Lit))]_SelfLitIntensity ("Self Lit Intensity", Range(0, 1)) = 0
        _SelfLitPower ("Self Lit Power", Float ) = 1
        _SelfLitColor ("Self Lit Color", Color) = (1,1,1,1)
        _MaskSelfLit ("Mask Self Lit", 2D) = "white" {}

        [Space(20)][Header((Gloss))]_GlossIntensity ("Gloss Intensity", Float ) = 0
        _Glossiness ("Glossiness", Range(0, 1)) = 0.5
        _GlossColor ("Gloss Color", Color) = (1,1,1,1)
        [MaterialToggle] _SoftGloss ("Soft Gloss", Float ) = 0
        _GlossMask ("Gloss Mask", 2D) = "white" {}

        [Space(20)][Header((Shadow))]_ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 1
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        [MaterialToggle] _MainTextureColorShadow ("Main Texture Color Shadow", Float ) = 0
        _ReceiveShadowsIntensity ("Receive Shadows Intensity", Range(0, 1)) = 1
        _ShadowPTextureIntensity ("Shadow PTexture Intensity", Range(0, 1)) = 0
        _ShadowPTexture ("Shadow PTexture", 2D) = "black" {}

        [Space(20)][Header((Self Shadow))]_SelfShadowIntensity ("Self Shadow Intensity", Range(0, 1)) = 1
        _SelfShadowSize ("Self Shadow Size", Range(0, 1)) = 0.56
        _SelfShadowHardness ("Self Shadow Hardness", Range(0, 1)) = 1
        [MaterialToggle] _SelfShadowatViewDirection ("Self Shadow at View Direction", Float ) = 0

        [Space(20)][Header((AO))]_AOIntensity ("AO Intensity", Range(0, 1)) = 0
        _AOTexture ("AO Texture", 2D) = "white" {}
        [MaterialToggle] _MainTextureColorAO ("Main Texture Color AO", Float ) = 0
        [MaterialToggle] _ShowAOonLight ("Show AO on Light", Float ) = 0
        [MaterialToggle] _ShowAOonAmbientLight ("Show AO on Ambient Light", Float ) = 1

        [Space(20)][Header((FReflection))]_FReflectionIntensity ("FReflection Intensity", Range(0, 1)) = 0
        _FReflection ("FReflection", 2D) = "white" {}
        _MaskFReflection ("Mask FReflection", 2D) = "white" {}

        [Space(20)][Header((Fresnel))]_FresnelIntensity ("Fresnel Intensity", Range(0, 1)) = 0
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelFill ("Fresnel Fill", Float ) = 1
        [MaterialToggle] _HardEdgeFresnel ("Hard Edge Fresnel", Float ) = 0
        [MaterialToggle] _FresnelVisibleOnDarkAmbientLight ("Fresnel Visible On Dark/Ambient Light", Float ) = 0
        [MaterialToggle] _FresnelOnLight ("Fresnel On Light", Float ) = 0
        [MaterialToggle] _FresnelOnShadow ("Fresnel On Shadow", Float ) = 0
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _Color;
            uniform float _Glossiness;
            uniform float4 _GlossColor;
            uniform float4 _ShadowColor;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _SelfShadowHardness;
            uniform float _NormalIntensity;
            uniform float _Saturation;
            uniform float _SelfShadowIntensity;
            uniform sampler2D _MaskSelfLit; uniform float4 _MaskSelfLit_ST;
            uniform sampler2D _FReflection; uniform float4 _FReflection_ST;
            uniform sampler2D _MaskFReflection; uniform float4 _MaskFReflection_ST;
            uniform float _FReflectionIntensity;
            uniform sampler2D _GlossMask; uniform float4 _GlossMask_ST;
            uniform fixed _SoftGloss;
            uniform float _SelfLitIntensity;
            uniform fixed _Transparent;
            uniform float _SelfShadowSize;
            uniform float _GlossIntensity;
            uniform float4 _SelfLitColor;
            uniform float _SelfLitPower;
            uniform float _ReduceWhite;
            uniform float _FresnelFill;
            uniform float4 _FresnelColor;
            uniform float _FresnelIntensity;
            uniform fixed _HardEdgeFresnel;
            uniform sampler2D _ShadowPTexture; uniform float4 _ShadowPTexture_ST;
            uniform float _ShadowPTextureIntensity;
            uniform fixed _SelfShadowatViewDirection;
            uniform float _ReceiveShadowsIntensity;
            uniform sampler2D _AOTexture; uniform float4 _AOTexture_ST;
            uniform float _AOIntensity;
            uniform fixed _ShowAOonLight;
            uniform fixed _ShowAOonAmbientLight;
            uniform float _ShadowIntensity;
            uniform fixed _MainTextureColorShadow;
            uniform fixed _MainTextureColorAO;
            uniform fixed _FresnelVisibleOnDarkAmbientLight;
            uniform fixed _FresnelOnLight;
            uniform fixed _FresnelOnShadow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform ));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                clip(lerp( 1.0, _Texture_var.a, _Transparent ) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float node_6003 = pow(1.0-max(0,dot(normalDirection, viewDirection)),exp2((1.0 - _FresnelFill)));
                float _HardEdgeFresnel_var = lerp( node_6003, smoothstep( 0.38, 0.4, node_6003 ), _HardEdgeFresnel );
                float node_5638 = 1.0;
                float node_9545 = smoothstep( lerp(0.3,0.899,_SelfShadowHardness), 0.9, saturate((0.5*dot(lerp( lightDirection, viewDirection, _SelfShadowatViewDirection ),normalDirection)+0.5*lerp(2.8,0.79,_SelfShadowSize))) ); 
                float node_3614 = (attenuation*node_9545);
                float node_4087 = lerp(0.0,_HardEdgeFresnel_var,(lerp( node_5638, node_3614, _FresnelOnLight )*lerp( node_5638, (1.0 - node_3614), _FresnelOnShadow )));
                float node_5221 = (_HardEdgeFresnel_var*node_4087);
                float3 node_137 = ((node_5221*_FresnelColor.rgb)*_FresnelIntensity);
                float3 node_456 = (0.0*node_137);
                float3 node_4607 = (_Texture_var.rgb*_Color.rgb);
                float3 node_7134 = (float3(i.screenPos.rg,0.0)+normalDirection);
                float4 _FReflection_var = tex2D(_FReflection,TRANSFORM_TEX(node_7134, _FReflection));
                float4 _MaskFReflection_var = tex2D(_MaskFReflection,TRANSFORM_TEX(i.uv0, _MaskFReflection));
                float3 node_3766 = lerp(node_4607,lerp(_FReflection_var.rgb,node_4607,(1.0 - _MaskFReflection_var.rgb)),_FReflectionIntensity); 
                float node_8557 = 1.0;
                float node_6822 = 0.0;
                float node_2585 = 1.0;
                float4 _AOTexture_var = tex2D(_AOTexture,TRANSFORM_TEX(i.uv0, _AOTexture));
                float3 node_3573 = lerp(float3(node_8557,node_8557,node_8557),lerp(lerp(float3(node_6822,node_6822,node_6822),(_Texture_var.rgb*0.9),_MainTextureColorAO),float3(node_2585,node_2585,node_2585),lerp(_AOTexture_var.rgb,dot(_AOTexture_var.rgb,float3(0.3,0.59,0.11)),1.0)),lerp(0.0,lerp(1.0,10.0,_MainTextureColorAO),_AOIntensity));
                float3 node_4224 = (node_3766*UNITY_LIGHTMODEL_AMBIENT.rgb*lerp( 1.0, node_3573, _ShowAOonAmbientLight ));
                float4 _MaskSelfLit_var = tex2D(_MaskSelfLit,TRANSFORM_TEX(i.uv0, _MaskSelfLit));
                float node_5038 = (1.0 - _ReduceWhite);
                float node_3161 = (1.0 - _Saturation);
                float3 emissive = lerp(saturate(min((lerp(node_456,node_137,_FresnelVisibleOnDarkAmbientLight)+lerp(node_4224,lerp(node_4224,(_SelfLitColor.rgb*node_3766*_SelfLitPower),_MaskSelfLit_var.rgb),lerp(0.0,1.0,_SelfLitIntensity))),node_5038)),dot(saturate(min((lerp(node_456,node_137,_FresnelVisibleOnDarkAmbientLight)+lerp(node_4224,lerp(node_4224,(_SelfLitColor.rgb*node_3766*_SelfLitPower),_MaskSelfLit_var.rgb),lerp(0.0,1.0,_SelfLitIntensity))),node_5038)),float3(0.3,0.59,0.11)),node_3161);
                float node_9036 = (lerp(1.0,attenuation,_ReceiveShadowsIntensity)*lerp(1.0,node_9545,_SelfShadowIntensity));
                float node_505 = lerp(1.0,node_9036,lerp(0.0,lerp(1.0,2.6,_MainTextureColorShadow),_ShadowIntensity));
                float node_6054 = (1.0 - node_505);
                float node_988 = 0.0;
                float node_9082 = 0.0;
                float node_557 = 1.0;
                float4 _ShadowPTexture_var = tex2D(_ShadowPTexture,TRANSFORM_TEX(float2(i.screenPos.x*(_ScreenParams.r/_ScreenParams.g), i.screenPos.y).rg, _ShadowPTexture));
                float node_3933 = 0.0;
                float node_356 = 1.0;
                float node_7287 = 0.0;
                float node_9744 = pow(max(0,dot(normalDirection,halfDirection)),exp2(lerp(-2,15,_Glossiness)));
                float _SoftGloss_var = lerp( smoothstep( 0.79, 0.9, (node_9744*3.0) ), node_9744, _SoftGloss );
                float4 _GlossMask_var = tex2D(_GlossMask,TRANSFORM_TEX(i.uv0, _GlossMask));
                float node_2583 = 0.0;
                float node_8388_if_leA = step(_GlossIntensity,node_2583);
                float node_8388_if_leB = step(node_2583,_GlossIntensity);
                float node_4309 = 0.0;
                float3 finalColor = emissive + lerp(saturate(min(((((node_3766*(lerp(float3(node_505,node_505,node_505),lerp(lerp((node_6054*_ShadowColor.rgb),lerp(float3(node_988,node_988,node_988),saturate((1.0-((1.0-0.44)/_Texture_var.rgb))),node_6054),_MainTextureColorShadow),lerp(float3(node_9082,node_9082,node_9082),(lerp(lerp(float3(node_557,node_557,node_557),_ShadowPTexture_var.rgb,_ShadowPTexture_var.a),float3(node_3933,node_3933,node_3933),node_9036)*lerp(0.54,1.65,_MainTextureColorShadow)),_ShadowIntensity),_ShadowPTextureIntensity),0.65)*2.86))*lerp( lerp(node_3573,float3(node_356,node_356,node_356),node_9036), node_3573, _ShowAOonLight ))+(lerp(float3(node_7287,node_7287,node_7287),lerp(float3(_SoftGloss_var,_SoftGloss_var,_SoftGloss_var),(_SoftGloss_var*_GlossColor.rgb),2.22),_GlossMask_var.rgb)*lerp((node_8388_if_leA*node_2583)+(node_8388_if_leB*_GlossIntensity),_GlossIntensity,node_8388_if_leA*node_8388_if_leB))+lerp(node_137,float3(node_4309,node_4309,node_4309),_FresnelVisibleOnDarkAmbientLight))*_LightColor0.rgb),node_5038)),dot(saturate(min(((((node_3766*(lerp(float3(node_505,node_505,node_505),lerp(lerp((node_6054*_ShadowColor.rgb),lerp(float3(node_988,node_988,node_988),saturate((1.0-((1.0-0.44)/_Texture_var.rgb))),node_6054),_MainTextureColorShadow),lerp(float3(node_9082,node_9082,node_9082),(lerp(lerp(float3(node_557,node_557,node_557),_ShadowPTexture_var.rgb,_ShadowPTexture_var.a),float3(node_3933,node_3933,node_3933),node_9036)*lerp(0.54,1.65,_MainTextureColorShadow)),_ShadowIntensity),_ShadowPTextureIntensity),0.65)*2.86))*lerp( lerp(node_3573,float3(node_356,node_356,node_356),node_9036), node_3573, _ShowAOonLight ))+(lerp(float3(node_7287,node_7287,node_7287),lerp(float3(_SoftGloss_var,_SoftGloss_var,_SoftGloss_var),(_SoftGloss_var*_GlossColor.rgb),2.22),_GlossMask_var.rgb)*lerp((node_8388_if_leA*node_2583)+(node_8388_if_leB*_GlossIntensity),_GlossIntensity,node_8388_if_leA*node_8388_if_leB))+lerp(node_137,float3(node_4309,node_4309,node_4309),_FresnelVisibleOnDarkAmbientLight))*_LightColor0.rgb),node_5038)),float3(0.3,0.59,0.11)),node_3161);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform fixed _Transparent;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                clip(lerp( 1.0, _Texture_var.a, _Transparent ) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
