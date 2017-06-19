
#ifndef CONVOLVE_1D_VARYING_COUNT
# define CONVOLVE_1D_VARYING_COUNT   8
#endif

#if CONVOLVE_1D_VARYING_COUNT > 8
# error varying count limit exceeded
#endif

#if CONVOLVE_1D_VARYING_COUNT <= 0
# error varying count must be positive
#endif

#ifndef CONVOLVE_1D_VARYING_SINGLE_ZERO
# define CONVOLVE_1D_VARYING_SINGLE_ZERO     1
#endif

varying mediump vec2 vConvolve1DTexCoord[CONVOLVE_1D_VARYING_COUNT];

mediump vec3 Convolve1D(sampler2D sampler) {

#if CONVOLVE_1D_VARYING_SINGLE_ZERO == 1
    mediump vec3 color = texture2D(sampler, vConvolve1DTexCoord[0]).rgb * CONVOLVE_1D_CO0;
#else
    mediump vec3 color = vec3(0.0, 0.0, 0.0);
    color += texture2D(sampler,  vConvolve1DTexCoord[0]).rgb * CONVOLVE_1D_CO0;
#endif

#if CONVOLVE_1D_VARYING_COUNT > 1
    color += texture2D(sampler,  vConvolve1DTexCoord[1]).rgb * CONVOLVE_1D_CO1;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 2
    color += texture2D(sampler,  vConvolve1DTexCoord[2]).rgb * CONVOLVE_1D_CO2;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 3
    color += texture2D(sampler,  vConvolve1DTexCoord[3]).rgb * CONVOLVE_1D_CO3;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 4
    color += texture2D(sampler,  vConvolve1DTexCoord[4]).rgb * CONVOLVE_1D_CO4;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 5
    color += texture2D(sampler,  vConvolve1DTexCoord[5]).rgb * CONVOLVE_1D_CO5;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 6
    color += texture2D(sampler,  vConvolve1DTexCoord[6]).rgb * CONVOLVE_1D_CO6;
#endif
#if CONVOLVE_1D_VARYING_COUNT > 7
    color += texture2D(sampler,  vConvolve1DTexCoord[7]).rgb * CONVOLVE_1D_CO7;
#endif

    return color;
}
