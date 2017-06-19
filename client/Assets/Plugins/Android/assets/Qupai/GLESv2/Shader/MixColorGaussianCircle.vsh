attribute vec2 aPosition;
attribute vec4 aTexCoord0;

uniform mat4 uPositionTransform;

varying vec2 blurCoordinates[5];

void main()
{

  gl_Position = uPositionTransform * vec4(aPosition, 0.0, 1.0);

  vec2 singleStepOffset = vec2(2.0/480.0, 2.0/480.0);

  blurCoordinates[0] = aTexCoord0.xy;
  blurCoordinates[1] = aTexCoord0.xy + singleStepOffset * 1.407333;
  blurCoordinates[2] = aTexCoord0.xy - singleStepOffset * 1.407333;
  blurCoordinates[3] = aTexCoord0.xy + singleStepOffset * 3.294215;
  blurCoordinates[4] = aTexCoord0.xy - singleStepOffset * 3.294215;

}
