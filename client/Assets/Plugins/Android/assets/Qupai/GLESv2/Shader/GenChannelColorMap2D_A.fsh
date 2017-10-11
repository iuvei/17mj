/**
 * K: vec2<clampf> -> clampf
 * G: clampf -> clampf
 * F: clampf -> clampf
 *
 * F(K(TexCoord), G(Channel)) -> F'(K(TexCoord), Channel)
 */

varying mediump vec2        vTexCoord; // vec2(K(TexCoord), Channel)

uniform mediump sampler2D   sChannelColorMap;
uniform lowp    sampler2D   sTexCoordMap;

void main()
{
    mediump float Gr = texture2D(sChannelColorMap, vec2(vTexCoord.y, 1.0 / 6.0)).r;
    mediump float Gg = texture2D(sChannelColorMap, vec2(vTexCoord.y, 3.0 / 6.0)).g;
    mediump float Gb = texture2D(sChannelColorMap, vec2(vTexCoord.y, 5.0 / 6.0)).b;

    lowp float Fr = texture2D(sTexCoordMap, vec2(vTexCoord.x, Gr)).r;
    lowp float Fg = texture2D(sTexCoordMap, vec2(vTexCoord.x, Gg)).g;
    lowp float Fb = texture2D(sTexCoordMap, vec2(vTexCoord.x, Gb)).b;

    gl_FragColor = vec4(Fr, Fg, Fb, 1.0);
}