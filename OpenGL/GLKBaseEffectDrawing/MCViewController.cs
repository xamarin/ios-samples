using System;
using UIKit;
using OpenTK;
using OpenGLES;
using GLKit;
using MonoTouch;
using OpenTK.Graphics.ES20;

using CoreGraphics;

namespace GLKBaseEffectDrawing
{
	public class MCViewController : GLKViewController
	{
		float rotation;
		uint vertexArray;
		uint vertexBuffer;
		EAGLContext context;
		GLKBaseEffect effect;

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
			effect.Light0.Position = new Vector4 (-5f, -5f, 10f, 1f);
			effect.Light0.SpecularColor = new Vector4 (1f, 0f, 0f, 1f);

			effect.Light1.Enabled = true;
			effect.Light1.DiffuseColor = new Vector4 (1f, 0.4f, 0.4f, 1f);
			effect.Light1.Position = new Vector4 (15f, 15f, 10f, 1f);
			effect.Light1.SpecularColor = new Vector4 (1f, 0f, 0f, 1f);

			effect.Material.DiffuseColor = new Vector4 (0f, 0.5f, 1f, 1f);
			effect.Material.AmbientColor = new Vector4 (0f, 0.5f, 0f, 1f);
			effect.Material.SpecularColor = new Vector4 (1f, 0f, 0f, 1f);
			effect.Material.Shininess = 20f;
			effect.Material.EmissiveColor = new Vector4 (0.2f, 0f, 0.2f, 1f);

			GL.Enable (EnableCap.DepthTest);

			GL.Oes.GenVertexArrays (1, out vertexArray);
			GL.Oes.BindVertexArray (vertexArray);

			GL.GenBuffers (1, out vertexBuffer);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(Monkey.MeshVertexData.Length * sizeof(float)), 
			               Monkey.MeshVertexData, BufferUsage.StaticDraw);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Position);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Position, 3, VertexAttribPointerType.Float,
			                        false, 6 * sizeof(float), 0);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Normal);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Normal, 3, VertexAttribPointerType.Float,
			                        false, 6 * sizeof(float), 12);

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

			rotation += (float)TimeSinceLastUpdate * 0.5f;
		}

		public void Draw (object sender, GLKViewDrawEventArgs args)
		{
			GL.ClearColor (0.65f, 0.65f, 0.65f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Oes.BindVertexArray (vertexArray);

			effect.PrepareToDraw ();

			GL.DrawArrays (BeginMode.Triangles, 0, Monkey.MeshVertexData.Length / 6);
		}
	}
}

