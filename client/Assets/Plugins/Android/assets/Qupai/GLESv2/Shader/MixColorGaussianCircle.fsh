precision mediump float;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2; //map     xproMap.png
uniform sampler2D inputImageTexture3; //vigMap   vignetteMap.png

varying vec2 blurCoordinates[5];

void main()
{

  vec2 tex_coord = blurCoordinates[0];
  vec3 texel = texture2D(sTexture, blurCoordinates[0]).rgb;


  vec3 blurred = vec3(0.0);
  blurred += texel * 0.204164;
  blurred += texture2D(sTexture, blurCoordinates[1]).rgb * 0.304005;
  blurred += texture2D(sTexture, blurCoordinates[2]).rgb * 0.304005;
  blurred += texture2D(sTexture, blurCoordinates[3]).rgb * 0.093913;
  blurred += texture2D(sTexture, blurCoordinates[4]).rgb * 0.093913;

  vec2 tc = 2.0 * tex_coord - 1.0;
  float d = dot(tc, tc);

  vec3 color;
  color.r = texture2D(inputImageTexture3, vec2(d, texel.r)).r;
  color.g = texture2D(inputImageTexture3, vec2(d, texel.g)).g;
  color.b = texture2D(inputImageTexture3, vec2(d, texel.b)).b;

  float r = texture2D(inputImageTexture2, vec2(color.r, 1.0 / 6.0)).r;
  float g = texture2D(inputImageTexture2, vec2(color.g, 3.0 / 6.0)).g;
  float b = texture2D(inputImageTexture2, vec2(color.b, 5.0 / 6.0)).b;

  vec3 sharp_color = vec3(r, g, b);

  float distanceFromCenter = distance(vec2(0.5, 0.5), tex_coord);
  float blur_weight = smoothstep(0.4, 0.5, distanceFromCenter);

  gl_FragColor.rgb = mix(sharp_color, blurred, blur_weight);
  gl_FragColor.a = 1.0;
}
