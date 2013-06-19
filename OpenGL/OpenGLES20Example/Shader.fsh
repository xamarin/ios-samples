uniform sampler2D texture;
varying mediump vec2 fragmentTextureCoordinates;
void main()
{
   gl_FragColor = texture2D(texture, fragmentTextureCoordinates);
}
