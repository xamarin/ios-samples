using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Foundation;
using UIKit;

namespace Popovers
{
	public partial class DetailViewController : UIViewController
	{
		[Outlet]
		public UIToolbar Toolbar { get; set; }

		[Outlet]
		public NSObject DetailItem { get; set; }
	
		[Outlet]
		public UIPopoverController DetailViewPopover { get; set; }
		
		[Outlet]
		public UIPopoverController BarButtonItemPopover { get; set; }
		
		[Outlet]
		public UIBarButtonItem MyBarButtonItem { get; set; }
		
		[Outlet]
		public UIButton Button1 { get; set; }

		[Outlet]
		public UIButton Button2 { get; set; }

		[Outlet]
		public UIButton Button3 { get; set; }

		[Outlet]
		public UIButton Button4 { get; set; }

		[Outlet]
		public UIButton Button5 { get; set; }
		
		[Outlet]
		public NSObject LastTappedButton { get; set; }
		
		public UIPopoverController MainPopoverController { get; set; }

		class SplitViewDelegate : UISplitViewControllerDelegate {
			internal DetailViewController Parent { get; set; }

			public override void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc)
			{
				barButtonItem.Title =  "MasterList";
				var items = new List<UIBarButtonItem> (Parent.Toolbar.Items);
				items.Insert (0, barButtonItem);
				Parent.Toolbar.SetItems (items.ToArray (), true);
				Parent.MainPopoverController = pc;
			}
			
			public override void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button)
			{
				var items = new List<UIBarButtonItem> (Parent.Toolbar.Items);
				items.RemoveAt (0);
				Parent.Toolbar.SetItems (items.ToArray (), true);
				Parent.MainPopoverController = null;
				
			}
			
			public override void WillPresentViewController (UISplitViewController svc, UIPopoverController pc, UIViewController aViewController)
			{
				if (Parent.BarButtonItemPopover.PopoverVisible)
					Parent.BarButtonItemPopover.Dismiss (true);
			}
		}
		
		public DetailViewController (IntPtr handle) : base (handle)
		{		
			// lost connection to DetailView.xib, create buttons manually for now
			Button1 = new UIButton();
			Button2 = new UIButton();
			Button3 = new UIButton();
			Button4 = new UIButton();
			Button5 = new UIButton();
		}
		
		//loads the DetailViewController.xib file and connects it to this object
		public DetailViewController () : base ("DetailViewController", null)
		{		
			// lost connection to DetailView.xib, create buttons manually for now
			Button1 = new UIButton();
			Button2 = new UIButton();
			Button3 = new UIButton();
			Button4 = new UIButton();
			Button5 = new UIButton();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			var content = new PopoverContentViewController ();
			DetailViewPopover = new UIPopoverController (content);
			DetailViewPopover.PopoverContentSize = new SizeF (320, 320);
			DetailViewPopover.DidDismiss += delegate { LastTappedButton = null; };
			
			BarButtonItemPopover = new UIPopoverController (content);
			BarButtonItemPopover.PopoverContentSize = new SizeF (320, 320);
			BarButtonItemPopover.DidDismiss += delegate { LastTappedButton = null; };

		}

		[Action ("showPopover:")]
		public void ShowPopover (NSObject sender) {
			// Set the sender to a UIButton.
			UIButton tappedButton = (UIButton)sender;

			// Present the popover from the button that was tapped in the detail view.
			DetailViewPopover.PresentFromRect (tappedButton.Frame, View, UIPopoverArrowDirection.Any, true);
		
			// Set the last button tapped to the current button that was tapped.
			LastTappedButton = sender;
		}

		
		[Action ("showPopoverFromBarButtonItem:")]
		public void ShowPopoverFromBarButtonItem (NSObject sender) {
			// Set the sender to a UIBarButtonItem.
			UIBarButtonItem tappedButton = (UIBarButtonItem)sender;

			// If the master list popover is showing, dismiss it before presenting the popover from the bar button item.
			if (MainPopoverController != null)
				MainPopoverController.Dismiss (true);
			
			// If the popover is already showing from the bar button item, dismiss it. Otherwise, present it.
			if (!BarButtonItemPopover.PopoverVisible)
				BarButtonItemPopover.PresentFromBarButtonItem (tappedButton, UIPopoverArrowDirection.Any, true);
			else
				BarButtonItemPopover.Dismiss (true);
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			// If the detail popover is presented, dismiss it.
			if (DetailViewPopover != null)
				DetailViewPopover.Dismiss (true);
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			 if (LastTappedButton != null)
				ShowPopover (LastTappedButton);
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				Button1.Frame = new RectangleF (20, 64, 160, 160);
				Button2.Frame = new RectangleF (588, 64, 160, 160);
				Button3.Frame = new RectangleF (304, 422, 160, 160);
				Button4.Frame = new RectangleF (20, 824, 160, 160);
				Button5.Frame = new RectangleF (588, 824, 160, 160);
			} else {
				Button1.Frame = new RectangleF (20, 64, 160, 160);
				Button2.Frame = new RectangleF(524, 64, 160, 160);
				Button3.Frame = new RectangleF (272, 311, 160, 160);
				Button4.Frame = new RectangleF (20, 568, 160, 160);
				Button5.Frame = new RectangleF (524, 568, 160, 160);
			}
			return true;
		}

		public void WillHideViewController (object sender, UISplitViewHideEventArgs args)
		{
			args.BarButtonItem.Title =  "MasterList";
			var items = new List<UIBarButtonItem> (Toolbar.Items);
			items.Insert (0, args.BarButtonItem);
			Toolbar.SetItems (items.ToArray (), true);
			MainPopoverController = args.Pc;
		}
		
		public void WillShowViewController (object sender, UISplitViewShowEventArgs args)
		{
			var items = new List<UIBarButtonItem> (Toolbar.Items);
			items.RemoveAt (0);
			Toolbar.SetItems (items.ToArray (), true);
			MainPopoverController = null;
		}
		
		public void WillPresentViewController (object sender, UISplitViewPresentEventArgs args)
		{
			if (BarButtonItemPopover.PopoverVisible)
				BarButtonItemPopover.Dismiss (true);
		}

	}
}
