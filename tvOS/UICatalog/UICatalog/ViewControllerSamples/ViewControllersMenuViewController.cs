using System.Collections.Generic;

namespace UICatalog {
	public partial class ViewControllersMenuViewController : MenuTableViewController {

		public override List<string[]> SegueIdentifierMap {
			get {
				return new List<string[]> {
					new [] {
						"ShowAlertControllers",
						"ShowCollectionViewController",
						"ShowPageViewController",
						"ShowSearchViewController"
					}
				};
			}
		}
	}
}
