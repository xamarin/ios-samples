using UIKit;

namespace PhotoFilter
{
	public class ViewController : UIViewController
	{
		UILabel note;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Main app do nothing
			// Go to Photo > Edit then choose PhotoFilter to start app extension

			note = new UILabel {
				Text = "Note that the app in this sample only serves as a host for the extension. To use the sample extension, edit a photo or video using the Photos app, and tap the extension icon",
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
				TranslatesAutoresizingMaskIntoConstraints = false,
			};
			View.AddSubview (note);
			View.BackgroundColor = UIColor.White;

			View.AddConstraint (NSLayoutConstraint.Create (note, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterY, 1, 0));
			View.AddConstraints (NSLayoutConstraint.FromVisualFormat ("|-[note]-|", (NSLayoutFormatOptions)0, "note", note));
		}
	}
}