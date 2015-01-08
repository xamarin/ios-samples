using System;
using System.Globalization;

using UIKit;
using Foundation;

namespace PhotoHandoff
{
	public class DataSource : UIStateRestoring
	{
		class TitleStorage : DictionaryContainer
		{
			public TitleStorage(NSDictionary dictionary)
				: base(dictionary)
			{
			}

			public string GetTitle(string identifier)
			{
				return (string)(NSString)Dictionary [identifier];
			}
		}

		TitleStorage nameStorage;

		public DataSource ()
		{
			string pathToData = FetchPath ("Data", "plist");
			NSDictionary dict = NSDictionary.FromFile (pathToData);
			nameStorage = new TitleStorage (dict);
		}

		public nint NumberOfItemsInSection(nint section)
		{
			return 32;
		}

		public string IdentifierForIndexPath(NSIndexPath indexPath)
		{
			return indexPath.Row.ToString (CultureInfo.InvariantCulture);
		}

		public string GetTitleForIdentifier(string identifier)
		{
			string title = string.IsNullOrEmpty (identifier) ? "Image" : nameStorage.GetTitle (identifier);
			return title;
		}

		public UIImage ThumbnailForIdentifier(string identifier)
		{
			if (string.IsNullOrEmpty(identifier))
				return null;

			string pathToImage = FetchPath (identifier, "JPG");
			return new UIImage (pathToImage);
		}

		public UIImage ImageForIdentifier(string identifier)
		{
			if (string.IsNullOrEmpty(identifier))
				return null;

			string imageName = string.Format ("{0}_full", identifier);
			string pathToImage = FetchPath (imageName, "JPG");
			var image = new UIImage (pathToImage);
			return image;
		}

		static string FetchPath(string fileName, string ext)
		{
			return NSBundle.MainBundle.PathForResource (fileName, ext);
		}
	}
}