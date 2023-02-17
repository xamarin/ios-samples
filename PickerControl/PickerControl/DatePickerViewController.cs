using Foundation;
using System;
using UIKit;

namespace PickerControl {
	public partial class DatePickerViewController : UIViewController {

		public DatePickerViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			datePickerMode.SelectedSegment = 0;
			datePickerView.Mode = UIDatePickerMode.Time;
			//datePickerView.TimeZone = NSTimeZone.FromName("America/New_York");	
			//datePickerView.Locale = NSLocale.FromLocaleIdentifier("en_GB");

			var calendar = new NSCalendar (NSCalendarType.Gregorian);
			var currentDate = NSDate.Now;
			var components = new NSDateComponents ();

			components.Year = -60;

			NSDate minDate = calendar.DateByAddingComponents (components, NSDate.Now, NSCalendarOptions.None);

			//Uncomment to set min and max date

			//datePickerView.MinimumDate = minDate;
			//datePickerView.MaximumDate = (NSDate)DateTime.Today.AddYears(-7);
		}

		partial void DateModeValueChanged (UISegmentedControl sender)
		{
			dateLabel.Text = "";

			switch (sender.SelectedSegment) {
			case 0: // time
				datePickerView.Mode = UIDatePickerMode.Time;
				break;

			case 1: // date
				datePickerView.Mode = UIDatePickerMode.Date;
				break;

			case 2: // date & time
				datePickerView.Mode = UIDatePickerMode.DateAndTime;
				break;

			case 3: // counter
				datePickerView.Mode = UIDatePickerMode.CountDownTimer;
				datePickerView.MinuteInterval = 10;
				break;
			}

			datePickerView.Date = NSDate.Now;

		}

		partial void DateTimeChanged (UIDatePicker sender)
		{
			//Formatting for Date
			NSDateFormatter dateFormat = new NSDateFormatter ();
			dateFormat.DateFormat = "yyyy-MM-dd";

			//Formatting for Time
			NSDateFormatter timeFormat = new NSDateFormatter ();
			timeFormat.TimeStyle = NSDateFormatterStyle.Short;

			//Formatting for Date and Time
			NSDateFormatter dateTimeformat = new NSDateFormatter ();
			dateTimeformat.DateStyle = NSDateFormatterStyle.Long;
			dateTimeformat.TimeStyle = NSDateFormatterStyle.Short;

			// Figuring out when countdown is finished
			var currentTime = NSDate.Now;
			var countDownTimerTime = datePickerView.CountDownDuration;
			var finishCountdown = currentTime.AddSeconds (countDownTimerTime);

			//Formatting Countdown display
			NSDateFormatter coundownTimeformat = new NSDateFormatter ();
			coundownTimeformat.DateStyle = NSDateFormatterStyle.Medium;
			coundownTimeformat.TimeStyle = NSDateFormatterStyle.Medium;

			switch (datePickerMode.SelectedSegment) {
			case 0: // time
				dateLabel.Text = timeFormat.ToString (datePickerView.Date);
				break;

			case 1: // date
				dateLabel.Text = dateFormat.ToString (datePickerView.Date);
				break;

			case 2: // date & time
				dateLabel.Text = dateTimeformat.ToString (datePickerView.Date);
				break;

			case 3: // counter
				dateLabel.Text = "Alarm set for:" + coundownTimeformat.ToString (finishCountdown);
				break;
			}

		}


	}
}
