precision mediump float;

varying mediump vec2 vTexCoord; 

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;

void main()
{
  vec3 value = texture2D(sTexture, vTexCoord).rgb;

  float r = texture2D(inputImageTexture2, vec2(value.r, 0.5)).r;
  float g = texture2D(inputImageTexture2, vec2(value.g, 0.5)).g;
  float b = texture2D(inputImageTexture2, vec2(value.b, 0.5)).b;

  gl_FragColor.rgb = vec3(r, g, b);
  gl_FragColor.a = 1.0;
}