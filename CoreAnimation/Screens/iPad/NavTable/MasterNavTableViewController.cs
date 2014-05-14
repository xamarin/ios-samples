using System;
using UIKit;
using System.Collections.Generic;
using Example_CoreAnimation.Code.NavigationTable;

namespace Example_CoreAnimation.Screens.iPad.NavTable
{
	public class MasterNavTableViewController : UITableViewController
	{
		public event EventHandler<RowClickedEventArgs> RowClicked;

		private List<NavItemGroup> navItems;
		private NavItemTableSource tableSource;

		public MasterNavTableViewController () : base (UITableViewStyle.Grouped)
		{
			navItems = new List<NavItemGroup> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// create the navigation items
			var navGroup = new NavItemGroup ("UIView Animations");
			navGroup.Items = new List<NavItem> () {
				new NavItem ("Basic Animation", "", typeof(BasicUIViewAnimation.BasicUIViewAnimationScreen)),
				new NavItem ("Animation Customizer", "", typeof(CustomizableAnimationViewer.CustomizableAnimationViewerScreen)),
				new NavItem ("Transitions", "", typeof(ViewTransitions.Controller)),
				new NavItem ("Implicit Layer Animation", "", typeof(LayerAnimation.ImplicitAnimationScreen)),
				new NavItem ("Explicit Layer Animation", "", typeof(LayerAnimation.LayerAnimationScreen))
			};

			navItems.Add (navGroup);

			// create a table source from our nav items
			tableSource = new NavItemTableSource (navItems);
			
			// set the source on the table to our data source
			base.TableView.Source = tableSource;
			
			tableSource.RowClicked += (sender, e) => {
				if (RowClicked != null)
					RowClicked (sender, e);
			};
		}
	}
}

