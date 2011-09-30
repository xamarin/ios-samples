using System;
using MonoTouch.UIKit;
using Example_CoreAnimation.Code.NavigationTable;
using System.Reflection;
namespace Example_CoreAnimation.Screens.iPad.Home
{
	public class MainSplitView : UISplitViewController
	{
		#region  declarations 
		
		protected Screens.iPad.NavTable.MasterNavTableViewController masterView;
		protected UIViewController detailView;
		
		#endregion
		
		public MainSplitView () : base()
		{
			// create our master and detail views
			masterView = new Screens.iPad.NavTable.MasterNavTableViewController ();
			detailView = new Screens.iPad.BasicUIViewAnimation.BasicUIViewAnimationScreen ();

			// create an array of controllers from them and then assign it to the 
			// controllers property
			this.ViewControllers = new UIViewController[] {  masterView,  detailView };
			
			// in this example, i expose an event on the master view called RowClicked, and i listen 
			// for it in here, and then call a method on the detail view to update. this class thereby 
			// becomes the defacto controller for the screen (both views).
			 masterView.RowClicked += (object sender, RowClickedEventArgs e) => {
				this.HandleRowClicked (e);
			};
			
			// when the master view controller is hid (portrait mode), we add a button to 
			// the detail view that when clicked will show the master view in a popover controller
			this.WillHideViewController += (object sender, UISplitViewHideEventArgs e) => {
				(detailView as IDetailView).AddContentsButton (e.BarButtonItem);
			};

			// when the master view controller is shown (landscape mode), remove the button
			// since the controller is shown.
			this.WillShowViewController += (object sender, UISplitViewShowEventArgs e) => {
				(detailView as IDetailView).RemoveContentsButton ();
			};
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		protected void HandleRowClicked(RowClickedEventArgs e)
		{
			Console.WriteLine("Changing Screens");
			// if the nav item has a proper controller, push it on to the NavigationController
			// NOTE: we could also raise an event here, to loosely couple this, but isn't neccessary,
			// because we'll only ever use this this way
			if (e.Item.Controller != null)
			{
				UIView.BeginAnimations("DetailViewPush");
				detailView = e.Item.Controller;
				this.ViewControllers = new UIViewController[] { masterView,  detailView };
				UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromRight, this.ViewControllers[1].View, false);
				UIView.CommitAnimations();
			}
			else
			{
				if (e.Item.ControllerType != null)
				{
					//
					ConstructorInfo ctor = null;
					
					// if the nav item has constructor aguments
					if (e.Item.ControllerConstructorArgs.Length > 0) {
						// look for the constructor
						ctor = e.Item.ControllerType.GetConstructor (e.Item.ControllerConstructorTypes);
					} else {
						// search for the default constructor
						ctor = e.Item.ControllerType.GetConstructor (System.Type.EmptyTypes);
					}
					
					// if we found the constructor
					if (ctor != null) {
						//
						UIViewController instance = null;
						
						if (e.Item.ControllerConstructorArgs.Length > 0) {
							// instance the view controller
							instance = ctor.Invoke (e.Item.ControllerConstructorArgs) as UIViewController;
						} else {
							// instance the view controller
							instance = ctor.Invoke (null) as UIViewController;
						}
						
						if (instance != null) {
							// save the object
							e.Item.Controller = instance;
							
							// push the view controller onto the stack
							UIView.BeginAnimations("DetailViewPush");
							detailView = e.Item.Controller;
							this.ViewControllers = new UIViewController[] { masterView,  detailView};
							UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromRight, this.ViewControllers[1].View, false);
							UIView.CommitAnimations();
						}
						else
							Console.WriteLine ("instance of view controller not created");
					}
					else
						Console.WriteLine ("constructor not found");
				}
			}

		}

	}
}

