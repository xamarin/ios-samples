using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace Example_StandardControls.Screens.iPhone.ScrollView
{
	public class Controller : UIViewController
	{
		UIScrollView scrollView;
		UIImageView imageView;
		

		#region -= constructors =-

		public Controller () : base()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.White;
			
			this.Title = "Scroll View";
			
			// create our scroll view
			scrollView = new UIScrollView (
				new RectangleF (0, 0, this.View.Frame.Width, this.View.Frame.Height - this.NavigationController.NavigationBar.Frame.Height));
			this.View.AddSubview (scrollView);
			
			// create our image view
			imageView = new UIImageView (UIImage.FromFile ("Images/Icons/512_icon.png"));
			scrollView.ContentSize = imageView.Image.Size;
			scrollView.MaximumZoomScale = 3f;
			scrollView.MinimumZoomScale = .1f;
			scrollView.AddSubview (imageView);
			
			scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };
		}
		
	}
}
