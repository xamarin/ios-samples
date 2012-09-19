using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.CoreText;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CustomFonts
{
	public partial class RootViewController : UITableViewController
	{
		public List<UIFont> Fonts;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public RootViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Fonts", "Fonts");
			
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				this.ClearsSelectionOnViewWillAppear = false;
				this.ContentSizeForViewInPopover = new SizeF (320, 600);
			}

			TableView.Source = new TableSource (this);

			// Get all the fonts that have been embedded with our application
			var fontList = FontLoader.SharedFontLoader.AvailableFonts ();
			//fontList.Remove ("FallbackTestFont"); //get rid of the TestFallbackFont as we don't 
			//want to display it in our list of fonts.

			// Add some fonts to this list that are already registered 
			// (including fonts that were bundled using the UIAppFonts key in our Info.plist
			var range = new List<string> { "HelveticaNeue", "GillSans", "Optima-Regular", "Baskerville", "DigiGrad", "DigiShad", null } ;
			//fontList.AddRange (range.AsEnumerable ());

			fontList.Sort (); 
			//Fonts = fontList.sortedArrayUsingSelector();
			Fonts = fontList;
		}
		
		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) { 
				TableView.SelectRow (NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.Middle);
			}
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

	public class TableSource : UITableViewSource
	{
		RootViewController root;
		DetailViewController detail;
		NSString cellID = new NSString ("Cell");
	
		public TableSource (RootViewController root) : base ()
		{
			this.root = root;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellID);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, cellID);
				
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
			}
			
			// Configure the cell
			cell.TextLabel.Text = root.Fonts.ElementAt((int) indexPath.IndexAtPosition (1)).Name;
			return cell;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}		

		public override int RowsInSection (UITableView tableview, int section)
		{
			return root.Fonts.Count;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				if (detail == null) {
					var sb = UIStoryboard.FromName ("MainStoryboard_iPhone", null);
					detail = (DetailViewController) sb.InstantiateViewController ("detail");
				}
				root.NavigationController.PushViewController (detail, true);
			}

			string postName = root.Fonts.ElementAt ((int) indexPath.IndexAtPosition (1)).Name;

			detail.SetDetailItem (FontLoader.SharedFontLoader.FontWithName (postName, detail.CUSTOM_FONT_SIZE));
		}
	}
}