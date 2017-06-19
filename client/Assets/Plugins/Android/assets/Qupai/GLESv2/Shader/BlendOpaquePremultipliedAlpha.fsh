precision mediump float;

uniform sampler2D sTexture; //video
uniform sampler2D inputImageTexture2; //mv
uniform sampler2D inputImageTexture3; //alpha

varying vec2 vTexCoord;

void main()
{
  vec3 video  = texture2D(sTexture, vTexCoord).rgb;
  vec3 mv     = texture2D(inputImageTexture2, vTexCoord).rgb;
  float alpha = texture2D(inputImageTexture3, vTexCoord).r;

  gl_FragColor.rgb = video * (1.0 - alpha) + mv;
  gl_FragColor.a = 1.0;
}
