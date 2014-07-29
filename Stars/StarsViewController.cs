using System;
using System.Drawing;
using System.Collections.Generic;

using UIKit;
using GLKit;
using CoreMotion;
using Foundation;
using OpenGLES;

using OpenTK.Graphics.ES20;
using OpenTK;

namespace Stars
{
	public class StarsViewController : GLKViewController
	{
		//comment
		const double RadiansToDegrees = (180 / Math.PI);
		const int NumCubes = 11;
		const int NumStars = 400;
		const float MinStarHeight = 100;
		const float NearZ = 0.1f;
		const float FarZ = 1000;

		Vector3[] star = new Vector3 [NumStars];
		int[] perm = new int [NumStars];

		static readonly float[] gCubeVertexData = 
		{
			// Data layout for each line below is:
			// positionX, positionY, positionZ,     normalX, normalY, normalZ,
			0.5f, -0.5f, -0.5f,        1.0f, 0.0f, 0.0f,
			0.5f, 0.5f, -0.5f,         1.0f, 0.0f, 0.0f,
			0.5f, -0.5f, 0.5f,         1.0f, 0.0f, 0.0f,
			0.5f, -0.5f, 0.5f,         1.0f, 0.0f, 0.0f,
			0.5f, 0.5f, -0.5f,         1.0f, 0.0f, 0.0f,
			0.5f, 0.5f, 0.5f,          1.0f, 0.0f, 0.0f,
			
			0.5f, 0.5f, -0.5f,         0.0f, 1.0f, 0.0f,
			-0.5f, 0.5f, -0.5f,        0.0f, 1.0f, 0.0f,
			0.5f, 0.5f, 0.5f,          0.0f, 1.0f, 0.0f,
			0.5f, 0.5f, 0.5f,          0.0f, 1.0f, 0.0f,
			-0.5f, 0.5f, -0.5f,        0.0f, 1.0f, 0.0f,
			-0.5f, 0.5f, 0.5f,         0.0f, 1.0f, 0.0f,
			
			-0.5f, 0.5f, -0.5f,        -1.0f, 0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,       -1.0f, 0.0f, 0.0f,
			-0.5f, 0.5f, 0.5f,         -1.0f, 0.0f, 0.0f,
			-0.5f, 0.5f, 0.5f,         -1.0f, 0.0f, 0.0f,
			-0.5f, -0.5f, -0.5f,       -1.0f, 0.0f, 0.0f,
			-0.5f, -0.5f, 0.5f,        -1.0f, 0.0f, 0.0f,
			
			-0.5f, -0.5f, -0.5f,       0.0f, -1.0f, 0.0f,
			0.5f, -0.5f, -0.5f,        0.0f, -1.0f, 0.0f,
			-0.5f, -0.5f, 0.5f,        0.0f, -1.0f, 0.0f,
			-0.5f, -0.5f, 0.5f,        0.0f, -1.0f, 0.0f,
			0.5f, -0.5f, -0.5f,        0.0f, -1.0f, 0.0f,
			0.5f, -0.5f, 0.5f,         0.0f, -1.0f, 0.0f,
			
			0.5f, 0.5f, 0.5f,          0.0f, 0.0f, 1.0f,
			-0.5f, 0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
			0.5f, -0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
			0.5f, -0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
			-0.5f, 0.5f, 0.5f,         0.0f, 0.0f, 1.0f,
			-0.5f, -0.5f, 0.5f,        0.0f, 0.0f, 1.0f,
			
			0.5f, -0.5f, -0.5f,        0.0f, 0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,       0.0f, 0.0f, -1.0f,
			0.5f, 0.5f, -0.5f,         0.0f, 0.0f, -1.0f,
			0.5f, 0.5f, -0.5f,         0.0f, 0.0f, -1.0f,
			-0.5f, -0.5f, -0.5f,       0.0f, 0.0f, -1.0f,
			-0.5f, 0.5f, -0.5f,        0.0f, 0.0f, -1.0f
		};

		static readonly float[] gColorData =
		{
			255.0f, 51.0f, 51.0f,
			255.0f, 153.0f, 51.0f,
			255.0f, 255.0f, 51.0f,
			153.0f, 255.0f, 51.0f,
			51.0f, 255.0f, 51.0f,
			51.0f, 255.0f, 153.0f,
			51.0f, 255.0f, 255.0f,
			51.0f, 153.0f, 255.0f,
			51.0f, 51.0f, 255.0f,
			153.0f, 51.0f, 255.0f,
			153.0f, 153.0f, 255.0f
		};

		float rotation;
		int vertexArray;
		int vertexBuffer;
		UILabel rpyLabel;
		CMMotionManager motionManager;
		bool isDeviceMotionAvailable;
		EAGLContext context;
		List<GLKBaseEffect> effects;

		Random gen = new Random ();

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);

			if (context == null)
				Console.WriteLine ("Failed to create ES context");

			var view = (GLKView) View;
			view.DrawInRect += Draw;
			view.Context = context;
			view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;

			// for planet positions
			generateRandomPermutation ();

			// for star positions
			for (int i = 0; i < NumStars; i++) {
				star [i].X = (float) (gen.NextDouble () - .5f) * FarZ;
				star [i].Y = (float) (gen.NextDouble () - .5f) * FarZ;
				star [i].Z = (float) gen.NextDouble () * (FarZ - MinStarHeight) + MinStarHeight;
			}

			setupGL ();

			motionManager = new CMMotionManager ();
			isDeviceMotionAvailable = motionManager.DeviceMotionAvailable;

			// the label for roll, pitch and yaw reading
			rpyLabel = new UILabel (new Rectangle (0, 0, (int) UIScreen.MainScreen.Bounds.Size.Width, 30));

			rpyLabel.BackgroundColor = UIColor.Clear;
			rpyLabel.TextColor = UIColor.White;
			rpyLabel.TextAlignment = UITextAlignment.Center;
			rpyLabel.LineBreakMode = UILineBreakMode.WordWrap;
			rpyLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			View.AddSubview (rpyLabel);
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			tearDownGL ();

			if (EAGLContext.CurrentContext == context)
				EAGLContext.SetCurrentContext (null);

			context = null;
			rpyLabel = null;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (isDeviceMotionAvailable == true) {
				motionManager.DeviceMotionUpdateInterval = .01;
				motionManager.StartDeviceMotionUpdates ();
			} else {
				Console.WriteLine ("Device motion is not available on device");
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			if (motionManager.DeviceMotionActive)
				motionManager.StopDeviceMotionUpdates ();
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}


		public void setupGL ()
		{
			EAGLContext.SetCurrentContext (context);

			effects = new List<GLKBaseEffect> ();

			for (int i = 0; i < NumCubes; i++) {
				var effect = new GLKBaseEffect ();
				effect.Light0.Enabled = true;
				effect.Light0.DiffuseColor = new Vector4 (gColorData [3 * i] / 255.0f,
				                                          gColorData [3 * i + 1] / 255.0f,
				                                          gColorData [3 * i + 2] / 255.0f,
				                                          1);
				effects.Add (effect);
			}

			for (int i = 0; i < NumStars; i++) {
				var effect = new GLKBaseEffect ();
				effect.Light0.Enabled = true;
				effects.Add (effect);
			}

			GL.Enable (EnableCap.DepthTest);

			GL.Oes.GenVertexArrays (1, out vertexArray);
			GL.Oes.BindVertexArray (vertexArray);

			GL.GenBuffers (1, out vertexBuffer);
			GL.BindBuffer (BufferTarget.ArrayBuffer, vertexBuffer);

			GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr) (gCubeVertexData.Length * sizeof (float)), gCubeVertexData, BufferUsage.StaticDraw);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Position);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Position, 3, VertexAttribPointerType.Float, false, 24, 0);

			GL.EnableVertexAttribArray ((int) GLKVertexAttrib.Normal);
			GL.VertexAttribPointer ((int) GLKVertexAttrib.Normal, 3, VertexAttribPointerType.Float, false, 24, 12);

			GL.Oes.BindVertexArray (0);
		}

		public void tearDownGL ()
		{
			EAGLContext.SetCurrentContext (context);

			GL.DeleteBuffers (1, ref vertexBuffer);
			GL.Oes.DeleteVertexArrays (1, ref vertexArray);

			effects = null;
		}

		public override void Update ()
		{
			// we need to ensure device motion is available on device to continue
			if (!isDeviceMotionAvailable)
				return;

			var dm = motionManager.DeviceMotion;

			// in case we don't have any sample yet, simply return...
			if (dm == null)
				return;

			var r = dm.Attitude.RotationMatrix;

			float aspect = (float)Math.Abs (View.Bounds.Size.Width / View.Bounds.Size.Height);
			var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (65.0f), aspect, NearZ, FarZ);

			Matrix4 baseModelViewMatrix = new Matrix4 ((float) r.m11, (float) r.m21, (float) r.m31, 0,
			                                           (float) r.m12, (float) r.m22, (float) r.m32, 0,
			                                           (float) r.m13, (float) r.m23, (float) r.m33, 0,
			                                    	   0, 0, 0, 1);

			// Compute the model view matrix for the objects rendered with the GLKIT
			// the planets
			for (int i = 0; i < NumCubes; i++) {
				var effect = effects [i];
				effect.Transform.ProjectionMatrix = projectionMatrix;
				var modelViewMatrix = Matrix4.Identity; 

				modelViewMatrix = Matrix4.Mult (modelViewMatrix, Matrix4.CreateTranslation ((float) perm [i] * 5.0f + 10.0f, (float) i, 0)); 
				modelViewMatrix = Matrix4.Mult (modelViewMatrix, Matrix4.CreateFromAxisAngle (new Vector3 (0, 0, 1), (float) (2 * Math.PI * i / (float) NumCubes)));
				modelViewMatrix = Matrix4.Mult (Matrix4.CreateFromAxisAngle (new Vector3 (1, 1, 1), rotation), modelViewMatrix);

				modelViewMatrix = Matrix4.Mult (modelViewMatrix, baseModelViewMatrix);

				effect.Transform.ModelViewMatrix = modelViewMatrix;
			}

			// ...and the stars
			for (int j = 0; j < NumStars; j++) {
				var effect = effects [NumCubes + j];
				effect.Transform.ProjectionMatrix = projectionMatrix;
				var modelViewMatrix = Matrix4.CreateTranslation (star [j].X, star [j].Y, star [j].Z);

				modelViewMatrix = Matrix4.Mult (modelViewMatrix, baseModelViewMatrix);

				effect.Transform.ModelViewMatrix = modelViewMatrix;
			}

			rotation += (float) TimeSinceLastUpdate * 0.8f;

			rpyLabel.Text = String.Format ("roll: {0}° pitch: {1}° yaw: {2}°", 
			                               String.Format("{0:0.#}", dm.Attitude.Roll * RadiansToDegrees),
			                               String.Format("{0:0.#}", dm.Attitude.Pitch * RadiansToDegrees), 
			                               String.Format("{0:0.#}", dm.Attitude.Yaw * RadiansToDegrees));
			//...now we can print out coordinates
		}
		 
		public void Draw (object sender, GLKViewDrawEventArgs args)
		{
			if (!isDeviceMotionAvailable)
				return;

			GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Oes.BindVertexArray (vertexArray);

			// Render the objects with GLKit
			for (int i = 0; i < (NumCubes + NumStars); i++) {
				var effect = effects [i];
				effect.PrepareToDraw();
				GL.DrawArrays (BeginMode.Triangles, 0, 36);
			}
		}

		public void generateRandomPermutation ()
		{
			for (int i = 0; i < NumCubes; i++)
				perm [i] = i + 1;

			for (int i = 0; i < NumCubes; i++) {
				int j = gen.Next () % (i + 1);
				perm [i] = perm [j];
				perm [j] = i;
			}
		}
	}
}
