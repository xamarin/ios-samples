using System;
using OpenTK.Graphics.ES20;
using Foundation;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PerVertexDirectionalLighting
{
	public class GLProgram
	{
		int program,
			vertShader, 
			fragShader;

		List<string> attributes;

		public GLProgram (string vShaderFilename, string fShaderFilename)
		{
			attributes = new List<string> ();
			program = GL.CreateProgram ();

			string vertShaderPathName = NSBundle.MainBundle.PathForResource (vShaderFilename, "vsh");
			if (!compileShader (ref vertShader, ShaderType.VertexShader, vertShaderPathName))
				Console.WriteLine ("Failed to compile the vertex shader");

			string fragShaderPathName = NSBundle.MainBundle.PathForResource (fShaderFilename, "fsh");
			if (!compileShader (ref fragShader, ShaderType.FragmentShader, fragShaderPathName))
				Console.WriteLine ("Failed to compile the fragment shader");

			GL.AttachShader (program, vertShader);
			GL.AttachShader (program, fragShader);
		}

		bool compileShader (ref int shader, ShaderType type, string file)
		{
			int status;
			string source;

			using (StreamReader sr = new StreamReader(file))
				source = sr.ReadToEnd();

			shader = GL.CreateShader (type);
			GL.ShaderSource (shader, source);
			GL.CompileShader (shader);

			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);

			return status == (int) All.True;
		}
 
		public void AddAttribute (string attributeName)
		{
			if (!attributes.Contains (attributeName)) {
				attributes.Add (attributeName);
				GL.BindAttribLocation (program, attributes.IndexOf (attributeName), attributeName);
			}
		}

		public int GetAttributeIndex (string attributeName)
		{
			return attributes.IndexOf (attributeName);
		}

		public int GetUniformIndex (string uniformName)
		{
			return GL.GetUniformLocation (program, uniformName);
		}

		public bool Link ()
		{
			int status = 0;

			GL.LinkProgram (program);
			GL.ValidateProgram (program);

			GL.GetProgram (program, ProgramParameter.LinkStatus, out status);
			if (status == (int) All.False)
				return false;

			GL.DeleteShader (vertShader);
			GL.DeleteShader (fragShader);

			return true;
		}

		public void Use ()
		{
			GL.UseProgram (program);
		}

		string getLog (int obj)
		{
			int logLength = 0;

			GL.GetProgram (obj, ProgramParameter.InfoLogLength, out logLength);
			if (logLength < 1)
				return null;

			string log = GL.GetProgramInfoLog (obj);

			return log;
		}

		public string VertexShaderLog ()
		{
			return getLog (vertShader);
		}

		public string FragmentShaderLog ()
		{
			return getLog (fragShader);
		}

		public string ProgramLog ()
		{
			return getLog (program);
		}
	}
}

