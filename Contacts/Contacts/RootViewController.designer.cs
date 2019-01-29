// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Contacts
{
	[Register ("RootViewController")]
	partial class RootViewController
	{
		[Outlet]
		UIKit.UITableViewCell createNewContactCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell createNewContactExistingData { get; set; }

		[Outlet]
		UIKit.UITableViewCell displayEditCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell editContactCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell enableContactsPredicateCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell pickMultipleContactsCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell pickMultiplePropertiesCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell pickSingleContactCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell pickSinglePropertyCell { get; set; }

		[Outlet]
		UIKit.UITableViewCell selectContactsPredicateCell { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (createNewContactCell != null) {
				createNewContactCell.Dispose ();
				createNewContactCell = null;
			}

			if (createNewContactExistingData != null) {
				createNewContactExistingData.Dispose ();
				createNewContactExistingData = null;
			}

			if (displayEditCell != null) {
				displayEditCell.Dispose ();
				displayEditCell = null;
			}

			if (editContactCell != null) {
				editContactCell.Dispose ();
				editContactCell = null;
			}

			if (pickMultipleContactsCell != null) {
				pickMultipleContactsCell.Dispose ();
				pickMultipleContactsCell = null;
			}

			if (pickMultiplePropertiesCell != null) {
				pickMultiplePropertiesCell.Dispose ();
				pickMultiplePropertiesCell = null;
			}

			if (pickSingleContactCell != null) {
				pickSingleContactCell.Dispose ();
				pickSingleContactCell = null;
			}

			if (pickSinglePropertyCell != null) {
				pickSinglePropertyCell.Dispose ();
				pickSinglePropertyCell = null;
			}

			if (enableContactsPredicateCell != null) {
				enableContactsPredicateCell.Dispose ();
				enableContactsPredicateCell = null;
			}

			if (selectContactsPredicateCell != null) {
				selectContactsPredicateCell.Dispose ();
				selectContactsPredicateCell = null;
			}
		}
	}
}
