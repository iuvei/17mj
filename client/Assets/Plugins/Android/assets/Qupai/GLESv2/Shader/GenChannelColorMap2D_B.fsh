/**
 * K: vec2<clampf> -> clampf
 * G: vec2<clampf> -> clampf
 * F: clampf -> clampf
 *
 * F(G(K(TexCoord), Channel)) <==> F'(K(TexCoord), Channel)
 */

varying mediump vec2        vTexCoord; // vec2(K(TexCoord), Channel)

uniform mediump sampler2D   sTexCoordMap;
uniform lowp    sampler2D   sChannelColorMap;

void main()
{
    mediump vec3 G = texture2D(sTexCoordMap, vTexCoord).rgb;

    lowp float Fr = texture2D(sChannelColorMap, vec2(G.r, 1.0 / 6.0)).r;
    lowp float Fg = texture2D(sChannelColorMap, vec2(G.g, 3.0 / 6.0)).g;
    lowp float Fb = texture2D(sChannelColorMap, vec2(G.b, 5.0 / 6.0)).b;

    gl_FragColor = vec4(Fr, Fg, Fb, 1.0);
}

