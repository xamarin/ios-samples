// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace RecipesAndPrinting
{
	[Register ("RecipeDetailViewController")]
	partial class RecipeDetailViewController
	{
		[Outlet]
		UIKit.UIView tableHeaderView { get; set; }

		[Outlet]
		UIKit.UIButton photoButton { get; set; }

		[Outlet]
		UIKit.UILabel nameLabel { get; set; }

		[Action ("ShowPhoto:")]
		partial void ShowPhoto (Foundation.NSObject sender);
	}
}
