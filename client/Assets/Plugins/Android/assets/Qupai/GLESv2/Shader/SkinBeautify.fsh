precision mediump float;

#ifndef SKIN_RED
#error SKIN_RED
#endif

#ifndef SKIN_BLUE
#error SKIN_BLUE
#endif

varying highp vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D sGaussianTexture;
uniform sampler2D sColorPalette;

#define Blend_pLitf(base,blend)   clamp((base)+2.0*(blend)-1.0, 0.0, 1.0)
#define Blend_hLitf(base,blend) ((blend)<=0.5?(blend)*(base)/0.5:1.0-(1.0-(blend))*(1.0-(base))/0.5)

void main(void)
{
  vec3 oral =texture2D(sTexture, vTexCoord).rgb;
  vec3 gauss =texture2D(sGaussianTexture, vTexCoord).rgb;
  
  vec3 curve;
  curve.r = texture2D(sColorPalette,vec2(oral.r,0.5)).b;
  curve.g = texture2D(sColorPalette,vec2(oral.g,0.5)).b;
  curve.b = texture2D(sColorPalette,vec2(oral.b,0.5)).b;
  
  float G = oral.g;
  float G1 = 1.0 - gauss.g;
  G1 = Blend_pLitf(G, G1);
  float G2=mix(G,G1,0.5);
  
  G2=Blend_hLitf(G2, G2);
  G2=Blend_hLitf(G2, G2);
  G2=Blend_hLitf(G2, G2);
  vec3 temp=mix(curve,oral,G2);
  float Offset = clamp(oral.r - (SKIN_RED - 0.5), 0.0, 1.0);
  float alpha = min(Offset * 2.0, 1.0); //254 - (127 - Offset) * 2
  float OffsetJ = clamp(oral.b - SKIN_BLUE, 0.0, 1.0);
  alpha = max(alpha - OffsetJ / 2.0, 0.0);
  
  oral = mix(oral,temp,alpha);
  
  gl_FragColor = vec4(oral, 1);
}