#define SAMPLER_2D       0
#define SAMPLER_EXTERNAL 1

#ifndef SAMPLER
# define SAMPLER SAMPLER_2D
#endif

#ifndef TRANSLUCENT
# define TRANSLUCENT 0
#endif

#if SAMPLER == SAMPLER_EXTERNAL
# extension GL_OES_EGL_image_external : require
#endif

#if SAMPLER == SAMPLER_EXTERNAL
uniform samplerExternalOES          sTexture;
#else
uniform sampler2D                   sTexture;
#endif

varying mediump             vec2    vTexCoord;
#if TRANSLUCENT
varying lowp                float   vAlpha;
#endif

void main() {
    lowp vec4 color = texture2D(sTexture, vTexCoord);
#ifdef GAMMA
    lowp vec3 gamma_corrected = Gamma(color.rgb);
#else
    lowp vec3 gamma_corrected = color.rgb;
#endif

#if TRANSLUCENT
    gl_FragColor = vec4(gamma_corrected, vAlpha);
#else
    gl_FragColor = vec4(gamma_corrected, color.a);
#endif
}

