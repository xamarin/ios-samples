using System;
using UIKit;
namespace Example_SharedResources.Screens.iPhone.Photos
{
	public class TestCameraOverlayController : UIViewController
	{
		#region -= constructors =-

		public TestCameraOverlayController () : base()
		{
		}

		#endregion
		
		public override void LoadView ()
		{
			this.View = new CameraOverlayView ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			//---- set the background color of the view to clear
			this.View.BackgroundColor = UIColor.Clear;			
		}		
	}
}

