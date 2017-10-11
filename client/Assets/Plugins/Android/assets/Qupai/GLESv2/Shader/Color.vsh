
attribute   vec4    aColor;
attribute   vec2    aPosition;

varying     vec2    vTexCoord;
varying     vec4    vColor;

uniform     mat4    uPositionTransform;

void main()
{
    vColor = aColor;

    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);

}

