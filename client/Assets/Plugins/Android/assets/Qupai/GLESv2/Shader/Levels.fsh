
// Clamp((c - InBlack) / (InWhite - InBlack), 0, 1) ** Gamma * (OutWhite - OutBlack) + OutBlack

uniform mediump vec3        uInBlack;
uniform mediump vec3        uInWhite;
uniform mediump vec3        uOutBlack;
uniform mediump vec3        uOutWhite;

uniform mediump sampler2D   sImage;
varying mediump vec2        vTexCoord;

mediump vec3 Levels(mediump vec3 src) {
    mediump vec3 normalized_src = (src - uInBlack) / (uInWhite - uInBlack);
    mediump vec3 weight = clamp(normalized_src, vec3(0.0), vec3(1.0));

#ifdef GAMMA
    mediump vec3 gamma_corrected = Gamma(weight);
#else
    mediump vec3 gamma_corrected = weight;
#endif

    return mix(uOutBlack, uOutWhite, gamma_corrected);
}

void main() {
    mediump vec3 src = texture2D(sImage, vTexCoord).rgb;
    gl_FragColor = vec4(Levels(src), 1.0);
}

