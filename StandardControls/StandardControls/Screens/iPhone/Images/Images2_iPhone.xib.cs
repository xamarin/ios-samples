
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Example_StandardControls.Screens.iPhone.Images
{
	public partial class Images2_iPhone : UIViewController
	{
		UIImageView imageView1;
		UIImageView imgSpinningCircle;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public Images2_iPhone (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public Images2_iPhone (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public Images2_iPhone () : base("Images2_iPhone", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Images";
			
			// a simple image
			imageView1 = new UIImageView (UIImage.FromBundle ("Images/Icons/50_icon.png"));
			imageView1.Frame = new RectangleF (20, 20, imageView1.Image.CGImage.Width, imageView1.Image.CGImage.Height);
			View.AddSubview (imageView1);
			
			// an animating image
			imgSpinningCircle = new UIImageView();
			imgSpinningCircle.AnimationImages = new UIImage[] {
				UIImage.FromBundle ("Images/Spinning Circle_1.png"),
				UIImage.FromBundle ("Images/Spinning Circle_2.png"),
				UIImage.FromBundle ("Images/Spinning Circle_3.png"),
				UIImage.FromBundle ("Images/Spinning Circle_4.png")
			};
			imgSpinningCircle.AnimationRepeatCount = 0;
			imgSpinningCircle.AnimationDuration = .5;
			imgSpinningCircle.Frame = new RectangleF(150, 20, 100, 100);
			View.AddSubview(imgSpinningCircle);
			imgSpinningCircle.StartAnimating ();
		}
		
	}
}

