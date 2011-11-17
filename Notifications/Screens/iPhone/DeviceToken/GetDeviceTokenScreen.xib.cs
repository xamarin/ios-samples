
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_Notifications.Screens.iPhone.DeviceToken
{
	public partial class GetDeviceTokenScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public GetDeviceTokenScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public GetDeviceTokenScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public GetDeviceTokenScreen () : base("GetDeviceTokenScreen", null)
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
			
			this.lblToken.Text = ((AppDelegate)UIApplication.SharedApplication.Delegate).DeviceToken;
		}
	}
}

