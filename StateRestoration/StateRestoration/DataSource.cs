using System;
using System.IO;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace StateRestoration
{
	public class DataSource : UIStateRestoring
	{
		NSDictionary data;

		public DataSource ()
		{
			var dataPath = NSBundle.MainBundle.PathForResource ("Data", "plist");
			data = NSDictionary.FromFile (dataPath);
		}

		public nint GetItemsCount (nint section)
		{
			return 32;
		}

		public string GetIdentifier (NSIndexPath indexPath)
		{
			string identifier = indexPath.Row.ToString ();
			return identifier;
		}

		public string GetTitle (string identifier)
		{
			string title = string.IsNullOrEmpty (identifier) ? null : (NSString)data [identifier];
			return title ?? "Image";
		}

		public UIImage GetThumbnail (string identifier)
		{
			if (identifier == null)
				return null;

			var path = GetThumbPath (identifier);
			return UIImage.FromFile (path);
		}

		public UIImage GetFullImage (string identifier)
		{
			if (identifier == null)
				return null;

			string path = GetFullImgPath (identifier);
			return UIImage.FromFile (path);
		}

		static string GetThumbPath (string identifier)
		{
			string fileName = Path.ChangeExtension (identifier, "JPG");
			return Path.Combine ("Images/thumb", fileName);
		}

		static string GetFullImgPath (string identifier)
		{
			identifier = string.Format ("{0}_full", identifier);
			string fileName = Path.ChangeExtension (identifier, "JPG");

			return Path.Combine ("Images/full", fileName);
		}
	}
}

