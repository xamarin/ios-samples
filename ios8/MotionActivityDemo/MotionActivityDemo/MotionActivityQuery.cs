using System;
using MonoTouch.Foundation;

namespace MotionActivityDemo
{
	public class MotionActivityQuery : NSObject
	{
		public NSDate StartDate;
		public NSDate EndDate;
		public bool IsToday;

		public MotionActivityQuery (NSDate startDate, NSDate endDate, bool today)
		{
			StartDate = startDate;
			EndDate = endDate;
			IsToday = today;
		}

		public static MotionActivityQuery FromDate (NSDate date, int offset)
		{
			NSCalendar currentCalendar = NSCalendar.CurrentCalendar;
			NSDateComponents timeComponents = currentCalendar.Components (
				                                  NSCalendarUnit.Year | NSCalendarUnit.Month | NSCalendarUnit.Day, date);
			timeComponents.Hour = 0;
			timeComponents.Day = timeComponents.Day + offset;

			NSDate queryStart = currentCalendar.DateFromComponents (timeComponents);

			timeComponents.Day = timeComponents.Day + 1;
			NSDate queryEnd = currentCalendar.DateFromComponents (timeComponents);

			return new MotionActivityQuery (queryStart, queryEnd, offset == 0);
		}

		public string Description {
			get {
				if (IsToday)
					return "Today";

				NSDateFormatter formatter = new NSDateFormatter ();
				string format = NSDateFormatter.GetDateFormatFromTemplate ("EdMMM", 0, NSLocale.CurrentLocale);
				formatter.DateFormat = format;
				return formatter.StringFor (StartDate);
			}
		}
	}
}

