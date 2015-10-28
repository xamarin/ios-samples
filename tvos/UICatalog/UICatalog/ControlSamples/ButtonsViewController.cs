using System;

using Foundation;
using UIKit;

namespace UICatalog {
	public partial class ButtonsViewController : UIViewController {

		[Export ("initWithCoder:")]
		public ButtonsViewController (NSCoder coder): base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			ConfigureAttributedTextSystemButton ();
		}

		partial void ButtonClicked (UIButton sender)
		{
			Console.WriteLine ("A button was clicked.");
		}

		void ConfigureAttributedTextSystemButton ()
		{
			const string buttonTitle = "Button";

			var normalAttributedTitle = new NSAttributedString (buttonTitle, foregroundColor: UIColor.Blue, strikethroughStyle: NSUnderlineStyle.Single);
			AttributedTextButton.SetAttributedTitle (normalAttributedTitle, UIControlState.Normal);

			var highlightedAttributedTitle = new NSAttributedString (buttonTitle, foregroundColor: UIColor.Green, strikethroughStyle: NSUnderlineStyle.Thick);
			AttributedTextButton.SetAttributedTitle (highlightedAttributedTitle, UIControlState.Highlighted);
		}
	}
}

