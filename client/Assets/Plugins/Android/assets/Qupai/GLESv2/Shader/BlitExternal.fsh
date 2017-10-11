#extension GL_OES_EGL_image_external : require

uniform samplerExternalOES          sTexture;
varying mediump             vec2    vTexCoord;

void main() {
    gl_FragColor = texture2D(sTexture, vTexCoord);
}

