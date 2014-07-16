using System;
using CoreGraphics;

using OpenTK.Graphics.ES20;
using OpenTK;

using OpenGLES;
using CoreAnimation;
using Foundation;
using GLKit;
using UIKit;

namespace OpenGLScroller
{
	public class ViewController : GLKViewController
	{
		CADisplayLink displayLink;
		UIScrollView scrollView;

		public CubeView CubeView { get; set; }

		public ViewController () : base ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View = CubeView = new CubeView (View.Frame);

			scrollView = new UIScrollView ();
			scrollView.Frame = CubeView.ScrollableFrame;
			scrollView.ContentSize = CubeView.ScrollableContentSize;
			scrollView.ShowsHorizontalScrollIndicator = false;
			scrollView.Scrolled += Scrolled;
			scrollView.DraggingStarted += DraggingStarted;
			scrollView.DraggingEnded += DraggingEnded;
			scrollView.DecelerationEnded += DecelerationEnded;
			scrollView.Hidden = true;
			View.AddSubview (scrollView);

			UIView dummyView = new UIView (scrollView.Frame);
			dummyView.AddGestureRecognizer (scrollView.PanGestureRecognizer);
			View.AddSubview (dummyView);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
		
		void Scrolled (object sender, EventArgs ea)
		{
			CubeView.scrollOffset = scrollView.ContentOffset;
		}
		
		void DraggingStarted (object sender, EventArgs ea)
		{
			StartDisplayLinkIfNeeded ();
		}
		
		 void DraggingEnded (object sender, DraggingEventArgs ea)
		{
			if (!ea.Decelerate)
				StopDisplayLink ();
		}
		
		void DecelerationEnded (object sender, EventArgs ea)
		{
			StopDisplayLink ();
		}

		void StartDisplayLinkIfNeeded ()
		{
			if (displayLink == null) {
				displayLink = CADisplayLink.Create (() => CubeView.Display ());
				displayLink.AddToRunLoop (NSRunLoop.Main, NSRunLoop.UITrackingRunLoopMode); 
			}	
		}

		void StopDisplayLink ()
		{
			if (displayLink != null) {
				displayLink.Invalidate ();
				displayLink = null;
			}
		}
	}
}

