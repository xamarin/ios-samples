using System;
using CoreGraphics;
using UIKit;

namespace DropDestination {
	public partial class ViewController : UIViewController {
		#region Computed Properties
		public DroppableImageGridViewController Grid { get; set; } = new DroppableImageGridViewController (new CGSize (200, 200));
		public DroppableDeleteView DeleteView { get; set; }
		#endregion

		#region Constructors
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.UserInteractionEnabled = true;

			var gridView = Grid.View;
			View.AddSubview (gridView);

			DeleteView = new DroppableDeleteView ("Drop here to delete photos", (views) => {
				Grid.RemoveViewsInSet (views);
				View.LayoutIfNeeded ();
			});
			View.AddSubview (DeleteView);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			//Background.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
			Grid.View.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height - 150f);
			DeleteView.Frame = new CGRect (0, View.Frame.Height - 150f, View.Frame.Width, 150f);
		}
		#endregion
	}
}
