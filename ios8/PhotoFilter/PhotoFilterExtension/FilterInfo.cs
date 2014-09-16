using System;

using UIKit;
using Foundation;

namespace PhotoFilterExtension
{
	public class FilterInfo
	{
		private const string filterNameKey = "filterName";
		private const string displayNameKey = "displayName";
		private const string previewImageKey = "previewImage";

		public string FilterName { get; set; }
		public string DisplayName { get; set; }
		public string PreviewImage { get; set; }

		public FilterInfo ()
		{
		}

		public FilterInfo(NSDictionary storage)
		{
			FillFrom (storage);
		}

		public void FillFrom(NSDictionary storage)
		{
			FilterName = (string)(NSString)storage [filterNameKey];
			DisplayName = (string)(NSString)storage [displayNameKey];
			PreviewImage = (string)(NSString)storage [previewImageKey];
		}
	}
}

