//
// ViewTransitionsViewController.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2011 Xamarin <http://xamarin.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using UIKit;
using CoreGraphics;
using System;
using Foundation;
using CoreAnimation;

namespace ViewTransitions
{
	public partial class ViewTransitionsViewController : UIViewController
	{
		UIImageView view1, view2, view3;
		bool transitioning;

		public ViewTransitionsViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			view1 = new UIImageView (new UIImage ("Images/image1.png"));
			view1.ContentMode = UIViewContentMode.ScaleAspectFit;
			view1.AutoresizingMask = UIViewAutoresizing.All;

			view2 = new UIImageView (new UIImage ("Images/image2.png"));
			view2.Hidden = true;
			view2.ContentMode = UIViewContentMode.ScaleAspectFit;
			view2.AutoresizingMask = UIViewAutoresizing.All;

			view3 = new UIImageView (new UIImage ("Images/image3.png"));
			view3.Hidden = true;
			view3.ContentMode = UIViewContentMode.ScaleAspectFit;
			view3.AutoresizingMask = UIViewAutoresizing.All;

			imageView.AddSubview (view1);
			imageView.AddSubview (view2);
			imageView.AddSubview (view3);

			nextTransitionButton.Clicked += PerformTransition;
		}

		NSString[] types    = new NSString[] { CATransition.TransitionMoveIn, CATransition.TransitionPush, CATransition.TransitionReveal, CATransition.TransitionFade };
		NSString[] supTypes = new NSString[] { CATransition.TransitionFromLeft, CATransition.TransitionFromRight, CATransition.TransitionFromTop, CATransition.TransitionFromBottom };

		Random random = new Random ();

		void PerformTransition (object sender, EventArgs e)
		{
			if (transitioning)
				return;

			// First create a CATransition object to describe the transition
			var transition = new CATransition ();

			// Animate over 3/4 of a second
			transition.Duration = 0.75;

			// using the ease in/out timing function
			transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);

			// Now to set the type of transition
			int rnd = random.Next (types.Length);
			transition.Type = types[rnd].ToString ();
			if (rnd < 3) // no fade transition -> randomly choose a subtype
				transition.Subtype = supTypes[random.Next (supTypes.Length)].ToString ();

			// Assign a delegate that sets transitioning to false, after the animation stopped.
			this.transitioning = true;
			transition.Delegate = new AnimationDelegate (this);

			// Now add the animation to the image view layer. This will perform the transiton.
			imageView.Layer.AddAnimation (transition, null);

			// Now hide view 1 & show view 2 that causes Core Animation to animate view1 away and show view2
			view1.Hidden = true;
			view2.Hidden = false;

			// Now cycle through the views
			Swap (ref view1, ref view2);
			Swap (ref view2, ref view3);
		}

		class AnimationDelegate : CAAnimationDelegate
		{
			ViewTransitionsViewController ctrl;

			public AnimationDelegate (ViewTransitionsViewController ctrl)
			{
				this.ctrl = ctrl;
			}

			public override void AnimationStopped (CAAnimation anim, bool finished)
			{
				ctrl.transitioning = false;
			}
		}

		void Swap (ref UIImageView view1, ref UIImageView view2)
		{
			UIImageView tmp = view1;
			view1 = view2;
			view2 = tmp;
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			view1.Dispose ();
			view2.Dispose ();
			view3.Dispose ();
		}
	}
}
