using System;
using Foundation;
using UIKit;

namespace DragBoard {
	/// <summary>
	/// A custom view that can handle paste events.
	/// </summary>
	public partial class DragBoardView : UIView {
		public DragBoardView (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("Cork"));
		}

		public override bool CanBecomeFirstResponder {
			get { return true; }
		}
	}
}
