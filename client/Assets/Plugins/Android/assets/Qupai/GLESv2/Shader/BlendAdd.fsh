precision mediump float;

uniform sampler2D  sTexture; //video
uniform sampler2D inputImageTexture2; //mv

varying vec2 vTexCoord;

void main()
{
  vec3 a = texture2D(sTexture, vTexCoord).rgb;
  vec3 b = texture2D(inputImageTexture2, vTexCoord).rgb;

  gl_FragColor.rgb = a + b;
  gl_FragColor.a = 1.0;

}