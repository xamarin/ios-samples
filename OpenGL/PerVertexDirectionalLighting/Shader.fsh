uniform sampler2D texture;
varying mediump vec2 fragmentTextureCoordinates;
varying lowp vec4 frontColor;
void main()
{
   gl_FragColor = texture2D(texture, fragmentTextureCoordinates) * frontColor;
}
