
attribute   vec4    aColor_From;
attribute   vec2    aPosition_From;
attribute   vec4    aColor_To;
attribute   vec2    aPosition_To;

varying     vec2    vTexCoord;
varying     vec4    vColor;

uniform     mat4    uPositionTransform;

uniform     float   uWeight;

void main()
{
    vColor = mix(aColor_From, aColor_To, uWeight);

    vec2 position = mix(aPosition_From, aPosition_To, uWeight);

    gl_Position = uPositionTransform * vec4(position, 0, 1);
}

