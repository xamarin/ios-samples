//	
// This sample shows how to use the two Http stacks in MonoTouch:
// The System.Net.WebRequest.
// The Foundation.NSMutableUrlRequest
//

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using Foundation;
using UIKit;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;

namespace HttpClient
{
	public class Application
	{
		// URL where we fetch the wisdom from
		public const string WisdomUrl = "http://httpbin.org/ip";

		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}

		public static void Busy ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
		}
		
		public static void Done ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;	
		}
			
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.AddSubview (navigationController.View);
			
			button1.TouchDown += Button1TouchDown;
			TableViewSelector.Configure (this.stack, new string [] {
				"http  - WebRequest",
				"https - WebRequest",
				"http  - NSUrlConnection",
				"http  - HttpClient/CFNetwork"
			});
			                   
			window.MakeKeyAndVisible ();
			
			return true;
		}

		async void Button1TouchDown (object sender, EventArgs e)
		{
			// Do not queue more than one request
			if (UIApplication.SharedApplication.NetworkActivityIndicatorVisible)
				return;
			
			switch (stack.SelectedRow ()){
			case 0:
				new DotNet (this).HttpSample ();
				break;
			
			case 1:
				new DotNet (this).HttpSecureSample ();
				break;
				
			case 2:
				new Cocoa (this).HttpSample ();
				break;

			case 3:
				await new NetHttp (this).HttpSample ();
				break;
			}
		}
/*		
		public void RenderRssStream (Stream stream)
		{	
			var doc = XDocument.Load (new XmlTextReader (stream));
			var items = doc.XPathSelectElements ("./rss/channel/item/title");

			//
			// Since this is invoked on a separated thread, make sure that
			// we call UIKit only from the main thread.
			//
			InvokeOnMainThread (delegate {
				var table = new UITableViewController ();
				navigationController.PushViewController (table, true);
				
				// Put the data on a string [] so we can use our existing 
				// UITableView renderer for strings.
				string [] entries = new string [items.Count ()];
				int i = 0;
				foreach (var e in items)
					entries [i++] = e.Value;
				
				TableViewSelector.Configure (table.View as UITableView, entries);
			});
		}
*/		
		public void RenderStream (Stream stream)
		{
			var reader = new System.IO.StreamReader (stream);

			InvokeOnMainThread (delegate {
				var view = new UIViewController ();
				var label = new UILabel (new RectangleF (20, 20, 300, 80)){
					Text = "The HTML returned by the server:"
				};
				var tv = new UITextView (new RectangleF (20, 100, 300, 400)){
					Text = reader.ReadToEnd ()
				};
				view.Add (label);
				view.Add (tv);
					
				if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) { 
					view.EdgesForExtendedLayout = UIRectEdge.None;
				}

				navigationController.PushViewController (view, true);
			});			
		}
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{

		}
	}
}
