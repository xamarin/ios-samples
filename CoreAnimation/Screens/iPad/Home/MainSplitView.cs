using System;
using UIKit;
using System.Reflection;
using Example_CoreAnimation.Screens.iPad.NavTable;
using Example_CoreAnimation.Code.NavigationTable;
using ObjCRuntime;
using Foundation;

namespace Example_CoreAnimation.Screens.iPad.Home
{
	public class MainSplitView : UISplitViewController
	{
		private MasterNavTableViewController masterViewController;
		private UIViewController detailViewController;
		private Selector toggleMasterVisible;

		private bool MasterViewHidden { get; set; }

		public MainSplitView () : base ()
		{
			MasterViewHidden = true;

			// create our master and detail views
			masterViewController = new Screens.iPad.NavTable.MasterNavTableViewController ();
			detailViewController = new Screens.iPad.BasicUIViewAnimation.BasicUIViewAnimationScreen ();

			// create an array of controllers from them and then assign it to the 
			// controllers property
			ViewControllers = new UIViewController[] { masterViewController,  detailViewController };
			
			// in this example, i expose an event on the master view called RowClicked, and i listen 
			// for it in here, and then call a method on the detail view to update. this class thereby 
			// becomes the defacto controller for the screen (both views).
			masterViewController.RowClicked += (sender, e) => {
				HandleRowClicked (e);
			};
			
			// when the master view controller is hid (portrait mode), we add a button to 
			// the detail view that when clicked will show the master view in a popover controller
			WillHideViewController += (sender, e) => {
				toggleMasterVisible = e.BarButtonItem.Action;
			};

			ShouldHideViewController = (svc, vc, orientation) => {
				return MasterViewHidden &&
				(orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown);
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			(detailViewController as IDetailView).ContentsButtonClicked += ContentsButtonClickHandler;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			ToogleMasterViewVisibility (toInterfaceOrientation);
			base.WillRotate (toInterfaceOrientation, duration);
		}

		protected void HandleRowClicked (RowClickedEventArgs e)
		{
			Console.WriteLine ("Changing Screens");
			(detailViewController as IDetailView).ContentsButtonClicked -= ContentsButtonClickHandler;

			// if the nav item has a proper controller, push it on to the NavigationController
			// NOTE: we could also raise an event here, to loosely couple this, but isn't neccessary,
			// because we'll only ever use this this way
			if (e.Item.Controller != null) {
				UIView.BeginAnimations ("DetailViewPush");
				detailViewController = e.Item.Controller;
				ViewControllers = new UIViewController[] { masterViewController,  detailViewController };
				UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromRight, ViewControllers [1].View, false);
				UIView.CommitAnimations ();
			} else {
				if (e.Item.ControllerType != null) {
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
							UIView.BeginAnimations ("DetailViewPush");
							detailViewController = e.Item.Controller;
							ViewControllers = new UIViewController[] { masterViewController,  detailViewController };
							UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromRight, ViewControllers [1].View, false);
							UIView.CommitAnimations ();
						} else
							Console.WriteLine ("instance of view controller not created");
					} else
						Console.WriteLine ("constructor not found");
				}
			}

			ToogleMasterViewVisibility (InterfaceOrientation);
			(detailViewController as IDetailView).ContentsButtonClicked += ContentsButtonClickHandler;
		}

		private void ContentsButtonClickHandler (object sender, EventArgs e)
		{
			MasterViewHidden = false;
			ToogleMasterViewVisibility (InterfaceOrientation);
			MasterViewHidden = true;
		}

		private void ToogleMasterViewVisibility (UIInterfaceOrientation interfaceOrientation)
		{
			ShouldHideViewController.Invoke (this, masterViewController, interfaceOrientation);
			PerformSelector (toggleMasterVisible, null, 0);
		}
	}
}

