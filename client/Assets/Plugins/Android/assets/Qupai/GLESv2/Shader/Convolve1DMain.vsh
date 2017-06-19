#ifndef VERT_TRANSFORM
# define VERT_TRANSFORM 0
#endif

#if VERT_TRANSFORM == 0
uniform     mat4    uPositionTransform;
#endif

attribute   vec2    aTexCoord0;
attribute   vec2    aPosition;

void main() {
#if VERT_TRANSFORM == 0
    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
#else
    gl_Position = vec4(aPosition, 0, 1);
#endif
    Convolve1D(aTexCoord0);
}
