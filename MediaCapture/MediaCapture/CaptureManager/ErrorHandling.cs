using System;
using System.Text;

using Foundation;

namespace MediaCapture {
	public static class ErrorHandling {
		public static string GetExceptionDetailedText( Exception ex )
		{
			var sb = new StringBuilder ();
			Exception currentException = ex;
			while ( currentException != null ) {
				sb.AppendFormat ("{0}\r\n", currentException.Message);
				sb.AppendFormat ("{0}\r\n", currentException.StackTrace);
				sb.AppendFormat ("-- Source: {0}\r\n", currentException.Source);
				if ( currentException.TargetSite != null ) {
					sb.AppendFormat ("-- Target Site: {0}\r\n", currentException.TargetSite.Name);
				}
				sb.Append ("----------------------\r\n");
				currentException = currentException.InnerException;
			}
			return sb.ToString ();
		}

		public static string GetNSErrorString (NSError nsError)
		{
			if (nsError == null)
				return "No Error Info Available";

			try {
				var sb = new StringBuilder ();
				sb.AppendFormat ("Error Code:  {0}\r\n", nsError.Code.ToString ());
				sb.AppendFormat ("Description: {0}\r\n", nsError.LocalizedDescription);
				var userInfo = nsError.UserInfo;
				for (int i = 0; i < userInfo.Keys.Length; i++) {
					sb.AppendFormat("[{0}]: {1}\r\n", userInfo.Keys[i].ToString (), userInfo.Values[i].ToString ());
				}
				return sb.ToString ();
			} catch {
				return "Error parsing NSError object. Ironic, is it not ?";
			}
		}
	}
}

