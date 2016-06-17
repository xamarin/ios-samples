using System;
using System.Text;
using System.IO;

using UIKit;

namespace MediaCapture {
	public static class Utilities {
		static string debugDirectory = null;
		public static string DebugDirectory {
			get {
				if (debugDirectory == null) {
					string path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "Logs");
					if ( Directory.Exists(path) == false )
						Directory.CreateDirectory (path);
					debugDirectory = path;
				}
				return debugDirectory;
			}
		}

		public static string ExceptionLogFilePath {
			get {
				return Path.Combine (DebugDirectory, "ExceptionLog.txt");
			}
		}

		public static void ShowLastErrorLog ()
		{
			if (File.Exists (ExceptionLogFilePath) == true) {
				string exceptionText = File.ReadAllText (ExceptionLogFilePath);
				File.Delete (ExceptionLogFilePath);
				ShowMessage ("Logged Exception", exceptionText);
			}
		}

		public static void ShowMessage (string title, string message )
		{
			using(var alert = new UIAlertView (title, message, null, "OK", null)) {
				alert.Show ();
			}
		}

		// this utility API walks the stack of inner exceptions and builds a string containing everything known about the exception and then writes
		// it to the log file
		public static void LogUnhandledException( Exception ex)
		{
			var sb = new StringBuilder ();
			Exception currentException = ex;
			while (currentException != null) {
				sb.AppendFormat ("{0}\r\n", currentException.Message);
				sb.AppendFormat ("{0}\r\n", currentException.StackTrace);
				sb.AppendFormat ("-- Source: {0}\r\n", currentException.Source);
				if (currentException.TargetSite != null) {
					sb.AppendFormat ("-- Target Site: {0}\r\n", currentException.TargetSite.Name);
				}
				sb.Append ("----------------------\r\n");
				currentException = currentException.InnerException;
			}
			string text = sb.ToString ();
			if ( File.Exists (ExceptionLogFilePath) == true )
				File.Delete (ExceptionLogFilePath);

			File.WriteAllText (ExceptionLogFilePath, text);
		}
	}
}

