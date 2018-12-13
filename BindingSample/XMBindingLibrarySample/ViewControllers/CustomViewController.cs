using System;
using CoreGraphics;
using Foundation;
using MonoTouch.Dialog;
using UIKit;

using XMBindingLibrary;

namespace XMBindingLibrarySample
{
	public class CustomViewController : DialogViewController
	{
		XMCustomView customView;

		public CustomViewController()
			: base(new RootElement("XMCustomView Binding") { UnevenRows = true }, true)
		{
		}

		public override void LoadView()
		{
			base.LoadView();

			customView = new XMCustomView
			{
				// The XMCustomView Name Property
				Name = "Xamarin User",

				// The instance method uses a frame calculation.
				Frame = new CGRect(10, 10, View.Bounds.Width - 40, 150),
			};

			// This is the custom event we bound.
			// This is sometimes preferable to specifying a delegate.
			customView.ViewWasTouched += Handle_CustomViewViewWasTouched;

			// The XMCustomViewDelegate we bound
			// If we specify this it will OVERRIDE the event handler we specified
			// customView.Delegate = new CustomViewDelegate();

			// The instance method we bound.
			customView.CustomizeView($"Yo {customView.Name}, I heard you like bindings! Xamarin makes it super easy with binding projects. Try it out!");

			Root.Add(new[]
			{
				new Section("Custom View")
				{
					new UIViewElement("", customView, true, new UIEdgeInsets(0, 0, 40, 40)),
				},

				new Section("Operations")
				{
					new StringElement("Use Event", Handle_UseEvent),
					new StringElement("Use Delegate", Handle_UseDelegate),
					new StringElement("Do Touch", Handle_DoTouchOperation),
				}
			});
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = false;
			NavigationItem.BackBarButtonItem = new UIBarButtonItem("Utilities", UIBarButtonItemStyle.Plain, null);
		}

		void Handle_UseEvent()
		{
			customView.Delegate = null;

			// attach the event
			customView.ViewWasTouched += Handle_CustomViewViewWasTouched;
		}

		void Handle_UseDelegate()
		{
			// set the delegate
			customView.Delegate = new CustomViewDelegate(this);
		}

		void Handle_DoTouchOperation()
		{
			using (var temp = new CustomViewDelegate(this))
			{
				customView.DoTouch(temp);
			}
		}

		void Handle_CustomViewViewWasTouched(object sender, EventArgs e)
		{
			Handle_ViewTouched("EVENT");
		}

		void Handle_ViewTouched(string where)
		{
			NSThread.Current.BeginInvokeOnMainThread(() =>
			{
				var vc = UIAlertController.Create("Touched", $"Our bound XMCustomView was touched from the {where}!", UIAlertControllerStyle.Alert);
				vc.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(vc, true, null);
			});
		}

		class CustomViewDelegate : XMCustomViewDelegate
		{
			CustomViewController viewController;

			public CustomViewDelegate(CustomViewController viewController)
			{
				this.viewController = viewController;
			}

			public override void ViewWasTouched(XMCustomView view)
			{
				viewController.Handle_ViewTouched("DELEGATE");
			}
		}
	}
}
