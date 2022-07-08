using System;
using CoreGraphics;
using ObjCRuntime;

namespace Controls;

public partial class ControlsViewController : UIViewController
{
	public ControlsViewController (NativeHandle handle) : base (handle)
	{
	}

	public override void DidReceiveMemoryWarning ()
	{
		// Releases the view if it doesn't have a superview.
		base.DidReceiveMemoryWarning ();

		// Release any cached data, images, etc that aren't in use.
	}

	#region View lifecycle

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

		label1.Text = "New Label";
		if (View is not null)
			View.Add (label1);

//			new System.Threading.Thread (new System.Threading.ThreadStart (() => {
//				InvokeOnMainThread (() => {
//					label1.Text = "updated in thread";
//				});
//			})).Start ();

		Button1.TouchUpInside += (sender, e) => {
			label1.Text = "button1 clicked";

			//SIMPLE ALERT

			//var alert = UIAlertController.Create ("Title", "The message", UIAlertControllerStyle.Alert);
			//alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			//PresentViewController (alert, animated: true, completionHandler: null);

			// TWO BUTTON ALERT
			//var alert = UIAlertController.Create ("Alert Title", "Choose from two buttons", UIAlertControllerStyle.Alert);
			//alert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
			//alert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Default, null));
			//PresentViewController (alert, animated: true, completionHandler: null);

			//THREE BUTTON ALERT


			var alert = UIAlertController.Create ("custom buttons alert", "this alert has custom buttons", UIAlertControllerStyle.Alert);

			alert.AddAction(UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, null));
			alert.AddAction(UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
			alert.AddAction(UIAlertAction.Create ("custom button 1", UIAlertActionStyle.Default, null));
			PresentViewController(alert, animated: true, completionHandler: null);


			textfield1.ResignFirstResponder ();
			textview1.ResignFirstResponder ();
		};

		// SLIDER
		slider1.MinValue = -1;
		slider1.MaxValue = 2;
		slider1.Value = 0.5f;

		// customize
		//slider1.MinimumTrackTintColor = UIColor.Gray;
		//slider1.MaximumTrackTintColor = UIColor.Green;

		// BOOLEAN
		switch1.On = true;

		//DISMISS KEYBOARD ON RETURN BUTTON PRESS.
		this.textfield1.ShouldReturn += (textField) => {
			textField.ResignFirstResponder ();
			return true;
		};

		// LAYOUT OPTIONS
		label1.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
		textfield1.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
	}

	partial void slider1_valueChanged (UISlider sender)
	{
		sliderLabel.Text = ((UISlider)sender).Value.ToString ();
	}

	partial void button2_TouchUpInside (UIButton sender)
	{
		textfield1.ResignFirstResponder ();
		textview1.ResignFirstResponder ();

		new UIAlertView ("Button2 touched", "This method was declared as an event, which creates an [Action] in the designer.cs file",
			null, "Cancel", null)
			.Show ();
	}
	
	// Async/Await example
	
	async partial void button3_TouchUpInside (UIButton sender)
	{
		textfield1.ResignFirstResponder ();
		textview1.ResignFirstResponder ();

		label1.Text = "async method started";

		await Task.Delay (1000);

		label1.Text = "1 second passed";

		await Task.Delay (2000);

		label1.Text = "2 more seconds passed";

		await Task.Delay (1000);

		new UIAlertView ("Async method complete", "This method contained async awaits",
			null, "Cancel", null)
			.Show ();

		label1.Text = "async method completed";
	}

	partial void button4_TouchUpInside (UIButton sender)
	{
		//One Button Alert
		UIAlertView alert = new UIAlertView ("Title", "The message", null, "OK", null);
		alert.Show ();

		//Two button Alert
		//UIAlertView alert = new UIAlertView ("Alert Title", "Choose from two buttons", null, "OK", new string[] {"Cancel"});
		//alert.Clicked += (s, b) => {
		//	label1.Text = "Button " + b.ButtonIndex.ToString () + " clicked";
		//	Console.WriteLine ("Button " + b.ButtonIndex.ToString () + " clicked");
		//};
		//alert.Show ();
	}

	public override void ViewWillAppear (bool animated)
	{
		base.ViewWillAppear (animated);
	}

	public override void ViewDidAppear (bool animated)
	{
		base.ViewDidAppear (animated);
	}

	public override void ViewWillDisappear (bool animated)
	{
		base.ViewWillDisappear (animated);
	}

	public override void ViewDidDisappear (bool animated)
	{
		base.ViewDidDisappear (animated);
	}

	#endregion
}
