//
// AppDelegate.cs:
//
// Authors:
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
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using AudioToolbox;
using System.Diagnostics;
using System.Threading;

namespace AudioConverterFileConverter
{
	/*
	Since we perform conversion in a background thread, we must ensure that we handle interruptions appropriately.
	In this sample we're using a mutex protected variable tracking thread states. The background conversion threads state transistions from Done to Running
	to Done unless we've been interrupted in which case we are Paused blocking the conversion thread and preventing further calls
	to AudioConverterFillComplexBuffer (since it would fail if we were using the hardware codec).
	Once the interruption has ended, we unblock the background thread as the state transitions to Running once again.
	Any errors returned from AudioConverterFillComplexBuffer must be handled appropriately. Additionally, if the Audio Converter cannot
	resume conversion after an interruption, you should not call AudioConverterFillComplexBuffer again.
	*/
	public enum State
	{
		Running,
		Paused,
		Done
	}

	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		static State State = State.Done;
		static AutoResetEvent StateChanged = new AutoResetEvent (false);
		static readonly object StateLock = new object ();

		// class-level declarations
		
		public override UIWindow Window {
			get;
			set;
		}

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			AudioSession.Initialize ();
			AudioSession.Interrupted += delegate {
				Debug.WriteLine ("Session interrupted");

				ThreadStateBeginInterruption();
			};
			
			AudioSession.Resumed += delegate {
				Debug.WriteLine ("Session resumed");

				Debug.Print (AudioSession.InterruptionType.ToString ());

				AudioSession.SetActive (true);
				ThreadStateEndInterruption();
			};

			// our default category -- we change this for conversion and playback appropriately
			try {
				AudioSession.Category = AudioSessionCategory.SoloAmbientSound;
			} catch {
				Debug.Print ("ERROR: Cannot change audio session category");
			}

			AudioSession.AudioRouteChanged += delegate(object sender, AudioSessionRouteChangeEventArgs e) {
				var gg = e.PreviousInputRoute;

				Debug.Print ("Audio route change: {0}", e.Reason);
				Debug.Print ("Old route: {0}", e.PreviousOutputRoutes[0]);
				Debug.Print ("New route: {0}", e.CurrentOutputRoutes[0]);
			};

			AudioSession.SetActive (true);
			return true;
		}
		
		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
			ThreadStateBeginInterruption();
			AudioSession.SetActive (false);
			Debug.Write ("Audio Session Deactivated");
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		/// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		/// This method is called when the application is about to terminate. Save data, if needed. 
		public override void WillTerminate (UIApplication application)
		{
		}

		static void ThreadStateBeginInterruption()
		{
			Debug.Assert (NSThread.IsMain);

			lock (StateLock) {
				State = State.Paused;
			}
		}

		static void ThreadStateEndInterruption()
		{
			Debug.Assert (NSThread.IsMain);

			lock (StateLock) {
				if (State == State.Paused) {
					State = State.Running;
				
					StateChanged.Set ();
				}
			}
		}

		public static void ThreadStateSetRunning ()
		{
			lock (StateLock) {
				Debug.Assert (State == State.Done);
				State = State.Running;
			}
		}

		// block for state change to State.Running
		public static bool ThreadStatePausedCheck ()
		{
			var wasInterrupted = false;

			lock (StateLock) {
				Debug.Assert (State != State.Done);
			
				while (State == State.Paused) {
					StateChanged.WaitOne ();
					wasInterrupted = true;
				}
			
				// we must be running or something bad has happened
				Debug.Assert (State == State.Running);
			}

			return wasInterrupted;
		}

		public static void ThreadStateSetDone()
		{
			lock (StateLock) {
				Debug.Assert (State != State.Done);
				State = State.Done;
			}
		}
	}
}

