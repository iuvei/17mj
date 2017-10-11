precision mediump float;

varying vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;


vec3 BlendOverlay3f(vec3 a, vec3 b) {
    return mix(a * b * 2.0, 1.0 - 2.0 * (1.0 - a) * (1.0 - b), step(0.5, a));
}

void main()
{
  vec3 a = texture2D(sTexture, vTexCoord).rgb;
  vec3 b = texture2D(inputImageTexture2, vTexCoord).rgb;

  gl_FragColor.rgb = BlendOverlay3f(a, b);
  gl_FragColor.a = 1.0;
}
