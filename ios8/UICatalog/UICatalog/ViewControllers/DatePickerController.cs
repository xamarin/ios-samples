using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class DatePickerController : UIViewController
	{
		[Outlet]
		private UIDatePicker DatePicker { get; set; }

		[Outlet]
		private UILabel DateLabel { get; set; }

		// Use a date formatter to format the "date" property of "datePicker".
		private NSDateFormatter _dateFormater;
		private NSDateFormatter DateFormater {
			get {
				_dateFormater = _dateFormater ?? new NSDateFormatter {
					DateStyle = NSDateFormatterStyle.Medium,
					TimeStyle = NSDateFormatterStyle.Short,
				};

				return _dateFormater;
			}
		}

		public DatePickerController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ConfigureDatePicker ();
		}

		private void ConfigureDatePicker()
		{
			DatePicker.Mode = UIDatePickerMode.DateAndTime;

			// Set min/max date for the date picker.
			// As an example we will limit the date between now and 7 days from now.
			var now = DateTime.Now;
			DatePicker.MinimumDate = (NSDate)now;

			var sevenDaysFromNow = now.AddDays (7);
			DatePicker.MaximumDate = (NSDate)sevenDaysFromNow;

			// Display the "minutes" interval by increments of 1 minute (this is the default).
			DatePicker.MinuteInterval = 1;
			DatePicker.ValueChanged += updateDatePickerLabel;
			updateDatePickerLabel (DatePicker, EventArgs.Empty);
		}

		private void updateDatePickerLabel (object sender, EventArgs e)
		{
			DateLabel.Text = DateFormater.ToString(DatePicker.Date);
		}
	}
}
