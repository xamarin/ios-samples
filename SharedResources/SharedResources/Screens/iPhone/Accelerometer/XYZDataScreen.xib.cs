
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Example_SharedResources.Screens.iPhone.Accelerometer
{
	public partial class XYZDataScreen : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public XYZDataScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public XYZDataScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public XYZDataScreen () : base("XYZDataScreen", null)
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
			
			Title = "XYZ Data";
			
			// update interval is set in milliseconds
			UIAccelerometer.SharedAccelerometer.UpdateInterval = 100;
			
			// update the XYZ data when the accelerometer receives data
			UIAccelerometer.SharedAccelerometer.Acceleration += (object sender, UIAccelerometerEventArgs e) => {
				InvokeOnMainThread( () => {
					lblX.Text = e.Acceleration.X.ToString();
					lblY.Text = e.Acceleration.Y.ToString();
					lblZ.Text = e.Acceleration.Z.ToString();
				} );
			};
			
		}
	}
}

