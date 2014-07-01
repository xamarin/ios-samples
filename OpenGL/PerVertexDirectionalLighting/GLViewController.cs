using System;
using OpenTK;
using UIKit;
using OpenTK.Graphics.ES20;

namespace PerVertexDirectionalLighting
{
	public class GLViewController : UIViewController
	{
		GLProgram program;
		GLTexture texture;

		float rot = 0f;

		int positionAttribute,
		    textureCoordinateAttribute,
		    normalsAttribute,
		    matrixUniform,
			textureUniform,
		    lightDirectionUniform,
			lightDiffuseColorUniform;

		float[] rotationMatrix = new float[16],
			    translationMatrix = new float[16],
			    modelViewMatrix = new float[16],
		        projectionMatrix = new float[16],
		        matrix = new float[16];

		public GLViewController ()
		{
		}

		public void Setup ()
		{
			program = new GLProgram ("Shader", "Shader");

			program.AddAttribute ("position");
			program.AddAttribute ("textureCoordinate");
			program.AddAttribute ("normalsAttribute");

			if (!program.Link ()) {
				Console.WriteLine ("Link failed.");
				Console.WriteLine (String.Format ("Program Log: {0}", program.ProgramLog ()));
				Console.WriteLine (String.Format ("Fragment Log: {0}", program.FragmentShaderLog ()));
				Console.WriteLine (String.Format ("Vertex Log: {0}", program.VertexShaderLog ()));

				(View as GLView).StopAnimation ();
				program = null;

				return;
			}

			positionAttribute = program.GetAttributeIndex ("position");
			textureCoordinateAttribute = program.GetAttributeIndex ("textureCoordinate");
			normalsAttribute = program.GetAttributeIndex ("normalsAttribute");

			matrixUniform = program.GetUniformIndex ("matrix");
			textureUniform = program.GetUniformIndex ("texture");
			lightDirectionUniform = program.GetUniformIndex ("lightDirection");
			lightDiffuseColorUniform = program.GetUniformIndex ("lightDiffuseColor");

			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.CullFace);
			GL.Enable (EnableCap.Texture2D);
			GL.Enable (EnableCap.Blend);
			GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.Zero);

			texture = new GLTexture ("DieTexture.png");
		}

		public void Draw ()
		{
			Vector3[] vertices = {
				new Vector3 { X = -0.276385f, Y = -0.850640f, Z = -0.447215f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f},  
				new Vector3 { X = 0.723600f, Y = -0.525720f, Z = -0.447215f}, 
				new Vector3 { X = 0.723600f, Y = -0.525720f, Z = -0.447215f}, 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f},  
				new Vector3 { X = 0.723600f, Y = 0.525720f, Z = -0.447215f},  
				new Vector3 { X = -0.894425f, Y = 0.000000f, Z = -0.447215f}, 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f},  
				new Vector3 { X = -0.276385f, Y = -0.850640f, Z = -0.447215f},
				new Vector3 { X = -0.276385f, Y = 0.850640f, Z = -0.447215f}, 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f},  
				new Vector3 { X = -0.894425f, Y = 0.000000f, Z = -0.447215f}, 
				new Vector3 { X = 0.723600f, Y = 0.525720f, Z = -0.447215f},  
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f},  
				new Vector3 { X = -0.276385f, Y = 0.850640f, Z = -0.447215f}, 
				new Vector3 { X = 0.723600f, Y = -0.525720f, Z = -0.447215f}, 
				new Vector3 { X = 0.723600f, Y = 0.525720f, Z = -0.447215f},  
				new Vector3 { X = 0.894425f, Y = 0.000000f, Z = 0.447215f}, 	 
				new Vector3 { X = -0.276385f, Y = -0.850640f, Z = -0.447215f},
				new Vector3 { X = 0.723600f, Y = -0.525720f, Z = -0.447215f}, 
				new Vector3 { X = 0.276385f, Y = -0.850640f, Z = 0.447215f},  
				new Vector3 { X = -0.894425f, Y = 0.000000f, Z = -0.447215f}, 
				new Vector3 { X = -0.276385f, Y = -0.850640f, Z = -0.447215f},
				new Vector3 { X = -0.723600f, Y = -0.525720f, Z = 0.447215f}, 
				new Vector3 { X = -0.276385f, Y = 0.850640f, Z = -0.447215f}, 
				new Vector3 { X = -0.894425f, Y = 0.000000f, Z = -0.447215f}, 
				new Vector3 { X = -0.723600f, Y = 0.525720f, Z = 0.447215f},  
				new Vector3 { X = 0.723600f, Y = 0.525720f, Z = -0.447215f},  
				new Vector3 { X = -0.276385f, Y = 0.850640f, Z = -0.447215f}, 
				new Vector3 { X = 0.276385f, Y = 0.850640f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.894425f, Y = 0.000000f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.276385f, Y = -0.850640f, Z = 0.447215f},  
				new Vector3 { X = 0.723600f, Y = -0.525720f, Z = -0.447215f}, 
				new Vector3 { X = 0.276385f, Y = -0.850640f, Z = 0.447215f},  
				new Vector3 { X = -0.723600f, Y = -0.525720f, Z = 0.447215f}, 
				new Vector3 { X = -0.276385f, Y = -0.850640f, Z = -0.447215f},
				new Vector3 { X = -0.723600f, Y = -0.525720f, Z = 0.447215f}, 
				new Vector3 { X = -0.723600f, Y = 0.525720f, Z = 0.447215f},  
				new Vector3 { X = -0.894425f, Y = 0.000000f, Z = -0.447215f}, 
				new Vector3 { X = -0.723600f, Y = 0.525720f, Z = 0.447215f},  
				new Vector3 { X = 0.276385f, Y = 0.850640f, Z = 0.447215f}, 	 
				new Vector3 { X = -0.276385f, Y = 0.850640f, Z = -0.447215f}, 
				new Vector3 { X = 0.276385f, Y = 0.850640f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.894425f, Y = 0.000000f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.723600f, Y = 0.525720f, Z = -0.447215f},  
				new Vector3 { X = 0.276385f, Y = -0.850640f, Z = 0.447215f},  
				new Vector3 { X = 0.894425f, Y = 0.000000f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f}, 	 
				new Vector3 { X = -0.723600f, Y = -0.525720f, Z = 0.447215f}, 
				new Vector3 { X = 0.276385f, Y = -0.850640f, Z = 0.447215f},  
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f}, 	 
				new Vector3 { X = -0.723600f, Y = 0.525720f, Z = 0.447215f},  
				new Vector3 { X = -0.723600f, Y = -0.525720f, Z = 0.447215f}, 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f}, 	 
				new Vector3 { X = 0.276385f, Y = 0.850640f, Z = 0.447215f}, 	 
				new Vector3 { X = -0.723600f, Y = 0.525720f, Z = 0.447215f},  
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f}, 	 
				new Vector3 { X = 0.894425f, Y = 0.000000f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.276385f, Y = 0.850640f, Z = 0.447215f}, 	 
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f}, 
			};

			TextureCoord[] textureCoordinates = {
				new TextureCoord { S = .648752f, T = 0.445995f},
				new TextureCoord { S = 0.914415f, T = 0.532311f},
				new TextureCoord { S = 0.722181f, T = 0.671980f},
				new TextureCoord { S = 0.722181f, T = 0.671980f},
				new TextureCoord { S = 0.914415f, T = 0.532311f},
				new TextureCoord { S = 0.914415f, T = 0.811645f},
				new TextureCoord { S = 0.254949f, T = 0.204901f},
				new TextureCoord { S = 0.254949f, T = 0.442518f},
				new TextureCoord { S = 0.028963f, T = 0.278329f},
				new TextureCoord { S = 0.480936f, T = 0.278329f},
				new TextureCoord { S = 0.254949f, T = 0.442518f},
				new TextureCoord { S = 0.254949f, T = 0.204901f},
				new TextureCoord { S = 0.838115f, T = 0.247091f},
				new TextureCoord { S = 0.713611f, T = 0.462739f},
				new TextureCoord { S = 0.589108f, T = 0.247091f},
				new TextureCoord { S = 0.722181f, T = 0.671980f},
				new TextureCoord { S = 0.914415f, T = 0.811645f},
				new TextureCoord { S = 0.648752f, T = 0.897968f},
				new TextureCoord { S = 0.648752f, T = 0.445995f},
				new TextureCoord { S = 0.722181f, T = 0.671980f},
				new TextureCoord { S = 0.484562f, T = 0.671981f},
				new TextureCoord { S = 0.254949f, T = 0.204901f},
				new TextureCoord { S = 0.028963f, T = 0.278329f},
				new TextureCoord { S = 0.115283f, T = 0.012663f},
				new TextureCoord { S = 0.480936f, T = 0.278329f},
				new TextureCoord { S = 0.254949f, T = 0.204901f},
				new TextureCoord { S = 0.394615f, T = 0.012663f},
				new TextureCoord { S = 0.838115f, T = 0.247091f},
				new TextureCoord { S = 0.589108f, T = 0.247091f},
				new TextureCoord { S = 0.713609f, T = 0.031441f},
				new TextureCoord { S = 0.648752f, T = 0.897968f},
				new TextureCoord { S = 0.484562f, T = 0.671981f},
				new TextureCoord { S = 0.722181f, T = 0.671980f},
				new TextureCoord { S = 0.644386f, T = 0.947134f},
				new TextureCoord { S = 0.396380f, T = 0.969437f},
				new TextureCoord { S = 0.501069f, T = 0.743502f},
				new TextureCoord { S = 0.115283f, T = 0.012663f},
				new TextureCoord { S = 0.394615f, T = 0.012663f},
				new TextureCoord { S = 0.254949f, T = 0.204901f},
				new TextureCoord { S = 0.464602f, T = 0.031442f},
				new TextureCoord { S = 0.713609f, T = 0.031441f},
				new TextureCoord { S = 0.589108f, T = 0.247091f},
				new TextureCoord { S = 0.713609f, T = 0.031441f},
				new TextureCoord { S = 0.962618f, T = 0.031441f},
				new TextureCoord { S = 0.838115f, T = 0.247091f},
				new TextureCoord { S = 0.028963f, T = 0.613069f},
				new TextureCoord { S = 0.254949f, T = 0.448877f},
				new TextureCoord { S = 0.254949f, T = 0.686495f},
				new TextureCoord { S = 0.115283f, T = 0.878730f},
				new TextureCoord { S = 0.028963f, T = 0.613069f},
				new TextureCoord { S = 0.254949f, T = 0.686495f},
				new TextureCoord { S = 0.394615f, T = 0.878730f},
				new TextureCoord { S = 0.115283f, T = 0.878730f},
				new TextureCoord { S = 0.254949f, T = 0.686495f},
				new TextureCoord { S = 0.480935f, T = 0.613069f},
				new TextureCoord { S = 0.394615f, T = 0.878730f},
				new TextureCoord { S = 0.254949f, T = 0.686495f},
				new TextureCoord { S = 0.254949f, T = 0.448877f},
				new TextureCoord { S = 0.480935f, T = 0.613069f},
				new TextureCoord { S = 0.254949f, T = 0.686495f},
			};

			Vector3[] normals = {
				new Vector3 { X = -0.276376f, Y = -0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f },
				new Vector3 { X = 0.723594f, Y = -0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.723594f, Y = -0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f },
				new Vector3 { X = 0.723594f, Y = 0.525712f, Z = -0.447188f },
				new Vector3 { X = -0.894406f, Y = 0.000000f, Z = -0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f },
				new Vector3 { X = -0.276376f, Y = -0.850642f, Z = -0.447188f },
				new Vector3 { X = -0.276376f, Y = 0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f },
				new Vector3 { X = -0.894406f, Y = 0.000000f, Z = -0.447188f },
				new Vector3 { X = 0.723594f, Y = 0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = -1.000000f },
				new Vector3 { X = -0.276376f, Y = 0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.723594f, Y = -0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.723594f, Y = 0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.894406f, Y = 0.000000f, Z = 0.447188f },
				new Vector3 { X = -0.276376f, Y = -0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.723594f, Y = -0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.276376f, Y = -0.850642f, Z = 0.447188f },
				new Vector3 { X = -0.894406f, Y = 0.000000f, Z = -0.447188f },
				new Vector3 { X = -0.276376f, Y = -0.850642f, Z = -0.447188f },
				new Vector3 { X = -0.723594f, Y = -0.525712f, Z = 0.447188f },
				new Vector3 { X = -0.276376f, Y = 0.850642f, Z = -0.447188f },
				new Vector3 { X = -0.894406f, Y = 0.000000f, Z = -0.447188f },
				new Vector3 { X = -0.723594f, Y = 0.525712f, Z = 0.447188f },
				new Vector3 { X = 0.723594f, Y = 0.525712f, Z = -0.447188f },
				new Vector3 { X = -0.276376f, Y = 0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.276376f, Y = 0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.894406f, Y = 0.000000f, Z = 0.447188f },
				new Vector3 { X = 0.276376f, Y = -0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.723594f, Y = -0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.276376f, Y = -0.850642f, Z = 0.447188f },
				new Vector3 { X = -0.723594f, Y = -0.525712f, Z = 0.447188f },
				new Vector3 { X = -0.276376f, Y = -0.850642f, Z = -0.447188f },
				new Vector3 { X = -0.723594f, Y = -0.525712f, Z = 0.447188f },
				new Vector3 { X = -0.723594f, Y = 0.525712f, Z = 0.447188f },
				new Vector3 { X = -0.894406f, Y = 0.000000f, Z = -0.447188f },
				new Vector3 { X = -0.723594f, Y = 0.525712f, Z = 0.447188f },
				new Vector3 { X = 0.276376f, Y = 0.850642f, Z = 0.447188f },
				new Vector3 { X = -0.276376f, Y = 0.850642f, Z = -0.447188f },
				new Vector3 { X = 0.276376f, Y = 0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.894406f, Y = 0.000000f, Z = 0.447188f },
				new Vector3 { X = 0.723594f, Y = 0.525712f, Z = -0.447188f },
				new Vector3 { X = 0.276376f, Y = -0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.894406f, Y = 0.000000f, Z = 0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f },
				new Vector3 { X = -0.723594f, Y = -0.525712f, Z = 0.447188f },
				new Vector3 { X = 0.276376f, Y = -0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f },
				new Vector3 { X = -0.723594f, Y = 0.525712f, Z = 0.447188f },
				new Vector3 { X = -0.723594f, Y = -0.525712f, Z = 0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f },
				new Vector3 { X = 0.276376f, Y = 0.850642f, Z = 0.447188f },
				new Vector3 { X = -0.723594f, Y = 0.525712f, Z = 0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f },
				new Vector3 { X = 0.894406f, Y = 0.000000f, Z = 0.447188f },
				new Vector3 { X = 0.276376f, Y = 0.850642f, Z = 0.447188f },
				new Vector3 { X = 0.000000f, Y = 0.000000f, Z = 1.000000f },
			};

			GL.ClearColor (0f, 0f, 0f, 1f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			if (program != null)
				program.Use ();
			
			GL.VertexAttribPointer (positionAttribute, 3, VertexAttribPointerType.Float, false, 0, vertices);
			GL.EnableVertexAttribArray (positionAttribute);

			GL.VertexAttribPointer (textureCoordinateAttribute, 2, VertexAttribPointerType.Float, false, 0, textureCoordinates);
			GL.EnableVertexAttribArray (textureCoordinateAttribute);

			GL.VertexAttribPointer (normalsAttribute, 2, VertexAttribPointerType.Float, false, 0, normals);
			GL.EnableVertexAttribArray (normalsAttribute);

			Vector3 rotationVector = new Vector3 (1.0f, 1.0f, 1.0f);
			GLCommon.Matrix3DSetRotationByDegrees (ref rotationMatrix, rot, rotationVector);
			GLCommon.Matrix3DSetTranslation (ref translationMatrix, 0.0f, 0.0f, -3.0f);
			modelViewMatrix = GLCommon.Matrix3DMultiply (translationMatrix, rotationMatrix);

			GLCommon.Matrix3DSetPerspectiveProjectionWithFieldOfView (ref projectionMatrix, 45.0f, 0.1f, 100.0f,
				(float)View.Frame.Size.Width /
				(float)View.Frame.Size.Height);

			matrix = GLCommon.Matrix3DMultiply (projectionMatrix, modelViewMatrix);
			GL.UniformMatrix4 (matrixUniform, 1, false, matrix);

			GL.ActiveTexture (TextureUnit.Texture0);
			if (texture != null)
				texture.Use ();
			GL.Uniform1 (textureUniform, 0);

			GL.Uniform4 (lightDirectionUniform, 1.0f, 0.75f, 0.25f, 1.0f);
			GL.Uniform4 (lightDiffuseColorUniform, 0.8f, 0.8f, 1.0f, 1.0f);

			GL.DrawArrays (BeginMode.Triangles, 0, vertices.Length);

			rot += 2.0f;
			if (rot > 360.0f)
				rot -= 360.0f;
		}

		public override void LoadView ()
		{
			GLView view = new GLView ();
			view.Controller = this;

			View = view;
		}
	}
}