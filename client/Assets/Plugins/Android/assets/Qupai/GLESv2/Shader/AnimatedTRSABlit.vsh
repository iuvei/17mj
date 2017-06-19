
#ifndef TRANSLUCENT
# define TRANSLUCENT 0
#endif

attribute   vec2    aPosition_From;
attribute   vec2    aTexCoord0_From;
attribute   vec4    aTRS_From;
#if TRANSLUCENT
attribute   float   aAlpha_From;
#endif

attribute   vec2    aPosition_To;
attribute   vec2    aTexCoord0_To;
attribute   vec4    aTRS_To;
#if TRANSLUCENT
attribute   float   aAlpha_To;
#endif


#if TRANSLUCENT
varying     float   vAlpha;
#endif

varying     vec2    vTexCoord;

uniform     mat4    uPositionTransform;

uniform     float   uWeight;

void main()
{
    vTexCoord = mix(aTexCoord0_From, aTexCoord0_To, uWeight);

    vec2 position = mix(aPosition_From.xy, aPosition_To.xy, uWeight);
    vec4 trs = mix(aTRS_From, aTRS_To, uWeight);

    float rotation = trs.z;
    vec2 translation = trs.xy * trs.w;

    mat2 rot_mat = mat2(cos(rotation), sin(rotation), -sin(rotation), cos(rotation));

    vec2 new_pos = rot_mat * translation + position;

    gl_Position = uPositionTransform * vec4(new_pos, 0, 1);

#if TRANSLUCENT
    vAlpha = mix(aAlpha_From, aAlpha_To, uWeight);
#endif
}

