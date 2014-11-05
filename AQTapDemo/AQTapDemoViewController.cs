//
// AQTapDemoViewController.cs:
//
// Authors:
//   Chris Adamson (cadamson@subfurther.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using CoreGraphics;

using Foundation;
using UIKit;

namespace AQTapDemo
{
	public partial class AQTapDemoViewController : UIViewController
	{
		CCFWebRadioPlayer player;

		public AQTapDemoViewController () : base ("AQTapDemoViewController", null)
		{
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			const string STATION_URL_STRING = "http://1661.live.streamtheworld.com:80/CBC_R3_WEB_SC";
			player = new CCFWebRadioPlayer (new NSUrl (STATION_URL_STRING));
			stationURLLabel.Text = player.StationURL.AbsoluteString;
			player.Start ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		partial void handlePitchSliderValueChanged (NSObject sender)
		{
			ResetPitch ();
		}

		partial void handleResetTo1Tapped (NSObject sender)
		{
			pitchSlider.Value = 1.0f;
			ResetPitch ();
		}

		void ResetPitch ()
		{
			player.SetPitch (pitchSlider.Value);
			pitchLabel.Text = pitchSlider.Value.ToString ("0.000");
		}
	}
}

