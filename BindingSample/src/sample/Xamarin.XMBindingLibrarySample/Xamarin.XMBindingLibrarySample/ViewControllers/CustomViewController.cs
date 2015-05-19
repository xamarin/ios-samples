//
//  Copyright 2012  abhatia
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using MonoTouch.Dialog;
using XMBindingLibrarySample;
using UIKit;
using Foundation;
using CoreGraphics;

namespace Xamarin.XMBindingLibrarySample
{
	public class CustomViewController : DialogViewController
	{
		XMCustomView _CustomView;

		public CustomViewController()
			: base(new RootElement(@"XMCustomView Binding") { UnevenRows = true }, true)
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public override void LoadView()
		{
			base.LoadView();
			_CustomView = new XMCustomView();

			// This is the custom event we bound.
			// This is sometimes preferable to specifying a delegate.
			_CustomView.ViewWasTouched += Handle_CustomViewViewWasTouched;

			// The XMCustomViewDelegate we bound
			// If we specify this it will OVERRIDE the event handler we specified
//			_CustomView.Delegate = new CustomViewDelegate();

			// The XMCustomView Name Property
			_CustomView.Name = @"Anuj";

			// The instance method uses a frame calculation.
			_CustomView.Frame = new CGRect(10, 25, 200, 200);

			// The instance method we bound.
			_CustomView.CustomizeViewWithText(string.Format(@"Yo {0}, I hurd you like bindings! MonoTouch makes it super easy with BTOUCH. Try it out!",
			                                                _CustomView.Name ?? "Dawg"));

			var section = new Section("Custom View") {
				new CustomViewElement(_CustomView),
			};

			this.Root.Add(section);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.NavigationItem.HidesBackButton = false;
			this.NavigationItem.BackBarButtonItem = new UIBarButtonItem("Utilities", UIBarButtonItemStyle.Bordered, null);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
		}

		private void Handle_CustomViewViewWasTouched (object sender, EventArgs e)
		{
			var customView =  sender as XMCustomView;

			if(customView == null) {
				return;
			}

			// Remember, the block of this method is called on a Background Thread...
			// In order to push a notification to the view we need to call within the scope of the main thread.

			using(var pool = new NSAutoreleasePool()) {
				pool.BeginInvokeOnMainThread(() => {
					using(var alert = new UIAlertView("View Was Touched", "Our bound XMCustomView was Touched!", null, "OK, Cool!", null)) {
						alert.Show();
					}
				});
			}
		}

		class CustomViewDelegate : XMCustomViewDelegate
		{
			public override void ViewWasTouched(XMCustomView view)
			{
				Console.WriteLine("Hey! Our XMCustomView was touched with frame: {0}!", view.Frame);
			}
		}

		public class CustomViewElement : UIViewElement, IElementSizing
		{
			public CustomViewElement(UIView customView)
				: base("", customView, true)
			{
			}

			public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
			{
//				base.Selected(dvc, tableView, path);
			}
		}
	}
}