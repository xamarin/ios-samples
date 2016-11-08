using System;

using UIKit;

namespace CustomTransitions
{
	public partial class CPSecondViewController : UIViewController
	{
		public CPSecondViewController(IntPtr handle)
			: base (handle)
		{
			
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			UpdatePreferredContentSizeWithTraitCollection(TraitCollection);
		}


		public override void WillTransitionToTraitCollection(UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			//base.WillTransitionToTraitCollection(traitCollection, coordinator);
			UpdatePreferredContentSizeWithTraitCollection(traitCollection);
		}


		public void UpdatePreferredContentSizeWithTraitCollection(UITraitCollection traitCollection)
		{
			float sizeHeigt;
			if (traitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Compact)
			{
				sizeHeigt = 270F;
			}
			else {
				sizeHeigt = 420F;
			}

			PreferredContentSize = new CoreGraphics.CGSize(View.Bounds.Width, sizeHeigt);
			slider.MaxValue = (float)PreferredContentSize.Height;
			slider.MinValue = 220.0F;
			slider.Value = slider.MaxValue;
		}


		partial void SliderValueChange(UISlider sender)
		{
			PreferredContentSize = new CoreGraphics.CGSize(View.Bounds.Width, sender.Value);
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

