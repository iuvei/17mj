precision mediump float;
varying vec2 vTexCoord;
 
uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;

void main()
{
  vec4 bottom = vec4(0.0);
  vec4 top = texture2D(inputImageTexture, textureCoordinate);
  vec4 alpha = texture2D(inputImageTexture2, textureCoordinate);

  gl_FragColor = mix(bottom, top, alpha);
}