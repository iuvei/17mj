precision mediump float;
varying mediump vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;
uniform sampler2D inputImageTexture3;

void main()
{
    vec4 oralData = texture2D( sTexture, vTexCoord).rgba;
    vec3 graymat = vec3(0.29,0.586,0.114);
    float gray = dot(graymat,oralData.rgb);
    oralData.r = texture2D( inputImageTexture2, vec2(gray,oralData.r)).r;
    oralData.g = texture2D( inputImageTexture2, vec2(gray,oralData.g)).r;
    oralData.b = texture2D( inputImageTexture2, vec2(gray,oralData.b)).r;
    float x = 1.6;
    float rm = 0.30859375*(1.0-x);
    float gm =  0.609375*(1.0-x);
    float bm = 0.08203125*(1.0-x);
    float all = rm * oralData.r + gm * oralData.g + bm * oralData.b;
    oralData.r = max(0.0,min(1.0,all + x * oralData.r));
    oralData.g = max(0.0,min(1.0,all + x * oralData.g));
    oralData.b = max(0.0,min(1.0,all + x * oralData.b));

    oralData.r = texture2D( inputImageTexture3, vec2(oralData.r,0.5)).r;
    oralData.g = texture2D( inputImageTexture3, vec2(oralData.g,0.5)).g;
    oralData.b = texture2D( inputImageTexture3, vec2(oralData.b,0.5)).b;
    gl_FragColor = oralData;
}