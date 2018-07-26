using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using SceneKit;
using ARKit;
using CoreFoundation;

namespace PlacingObjects
{
	public partial class SettingsViewController : UITableViewController
	{
		public SettingsViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			PopulateSettings();
		}

		private void PopulateSettings() {
			DragOnInfinitePlanesSwitch.On = AppSettings.DragOnInfinitePlanes;
			ScaleWithPinchGestureSwitch.On = AppSettings.ScaleWithPinchGesture;
		}
		[Action("didChangeSetting:")]
		public void SettingChanged(UISwitch sender)
		{
			if (sender == DragOnInfinitePlanesSwitch)
			{
				AppSettings.DragOnInfinitePlanes = DragOnInfinitePlanesSwitch.On;
			}
			if (sender == ScaleWithPinchGestureSwitch)
			{
				AppSettings.ScaleWithPinchGesture = ScaleWithPinchGestureSwitch.On;
			}
		}
	}
}
