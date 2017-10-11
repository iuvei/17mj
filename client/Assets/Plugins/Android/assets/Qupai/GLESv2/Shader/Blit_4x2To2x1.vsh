/**
 *    4*2         =>   2*1 RG    ==    1*1 RGRG
 *   0
 * 0 +-+-+-+-+         0               0
 *   | | | | |       0 +-+-+         0 +--+
 *   +-+-+-+-+    =>   | | |     ==    |  |
 *   | | | | |         +-+-+ 1         +--+ 1
 *   +-+-+-+-+ 1           1              1
 *           1
 */
 
#ifndef VERT_TRANSFORM
# define VERT_TRANSFORM 0
#endif

#ifndef TEX_TRANSFORM
# define TEX_TRANSFORM 0
#endif

const       float       kOffset = 1.0;

uniform     float       uImageWidth;

#if VERT_TRANSFORM != 0
uniform     mat4        uPositionTransform;
#endif

#if TEX_TRANSFORM != 0
uniform     mat4        uTexTransform;
#endif

attribute   vec2        aTexCoord0;
attribute   vec2        aPosition;

varying     vec2        vTexCoord0;
varying     vec2        vTexCoord1;

void main()
{
    float texel_width = 1.0 / uImageWidth;

#if TEX_TRANSFORM != 0
    vec2 tex_coord  = (uTexTransform * vec4(aTexCoord0, 0, 1)).xy;
    vec2 offset     = (uTexTransform * vec4(texel_width * kOffset, 0, 0, 1)).xy;
#else
    vec2 tex_coord  = aTexCoord0;
    vec2 offset     = vec2(texel_width * kOffset, 0);
#endif

    vTexCoord0 = tex_coord - offset;
    vTexCoord1 = tex_coord + offset;

#if VERT_TRANSFORM != 0
    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
#else
    gl_Position = vec4(aPosition, 0, 1);
#endif

}