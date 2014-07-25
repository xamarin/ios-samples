
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_SharedResources.Screens.iPhone.Battery
{
	public partial class BatteryStatusScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public BatteryStatusScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public BatteryStatusScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public BatteryStatusScreen () : base("BatteryStatusScreen", null)
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
			Title = "Battery";
			
			// turn on battery monitoring
			UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;
			// set the battery level on the progress bar
			barBatteryLevel.Progress = UIDevice.CurrentDevice.BatteryLevel;
			// the the battery state label
			lblBatteryState.Text = UIDevice.CurrentDevice.BatteryState.ToString ();
			
			// add a notification handler for battery level changes
			NSNotificationCenter.DefaultCenter.AddObserver (
				UIDevice.BatteryLevelDidChangeNotification,
				(NSNotification n) => { 
					barBatteryLevel.Progress = UIDevice.CurrentDevice.BatteryLevel;
					n.Dispose();
				});
			
			// add a notification handler for battery state changes
			NSNotificationCenter.DefaultCenter.AddObserver (
				UIDevice.BatteryStateDidChangeNotification,
				(NSNotification n) => { 
					lblBatteryState.Text = UIDevice.CurrentDevice.BatteryState.ToString(); 
					n.Dispose();
				});
			
			
		}
	}
}

