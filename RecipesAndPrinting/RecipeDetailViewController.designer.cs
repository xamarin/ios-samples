// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RecipesAndPrinting
{
	[Register ("RecipeDetailViewController")]
	partial class RecipeDetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIView tableHeaderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton photoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel nameLabel { get; set; }

		[Action ("ShowPhoto:")]
		partial void ShowPhoto (MonoTouch.Foundation.NSObject sender);
	}
}
