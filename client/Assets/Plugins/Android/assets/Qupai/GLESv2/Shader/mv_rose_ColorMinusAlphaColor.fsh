precision mediump float;
uniform sampler2D sTexture; //video
uniform sampler2D inputImageTexture2; //mv
uniform sampler2D inputImageTexture3; //alpha
varying vec2 vTexCoord;
void main()
{
vec4 video = texture2D(sTexture, vTexCoord);
vec4 mv    = texture2D(inputImageTexture2, vTexCoord);
vec4 alpha = texture2D(inputImageTexture3, vTexCoord);
gl_FragColor = video * (1.0 - alpha.r) + mv;
}