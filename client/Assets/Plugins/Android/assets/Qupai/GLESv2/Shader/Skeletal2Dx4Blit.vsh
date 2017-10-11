precision highp float;

#ifndef BONE_COUNT
#define BONE_COUNT 18
#endif

uniform mat4 uPositionTransform;
uniform mat4 uBoneList[BONE_COUNT];

attribute vec2 aPosition;
attribute vec2 aTexCoord;
attribute vec4 aIndexList;
attribute vec4 aWeightList;

varying vec2 vTexCoord;

void main()
{
  vTexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);

  mat4 bone_transform =
      + uBoneList[int(aIndexList.x)] * aWeightList.x
      + uBoneList[int(aIndexList.y)] * aWeightList.y
      + uBoneList[int(aIndexList.z)] * aWeightList.z
      + uBoneList[int(aIndexList.w)] * aWeightList.w;
  vec2 position = (bone_transform * vec4(aPosition, 0, 1)).xy;
  gl_Position = uPositionTransform * vec4(position, 0, 1);
}
