using System;
using CoreGraphics;

using OpenTK.Graphics.ES20;
using OpenTK;

using OpenGLES;
using CoreAnimation;
using Foundation;
using GLKit;
using UIKit;

namespace OpenGLScroller
{
	public class CubeView : GLKView
	{
		const int NumLittleCubes = 20;
		const float LittleCubeWidth = (320 / 3);
		const float ScrollerHeight = LittleCubeWidth;
		const float UnitLittleCubeWidth = 2;

		int vertexArray;
		int vertexBuffer;

		float[] gCubeVertexData = 
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

		struct CubeInfo
		{
			public float Red;		
			public float Green;
			public float Blue;		
			public float XAxis;		
			public float YAxis;
			public float ZAxis;
			public float Speed;
			public float RotationRadians;
			
			public CubeInfo (float r, float g, float b, float x, float y, float z, float s, float rr)
			{
				Red = r;
				Green = g;
				Blue = b;
				XAxis = x;
				YAxis = y;
				ZAxis = z;
				Speed = s;
				RotationRadians = rr;
			}
		};

		CubeInfo [] littleCube = new CubeInfo [NumLittleCubes];
		CubeInfo bigCube = new CubeInfo ();
		CubeInfo bigCubeDirections = new CubeInfo ();

		CubeInfo minimums = new CubeInfo (0.1f, 0.1f, 0.1f, -1.0f, -1.0f, -1.0f, -0.5f, 0.0f);
		CubeInfo maximums = new CubeInfo (1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, (float) Math.PI * 2);
		CubeInfo deltas = new CubeInfo (0.02f, 0.018f, 0.016f, 0.01f, 0.02f, 0.03f, 0.01f, 0.0f);

		double timeOfLastRenderedFrame;
		public CGPoint scrollOffset;

		GLKBaseEffect effect;
		Random gen = new Random();

		public CubeView (CGRect frame) : base (frame)
		{
			SetupGL ();
		}

		public CGRect ScrollableFrame {
			get {
				return new CGRect (0, 30, 320, (int)ScrollerHeight);
			}
		}

		public CGSize ScrollableContentSize {
			get {
				float width = NumLittleCubes * LittleCubeWidth;
				return new CGSize ((int)width, (int)ScrollerHeight);
			}
		}

		public CGPoint GetScrollOffset (CGPoint offset)
		{
			nfloat fractionalPart = offset.X % LittleCubeWidth;

			bool roundDown = fractionalPart < (LittleCubeWidth / 2);

			if (roundDown) {
				offset.X -= (int) fractionalPart;
			} else {
				offset.X += (int) (LittleCubeWidth - fractionalPart);
			}

			return offset;
		}


		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				base.Frame = value;

				float aspect = (float) Math.Abs (Bounds.Width / Bounds.Height);
				
				Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (65), aspect, 0.1f, 100);
				if (effect != null)
					effect.Transform.ProjectionMatrix = projectionMatrix;
			}
		}

		public override void Draw (CGRect rect)
		{
			UpdateCubes ();

			GL.ClearColor (0.15f, 0.15f, 0.15f, 1);
			GL.Clear (ClearBufferMask.ColorBufferBit);
			GL.Clear (ClearBufferMask.DepthBufferBit);

			GL.Oes.BindVertexArray (vertexArray);

			for (int i = 0; i < NumLittleCubes; i++) {
				var cube = littleCube [i];

				float translationX = (float)(((i - 1) * UnitLittleCubeWidth) - (scrollOffset.X * UnitLittleCubeWidth / LittleCubeWidth));
				var cubeMatrix = Matrix4.CreateTranslation (translationX, 2.8f, -7);
				cubeMatrix = Matrix4.Mult (Matrix4.CreateFromAxisAngle (new Vector3 (cube.XAxis, cube.YAxis, cube.ZAxis), cube.RotationRadians), cubeMatrix);

				effect.Light0.DiffuseColor = new Vector4 (cube.Red, cube.Green, cube.Blue, 1.0f);
				effect.Transform.ModelViewMatrix = cubeMatrix;

				effect.PrepareToDraw();

				GL.DrawArrays (BeginMode.Triangles, 0, 36);
			}

			var bigCubeMatrix = Matrix4.CreateTranslation (0, -0.5f, -3);

			bigCubeMatrix = Matrix4.Mult (Matrix4.CreateFromAxisAngle (new Vector3 (bigCube.XAxis, bigCube.YAxis, bigCube.ZAxis), bigCube.RotationRadians), bigCubeMatrix);

			effect.Light0.DiffuseColor = new Vector4 (bigCube.Red, bigCube.Green, bigCube.Blue, 0.5f);
			effect.Transform.ModelViewMatrix = bigCubeMatrix;

			effect.PrepareToDraw ();

			GL.DrawArrays (BeginMode.Triangles, 0, 36);
		}

		public override void TouchesBegan (NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesCancelled (NSSet touches, UIKit.UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIKit.UIEvent evt)
		{
			var touch = (UITouch) touches.AnyObject;
			if (touch.TapCount == 1) {
				var endTouch = new CGPoint (touch.LocationInView (this).X, touch.LocationInView (this).Y);
				HandleTapAtPoint (endTouch);
			}
		}

		public void HandleTapAtPoint (CGPoint point)
		{
			var bigCubeRect = new CGRect (70, 210, 180, 180);
			CGRect scrollviewRect = ScrollableFrame;

			if (bigCubeRect.Contains (point.X, point.Y)) {
				HandleBigCubeTap ();
			} else if (scrollviewRect.Contains (point.X, point.Y)) {
				var adjustedX = point.X + scrollOffset.X;

				int cubeIndex = (int) Math.Floor (adjustedX / LittleCubeWidth);
				HandleLittleCubeTap (cubeIndex);
			}
		}

		public void HandleBigCubeTap ()
		{
			RandomizeBigCube ();
		}

		public void HandleLittleCubeTap (int index)
		{
			littleCube [index] = bigCube;
		}


		public void SetupGL ()
		{
			Context = new EAGLContext (EAGLRenderingAPI.OpenGLES2);
			DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;

			effect = new GLKBaseEffect ();
			effect.Light0.Enabled = true;

			EAGLContext.SetCurrentContext (Context);

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

			RandomizeBigCube ();

			for (int i = 0; i< NumLittleCubes; i++)
				littleCube[i] = bigCube;

			timeOfLastRenderedFrame = CAAnimation.CurrentMediaTime();
		}

		public void RandomizeBigCube()
		{
			bigCube.Red = (minimums.Red + ((float) gen.NextDouble() * (maximums.Red - minimums.Red)));
			bigCube.Green = (minimums.Green + ((float) gen.NextDouble() * (maximums.Green - minimums.Green)));
			bigCube.Blue = (minimums.Blue + ((float) gen.NextDouble() * (maximums.Blue - minimums.Blue)));
			bigCube.XAxis = (minimums.XAxis + ((float) gen.NextDouble() * (maximums.XAxis - minimums.XAxis)));
			bigCube.YAxis = (minimums.YAxis + ((float) gen.NextDouble() * (maximums.YAxis - minimums.YAxis)));
			bigCube.ZAxis = (minimums.ZAxis + ((float) gen.NextDouble() * (maximums.ZAxis - minimums.ZAxis)));
			bigCube.Speed = (minimums.Speed + ((float) gen.NextDouble() * (maximums.Speed - minimums.Speed)));
			bigCube.RotationRadians = (minimums.RotationRadians + ((float) gen.NextDouble() * (maximums.RotationRadians - minimums.RotationRadians)));

			bigCubeDirections.Red = PositiveOrNegative();
			bigCubeDirections.Green = PositiveOrNegative();
			bigCubeDirections.Blue = PositiveOrNegative();
			bigCubeDirections.XAxis = PositiveOrNegative();
			bigCubeDirections.YAxis = PositiveOrNegative();
			bigCubeDirections.ZAxis = PositiveOrNegative();
			bigCubeDirections.Speed = PositiveOrNegative();
			bigCubeDirections.RotationRadians = 0.0f;
		}

		public void UpdateCubes ()
		{
			double elapsedTime = CAAnimation.CurrentMediaTime () - timeOfLastRenderedFrame;

			bigCube.Red = bigCube.Red + (bigCubeDirections.Red * deltas.Red * (float) elapsedTime);
			if (bigCube.Red < minimums.Red || bigCube.Red > maximums.Red) {
				bigCube.Red = Math.Max (minimums.Red, Math.Min (maximums.Red, bigCube.Red));
				bigCubeDirections.Red *= -1;
			}

			bigCube.Green = bigCube.Green + (bigCubeDirections.Green * deltas.Green * (float) elapsedTime);
			if (bigCube.Green < minimums.Green || bigCube.Green > maximums.Green) {
				bigCube.Green = Math.Max (minimums.Green, Math.Min (maximums.Green, bigCube.Green));
				bigCubeDirections.Green *= -1;
			}

			bigCube.Blue = bigCube.Blue + (bigCubeDirections.Blue * deltas.Blue * (float) elapsedTime);
			if (bigCube.Blue < minimums.Blue || bigCube.Blue > maximums.Blue) {
				bigCube.Blue = Math.Max (minimums.Blue, Math.Min (maximums.Blue, bigCube.Blue));
				bigCubeDirections.Blue *= -1;
			}

			bigCube.XAxis = bigCube.XAxis + (bigCubeDirections.XAxis * deltas.XAxis * (float) elapsedTime);
			if (bigCube.XAxis < minimums.XAxis || bigCube.XAxis > maximums.XAxis) {
				bigCube.XAxis = Math.Max (minimums.XAxis, Math.Min (maximums.XAxis, bigCube.XAxis));
				bigCubeDirections.XAxis *= -1;
			}

			bigCube.YAxis = bigCube.YAxis + (bigCubeDirections.YAxis * deltas.YAxis * (float) elapsedTime);
			if (bigCube.YAxis < minimums.YAxis || bigCube.YAxis > maximums.YAxis) {
				bigCube.YAxis = Math.Max (minimums.YAxis, Math.Min (maximums.YAxis, bigCube.YAxis));
				bigCubeDirections.YAxis *= -1;
			}

			bigCube.ZAxis = bigCube.ZAxis + (bigCubeDirections.ZAxis * deltas.ZAxis * (float) elapsedTime);
			if (bigCube.ZAxis < minimums.ZAxis || bigCube.Red > maximums.ZAxis) {
				bigCube.ZAxis = Math.Max (minimums.ZAxis, Math.Min (maximums.ZAxis, bigCube.ZAxis));
				bigCubeDirections.ZAxis *= -1;
			}

			bigCube.Speed = bigCube.Speed + (bigCubeDirections.Speed * deltas.Speed * (float) elapsedTime);
			if (bigCube.Speed < minimums.Speed || bigCube.Speed > maximums.Speed) {
				bigCube.Speed = Math.Max (minimums.Speed, Math.Min (maximums.Speed, bigCube.Speed));
				bigCubeDirections.Speed *= -1;
			}

			bigCube.RotationRadians = UpdatedRotationRadians (bigCube.RotationRadians, bigCube.Speed, elapsedTime);

			for (int i = 0; i < NumLittleCubes; i++)
				littleCube [i].RotationRadians = UpdatedRotationRadians (littleCube [i].RotationRadians, littleCube [i].Speed, elapsedTime);

			timeOfLastRenderedFrame = CAAnimation.CurrentMediaTime ();
		}

		public float UpdatedRotationRadians (float radians, float speed, double elapsedTime)
		{
			float speedInRadians = speed * (float) Math.PI * 2;
			float radiansDelta = speedInRadians * (float) elapsedTime;
			return (radians + radiansDelta) % ((float) Math.PI * 2);
		}

		public float PositiveOrNegative ()
		{
			bool temp = gen.Next (100) % 2 == 0;
			return temp ? 1.0f : -1.0f;
		}
	}
}
