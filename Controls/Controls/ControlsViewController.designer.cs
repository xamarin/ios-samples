// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace Controls
{
	[Register ("ControlsViewController")]
	partial class ControlsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Button1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Button2 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton button3 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton button4 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel label1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider slider1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel sliderLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch switch1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField textfield1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView textview1 { get; set; }

		[Action ("button2_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void button2_TouchUpInside (UIButton sender);

		[Action ("button3_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void button3_TouchUpInside (UIButton sender);

		[Action ("button4_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void button4_TouchUpInside (UIButton sender);

		[Action ("slider1_valueChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void slider1_valueChanged (UISlider sender);

		void ReleaseDesignerOutlets ()
		{
			if (Button1 != null) {
				Button1.Dispose ();
				Button1 = null;
			}
			if (Button2 != null) {
				Button2.Dispose ();
				Button2 = null;
			}
			if (button3 != null) {
				button3.Dispose ();
				button3 = null;
			}
			if (button4 != null) {
				button4.Dispose ();
				button4 = null;
			}
			if (label1 != null) {
				label1.Dispose ();
				label1 = null;
			}
			if (slider1 != null) {
				slider1.Dispose ();
				slider1 = null;
			}
			if (sliderLabel != null) {
				sliderLabel.Dispose ();
				sliderLabel = null;
			}
			if (switch1 != null) {
				switch1.Dispose ();
				switch1 = null;
			}
			if (textfield1 != null) {
				textfield1.Dispose ();
				textfield1 = null;
			}
			if (textview1 != null) {
				textview1.Dispose ();
				textview1 = null;
			}
		}
	}
}
