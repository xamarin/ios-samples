/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A view controller which is always shown in dark mode, regardless of whether the device is in light or dark mode.
*/

namespace AdoptingIOSDarkMode;
public partial class AlwaysDarkViewController : UIViewController {
	public AlwaysDarkViewController (IntPtr handle) : base (handle)
	{
	}

	public override void AwakeFromNib ()
	{
		base.AwakeFromNib ();

		// DARK MODE ADOPTION: This is an example of how a view
		// controller's overrideUserInterfaceStyle property can be set
		// to force it to have a specific style, regardless of the style
		// used by the system or by its parent view controller or
		// presentation controller.
		//
		// Do the same when the view controller is created from
		// a storyboard.

		OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark;
	}
}
