using System;
using Foundation;
using UIKit;

namespace PlacingObjects
{
	[Register("ObjectCell")]
	public class ObjectCell : UITableViewCell
	{
		public readonly static string Identifier = "ObjectCell";

		[Outlet]
		UILabel objectTitleLabel { get; set; }

		[Outlet]
		UIImageView objectImageView { get; set; }

		VirtualObjectDefinition virtualObject;

		public VirtualObjectDefinition VirtualObject
		{
			get => virtualObject;

			set
			{
				objectTitleLabel.Text = value.DisplayName;
				objectImageView.Image = value.ThumbImage;
				virtualObject = value;
			}
		}

		public ObjectCell(IntPtr ptr) : base(ptr)
		{

		}
	}
}
