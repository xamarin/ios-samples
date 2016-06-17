using System.IO;
using System.Xml.Linq;
using System.Linq;
using Foundation;
using UIKit;

namespace MonoDevelopTouchCells
{
	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		DetailViewController detailViewController = new DetailViewController ();

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			InitializeTableData ();

			navigationController.NavigationBar.Translucent = false;
			window.RootViewController = navigationController.TopViewController;
			window.AddSubview (navigationController.View);

			window.MakeKeyAndVisible ();

			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}

		public void ShowDetail (CustomCell cell)
		{
			this.detailViewController.ShowDetail (cell);
			this.navigationController.PushViewController (
				this.detailViewController, true);
		}

		void InitializeTableData ()
		{
			string source = Path.Combine (Directory.GetCurrentDirectory (), "data.xml");

			source = "file://" + source;

			XDocument xdoc = XDocument.Load (source);//Path.Combine(Directory.GetCurrentDirectory(), "data.xml"));

			var items = from c in xdoc.Descendants ("item")
				select new Item {
				Title = (string)c.Element ("title"),
				Checked = (bool)c.Element ("checked"),
			};

			myTableView.Delegate = new TableViewDelegate ();
			myTableView.InvokeOnMainThread (delegate {
				myTableView.DataSource = new DataSource (items);
			});
		}
	}
}

