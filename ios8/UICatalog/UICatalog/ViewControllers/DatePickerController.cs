using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("DatePickerController")]
	public class DatePickerController : UIViewController
	{
		[Outlet]
		UIDatePicker DatePicker { get; set; }

		[Outlet]
		UILabel DateLabel { get; set; }

		// Use a date formatter to format the "date" property of "datePicker".
		NSDateFormatter dateFormater;

		NSDateFormatter DateFormater {
			get {
				dateFormater = dateFormater ?? new NSDateFormatter {
					DateStyle = NSDateFormatterStyle.Medium,
					TimeStyle = NSDateFormatterStyle.Short,
				};

				return dateFormater;
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

		void ConfigureDatePicker ()
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

		void updateDatePickerLabel (object sender, EventArgs e)
		{
			DateLabel.Text = DateFormater.ToString (DatePicker.Date);
		}
	}
}
