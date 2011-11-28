using System;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;

namespace Example_SplitView.Screens.MainSplitView
{
	public class MainSplitView : UISplitViewController
	{		
		protected Screens.MasterView.MasterTableView masterView;
		protected Screens.DetailView.DetailViewScreen detailView;
		
		public MainSplitView () : base()
		{
			// create our master and detail views
			masterView = new Screens.MasterView.MasterTableView ();
			detailView = new Screens.DetailView.DetailViewScreen ();

			// create an array of controllers from them and then assign it to the 
			// controllers property
			ViewControllers = new UIViewController[] { masterView, detailView };
			
			// in this example, i expose an event on the master view called RowClicked, and i listen 
			// for it in here, and then call a method on the detail view to update. this class thereby 
			// becomes the defacto controller for the screen (both views).
			masterView.RowClicked += (object sender, MasterView.MasterTableView.RowClickedEventArgs e) => {
				detailView.Text = e.Item;
			};
			
			// when the master view controller is hid (portrait mode), we add a button to 
			// the detail view that when clicked will show the master view in a popover controller
			this.WillHideViewController += (object sender, UISplitViewHideEventArgs e) => {
				detailView.AddContentsButton(e.BarButtonItem);
			};

			// when the master view controller is shown (landscape mode), remove the button
			// since the controller is shown.
			this.WillShowViewController += (object sender, UISplitViewShowEventArgs e) => {
				detailView.RemoveContentsButton ();
			};
		}
	}
}

