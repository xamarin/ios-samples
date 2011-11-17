
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.CustomizingNavBar
{
	public partial class CustomizingNavBarScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public CustomizingNavBarScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CustomizingNavBarScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CustomizingNavBarScreen () : base("CustomizingNavBarScreen", null)
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
			
			this.NavigationItem.Title = "Customizing Nav Bar";
			
			// toggle the navigation bar transparency
			this.swchTransparent.ValueChanged += (s, e) => {
				this.NavigationController.NavigationBar.Translucent = this.swchTransparent.On;
			};
			
			// change the navigation bar color (toggles between default and red)
			this.btnChangeTintColor.TouchUpInside += (s, e) => {
				
				// if it's red, reset it by setting it to null
				if(this.NavigationController.NavigationBar.TintColor == UIColor.Red)
					this.NavigationController.NavigationBar.TintColor = null;
				// otherwise, make it red
				else
					this.NavigationController.NavigationBar.TintColor = UIColor.Red;			
			};
			
			// toggle the bar style between black and default
			this.btnChangeBarStyle.TouchUpInside += (s, e) => {
				if(this.NavigationController.NavigationBar.BarStyle == UIBarStyle.Default)
					this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
				else
					this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
			};
			
			// show/hide the right bar button item
			this.btnShowHideRightButton.TouchUpInside += (s, e) => {
				if(this.NavigationItem.RightBarButtonItem == null)
					this.NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Action, null), true);
				else
					this.NavigationItem.SetRightBarButtonItem(null, true);
			};
			
			// setup the toolbar items. the navigation controller gets the items from the controller
			// because they're specific to each controller on the stack, hence 'this.SetToolbarItems' rather
			// than this.NavigationController.Toolbar.SetToolbarItems
			this.SetToolbarItems( new UIBarButtonItem[] {
				new UIBarButtonItem(UIBarButtonSystemItem.Refresh)
				, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 }
				, new UIBarButtonItem(UIBarButtonSystemItem.Pause)
			}, false);
			
			// toggle the display of the toolbar
			this.btnShowToolbar.TouchUpInside += (s, e) => {
				this.NavigationController.ToolbarHidden = !this.NavigationController.ToolbarHidden;	
			};
		}
	}
}

