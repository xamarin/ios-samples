using System;
using UIKit;

namespace AdaptivePhotos
{
	public class EmptyViewController : CustomViewController
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var view = new UIView {
				BackgroundColor = UIColor.White
			};

			var label = new UILabel ();
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			label.Text = "No Conversation Selected";
			label.TextColor = UIColor.FromWhiteAlpha (0.0f, 0.4f);
			label.Font = UIFont.PreferredHeadline;
			view.AddSubview (label);

			view.AddConstraint (NSLayoutConstraint.Create (label, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal,
				view, NSLayoutAttribute.CenterX, 1.0f, 0.0f));
			view.AddConstraint (NSLayoutConstraint.Create (label, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal,
				view, NSLayoutAttribute.CenterY, 1.0f, 0.0f));

			View = view;
		}
	}
}

