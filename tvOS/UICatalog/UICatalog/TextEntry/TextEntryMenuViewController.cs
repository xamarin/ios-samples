using System.Collections.Generic;

namespace UICatalog {
	public partial class TextEntryMenuViewController : MenuTableViewController {

		public override List<string[]> SegueIdentifierMap {
			get {
				return new List<string[]> {
					new [] {
						"ShowSimpleForm",
						"ShowAlertForm"
					}
				};
			}
		}
	}
}
