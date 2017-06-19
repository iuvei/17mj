/**
 *
 *    4*1         =>      4*1 R      ==  1*1 RRRR
 *   0                  0                  0
 * 0 +-+-+-+-+        0 +-+-+-+-+        0 +--+
 *   | | | | |    =>    | | | | |          |  |
 *   +-+-+-+-+ 1        +-+-+-+-+ 1        +--+ 1
 *           1                  1             1
 */

#ifndef VERT_TRANSFORM
# define VERT_TRANSFORM 0
#endif

#ifndef TEX_TRANSFORM
# define TEX_TRANSFORM 0
#endif

const       float   kOffset0 = 0.5;
const       float   kOffset1 = 1.5;

uniform     float   uImageWidth;

#if VERT_TRANSFORM != 0
uniform     mat4    uPositionTransform;
#endif

#if TEX_TRANSFORM != 0
uniform     mat4    uTexTransform;
#endif

attribute   vec2    aTexCoord0;
attribute   vec2    aPosition;

varying     vec2    vTexCoord0;
varying     vec2    vTexCoord1;
varying     vec2    vTexCoord2;
varying     vec2    vTexCoord3;

void main()
{
    float texel_width = 1.0 / float(uImageWidth);

#if TEX_TRANSFORM != 0
    vec2 tex_coord    = (uTexTransform * vec4(aTexCoord0, 0, 1)).xy;
    vec2 offset_0     = (uTexTransform * vec4(texel_width * kOffset0, 0, 0, 1)).xy;
    vec2 offset_1     = (uTexTransform * vec4(texel_width * kOffset1, 0, 0, 1)).xy;
#else
    vec2 tex_coord    = aTexCoord0;
    vec2 offset_0     = vec2(texel_width * kOffset0, 0);
    vec2 offset_1     = vec2(texel_width * kOffset1, 0);
#endif

    vTexCoord0 = tex_coord - offset_1;
    vTexCoord1 = tex_coord - offset_0;
    vTexCoord2 = tex_coord + offset_0;
    vTexCoord3 = tex_coord + offset_1;

#if VERT_TRANSFORM != 0
    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
#else
    gl_Position = vec4(aPosition, 0, 1);
#endif

}