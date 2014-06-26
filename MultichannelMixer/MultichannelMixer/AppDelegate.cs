//
// AppDelegate.cs:
//
// Authors:
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2014 Xamarin Inc
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
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Foundation;
using UIKit;
using AVFoundation;

namespace MultichannelMixer
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		
		public override UIWindow Window {
			get;
			set;
		}
		
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		public override async void FinishedLaunching (UIApplication application)
		{
			// Configure the audio session
			var sessionInstance = AVAudioSession.SharedInstance ();

			// our default category -- we change this for conversion and playback appropriately
			NSError error = sessionInstance.SetCategory (AVAudioSessionCategory.Playback);

			if (!sessionInstance.SetPreferredIOBufferDuration (0.005, out error))
				throw new ApplicationException ();

			if (!sessionInstance.SetPreferredSampleRate (44100, out error))
				throw new ApplicationException ();

			// add interruption handler
			sessionInstance.BeginInterruption += (object sender, EventArgs e) => ((MultichannelMixerViewController)Window.RootViewController).StopForInterruption ();
			sessionInstance.EndInterruption += (object sender, EventArgs e) => AVAudioSession.SharedInstance ().SetActive (true); // make sure to activate the session

			// activate the audio session
			if (!sessionInstance.SetActive (true, out error))
				throw new ApplicationException ();

			Debug.Print ("Hardware Sample Rate: {0} Hz", sessionInstance.SampleRate);

			// initialize the mixerController object
			var controller = (MultichannelMixerViewController)Window.RootViewController;
			controller.Mixer.InitializeAUGraph ();
		}
	}
}

