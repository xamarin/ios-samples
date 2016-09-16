using Foundation;
using System;
using UIKit;
using WatchKit;

namespace Watchkit2Extension
{
	public partial class GestureDetailController : WKInterfaceController
	{
		public GestureDetailController(IntPtr handle) : base(handle)
		{
		}

		partial void LongPressRecognized(Foundation.NSObject sender)
		{ 
			longPressLabel.SetTextColor(UIColor.Green);

			ScheduleReset();
		}

		partial void PanRecognized(Foundation.NSObject sender)
		{
			var panGesture = sender as WKPanGestureRecognizer;
			if (panGesture != null)
			{
				panLabel.SetText("offset:" + panGesture.TranslationInObject.ToString());

				ScheduleReset();
			} 
		}

		partial void SwipeRecognized(Foundation.NSObject sender)
		{
			swipeGroup.SetBackgroundColor(UIColor.Green);
			ScheduleReset(); 
		}

		partial void TapRecognized(Foundation.NSObject sender)
		{
			tapGroup.SetBackgroundColor(UIColor.Green);
			ScheduleReset();	
		}

		NSTimer timer;
		void ScheduleReset() 
		{
			if (timer != null)
			{
				timer.Invalidate();
			}
			timer = NSTimer.CreateTimer( TimeSpan.FromSeconds(1), ResetAllGroups);
			NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
		}

		void ResetAllGroups (NSTimer obj)
		{
			tapGroup.SetBackgroundColor(UIColor.Clear);
			swipeGroup.SetBackgroundColor(UIColor.Clear);
		}
	}
}