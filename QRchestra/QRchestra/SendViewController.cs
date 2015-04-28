using UIKit;
using System.Collections.Generic;
using CoreAnimation;
using Foundation;
using System;
using CoreImage;
using CoreGraphics;

namespace QRchestra
{
	public partial class SendViewController : UIViewController
	{
		public Action<SendViewController> Finished;

		string[] mainBank = new [] { "67", "71", "74", "80" };
		string[] altBank = new [] { "60", "64", "67", "72" };
		int[] currentBank = new [] { 0, 0, 0, 0 };
		const int maxNumberOfBanks = 3;

		List<UIImageView> keyImageViews;

		public SendViewController () : base ("SendViewController", null)
		{
			PreferredContentSize = new CGSize (320f, 480f);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tapGestureRecognizer1.ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously;
			tapGestureRecognizer2.ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously;
			tapGestureRecognizer3.ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously;
			tapGestureRecognizer4.ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously;

			keyImageViews = new List<UIImageView> {
				keyImageView1,
				keyImageView2,
				keyImageView3,
				keyImageView4
			};

			foreach (var imageView in keyImageViews)
				imageView.Layer.MagnificationFilter = CALayer.FilterNearest;

			setNotesToDefaults ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		partial void done (NSObject sender)
		{
			if (Finished != null)
				Finished (this);
		}

		void setNotesToDefaults ()
		{
			for (int i = 0; i < keyImageViews.Count; i++)
				keyImageViews [i].Image = machineReadableCodeFromMessage (mainBank [i]);
		}

		UIImage machineReadableCodeFromMessage (string message)
		{
			var mrcFilter = CIFilter.FromName ("CIQRCodeGenerator");
			NSData messageData = NSData.FromString (new NSString (message), NSStringEncoding.UTF8);
			mrcFilter.SetValueForKey (messageData, (NSString)"inputMessage");

			var barcodeCIImage = (CIImage)mrcFilter.ValueForKey ((NSString)"outputImage");
			CGRect extent = barcodeCIImage.Extent;

			CGImage barcodeCGImage = CIContext.CreateCGImage (barcodeCIImage, extent);
			UIImage image = new UIImage (barcodeCGImage);
			return image;
		}

		static CIContext ciContext;
		public static CIContext CIContext {
			get {
				if (ciContext == null) {
					CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
					CIContextOptions ciOptions = new CIContextOptions { OutputColorSpace = colorSpace };

					ciContext = CIContext.FromOptions (ciOptions);
				}

				return ciContext;
			}
		}

		partial void handleTap (UITapGestureRecognizer tapGestureRecognizer)
		{
			if (!(tapGestureRecognizer.View is UIImageView))
				return;

			var keyImageView = (UIImageView)tapGestureRecognizer.View;
			int keyIndex = keyImageViews.IndexOf (keyImageView);

			if (keyIndex < 0)
				return;

			currentBank [keyIndex]++;
			if (currentBank [keyIndex] == maxNumberOfBanks)
				currentBank [keyIndex] = 0;

			if (currentBank [keyIndex] == 0)
				keyImageView.Image = machineReadableCodeFromMessage (mainBank [keyIndex]);
			else if (currentBank [keyIndex] == 1)
				keyImageView.Image = machineReadableCodeFromMessage (altBank [keyIndex]);
			else if (currentBank [keyIndex] == 2)
				keyImageView.Image = null;

			CATransaction.Begin ();
			var transition = CATransition.CreateAnimation ();
			transition.Duration = 0.3f;
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
			transition.Type = CATransition.TransitionReveal;
			transition.Subtype = CATransition.TransitionFromBottom;
			keyImageView.Layer.AddAnimation (transition, null);
			CATransaction.Commit ();
		}

		public bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			return true;
		}
	}
}

