
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_Drawing.Screens.iPad.Home
{
	public partial class HomeScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public HomeScreen (IntPtr handle) : base(handle) { }

		[Export("initWithCoder:")]
		public HomeScreen (NSCoder coder) : base(coder) { }

		public HomeScreen () : base("HomeScreen", null) { }

		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			btnDrawRectVsPath.TouchUpInside += delegate {
				NavigationController.PushViewController (new DrawRectVsPath.Controller (), true);
			};
			btnDrawUsingCGBitmapContext.TouchUpInside += delegate {
				NavigationController.PushViewController (new DrawOffScreenUsingCGBitmapContext.Controller (), true);
			};
			btnDrawUsingLayers.TouchUpInside += delegate {
				NavigationController.PushViewController (new Layers.Controller (), true);
			};
			btnOnScreenCoords.TouchUpInside += delegate {
				NavigationController.PushViewController (new CoordinatesOnScreen.Controller (), true);
			};
			btnOffScreenCoords.TouchUpInside += delegate {
				NavigationController.PushViewController (new CoordinatesOffScreen.Controller (), true); 
			};
			btnOnScreenUncorrectedText.TouchUpInside += delegate {
				NavigationController.PushViewController (new OnScreenUncorrectedTextRotation.Controller (), true);
			};
			btnImage.TouchUpInside += delegate { 
				NavigationController.PushViewController (new Images.Controller (), true);
			};
			btnOffScreenFlag.TouchUpInside += delegate {
				NavigationController.PushViewController (new FlagOffScreen.Controller (), true);
			};
			btnOnScreenFlag.TouchUpInside += delegate {
				NavigationController.PushViewController (new FlagOnScreen.Controller (), true);
			};
			btnPatterns.TouchUpInside += delegate {
				NavigationController.PushViewController (new ColorPattern.Controller (), true);
			};
			btnStencilPattern.TouchUpInside += delegate { 
				NavigationController.PushViewController (new StencilPattern.Controller (), true); 
			};
			btnShadows.TouchUpInside += delegate {
				NavigationController.PushViewController (new Shadows.Controller (), true);
			};
			btnHitTesting.TouchUpInside += delegate {
				NavigationController.PushViewController (new HitTesting.Controller (), true);
			};
			btnTouchDrawing.TouchUpInside += delegate {
				NavigationController.PushViewController (new TouchDrawing.Controller (), true);
			};
			btnTransformations.TouchUpInside += delegate {
				NavigationController.PushViewController (new Transformations.Controller (), true);
			};
			
		}
		
		
	}
}

