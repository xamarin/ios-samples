
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_SharedResources.Screens.iPhone.Network
{
	public partial class ActivityIndicatorScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ActivityIndicatorScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ActivityIndicatorScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ActivityIndicatorScreen () : base("ActivityIndicatorScreen", null)
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
			this.Title = "Network Activity Indicator";
			
			// show/hide the network activity indicator based on the switch value
			this.swtchActivityIndicator1.ValueChanged += (s, e) => {
				(UIApplication.SharedApplication.Delegate as AppDelegate).SetNetworkActivityIndicator (this.swtchActivityIndicator1.On);
			};
			// show/hide the network activity indicator based on the switch value
			this.swtchActivityIndicator2.ValueChanged += (s, e) => {
				(UIApplication.SharedApplication.Delegate as AppDelegate).SetNetworkActivityIndicator (this.swtchActivityIndicator2.On);
			};
		}
	}
}

