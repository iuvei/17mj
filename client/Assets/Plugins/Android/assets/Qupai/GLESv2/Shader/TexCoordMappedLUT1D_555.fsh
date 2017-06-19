precision mediump float;
varying mediump vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2; //blowout;
uniform sampler2D inputImageTexture3; //overlay;
uniform sampler2D inputImageTexture4; //map

void main()
{
  vec4 texel = texture2D(sTexture, vTexCoord);
  vec3 bbTexel = texture2D(inputImageTexture2, vTexCoord).rgb;

  texel.r = texture2D(inputImageTexture3, vec2(bbTexel.r, texel.r)).r;
  texel.g = texture2D(inputImageTexture3, vec2(bbTexel.g, texel.g)).g;
  texel.b = texture2D(inputImageTexture3, vec2(bbTexel.b, texel.b)).b;

  vec4 mapped;
  mapped.r = texture2D(inputImageTexture4, vec2(texel.r, .5)).r;
  mapped.g = texture2D(inputImageTexture4, vec2(texel.g, .5)).g;
  mapped.b = texture2D(inputImageTexture4, vec2(texel.b, .5)).b;
  mapped.a = 1.0;

  gl_FragColor = mapped;
}
