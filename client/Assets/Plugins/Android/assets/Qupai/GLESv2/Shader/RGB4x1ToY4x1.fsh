
#ifndef COLOR_BUFFER_FORMAT
# define COLOR_BUFFER_FORMAT PIXEL_FORMAT_RGBA
#endif

varying mediump vec2        vTexCoord0;
varying mediump vec2        vTexCoord1;
varying mediump vec2        vTexCoord2;
varying mediump vec2        vTexCoord3;

uniform lowp    sampler2D   sTexture;

mediump float RGBToY(mediump vec3 value) {
#ifdef GAMMA
    mediump vec3 gamma_corrected = Gamma(value);
#else
    mediump vec3 gamma_corrected = value;
#endif

#if COLOR_MATRIX == COLOR_MATRIX_PC601
    mediump vec4 RGB_TO_Y = vec4(0.299, 0.587, 0.114, 0);
#elif COLOR_MATRIX = COLOR_MATRIX_BT601
    mediump vec4 RGB_TO_Y = vec4(0.257, 0.504, 0.098, 16.0 / 255.0);
#else
# error COLOR_MATRIX
#endif
    return dot(vec4(gamma_corrected, 1), RGB_TO_Y);
}

void main() {

    lowp vec3 t0 = texture2D(sTexture, vTexCoord0).rgb;
    lowp vec3 t1 = texture2D(sTexture, vTexCoord1).rgb;
    lowp vec3 t2 = texture2D(sTexture, vTexCoord2).rgb;
    lowp vec3 t3 = texture2D(sTexture, vTexCoord3).rgb;

    lowp float y0 = RGBToY(t0);
    lowp float y1 = RGBToY(t1);
    lowp float y2 = RGBToY(t2);
    lowp float y3 = RGBToY(t3);

#if     COLOR_BUFFER_FORMAT == PIXEL_FORMAT_RGBA
    gl_FragColor = vec4(y0, y1, y2, y3);
#elif   COLOR_BUFFER_FORMAT == PIXEL_FORMAT_BGRA
    gl_FragColor = vec4(y2, y1, y0, y3);
#else
# error unsupported color buffer formmat
#endif
}

