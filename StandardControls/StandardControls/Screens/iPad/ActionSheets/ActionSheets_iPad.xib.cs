
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPad.ActionSheets
{
	public partial class ActionSheets_iPad : UIViewController
	{
		UIActionSheet actionSheet;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public ActionSheets_iPad (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ActionSheets_iPad (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ActionSheets_iPad () : base("ActionSheets_iPad", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Action Sheets";
			
			btnSimpleActionSheet.TouchUpInside += HandleBtnSimpleActionSheetTouchUpInside;
			btnActionSheetWithOtherButtons.TouchUpInside += HandleBtnActionSheetWithOtherButtonsTouchUpInside;
		}

		protected void HandleBtnSimpleActionSheetTouchUpInside (object sender, EventArgs e)
		{
			// create an action sheet using the qualified constructor
			actionSheet = new UIActionSheet ("simple action sheet", null, "cancel", "delete", null);
			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) { Console.WriteLine ("Button " + b.ButtonIndex.ToString () + " clicked"); };
			actionSheet.ShowInView (View);
		}

		protected void HandleBtnActionSheetWithOtherButtonsTouchUpInside (object sender, EventArgs e)
		{
			actionSheet = new UIActionSheet ("action sheet with other buttons");
			actionSheet.AddButton ("delete");
			actionSheet.AddButton ("a different option!");
			actionSheet.AddButton ("another option");
			actionSheet.DestructiveButtonIndex = 0;
			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) { Console.WriteLine ("Button " + b.ButtonIndex.ToString () + " clicked"); };
			actionSheet.ShowInView (View);
		}
	}
}

