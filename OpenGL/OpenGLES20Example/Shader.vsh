attribute vec4 position;
attribute vec2 textureCoordinates;
uniform mat4 matrix;
varying vec2 fragmentTextureCoordinates; 
void main()
{
    gl_Position = matrix * position;
    fragmentTextureCoordinates = textureCoordinates;
}