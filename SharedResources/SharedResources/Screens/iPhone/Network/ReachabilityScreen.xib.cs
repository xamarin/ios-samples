
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_SharedResources.Screens.iPhone.Network
{
	public partial class ReachabilityScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ReachabilityScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ReachabilityScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ReachabilityScreen () : base("ReachabilityScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
	}
}

