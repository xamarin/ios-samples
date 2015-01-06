using System;

using UIKit;
using ObjCRuntime;
using CoreAnimation;
using Foundation;
using CoreGraphics;

namespace AudioTapProcessor
{
	[Register("GradientView")]
	public class GradientView : UIView
	{
		static Class LayerClass {
			[Export("layerClass")]
			get {
				return new Class (typeof(CAGradientLayer));
			}
		}

		public GradientView (IntPtr handle)
			: base(handle)
		{
			SetupLayerTree ();
		}

		void SetupLayerTree()
		{
			// Setup gradient layer.
			((CAGradientLayer)Layer).Colors = new CGColor[] {
				UIColor.DarkGray.CGColor,
				UIColor.FromWhiteAlpha (0.125f, 1).CGColor,
				UIColor.Black.CGColor,
				UIColor.Black.CGColor,
			};

			((CAGradientLayer)Layer).Locations = new NSNumber[] {
				new NSNumber (0),
				new NSNumber (0.5),
				new NSNumber (0.5),
				new NSNumber (1)
			};
		}
	}
}