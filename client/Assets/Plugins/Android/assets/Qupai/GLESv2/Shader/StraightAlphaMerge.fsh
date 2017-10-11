precision mediump float;

varying mediump vec2 vTexCoord; 

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;

void main()
{
  vec3  color = texture2D(sTexture, vTexCoord).rgb;
  float alpha = texture2D(inputImageTexture2, vTexCoord).r;

  gl_FragColor = vec4(color * alpha, alpha);
}