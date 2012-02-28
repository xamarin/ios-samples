
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Example_StandardControls.Screens.iPad.DatePicker
{
	public partial class DatePickerSimple_iPad : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public DatePickerSimple_iPad (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public DatePickerSimple_iPad (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public DatePickerSimple_iPad () : base("DatePickerSimple_iPad", null)
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
			
			Title = "Simple Date Picker";
			
			pkrDate.ValueChanged += (s, e) => { this.lblDate.Text = (s as UIDatePicker).Date.ToString (); };
		}
	}
}

