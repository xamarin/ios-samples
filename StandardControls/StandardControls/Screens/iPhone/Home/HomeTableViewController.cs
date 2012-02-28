using System;
using MonoTouch.UIKit;
using Xamarin.Code;
using System.Collections.Generic;

namespace Example_StandardControls.Screens.iPhone.Home
{
	public class HomeNavController : UITableViewController
	{
		// declare vars
		List<NavItemGroup> navItems = new List<NavItemGroup>();
		NavItemTableSource tableSource;
		
		public HomeNavController () : base(UITableViewStyle.Grouped)
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			// hide the nav bar when this controller appears
			this.NavigationController.SetNavigationBarHidden (true, true);
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// show the nav bar when other controllers appear
			this.NavigationController.SetNavigationBarHidden (false, true);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			
			// create the navigation items
			NavItemGroup navGroup = new NavItemGroup ("Form Controls");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Labels", "", typeof(Labels.LabelsScreen_iPhone)));
			navGroup.Items.Add (new NavItem ("Text Fields", "", typeof(TextFields.TextFields_iPhone)));
			navGroup.Items.Add (new NavItem ("Sliders", "", typeof(Sliders.Sliders_iPhone)));
			navGroup.Items.Add (new NavItem ("Buttons", "", typeof(Buttons.ButtonsScreen_iPhone)));
			navGroup.Items.Add (new NavItem ("Switches", "", typeof(Switches.Switches_iPhone)));
			navGroup.Items.Add (new NavItem ("Segmented Buttons", "", typeof(SegmentedControl.SegmentedControls_iPhone)));
			navGroup.Items.Add (new NavItem ("Segmented Buttons 2", "", typeof(SegmentedControl.SegmentedControls2_iPhone)));
			
			navGroup = new NavItemGroup ("Content Controls");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Scroll View", "", typeof(ScrollView.Controller)));
			navGroup.Items.Add (new NavItem ("Tap to Zoom Scroll View", "", typeof(TapToZoomScrollView.Controller)));
			navGroup.Items.Add (new NavItem ("Pager Control", "", typeof(PagerControl.PagerControl_iPhone)));
			navGroup.Items.Add (new NavItem ("Image Control", "", typeof(Images.Images_iPhone)));
			navGroup.Items.Add (new NavItem ("More Image Controls", "", typeof(Images.Images2_iPhone)));
			
			navGroup = new NavItemGroup ("Process Controls");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Activity Spinners", "", typeof(ActivitySpinner.ActivitySpinnerScreen_iPhone)));
			navGroup.Items.Add (new NavItem ("Progress Bars", "", typeof(ProgressBars.ProgressBars_iPhone)));
			
			navGroup = new NavItemGroup ("Popups");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Alert Views", "", typeof(AlertViews.AlertViewsScreen_iPhone)));
			navGroup.Items.Add (new NavItem ("Action Sheets", "", typeof(ActionSheets.ActionSheets_iPhone)));

			navGroup = new NavItemGroup ("Pickers");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Simple Date Picker", "", typeof(DatePicker.DatePickerSimple_iPhone)));
			navGroup.Items.Add (new NavItem ("Date Picker", "", typeof(DatePicker.DatePicker_iPhone)));
			navGroup.Items.Add (new NavItem ("Simple Custom Picker", "", typeof(PickerView.PickerView1_iPhone)));
			navGroup.Items.Add (new NavItem ("Custom Picker with Multiple Components", "", typeof(PickerView.PickerWithMultipleComponents_iPhone)));

			navGroup = new NavItemGroup ("Toolbars");
			navItems.Add (navGroup);
			navGroup.Items.Add (new NavItem ("Toolbar 1", "", typeof(Toolbar.Toolbar1_iPhone)));
			navGroup.Items.Add (new NavItem ("Programmatic Toolbar", "", typeof(Toolbar.ProgrammaticToolbar_Controller)));
			navGroup.Items.Add (new NavItem ("Toolbar Items", "", typeof(Toolbar.ToolbarItems)));
			
			// create a table source from our nav items
			tableSource = new NavItemTableSource (this.NavigationController, navItems);
			
			// set the source on the table to our data source
			base.TableView.Source = tableSource;
		}
	}
}

