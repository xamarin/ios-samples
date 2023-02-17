using Foundation;
using System;
using UIKit;

namespace UICatalog {
	public partial class DatePickerController : UIViewController {
		private readonly NSDateFormatter dateFormater = new NSDateFormatter {
			DateStyle = NSDateFormatterStyle.Medium,
			TimeStyle = NSDateFormatterStyle.Short,
		};

		public DatePickerController (IntPtr handle) : base (handle) { }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.UpdateLabel ();
		}

		partial void PickerValurChanged (NSObject sender)
		{
			this.UpdateLabel ();
		}

		private void UpdateLabel ()
		{
			dateLabel.Text = this.dateFormater.ToString (this.datePicker.Date);
		}
	}
}
