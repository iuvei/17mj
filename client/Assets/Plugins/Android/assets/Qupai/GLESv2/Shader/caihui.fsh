precision mediump float;

varying vec2 vTexCoord;

uniform sampler2D sTexture;

void main(void)
{
    int width = 320;
    int height = 480;
    vec4 grayMat = vec4(0.299,0.587,0.114,0.0);
    vec4 color = texture2D(sTexture,vTexCoord);
    float g = dot(color,grayMat);
    float tx;
    float ty;
    tx = 1.0 / float(width);
    ty = 1.0 / float(height);
    vec4 tmp = vec4(0.0);
    vec4 c1;
    c1 = texture2D(sTexture,vTexCoord + vec2(-1.0*tx,-1.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(0.0*tx,-1.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(1.0*tx,-1.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(-1.0*tx,0.0*ty));
    tmp = max(tmp,c1);
    c1 = color;
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(1.0*tx,0.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(-1.0*tx,1.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(0.0*tx,1.0*ty));
    tmp = max(tmp,c1);
    c1 = texture2D(sTexture,vTexCoord + vec2(1.0*tx,1.0*ty));
    tmp = max(tmp,c1);
    vec4 dd;
    float threshold = 57.0/255.0;
    dd = color/tmp;
    g = clamp(g,0.0,threshold);
    float ratio = g/threshold;
    dd = ratio*dd + (1.0 - ratio)*color;
    g = dot(grayMat,dd);
    g = min(1.0,max(0.0,g));
    vec4 yiq;
    mat3 rgb2yiq = mat3(0.299,0.596,0.211,0.587,-0.275,-0.532,0.114,-0.322,0.312);
    yiq.rgb = rgb2yiq*color.rgb;
    yiq.r =  max(min(pow(g, 2.7), 1.0),0.0);
    vec4 rgb;
    mat3 yiq2rgb = mat3(1.0,1.0,1.0,0.956,-0.272,-1.106,0.621,-1.703,0.0);
    rgb.rgb = yiq2rgb*yiq.rgb;
    rgb.a = 1.0;
    gl_FragColor = rgb;
}