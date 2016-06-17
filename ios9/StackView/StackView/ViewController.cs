using System;

using UIKit;

namespace StackView
{
	public partial class ViewController : UIViewController
	{
		#region Computed Properties
		public int Rating { get; set;} = 0;
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#endregion

		#region Actions
		partial void IncreaseRating (Foundation.NSObject sender) {

			// Maximum of 5 "stars"
			if (++Rating > 5 ) {
				// Abort
				Rating = 5;
				return;
			}

			// Create new rating icon and add it to stack
			var icon = new UIImageView (new UIImage("icon.png"));
			icon.ContentMode = UIViewContentMode.ScaleAspectFit;
			RatingView.AddArrangedSubview(icon);

			// Animate stack
			UIView.Animate(0.25, ()=>{
				// Adjust stack view
				RatingView.LayoutIfNeeded();
			});

		}

		partial void DecreaseRating (Foundation.NSObject sender) {

			// Minimum of zero "stars"
			if (--Rating < 0) {
				// Abort
				Rating =0;
				return;
			}

			// Get the last subview added
			var icon = RatingView.ArrangedSubviews[RatingView.ArrangedSubviews.Length-1];

			// Remove from stack and screen
			RatingView.RemoveArrangedSubview(icon);
			icon.RemoveFromSuperview();

			// Animate stack
			UIView.Animate(0.25, ()=>{
				// Adjust stack view
				RatingView.LayoutIfNeeded();
			});
		}
		#endregion
	}
}

