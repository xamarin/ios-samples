using System;
using System.IO;
using System.Linq;
using Foundation;

namespace ElizaCore
{
	public class LineSourceFromAssembly :ILineSource
	{
		private readonly StreamReader reader;

		public LineSourceFromAssembly(string script){

			//var documents = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			var documents = NSBundle.MainBundle.BundlePath;
			string path = Path.Combine (documents, script);

			//Does the file exist?
			if (!File.Exists (path)) {
				Console.WriteLine ("Error: {0}", path);
				throw new ArgumentException ("Script not found:" + path);
			}
			reader = File.OpenText (path);
		}

		#region ILineSource implementation
		public string ReadLine ()
		{
			return reader.ReadLine ();

		}
		public void Close ()
		{
			reader.Dispose ();
		}
		#endregion
	}
}
