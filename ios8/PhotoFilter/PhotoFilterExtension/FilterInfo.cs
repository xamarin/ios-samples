using System;

using UIKit;
using Foundation;

namespace PhotoFilterExtension
{
	public class FilterInfo
	{
		const string filterNameKey = "filterName";
		const string displayNameKey = "displayName";
		const string previewImageKey = "previewImage";

		public string FilterName { get; set; }

		public string DisplayName { get; set; }

		public string PreviewImage { get; set; }

		public FilterInfo ()
		{
		}

		public FilterInfo (NSDictionary storage)
		{
			FillFrom (storage);
		}

		public void FillFrom (NSDictionary storage)
		{
			FilterName = (NSString)storage [filterNameKey];
			DisplayName = (NSString)storage [displayNameKey];
			PreviewImage = (NSString)storage [previewImageKey];
		}
	}
}

