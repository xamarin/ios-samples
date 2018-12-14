using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using GLCameraRipple.Helpers;
using GLKit;
using OpenGLES;
using OpenTK.Graphics.ES20;
using System;
using UIKit;

namespace GLCameraRipple
{
    public partial class ViewController : GLKViewController
    {
        // OpenGL components
        private const int UNIFORM_Y = 0;
        private const int UNIFORM_UV = 1;
        private const int ATTRIB_VERTEX = 0;
        private const int ATTRIB_TEXCOORD = 1;

        private int indexVbo, positionVbo, texcoordVbo;
        private int[] uniforms = new int[2];
        private int program;

        private EAGLContext context;
        private CGSize size;
        private int meshFactor;

        private AVCaptureSession session;

        private DataOutputDelegate dataOutputDelegate;

        protected ViewController(IntPtr handle) : base(handle) { }

        public RippleModel Ripple { get; private set; }

        public CVOpenGLESTextureCache VideoTextureCache { get; private set; }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
            var glkView = View as GLKView;
            glkView.Context = context;
            glkView.MultipleTouchEnabled = true;

            PreferredFramesPerSecond = 60;
            size = UIScreen.MainScreen.Bounds.Size.ToRoundedCGSize();
            View.ContentScaleFactor = UIScreen.MainScreen.Scale;

            var isPad = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
            meshFactor = isPad ? 8 : 4;
            SetupGL();
            SetupAVCapture(isPad ? AVCaptureSession.PresetiFrame1280x720 : AVCaptureSession.Preset640x480);
        }

        public override void Update()
        {
            if (this.Ripple != null)
            {
                this.Ripple.RunSimulation();
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)this.Ripple.VertexSize, this.Ripple.TexCoords, BufferUsage.DynamicDraw);
            }
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            if (this.Ripple != null)
            {
                GL.DrawElements(BeginMode.TriangleStrip, this.Ripple.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }
        }

        #region touches

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            this.ProcessTouches(touches);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            this.ProcessTouches(touches);
        }

        private void ProcessTouches(NSSet touches)
        {
            if (this.Ripple != null)
            {
                foreach (UITouch touch in touches.ToArray<UITouch>())
                {
                    this.Ripple.InitiateRippleAtLocation(touch.LocationInView(touch.View));
                }
            }
        }

        #endregion

        #region setup 

        private void SetupGL()
        {
            EAGLContext.SetCurrentContext(this.context);
            if (this.LoadShaders())
            {
                GL.UseProgram(this.program);
                GL.Uniform1(this.uniforms[UNIFORM_Y], 0);
                GL.Uniform1(this.uniforms[UNIFORM_UV], 1);
            }
        }

        private void SetupAVCapture(NSString sessionPreset)
        {
            if ((this.VideoTextureCache = CVOpenGLESTextureCache.FromEAGLContext(this.context)) == null)
            {
                Console.WriteLine("Could not create the CoreVideo TextureCache");
                return;
            }

            this.session = new AVCaptureSession();
            this.session.BeginConfiguration();

            // Preset size
            this.session.SessionPreset = sessionPreset;

            // Input device
            var videoDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevice == null)
            {
                Console.WriteLine("No video device");
                return;
            }

            var input = new AVCaptureDeviceInput(videoDevice, out NSError error);
            if (error != null)
            {
                Console.WriteLine("Error creating video capture device");
                return;
            }

            this.session.AddInput(input);

            // Create the output device
            using (var dataOutput = new AVCaptureVideoDataOutput())
            {
                dataOutput.AlwaysDiscardsLateVideoFrames = true;

                // YUV 420, use "BiPlanar" to split the Y and UV planes in two separate blocks of
                // memory, then we can index 0 to get the Y and 1 for the UV planes in the frame decoding
                //VideoSettings = new AVVideoSettings (CVPixelFormatType.CV420YpCbCr8BiPlanarFullRange)

                this.dataOutputDelegate = new DataOutputDelegate(this);

                //
                // This dispatches the video frames into the main thread, because the OpenGL
                // code is accessing the data synchronously.
                //
                dataOutput.SetSampleBufferDelegateQueue(this.dataOutputDelegate, DispatchQueue.MainQueue);
                this.session.AddOutput(dataOutput);
            }

            this.session.CommitConfiguration();
            this.session.StartRunning();
        }

        private bool LoadShaders()
        {
            this.program = GL.CreateProgram();
            if (this.CompileShader(out int vertShader, ShaderType.VertexShader, "Shaders/Shader.vsh"))
            {
                if (this.CompileShader(out int fragShader, ShaderType.FragmentShader, "Shaders/Shader.fsh"))
                {
                    // Attach shaders
                    GL.AttachShader(this.program, vertShader);
                    GL.AttachShader(this.program, fragShader);

                    // Bind attribtue locations
                    GL.BindAttribLocation(this.program, ATTRIB_VERTEX, "position");
                    GL.BindAttribLocation(this.program, ATTRIB_TEXCOORD, "texCoord");

                    if (this.LinkProgram(this.program))
                    {
                        // Get uniform locations
                        this.uniforms[UNIFORM_Y] = GL.GetUniformLocation(this.program, "SamplerY");
                        this.uniforms[UNIFORM_UV] = GL.GetUniformLocation(this.program, "SamplerUV");

                        // Delete these ones, we do not need them anymore
                        GL.DeleteShader(vertShader);
                        GL.DeleteShader(fragShader);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to link the shader programs");
                        GL.DeleteProgram(this.program);
                        this.program = 0;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to compile fragment shader");
                }

                GL.DeleteShader(vertShader);
            }
            else
            {
                Console.WriteLine("Failed to compile vertex shader");
            }

            GL.DeleteProgram(this.program);
            return false;
        }

        private bool CompileShader(out int shader, ShaderType type, string path)
        {
            var shaderProgram = System.IO.File.ReadAllText(path);
            var length = shaderProgram.Length;
            shader = GL.CreateShader(type);

            GL.ShaderSource(shader, 1, new string[] { shaderProgram }, ref length);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                GL.DeleteShader(shader);
                return false;
            }
            return true;
        }

        private bool LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, ProgramParameter.LinkStatus, out int status);
            if (status == 0)
            {
                GL.GetProgram(program, ProgramParameter.InfoLogLength, out int length);
                var stringBuilder = new System.Text.StringBuilder(length);
                GL.GetProgramInfoLog(program, length, out length, stringBuilder);

                Console.WriteLine($"Link error: {stringBuilder}");
            }

            return status != 0;
        }

        private unsafe void SetupBuffers()
        {
            GL.GenBuffers(1, out indexVbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexVbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)this.Ripple.IndexSize, this.Ripple.Indices, BufferUsage.StaticDraw);

            GL.GenBuffers(1, out positionVbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)this.Ripple.VertexSize, this.Ripple.Vertices, BufferUsage.StaticDraw);

            GL.EnableVertexAttribArray(ATTRIB_VERTEX);

            GL.VertexAttribPointer(ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
            GL.GenBuffers(1, out texcoordVbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoordVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)this.Ripple.VertexSize, this.Ripple.TexCoords, BufferUsage.DynamicDraw);

            GL.EnableVertexAttribArray(ATTRIB_TEXCOORD);
            GL.VertexAttribPointer(ATTRIB_TEXCOORD, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        }

        public void SetupRipple(int width, int height)
        {
            this.Ripple = new RippleModel(this.size, this.meshFactor, 5, new CGSize(width, height));
            this.SetupBuffers();
        }

        #endregion

        private void TeardownAVCapture() { }

        private void TeardownGL() { }
    }
}