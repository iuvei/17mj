
#ifndef TEX_TRANSFORM
# define TEX_TRANSFORM 0
#endif

#ifndef VERT_TRANSFORM
# define VERT_TRANSFORM 0
#endif

attribute   vec2    aTexCoord0;
attribute   vec2    aPosition;

varying     vec2    vTexCoord;

#if TEX_TRANSFORM != 0
uniform     mat4    uTexTransform;
#endif

#if VERT_TRANSFORM != 0
uniform     mat4    uPositionTransform;
#endif

void main()
{
#if TEX_TRANSFORM != 0
    vTexCoord = (uTexTransform * vec4(aTexCoord0, 0, 1)).xy;
#else
    vTexCoord = aTexCoord0;
#endif

#if VERT_TRANSFORM != 0
    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
#else
    gl_Position = vec4(aPosition, 0, 1);
#endif

}

