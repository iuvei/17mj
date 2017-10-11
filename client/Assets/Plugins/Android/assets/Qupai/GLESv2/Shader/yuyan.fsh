precision mediump float;
varying mediump vec2 vTexCoord;

uniform sampler2D sTexture;

mediump vec2 center = vec2(0.5, 0.5);
mediump float radius = 0.71;
mediump float aspectRatio = 1.0;
mediump float refractiveIndex = 0.51;
// uniform vec3 lightPosition;
const mediump vec3 lightPosition = vec3(-0.5, 0.5, 1.0);
const mediump vec3 ambientLightPosition = vec3(0.0, 0.0, 1.0);

 void main()
 {
     mediump vec2 textureCoordinateToUse = vec2(vTexCoord.x, (vTexCoord.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
     mediump float distanceFromCenter = distance(center, textureCoordinateToUse);
     lowp float checkForPresenceWithinSphere = step(distanceFromCenter, radius);

     distanceFromCenter = distanceFromCenter / radius;

     mediump float normalizedDepth = radius * sqrt(1.0 - distanceFromCenter * distanceFromCenter);
     mediump vec3 sphereNormal = normalize(vec3(textureCoordinateToUse - center, normalizedDepth));

     mediump vec3 refractedVector = 2.0 * refract(vec3(0.0, 0.0, -1.0), sphereNormal, refractiveIndex);
     refractedVector.xy = -refractedVector.xy;

     mediump vec3 finalSphereColor = texture2D(sTexture, (refractedVector.xy + 1.0) * 0.5).rgb;

     // Grazing angle lighting
     mediump float lightingIntensity = 2.5 * (1.0 - pow(clamp(dot(ambientLightPosition, sphereNormal), 0.0, 1.0), 0.25));
     //finalSphereColor += lightingIntensity;

     // Specular lighting
     //lightingIntensity  = clamp(dot(normalize(lightPosition), sphereNormal), 0.0, 1.0);
     //lightingIntensity  = pow(lightingIntensity, 15.0);
     //finalSphereColor += vec3(0.8, 0.8, 0.8) * lightingIntensity;

     gl_FragColor = vec4(finalSphereColor, 1.0) * checkForPresenceWithinSphere;
 }
