
#ifndef CONVOLVE_1D_VARYING_COUNT
# define CONVOLVE_1D_VARYING_COUNT   8
#endif

#if CONVOLVE_1D_VARYING_COUNT > 8
# error varying count limit exceeded
#endif

#if CONVOLVE_1D_VARYING_COUNT <= 0
# error varying count must be positive
#endif

varying vec2 vConvolve1DTexCoord[CONVOLVE_1D_VARYING_COUNT];

void Convolve1D(vec2 o)
{
    vConvolve1DTexCoord[0] = o + vec2(CONVOLVE_1D_OFF0);
#if CONVOLVE_1D_VARYING_COUNT > 1
    vConvolve1DTexCoord[1] = o + vec2(CONVOLVE_1D_OFF1);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 2
    vConvolve1DTexCoord[2] = o + vec2(CONVOLVE_1D_OFF2);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 3
    vConvolve1DTexCoord[3] = o + vec2(CONVOLVE_1D_OFF3);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 4
    vConvolve1DTexCoord[4] = o + vec2(CONVOLVE_1D_OFF4);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 5
    vConvolve1DTexCoord[5] = o + vec2(CONVOLVE_1D_OFF5);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 6
    vConvolve1DTexCoord[6] = o + vec2(CONVOLVE_1D_OFF6);
#endif
#if CONVOLVE_1D_VARYING_COUNT > 7
    vConvolve1DTexCoord[7] = o + vec2(CONVOLVE_1D_OFF7);
#endif
}
