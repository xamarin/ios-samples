
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Foundation;
using UIKit;

namespace MonoDevelopTouchCells
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		private DetailViewController detailViewController = new DetailViewController ();
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			this.InitializeTableData();

			navigationController.NavigationBar.Translucent = false;
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			       
			window.AddSubview(this.navigationController.View);
			
			window.MakeKeyAndVisible ();

			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
		
		public void ShowDetail(CustomCell cell) {
			this.detailViewController.ShowDetail(cell);
			this.navigationController.PushViewController(
				this.detailViewController, true);
		}
		
		private void InitializeTableData() {
			string source = Path.Combine(Directory.GetCurrentDirectory(), "data.xml");
			
			source = "file://" + source;
			
			XDocument xdoc = XDocument.Load(source);//Path.Combine(Directory.GetCurrentDirectory(), "data.xml"));
			
			var items = from c in xdoc.Descendants("item")
					select new Item {
						Title = (string)c.Element("title"),
						Checked = (bool)c.Element("checked"),
					}; 
			
			myTableView.Delegate = new TableViewDelegate();
			myTableView.InvokeOnMainThread (delegate {
			  myTableView.DataSource = new DataSource(items);
			});
		}
		
	}
}
