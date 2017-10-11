precision mediump float;
uniform sampler2D sTexture; //video
uniform sampler2D inputImageTexture2; //screen
varying vec2 vTexCoord;
void main()
{
	vec4 video = texture2D(sTexture, vTexCoord);
	vec4 screen = texture2D(inputImageTexture2, vTexCoord);
	mediump vec4 whiteColor = vec4(1.0);
	gl_FragColor = whiteColor - ((whiteColor - screen) * (whiteColor - video));
}