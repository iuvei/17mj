#ifndef DARKEN
#define DARKEN 0
#endif
uniform sampler2D   sTexture;

void main() {
#if DARKEN == 1
    gl_FragColor = vec4(Convolve1D(sTexture) / 2.0, 1.0);
#else
    gl_FragColor = vec4(Convolve1D(sTexture), 1.0);
#endif
}
