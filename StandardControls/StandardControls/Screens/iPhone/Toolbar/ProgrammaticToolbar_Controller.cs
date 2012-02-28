using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Example_StandardControls.Controls;

namespace Example_StandardControls.Screens.iPhone.Toolbar
{
	public class ProgrammaticToolbar_Controller : UIViewController
	{
		UIToolbar toolbar;

		#region -= constructors =-

		public ProgrammaticToolbar_Controller () : base()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Programmatic Toolbar";
			
			// set the background color of the view to white
			this.View.BackgroundColor = UIColor.White;
			
			// new up the toolbar
			float toolbarHeight = 44;
			toolbar = new UIToolbar (new RectangleF (0
					, this.View.Frame.Height - this.NavigationController.NavigationBar.Frame.Height
					, this.View.Frame.Width, toolbarHeight));
			toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;
			
			
			// button one
			string buttonTitle = "One";
			UIBarButtonItem btnOne = new UIBarButtonItem (buttonTitle, UIBarButtonItemStyle.Bordered, null);
			btnOne.Clicked += (s, e) => { new UIAlertView ("click!", "btnOne clicked", null, "OK", null).Show (); };
			
			// fixed width
			UIBarButtonItem fixedWidth = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);
			fixedWidth.Width = 25;
			
			// button two 
			UIBarButtonItem btnTwo = new UIBarButtonItem ("second", UIBarButtonItemStyle.Bordered, null);
			
			// flexible width space
			UIBarButtonItem flexibleWidth = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);
			
			// button three
			UIBarButtonItem btnThree = new UIBarButtonItem ("3", UIBarButtonItemStyle.Bordered, null);
			
			// button four
			UIBarButtonItem btnFour = new UIBarButtonItem ("another!", UIBarButtonItemStyle.Bordered, null);
			
			// create the items array
			UIBarButtonItem[] items = new UIBarButtonItem[] { 
				btnOne, fixedWidth, btnTwo, flexibleWidth, btnThree, btnFour };
			
			// add the items to the toolbar
			toolbar.SetItems (items, false);			
			
			// add the toolbar to the page
			this.View.AddSubview (toolbar);
		}

		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}
