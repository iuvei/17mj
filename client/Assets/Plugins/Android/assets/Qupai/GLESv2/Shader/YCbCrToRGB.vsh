
#if     PIXEL_LAYOUT == PIXEL_LAYOUT_PLANAR
# define    PLANE_COUNT 3
#elif   PIXEL_LAYOUT == PIXEL_LAYOUT_SEMIPLANAR
# define    PLANE_COUNT 2
#else
# error     unsupported PIXEL_LAYOUT
#endif

#ifndef TEX_TRANSFORM
# define TEX_TRANSFORM 0
#endif

#if TEX_TRANSFORM
uniform     mat4    uTexTransform;
#endif

uniform     mat4    uPositionTransform;

attribute   vec2    aTexCoord0;
#if PLANE_COUNT > 1
attribute   vec2    aTexCoord1;
#endif
#if PLANE_COUNT > 2
attribute   vec2    aTexCoord2;
#endif

attribute   vec2    aPosition;

varying     vec2    vTexCoord[PLANE_COUNT];


#if TEX_TRANSFORM
vec2 YCbCrToRGB_TexTransform(vec2 tex_coord)
{
    vec4 result = uTexTransform * vec4(aTexCoord0, 0, 1);

    return result.xy;
}
#else
vec2 YCbCrToRGB_TexTransform(vec2 tex_coord)
{
    return tex_coord;
}
#endif

void main()
{

    vTexCoord[0] = YCbCrToRGB_TexTransform(aTexCoord0);
#if PLANE_COUNT > 1
    vTexCoord[1] = YCbCrToRGB_TexTransform(aTexCoord1);
#endif
#if PLANE_COUNT > 2
    vTexCoord[2] = YCbCrToRGB_TexTransform(aTexCoord2);
#endif

    gl_Position = uPositionTransform * vec4(aPosition, 0, 1);
}
