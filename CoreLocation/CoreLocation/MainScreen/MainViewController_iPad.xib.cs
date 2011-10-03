
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_CoreLocation.MainScreen
{
	public partial class MainViewController_iPad : UIViewController, IMainScreen
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public MainViewController_iPad (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MainViewController_iPad (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MainViewController_iPad () : base("MainViewController_iPad", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public UILabel LblAltitude
		{
			get { return this.lblAltitude; }
		}
		public UILabel LblLatitude
		{
			get { return this.lblLatitude; }
		}
		public UILabel LblLongitude
		{
			get { return this.lblLongitude; }
		}
		public UILabel LblCourse
		{
			get { return this.lblCourse; }
		}
		public UILabel LblMagneticHeading
		{
			get { return this.lblMagneticHeading; }
		}
		public UILabel LblSpeed
		{
			get { return this.lblSpeed; }
		}
		public UILabel LblTrueHeading
		{
			get { return this.lblTrueHeading; }
		}
		public UILabel LblDistanceToParis
		{
			get { return this.lblDistanceToParis; }
		}
		
	}
}
