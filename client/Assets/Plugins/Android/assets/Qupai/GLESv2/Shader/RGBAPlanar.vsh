
attribute   vec2    aPosition;
attribute   vec2    aTexCoord0;
attribute   vec2    aTexCoord1;
attribute   vec2    aTexCoord2;
attribute   vec2    aTexCoord3;

varying     vec2    vTexCoord[4];

uniform     mat4    uPositionTransform;

void main()
{
    vTexCoord[0] = aTexCoord0;
    vTexCoord[1] = aTexCoord1;
    vTexCoord[2] = aTexCoord2;
    vTexCoord[3] = aTexCoord3;

    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
}

