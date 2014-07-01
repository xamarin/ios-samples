using System;
using UIKit;
using OpenTK;
using OpenGLES;
using GLKit;
using MonoTouch;
using OpenTK.Graphics.ES20;

using CoreGraphics;
using Foundation;

namespace GLKBaseEffectDrawingTexture
{
	public class MCViewController : GLKViewController
	{
		float rotation;

		uint vertexArray;
		uint vertexBuffer;

		EAGLContext context;
		GLKBaseEffect effect;
		GLKTextureInfo texture;

		public MCViewController ()
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			if (context == null)
				Console.WriteLine ("Failed to create ES context");

			GLKView view = View as GLKView;
			view.Context = context;
			view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
			view.DrawInRect += Draw;

			setupGL ();
		}

		void setupGL ()
		{
			EAGLContext.SetCurrentContext (context);

			effect = new GLKBaseEffect ();
			effect.LightingType = GLKLightingType.PerPixel;

			effect.Light0.Enabled = true;
			effect.Light0.DiffuseColor = new Vector4 (1.0f, 0.4f, 0.4f, 1.0f);

			GL.Enable (EnableCap.DepthTest);

			GL.Oes.GenVertexArrays (1, out vertexArray);
			GL.Oes.BindVertexArray (vertexArray);

			GL.GenBuffers (1, out vertexBuffer);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr) (Monkey.MeshVertexData.Length * sizeof (float)), 
			               Monkey.MeshVertexData, BufferUsage.StaticDraw);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Position);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Position, 3, VertexAttribPointerType.Float,
			                        false, 8 * sizeof (float), 0);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Normal);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Normal, 3, VertexAttribPointerType.Float,
			                        false, 8 * sizeof(float), 12);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.TexCoord0);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.TexCoord0, 2, VertexAttribPointerType.Float,
			                        false, 8 * sizeof(float), 24);

			GL.ActiveTexture (TextureUnit.Texture0);
			string path = NSBundle.MainBundle.PathForResource ("monkey", "png");

			NSError error;
			NSDictionary options = NSDictionary.FromObjectAndKey (NSNumber.FromBoolean (true),
			                                                      GLKTextureLoader.OriginBottomLeft);

			texture = GLKTextureLoader.FromFile (path, options, out error);

			if (texture == null)
				Console.WriteLine (String.Format("Error loading texture: {0}", error.LocalizedDescription));

			GLKEffectPropertyTexture tex = new GLKEffectPropertyTexture ();
			tex.Enabled = true;
			tex.EnvMode = GLKTextureEnvMode.Decal;
			tex.GLName = texture.Name;

			effect.Texture2d0.GLName = tex.GLName;

			GL.Oes.BindVertexArray (0);
		}

		public override void Update ()
		{
			float aspect = (float)Math.Abs (View.Bounds.Size.Width / View.Bounds.Size.Height);

			Matrix4 projectionMatrix = 
				Matrix4.CreatePerspectiveFieldOfView ((float) (Math.PI * 65f / 180.0f),
				                                      aspect, 0.1f, 100.0f);

			effect.Transform.ProjectionMatrix = projectionMatrix;

			Matrix4 modelViewMatrix = Matrix4.CreateTranslation (new Vector3 (0f, 0f, -3.5f));
			modelViewMatrix = Matrix4.Mult (Matrix4.CreateFromAxisAngle (new Vector3 (1f, 1f, 1f), rotation), modelViewMatrix);

			effect.Transform.ModelViewMatrix = modelViewMatrix;

			rotation += (float) TimeSinceLastUpdate * 0.5f;
		}

		public void Draw (object sender, GLKViewDrawEventArgs args)
		{
			GL.ClearColor (0.65f, 0.65f, 0.65f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Oes.BindVertexArray (vertexArray);

			effect.PrepareToDraw ();

			GL.DrawArrays (BeginMode.Triangles, 0, Monkey.MeshVertexData.Length / 8);
		}
	}
}

