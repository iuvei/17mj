precision mediump float;

uniform sampler2D sTexture; //video
uniform sampler2D inputImageTexture2; //screen

varying vec2 vTexCoord;

void main()
{
  vec3 video = texture2D(sTexture, vTexCoord).rgb;
  vec3 screen = texture2D(inputImageTexture2, vTexCoord).rgb;

  gl_FragColor.rgb = 1.0 - (1.0 - screen) * (1.0 - video);
  gl_FragColor.a = 1.0;
}
