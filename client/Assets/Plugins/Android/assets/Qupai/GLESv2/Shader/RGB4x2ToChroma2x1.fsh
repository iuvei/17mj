
#ifndef COLOR_BUFFER_FORMAT
# define COLOR_BUFFER_FORMAT PIXEL_FORMAT_RGBA
#endif

#ifndef COLOR_MATRIX
# define COLOR_MATRIX COLOR_MATRIX_PC601
#endif

#ifndef OUT_PIXEL_FORMAT
# define OUT_PIXEL_FORMAT PIXEL_FORMAT_CbCr
#endif

varying mediump     vec2         vTexCoord0;
varying mediump     vec2         vTexCoord1;

uniform lowp        sampler2D    sTexture;

mediump vec2 RGBToCbCr(mediump vec3 value) {

#ifdef GAMMA
    mediump vec3 gamma_corrected = Gamma(value);
#else
    mediump vec3 gamma_corrected = value;
#endif

#if COLOR_MATRIX == COLOR_MATRIX_PC601
    mediump vec4 RGB_TO_ChromaB = vec4(-0.148, -0.291, 0.439, 0.5);
    mediump vec4 RGB_TO_ChromaR = vec4(0.439, -0.368, -0.071, 0.5);
#else
# error COLOR_MATRIX
#endif

    mediump float chroma_b = dot(RGB_TO_ChromaB, vec4(gamma_corrected, 1));
    mediump float chroma_r = dot(RGB_TO_ChromaR, vec4(gamma_corrected, 1));

#if     OUT_PIXEL_FORMAT == PIXEL_FORMAT_CrCb
    mediump vec2 chroma = vec2(chroma_r, chroma_b);
#elif   OUT_PIXEL_FORMAT == PIXEL_FORMAT_CbCr
    mediump vec2 chroma = vec2(chroma_b, chroma_r);
#endif
    return chroma;
}

void main() {

    lowp vec3 t0 = texture2D(sTexture, vTexCoord0).rgb;
    lowp vec3 t1 = texture2D(sTexture, vTexCoord1).rgb;

    lowp vec2 chroma0 = RGBToCbCr(t0);
    lowp vec2 chroma1 = RGBToCbCr(t1);

#if     COLOR_BUFFER_FORMAT == PIXEL_FORMAT_RGBA
    gl_FragColor = vec4(chroma0.x, chroma0.y, chroma1.x, chroma1.y);
#elif   COLOR_BUFFER_FORMAT == PIXEL_FORMAT_BGRA
    gl_FragColor = vec4(chroma1.x, chroma0.x, chroma0.y, chroma1.y);
#else
# error unsupported color buffer formmat
#endif
}

