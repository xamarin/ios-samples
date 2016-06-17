using System.Collections.Generic;

namespace UICatalog {
	public partial class ControlsMenuViewController : MenuTableViewController {

		List<string[]> segueIdentifierMap;
		public override List<string[]> SegueIdentifierMap {
			get {
				segueIdentifierMap = segueIdentifierMap ?? new List<string[]> {
					new [] {
						"ShowButtons",
						"ShowProgressViews",
						"ShowSegmentedControls"
					}
				};

				return segueIdentifierMap;
			}
		}
	}
}
