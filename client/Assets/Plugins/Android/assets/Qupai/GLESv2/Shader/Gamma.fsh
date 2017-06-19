
mediump vec3 Gamma(mediump vec3 c) {
#ifdef GAMMA
    return pow(c, vec3(GAMMA, GAMMA, GAMMA));
#else
    return c;
#endif
}
