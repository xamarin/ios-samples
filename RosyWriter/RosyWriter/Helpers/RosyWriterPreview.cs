using CoreAnimation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using ObjCRuntime;
using OpenGLES;
using OpenTK.Graphics.ES20;
using System;
using UIKit;

namespace RosyWriter
{
    public partial class RosyWriterPreview : UIView
    {
        // Open GL Stuff
        private const int UNIFORM_Y = 0;
        private const int UNIFORM_UV = 1;
        private const int ATTRIB_VERTEX = 0;
        private const int ATTRIB_TEXCOORD = 1;

        private readonly EAGLContext context;
        private CVOpenGLESTextureCache videoTextureCache;
        private uint frameBuffer, colorBuffer;
        private int renderBufferWidth, renderBufferHeight;

        private int glProgram;

        public RosyWriterPreview(IntPtr handle) : base(handle)
        {
            // Initialize OpenGL ES 2
            var eagleLayer = Layer as CAEAGLLayer;
            eagleLayer.Opaque = true;
            eagleLayer.DrawableProperties = new NSDictionary(EAGLDrawableProperty.RetainedBacking, false,
                                                             EAGLDrawableProperty.ColorFormat, EAGLColorFormat.RGBA8);

            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
            if (!EAGLContext.SetCurrentContext(context))
            {
                throw new ApplicationException("Could not set EAGLContext");
            }
        }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            return new Class(typeof(CAEAGLLayer));
        }

        #region Setup

        private bool CreateFrameBuffer()
        {
            var success = true;

            GL.Disable(EnableCap.DepthTest);

            GL.GenFramebuffers(1, out frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

            GL.GenRenderbuffers(1, out colorBuffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorBuffer);

            context.RenderBufferStorage((uint)All.Renderbuffer, (CAEAGLLayer)Layer);

            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferWidth, out renderBufferWidth);
            GL.GetRenderbufferParameter(RenderbufferTarget.Renderbuffer, RenderbufferParameterName.RenderbufferHeight, out renderBufferHeight);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, RenderbufferTarget.Renderbuffer, colorBuffer);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("Failure with framebuffer generation");
                success = false;
            }

            // Create a new CVOpenGLESTexture Cache
            videoTextureCache = CVOpenGLESTextureCache.FromEAGLContext(context);

            glProgram = CreateProgram();

            return success && (glProgram != 0);
        }

        private static int CreateProgram()
        {
            // Create shader program
            int program = GL.CreateProgram();

            // Create and Compile Vertex Shader
            int vertShader = 0;
            int fragShader = 0;
            var success = true;
            success = success && CompileShader(out vertShader, ShaderType.VertexShader, "Shaders/passThrough.vsh");

            // Create and Compile fragment shader
            success = success && CompileShader(out fragShader, ShaderType.FragmentShader, "Shaders/passThrough.fsh");

            // Attach Vertext Shader
            GL.AttachShader(program, vertShader);

            // Attach fragment shader
            GL.AttachShader(program, fragShader);

            // Bind attribute locations
            GL.BindAttribLocation(program, ATTRIB_VERTEX, "position");
            GL.BindAttribLocation(program, ATTRIB_TEXCOORD, "textureCoordinate");

            // Link program
            success = success && LinkProgram(program);
            if (success)
            {
                // Delete these ones, we do not need them anymore
                GL.DeleteShader(vertShader);
                GL.DeleteShader(fragShader);
            }
            else
            {
                Console.WriteLine("Failed to compile and link the shader programs");
                GL.DeleteProgram(program);
                program = 0;
            }

            return program;
        }

        private static bool LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, ProgramParameter.LinkStatus, out int status);

            if (status == 0)
            {
                GL.GetProgram(program, ProgramParameter.InfoLogLength, out int len);
                var sb = new System.Text.StringBuilder(len);
                GL.GetProgramInfoLog(program, len, out len, sb);
                Console.WriteLine("Link error: {0}", sb);
            }
            return status != 0;
        }

        private static bool CompileShader(out int shader, ShaderType type, string path)
        {
            string shaderProgram = System.IO.File.ReadAllText(path);
            int len = shaderProgram.Length;
            shader = GL.CreateShader(type);

            GL.ShaderSource(shader, 1, new[] { shaderProgram }, ref len);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);

            if (status == 0)
            {
                GL.DeleteShader(shader);
                return false;
            }

            return true;
        }

        #endregion

        #region Rendering

        public void DisplayPixelBuffer(CVImageBuffer imageBuffer)
        {
            // First check to make sure we have a FrameBuffer to write to.
            if (frameBuffer == 0)
            {
                var success = CreateFrameBuffer();
                if (!success)
                {
                    Console.WriteLine("Problem initializing OpenGL buffers.");
                    return;
                }
            }

            if (videoTextureCache == null)
            {
                Console.WriteLine("Video Texture Cache not initialized");
                return;
            }

            if (!(imageBuffer is CVPixelBuffer pixelBuffer))
            {
                Console.WriteLine("Could not get Pixel Buffer from Image Buffer");
                return;
            }

            // Create a CVOpenGLESTexture from the CVImageBuffer
            var frameWidth = (int)pixelBuffer.Width;
            var frameHeight = (int)pixelBuffer.Height;

            using (var texture = videoTextureCache.TextureFromImage(imageBuffer, true, All.Rgba, frameWidth, frameHeight, All.Bgra, DataType.UnsignedByte, 0, out CVReturn ret))
            {
                if (texture == null || ret != CVReturn.Success)
                {
                    Console.WriteLine("Could not create Texture from Texture Cache");
                    return;
                }

                GL.BindTexture(texture.Target, texture.Name);

                // Set texture parameters
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

                // Set the view port to the entire view
                GL.Viewport(0, 0, renderBufferWidth, renderBufferHeight);

                var squareVerticies = new float[,] 
                {
                     { -1.0F, -1.0F},
                     { 1.0F, -1.0F },
                     { -1.0F, 1.0F },
                     { 1.0F, 1.0F }
                };

                // The texture verticies are setup such that we flip the texture vertically.
                // This is so that our top left origin buffers match OpenGL's bottom left texture coordinate system.
                var textureSamplingRect = TextureSamplingRectForCroppingTextureWithAspectRatio(new CGSize(frameWidth, frameHeight), Bounds.Size);

                var textureVertices = new float[,]
                {
                    {(float)textureSamplingRect.Left, (float)textureSamplingRect.Bottom},
                    {(float)textureSamplingRect.Right, (float)textureSamplingRect.Bottom},
                    {(float)textureSamplingRect.Left, (float)textureSamplingRect.Top},
                    {(float)textureSamplingRect.Right, (float)textureSamplingRect.Top}
                };

                // Draw the texture on the screen with OpenGL ES 2
                RenderWithSquareVerticies(squareVerticies, textureVertices);

                GL.BindTexture(texture.Target, texture.Name);

                // Flush the CVOpenGLESTexture cache and release the texture
                videoTextureCache.Flush(CVOptionFlags.None);
            }
        }

        private static CGRect TextureSamplingRectForCroppingTextureWithAspectRatio(CGSize textureAspectRatio, CGSize croppingAspectRatio)
        {
            CGRect normalizedSamplingRect;
            var cropScaleAmount = new CGSize(croppingAspectRatio.Width / textureAspectRatio.Width, croppingAspectRatio.Height / textureAspectRatio.Height);
            var maxScale = Math.Max(cropScaleAmount.Width, cropScaleAmount.Height);

            // HACK: double to nfloat
            var scaledTextureSize = new CGSize((nfloat)(textureAspectRatio.Width * maxScale), (nfloat)(textureAspectRatio.Height * maxScale));

            // Changed the floats width, height to nfloats
            nfloat width, height;
            if (cropScaleAmount.Height > cropScaleAmount.Width)
            {
                width = croppingAspectRatio.Width / scaledTextureSize.Width;
                height = 1f;
                normalizedSamplingRect = new CGRect(0, 0, width, height);
            }
            else
            {
                height = croppingAspectRatio.Height / scaledTextureSize.Height;
                width = 1f;
                normalizedSamplingRect = new CGRect(0, 0, height, width);
            }

            // Center crop
            normalizedSamplingRect.X = (1f - normalizedSamplingRect.Size.Width) / 2f;
            normalizedSamplingRect.Y = (1f - normalizedSamplingRect.Size.Height) / 2f;

            return normalizedSamplingRect;
        }

        private void RenderWithSquareVerticies(float[,] squareVerticies, float[,] textureVerticies)
        {
            // Use Shader Program
            GL.UseProgram(glProgram);

            // Update attribute values
            GL.VertexAttribPointer(ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 0, squareVerticies);
            GL.EnableVertexAttribArray(ATTRIB_VERTEX);

            GL.VertexAttribPointer(ATTRIB_TEXCOORD, 2, VertexAttribPointerType.Float, false, 0, textureVerticies);
            GL.EnableVertexAttribArray(ATTRIB_TEXCOORD);

            // Validate program before drawing. (For Debugging purposes)
#if DEBUG
            GL.ValidateProgram(glProgram);
#endif
            GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);

            // Present
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorBuffer);
            context.PresentRenderBuffer((uint)All.Renderbuffer);
        }

        #endregion
    }
}