
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_ContentControls.Screens.iPhone.ExtraScreens
{
	public partial class CustomizableTabScreen : UIViewController
	{
		public string Number
		{
			get { return this.lblNumber.Text; }
			set
			{
				this.lblNumber.Text = value;
				this.Title = value;
			}
		}
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public CustomizableTabScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CustomizableTabScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CustomizableTabScreen ()// : base("CustomizableTabScreen", null)
		{
			NibBundle.LoadNib("CustomizableTabScreen", this, null);
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
	}
}

