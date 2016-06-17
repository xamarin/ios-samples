using System.Linq;

using UIKit;

namespace UICatalog {
	public partial class MenuSplitViewController : UISplitViewController {

		bool preferDetailViewControllerOnNextFocusUpdate;
			
		public override UIView PreferredFocusedView {
			get {
				UIView preferredFocusedView;

				if (preferDetailViewControllerOnNextFocusUpdate) {
					var viewController = ViewControllers.LastOrDefault ();
					preferredFocusedView = viewController?.PreferredFocusedView;
					preferDetailViewControllerOnNextFocusUpdate = false;
				} else {
					preferredFocusedView = base.PreferredFocusedView;
				}

				return preferredFocusedView;
			}
		}

		public void UpdateFocusToDetailViewController ()
		{
			SetNeedsFocusUpdate ();
			UpdateFocusIfNeeded ();
		}
	}
}
