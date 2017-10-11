/**
 * Per-Channel:
 * M: vec4<clampf> -> vec4<clampf>
 * K: vec2<clampf> -> clampf
 * F: vec2<clampf> -> clampf
 *
 * F(K(TexCoord), M(TexColor)[Channel])
 */

#define TEX_COORD_TRANSFORM_NONE    0
#define TEX_COORD_TRANSFORM_TEXTURE 1
#define TEX_COORD_TRANSFORM_RADIUS2 2

#define TEX_COLOR_TRANSFORM_NONE                0
#define TEX_COLOR_TRANSFORM_PALETTE_1D          1
#define TEX_COLOR_TRANSFORM_PALETTE_2D          2
#define TEX_COLOR_TRANSFORM_PALETTE_PSEUDO_3D   3
#define TEX_COLOR_TRANSFORM_GAMMA               4

#ifndef TEX_COORD_TRANSFORM
# define TEX_COORD_TRANSFORM TEX_COORD_TRANSFORM_NONE
#endif

#ifndef TEX_COLOR_TRANSFORM
# define TEX_COLOR_TRANSFORM TEX_COLOR_TRANSFORM_NONE
#endif

varying mediump vec2        vTexCoord;

uniform mediump sampler2D   sTexture;



#if   TEX_COORD_TRANSFORM == TEX_COORD_TRANSFORM_NONE

#elif TEX_COORD_TRANSFORM == TEX_COORD_TRANSFORM_TEXTURE

uniform mediump sampler2D   sTexCoordLUT;

mediump vec3 TexCoordTransform() {
    return texture2D(sTexCoordLUT, vTexCoord).rgb;
}

#elif TEX_COORD_TRANSFORM == TEX_COORD_TRANSFORM_RADIUS2

mediump vec3 TexCoordTransform() {
    mediump vec2 to_center = 2.0 * vTexCoord - 1.0;
    return vec3(dot(to_center, to_center));
}

#else
# error unsupported TEX_COORD_TRANSFORM
#endif

#if   TEX_COLOR_TRANSFORM == TEX_COLOR_TRANSFORM_NONE

lowp vec3 ColorTransform(mediump vec3 c)
{
    return c;
}

#elif   TEX_COLOR_TRANSFORM == TEX_COLOR_TRANSFORM_PALETTE_PSEUDO_3D

lowp vec3 ColorTransform(mediump vec3 c)
{
    return ColorPalettePseudo3D(c);
}

#elif TEX_COLOR_TRANSFORM == TEX_COLOR_TRANSFORM_PALETTE_2D

uniform lowp    sampler2D   sColorPalette;

lowp vec3 ColorTransform(mediump vec3 c)
{
    mediump vec3 trans_tex_coord = TexCoordTransform();

    lowp float r = texture2D(sColorPalette, vec2(trans_tex_coord.r, c.r)).r;
    lowp float g = texture2D(sColorPalette, vec2(trans_tex_coord.g, c.g)).g;
    lowp float b = texture2D(sColorPalette, vec2(trans_tex_coord.b, c.b)).b;

    return vec3(r, g, b);
}

#elif TEX_COLOR_TRANSFORM == TEX_COLOR_TRANSFORM_PALETTE_1D

uniform lowp    sampler2D   sColorPalette;

lowp vec3 ColorTransform(mediump vec3 c)
{
    lowp float r = texture2D(sColorPalette, vec2(c.r, 1.0 / 6.0)).r;
    lowp float g = texture2D(sColorPalette, vec2(c.g, 3.0 / 6.0)).g;
    lowp float b = texture2D(sColorPalette, vec2(c.b, 5.0 / 6.0)).b;

    return vec3(r, g, b);
}

#elif TEX_COLOR_TRANSFORM == TEX_COLOR_TRANSFORM_GAMMA


lowp vec3 ColorTransform(mediump vec3 c)
{
    return Gamma(c);
}

#else
# error unsupported TEX_COLOR_TRANSFORM
#endif


void main() {
    lowp vec4 tex_color = texture2D(sTexture, vTexCoord);

#if   defined(TEX_COLOR_MATRIX3)
    mediump vec3 c = mat3(TEX_COLOR_MATRIX3) * tex_color.rgb;
#elif defined(TEX_COLOR_MATRIX4)
    mediump vec3 c = (mat4(TEX_COLOR_MATRIX4) * tex_color).rgb;
#else
    mediump vec3 c = tex_color.rgb;
#endif

    gl_FragColor = vec4(ColorTransform(c), 1.0);
}

