
varying mediump vec2        vTexCoord[4];

uniform lowp    sampler2D   sTextureR;
uniform lowp    sampler2D   sTextureG;
uniform lowp    sampler2D   sTextureB;
uniform lowp    sampler2D   sTextureA;

void main()
{
    lowp float r = texture2D(sTextureR, vTexCoord[0]).r;
    lowp float g = texture2D(sTextureG, vTexCoord[1]).g;
    lowp float b = texture2D(sTextureB, vTexCoord[2]).b;
    lowp float a = texture2D(sTextureA, vTexCoord[3]).a;

    gl_FragColor = vec4(r, g, b, a);
}
