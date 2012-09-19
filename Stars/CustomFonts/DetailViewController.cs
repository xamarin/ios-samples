using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreText;

namespace CustomFonts
{
	public partial class DetailViewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		UIPopoverController popoverController;
		UIFont detailItem;

		public int CUSTOM_FONT_SIZE = 36;

		[Export("detailItem")]
		public UIFont DetailItem {
			get {
				return detailItem;
			}
			set {
				SetDetailItem (value);
			}
		}
		
		public DetailViewController (IntPtr handle) : base (handle)
		{

		}

		
		public void SetDetailItem (UIFont newDetailItem)
		{
			if (detailItem != newDetailItem) {
				detailItem = newDetailItem;
				
				// Update the view
				ConfigureView ();
			}
			
			if (this.popoverController != null)
				this.popoverController.Dismiss (true);
		}
		
		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (DetailItem != null) {
				Title = DetailItem.Name;
				var ctFont = new CTFont (Title, detailItem.PointSize);

				lowercaseLabel.ctFont = ctFont;
				capitalsLabel.ctFont = ctFont;
				digitsLabel.ctFont = ctFont;

				lowercaseLabel.SetNeedsDisplay ();
				capitalsLabel.SetNeedsDisplay ();
				digitsLabel.SetNeedsDisplay ();

				if (ctFont != null)
					ctFont.Dispose();
			}
		}
		
		#region View lifecycle
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var postName = FontLoader.SharedFontLoader.AvailableFonts().ElementAt(0).Name;
			lowercaseLabel.text = "aeiou";
			capitalsLabel.text = "AEIOU";
			digitsLabel.text = "12345";

			SetDetailItem (FontLoader.SharedFontLoader.FontWithName (postName, CUSTOM_FONT_SIZE));

			ConfigureView ();

			if (!UserInterfaceIdiomIsPhone)
				SplitViewController.Delegate = new SplitViewControllerDelegate ();
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

		
		#region Split View
		
		class SplitViewControllerDelegate : UISplitViewControllerDelegate
		{
			public override void WillHideViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem barButtonItem, UIPopoverController pc)
			{
				var dv = svc.ViewControllers [1] as DetailViewController;
				barButtonItem.Title = "Fonts";
				var items = new List<UIBarButtonItem> ();
				items.Add (barButtonItem);
				items.AddRange (dv.toolbar.Items);
				dv.toolbar.SetItems (items.ToArray (), true);
				dv.popoverController = pc;
			}
			
			public override void WillShowViewController (UISplitViewController svc, UIViewController aViewController, UIBarButtonItem button)
			{
				var dv = svc.ViewControllers [1] as DetailViewController;
				var items = new List<UIBarButtonItem> (dv.toolbar.Items);
				items.RemoveAt (0);
				dv.toolbar.SetItems (items.ToArray (), true);
				dv.popoverController = null;
			}
		}
		
		#endregion
	}
}

