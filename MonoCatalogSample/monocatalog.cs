//
// The UICatalog sample ported to C#
//
using System;
using System.Collections.Generic;
using System.Collections;

//
// Import the MonoTouch namespaces
//
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

//
// MainViewController: this class is instantiated by MonoTouch
// when the MainWindow.nib file is loaded.   It is the UITableViewController
// for our main UI
//
// Two nested classes are used: one that implemented the information
// repository (DataSource) and the other one that is used to catch
// notifications on the UITableView
//
public partial class MainViewController : UITableViewController {
	static NSString kCellIdentifier = new NSString ("MyIdentifier");

	struct Sample {
		public string Title;
		public UIViewController Controller;

		public Sample (string title, UIViewController controller)
		{
			Title = title;
			Controller = controller;
		}
	}

	Sample [] samples;

	//
	// Constructor invoked from the NIB loader
	//
	public MainViewController (IntPtr p) : base (p) {}

	//
	// The data source for our TableView
	//
	class DataSource : UITableViewDataSource {
		MainViewController mvc;

		public DataSource (MainViewController mvc)
		{
			this.mvc = mvc;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return mvc.samples.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (kCellIdentifier);
			if (cell == null){
				cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}
			cell.TextLabel.Text = mvc.samples [indexPath.Row].Title;
			return cell;
		}
	}

	//
	// This class receives notifications that happen on the UITableView
	//
	class TableDelegate : UITableViewDelegate {
		MainViewController mvc;

		public TableDelegate (MainViewController mvc)
		{
			this.mvc = mvc;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("MonoCatalog: Row selected {0}", indexPath.Row);

			var cont = mvc.samples [indexPath.Row].Controller;
			mvc.NavigationController.PushViewController (cont, true);
		}
	}

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		Title = "MonoTouch UICatalog";
		samples = new Sample [] {
			new Sample ("Address Book", new AddressBookController ()),
			new Sample ("Buttons", new ButtonsViewController ()),
			new Sample ("Controls", new ControlsViewController ()),
			new Sample ("Searchbar", new SearchBarController ()),
			new Sample ("TextField", new TextFieldController ()),
			new Sample ("TextView", new TextViewController ()),
			new Sample ("Pickers", new PickerViewController ()),
			new Sample ("Images", new ImagesViewController ()),
			new Sample ("Segments", new SegmentViewController ()),
			new Sample ("Alerts", new AlertsViewController ()),
			new Sample ("Transitions", new TransitionViewController ()),
			new Sample ("Toolbar", new ToolbarViewController ()),
			new Sample ("Web", new WebViewController ())
		};

		TableView.Delegate = new TableDelegate (this);
		TableView.DataSource = new DataSource (this);

		NavigationItem.BackBarButtonItem = new UIBarButtonItem () { Title = "Back" };
	}

}

//
// The name AppDelegate is referenced in the MainWindow.xib file.
//
public partial class AppDelegate : UIApplicationDelegate {
	//
	// This method is invoked when the application is ready to run
	//
	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		window.AddSubview (navigationController.View);
		window.MakeKeyAndVisible ();

		return true;
	}

	//
	// This method is required in iPhoneOS 3.0
	//
	public override void OnActivated (UIApplication application)
	{
	}
}

class Demo {
	static void Main (string [] args)
	{
		// It will load the main UI as specified in the
		// Info.plist file (MainWindow.nib)
		UIApplication.Main (args, null, null);
	}
}
