precision mediump float;
uniform sampler2D  sTexture; //video
uniform sampler2D inputImageTexture2; //mv
varying vec2 vTexCoord;
void main()
{
vec4 video = texture2D(sTexture, vTexCoord);
vec4 mv    = texture2D(inputImageTexture2, vTexCoord);
gl_FragColor = video + mv;
}