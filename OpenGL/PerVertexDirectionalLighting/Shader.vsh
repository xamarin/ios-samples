attribute vec4 position;
attribute vec2 textureCoordinate;
attribute vec4 normal;

uniform mat4 matrix;

uniform vec4 lightDirection;
uniform vec4 lightDiffuseColor;

varying vec2 fragmentTextureCoordinates; 
varying vec4 frontColor;
void main()
{

    vec4 normalizedNormal = normalize(matrix * normal);
    vec4 normalizedLightDirection = normalize(lightDirection);
    
    float nDotL = max(dot(normalizedNormal, normalizedLightDirection), 0.0);
    
    frontColor = nDotL * lightDiffuseColor;

    gl_Position = matrix * position;
    fragmentTextureCoordinates = textureCoordinate;
    

    
}