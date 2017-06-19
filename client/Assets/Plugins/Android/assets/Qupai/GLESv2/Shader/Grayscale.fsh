precision mediump float;

varying vec2 vTexCoord;

uniform sampler2D sTexture;

void main()
{
  vec3 color = texture2D(sTexture, vTexCoord).rgb;

  float gray = dot(vec3(0.3, 0.6, 0.1), color);

  gl_FragColor.rgb = vec3(gray);
  gl_FragColor.a = 1.0;

} 