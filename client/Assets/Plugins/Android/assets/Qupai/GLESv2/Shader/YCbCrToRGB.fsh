
#if     PIXEL_LAYOUT == PIXEL_LAYOUT_PLANAR
# define    PLANE_COUNT 3
#elif   PIXEL_LAYOUT == PIXEL_LAYOUT_SEMIPLANAR
# define    PLANE_COUNT 2
#else
# error     unsupported PIXEL_LAYOUT
#endif

#if     COLOR_MATRIX == COLOR_MATRIX_PC601 || COLOR_MATRIX == COLOR_MATRIX_PC709
const mediump float kYScale = 1.0;
const mediump float kYBias  = 0.0;
#elif   COLOR_MATRIX == COLOR_MATRIX_BT601 || COLOR_MATRIX == COLOR_MATRIX_BT709
const mediump float kYScale = 1.164;
const mediump float kYBias  = 16.0 / 255.0;
#endif

#if     COLOR_MATRIX == COLOR_MATRIX_BT601 || COLOR_MATRIX == COLOR_MATRIX_PC601
const mediump float kChromaRToR =  1.596;
const mediump float kChromaRToG = -0.813;
const mediump float kChromaBToG = -0.391;
const mediump float kChromaBToB =  2.018;
#elif   COLOR_MATRIX == COLOR_MATRIX_BT709 || COLOR_MATRIX == COLOR_MATRIX_PC709
const mediump float kChromaRToR =  1.793;
const mediump float kChromaRToG = -0.534;
const mediump float kChromaBToG = -0.213;
const mediump float kChromaBToB =  2.115;
#endif

varying mediump vec2        vTexCoord[PLANE_COUNT];


uniform lowp    sampler2D   sTexture0;  //  Y       Y       Y
#if     PLANE_COUNT > 1
uniform lowp    sampler2D   sTexture1;  //  Cb      CbCr    CrCb
# if    PLANE_COUNT > 2
uniform lowp    sampler2D   sTexture2;  //  Cr
# endif
#endif

#ifdef ACCURATE_CHROMA_SUBSAMPLING

uniform mediump vec2 uChromaSize;

mediump vec2 SubsamplingTexCoord(mediump vec2 tex_coord, mediump vec2 size) {
    mediump vec2 tex_offset = tex_coord * size;
    mediump vec2 tex_offset0 = floor(tex_offset + 0.25);
    mediump vec2 sample_tex_offset = tex_offset0 + clamp(tex_offset - tex_offset0, -0.25, 0.25) * 2.0;
    return sample_tex_offset / size;
}

lowp vec2 GetChroma() 
{

#if     PIXEL_LAYOUT == PIXEL_LAYOUT_PLANAR
# if    PIXEL_FORMAT == PIXEL_FORMAT_YCbCr

    mediump vec2 tex_coord_cb = SubsamplingTexCoord(vTexCoord[1], uChromaSize);
    mediump vec2 tex_coord_cr = SubsamplingTexCoord(vTexCoord[2], uChromaSize);

    return vec2(
            texture2D(sTexture1, tex_coord_cb).r,
            texture2D(sTexture2, tex_coord_cr).r
            );

# else
#  error    PIXEL_FORMAT unsupported
# endif

#elif   PIXEL_LAYOUT == PIXEL_LAYOUT_SEMIPLANAR

    mediump vec2 tex_coord = SubsamplingTexCoord(vTexCoord[1]);

# if    PIXEL_FORMAT == PIXEL_FORMAT_YCbCr

    return texture2D(sTexture1, tex_coord).ra;

# elif  PIXEL_FORMAT == PIXEL_FORMAT_YCrCb

    return texture2D(sTexture1, tex_coord).ar;

# else
#  error    PIXEL_FORMAT unsupported
# endif
#endif


}


#else

lowp vec2 GetChroma()
{

#if     PIXEL_LAYOUT == PIXEL_LAYOUT_PLANAR
# if    PIXEL_FORMAT == PIXEL_FORMAT_YCbCr

    return vec2(
            texture2D(sTexture1, vTexCoord[1]).r,
            texture2D(sTexture2, vTexCoord[2]).r
            );

# else
#  error    PIXEL_FORMAT unsupported
# endif

#elif   PIXEL_LAYOUT == PIXEL_LAYOUT_SEMIPLANAR
# if    PIXEL_FORMAT == PIXEL_FORMAT_YCbCr

    return texture2D(sTexture1, vTexCoord[1]).ra;

# elif  PIXEL_FORMAT == PIXEL_FORMAT_YCrCb

    return texture2D(sTexture1, vTexCoord[1]).ar;

# else
#  error    PIXEL_FORMAT unsupported
# endif
#endif

}

#endif

lowp vec3 YCbCrToRGB()
{
    mediump float y = kYScale * texture2D(sTexture0, vTexCoord[0]).r - kYBias;

    mediump vec2 chroma = GetChroma();

    mediump float chroma_b = chroma.x - 0.5;
    mediump float chroma_r = chroma.y - 0.5;

    return vec3(
            y + kChromaRToR * chroma_r,
            y + kChromaRToG * chroma_r + kChromaBToG * chroma_b,
            y                          + kChromaBToB * chroma_b
        );
}

void main()
{
    gl_FragColor = vec4(YCbCrToRGB(), 1.0);
}
