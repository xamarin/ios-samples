using System;
using System.Collections.Generic;

using UIKit;

namespace CoreAnimationExample
{
	public class MasterNavTableViewController : UITableViewController
	{
		public event EventHandler<RowClickedEventArgs> RowClicked;

		List<NavItemGroup> navItems;
		NavItemTableSource tableSource;

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
				new NavItem ("Basic Animation", "", typeof(BasicUIViewAnimationScreen)),
				new NavItem ("Animation Customizer", "", typeof(CustomizableAnimationViewerScreen)),
				new NavItem ("Transitions", "", typeof(Controller)),
				new NavItem ("Implicit Layer Animation", "", typeof(ImplicitAnimationScreen)),
				new NavItem ("Explicit Layer Animation", "", typeof(LayerAnimationScreen))
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

