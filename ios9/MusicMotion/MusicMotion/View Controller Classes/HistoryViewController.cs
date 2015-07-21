using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace MusicMotion {
	public partial class HistoryViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource, INSCoding {
		
		readonly string textCellIdentifier = "HistoryCell";

		List<Tuple<string, Func<Activity, string>>> recentActivityItems = new List<Tuple<string, Func<Activity, string>>> {
			new Tuple<string, Func<Activity, string>> ("Start Date", activity => activity.StartDateDescription),
			new Tuple<string, Func<Activity, string>> ("End Date", activity => activity.EndDateDescription),
			new Tuple<string, Func<Activity, string>> ("Duration", activity => activity.ActivityDuration),
			new Tuple<string, Func<Activity, string>> ("Pace Per Mile", activity => activity.ActivityDuration),
			new Tuple<string, Func<Activity, string>> ("Distance (Miles)", activity => activity.DistanceInMiles),
			new Tuple<string, Func<Activity, string>> ("Distance (Meters)", activity => activity.Distance.ToString ()),
			new Tuple<string, Func<Activity, string>> ("Number of Steps", activity => activity.NumberOfSteps.ToString ()),
			new Tuple<string, Func<Activity, string>> ("Floors Ascended", activity => activity.FloorsAscended.ToString ()),
			new Tuple<string, Func<Activity, string>> ("Floors Descended", activity => activity.FloorsDescended.ToString ())
		};

		MotionManager motionManager;

		public HistoryViewController (IntPtr handle) : base (handle)
		{
			Initilize ();
		}

		[Export ("initWithCoder:")]
		public HistoryViewController (NSCoder coder) : base (coder)
		{
			Initilize ();
		}

		void Initilize ()
		{
			motionManager = new MotionManager ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetNeedsStatusBarAppearanceUpdate ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			motionManager.QueryForRecentActivityData (HistoryTableView.ReloadData);
		}

		[Export ("numberOfSectionsInTableView:")]
		public nint NumberOfSections (UITableView tableView)
		{
			return motionManager.RecentActivities.Count;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableView, nint section)
		{
			var activity = motionManager.RecentActivities [(int)section].MotionActivity;
			return (activity.Running || activity.Walking) ? recentActivityItems.Count : 3;
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (textCellIdentifier, indexPath);
			var activity = motionManager.RecentActivities [indexPath.Section];
			var item = recentActivityItems [indexPath.Row];

			cell.TextLabel.Text = item.Item1;
			cell.DetailTextLabel.Text = item.Item2.Invoke (activity);
			cell.UserInteractionEnabled = false;

			return cell;
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, nint section)
		{
			return motionManager.RecentActivities [(int)section].ActivityType;
		}
	}
}
