using System;

using Foundation;
using UIKit;

namespace TextKitDemo
{
	public partial class PersonViewController : TextViewController
	{
		public PersonViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			shortDescriptionTextView.Selectable = false;
			descriptionTextView.Selectable = false;

			imageView.Image = UIImage.FromFile ("johnnyProfilePic.png");
			labelView.Text = "Johhny Appleseed";
			shortDescriptionTextView.Text = "In another moment down went Alice after it, " +
				"never once considering how in the world she was to get out again.";

			shortDescriptionTextView.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;

			if (model != null)
				descriptionTextView.AttributedText = model.GetAttributedText ();
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			imageView.Layer.MasksToBounds = true;
			imageView.Layer.CornerRadius = imageView.Bounds.Size.Width / 2.0f;
		}
		
		public override void PreferredContentSizeChanged ()
		{
			labelView.Font = UIFont.GetPreferredFontForTextStyle (labelView.Font.FontDescriptor.FontAttributes.TextStyle);
			shortDescriptionTextView.Font = UIFont.GetPreferredFontForTextStyle (shortDescriptionTextView.Font.FontDescriptor.FontAttributes.TextStyle);
			descriptionTextView.Font = UIFont.GetPreferredFontForTextStyle (descriptionTextView.Font.FontDescriptor.FontAttributes.TextStyle);

			UIFontDescriptor descriptor = descriptionTextView.Font.FontDescriptor;
			descriptionTextView.Font = UIFont.GetPreferredFontForTextStyle (descriptor.FontAttributes.TextStyle);
		}
	}
}

