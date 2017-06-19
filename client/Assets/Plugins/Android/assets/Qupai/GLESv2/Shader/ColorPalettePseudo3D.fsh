
// B as layer index, R as u, G as v

uniform mediump     sampler2D   sColorPalette;

const mediump vec2  kHalfTexel = vec2(0.5 / 16.0 / 16.0, 0.5 / 16.0);

mediump vec3 ColorPalettePseudo3DLayer(lowp float ix, lowp vec2 rg) {
    mediump vec2 lt = vec2(ix / 16.0, 0.0);
    mediump vec2 rb = vec2((ix + 1.0) / 16.0, 1.0);

    return texture2D(sColorPalette, mix(lt + kHalfTexel, rb - kHalfTexel, rg)).rgb;
}

lowp vec3 ColorPalettePseudo3D(mediump vec3 c)
{
    mediump float b = c.b * 15.0;

    mediump vec3 c0 = ColorPalettePseudo3DLayer(floor(b), c.rg);
    mediump vec3 c1 = ColorPalettePseudo3DLayer(ceil(b), c.rg);

    return mix(c0, c1, fract(b));
}

