precision mediump float;
varying mediump vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;
uniform sampler2D inputImageTexture3;
uniform sampler2D inputImageTexture4;
uniform sampler2D inputImageTexture5;

void main(void)
{
    mediump vec4 oralData =texture2D(sTexture, vTexCoord).rgba;
    mediump vec3 temp1 = texture2D(inputImageTexture2, vTexCoord).rgb;
    mediump vec3 temp2 = texture2D(inputImageTexture3, vTexCoord).rgb;

    oralData.r = oralData.r *0.7+0.3*texture2D(inputImageTexture5,vec2(temp2.r,oralData.r)).r;
    oralData.g = oralData.g *0.7+0.3*texture2D(inputImageTexture5,vec2(temp2.g,oralData.g)).r;
    oralData.b = oralData.b *0.7+0.3*texture2D(inputImageTexture5,vec2(temp2.b,oralData.b)).r;

    oralData.r = oralData.r *0.6+0.4*texture2D(inputImageTexture5,vec2(temp1.r,oralData.r)).r;
    oralData.g = oralData.g *0.6+0.4*texture2D(inputImageTexture5,vec2(temp1.g,oralData.g)).r;
    oralData.b = oralData.b *0.6+0.4*texture2D(inputImageTexture5,vec2(temp1.b,oralData.b)).r;

    oralData.r = texture2D( inputImageTexture4, vec2(oralData.r,0.5)).r;
    oralData.g = texture2D( inputImageTexture4, vec2(oralData.g,0.5)).g;
    oralData.b = texture2D( inputImageTexture4, vec2(oralData.b,0.5)).b;

    gl_FragColor = oralData;
}