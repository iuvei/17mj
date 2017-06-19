
uniform lowp sampler2D sTexture;
uniform lowp sampler2D inputImageTexture2;

varying mediump vec2 vTexCoord;

void main()
{
  lowp vec3 video = texture2D(sTexture, vTexCoord).rgb;

  lowp float gray = dot(vec3(0.3, 0.6, 0.1), video);

  lowp float alpha = texture2D(inputImageTexture2, vTexCoord).r;

  gl_FragColor = vec4(mix(vec3(gray), video, alpha), 1.0);
}

