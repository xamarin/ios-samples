using System;
using System.IO;
using System.Linq;

using CoreFoundation;
using Foundation;
using UIKit;

namespace DispatchSourceExamples {
	public enum DispatchSourceType {
		Timer = 0,
		Vnode,
		MemoryMonitor,
		ReadMonitor,
		WriteMonitor
	}

	public partial class DispatchSourceViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
	{
		const long NanosecondsInSecond = 1000000000;

		bool dispatchSourceIsInUse;
		DispatchSource dispatchSource;
		NSUrl testFileUrl;

		Test CurrentTestInfo { get; set; }

		public DispatchSourceType SelectedDispatchSource { get; set; }

		public NSUrl TestFileUrl {
			get {
				if (testFileUrl == null) {
					using (NSUrl fileURL = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).First ())
						testFileUrl = fileURL.Append ("test.txt", false);
				}

				return testFileUrl;
			}
		}

		public DispatchSourceViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = SelectedDispatchSource.ToString ();

			switch (SelectedDispatchSource) {
			case DispatchSourceType.Timer:
				CurrentTestInfo = new Test {
					FirstStateText = "Start Timer",
					SecondStateText = "Stop Timer",
					FirstAction = StartTimer,
					SecondAction = CancelDispatchSource
				};
				break;
			case DispatchSourceType.Vnode:
				CurrentTestInfo = new Test {
					FirstStateText = "Start Vnode Monitor",
					SecondStateText = "Test Vnode Monitor",
					FirstAction = StartVnodeMonitor,
					SecondAction = TestVnodeMonitor
				};
				break;
			case DispatchSourceType.MemoryMonitor:
				CurrentTestInfo = new Test {
					FirstStateText = "Start Memory Monitor",
					SecondStateText = "Test Memory Monitor",
					FirstAction = StartMemoryMonitor,
					SecondAction = TestMemoryMonitor
				};
				break;
			case DispatchSourceType.ReadMonitor:
				CurrentTestInfo = new Test {
					FirstStateText = "Test Read Monitor",
					FirstAction = TestReadMonitor
				};
				break;
			case DispatchSourceType.WriteMonitor:
				CurrentTestInfo = new Test {
					FirstStateText = "Test Write Monitor",
					FirstAction = StartWriteMonitor
				};
				break;
			}

			tableView.DataSource = this;
			tableView.Delegate = this;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			if (dispatchSource != null) {
				dispatchSource.Cancel ();
				dispatchSource.Dispose ();
			}
		}

		public override void ViewDidLayoutSubviews ()
		{
			dynamicViewHeight.Constant = NMath.Min (View.Bounds.Size.Height, tableView.ContentSize.Height);
			View.LayoutIfNeeded ();
		}

		void PrintResult (UITextView logView, string message)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				logView.Text = string.Format ("{0}\n{1}", logView.Text, message);
				textView.ScrollRangeToVisible (new NSRange (0, logView.Text.Length));
			});
		}

		[Export ("numberOfSectionsInTableView:")]
		public int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableview, nint section)
		{
			return 1;
		}

		[Export ("tableView:titleForHeaderInSection:")]
		public string TitleForHeader (UITableView tableView, int section)
		{
			return string.Empty;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (dispatchSourceIsInUse && CurrentTestInfo.SecondAction != null)
				CurrentTestInfo.SecondAction ();
			else if (CurrentTestInfo.FirstAction != null)
				CurrentTestInfo.FirstAction ();

			if (CurrentTestInfo.SecondAction != null)
				tableView.CellAt (indexPath).TextLabel.Text = dispatchSourceIsInUse ?
					CurrentTestInfo.FirstStateText : CurrentTestInfo.SecondStateText;
			
			dispatchSourceIsInUse = !dispatchSourceIsInUse;
			tableView.DeselectRow (indexPath, true);
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			string cellIdentifier = "tableCell";

			var cell = tableView.DequeueReusableCell (cellIdentifier);
			cell = cell ?? new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
			cell.TextLabel.Text = CurrentTestInfo.FirstStateText;

			return cell;
		}

		void StartTimer ()
		{
			dispatchSource = new DispatchSource.Timer (DispatchQueue.MainQueue);

			long delay = 2 * NanosecondsInSecond;
			long leeway = 5 * NanosecondsInSecond;

			((DispatchSource.Timer)dispatchSource).SetTimer (DispatchTime.Now, delay, leeway);

			dispatchSource.SetRegistrationHandler (() => {
				PrintResult (textView, "Timer registered");
			});

			dispatchSource.SetEventHandler (() => {
				PrintResult (textView, "Timer tick");
			});

			dispatchSource.SetCancelHandler (() => {
				PrintResult (textView, "Timer stopped");
			});

			dispatchSource.Resume ();
		}

		void CancelDispatchSource ()
		{
			if (dispatchSource != null)
				dispatchSource.Cancel ();
		}

		void StartVnodeMonitor ()
		{
			NSUrl fileURL = TestFileUrl;

			var stream = File.Create (fileURL.Path);
			int fileDescriptor = GetFileDescriptor (stream);

			dispatchSource = new DispatchSource.VnodeMonitor (fileDescriptor,
				VnodeMonitorKind.Delete | VnodeMonitorKind.Extend | VnodeMonitorKind.Write,
				DispatchQueue.MainQueue
			);

			dispatchSource.SetRegistrationHandler (() => {
				PrintResult (textView, "Vnode monitor registered");
			});

			dispatchSource.SetEventHandler (() => {
				var observedEvents = ((DispatchSource.VnodeMonitor)dispatchSource).ObservedEvents;
				string message = string.Format ("Vnode monitor event for {0}: {1}", fileURL.LastPathComponent, observedEvents);
				PrintResult (textView, message);
				dispatchSource.Cancel ();
				stream.Close ();
			});

			dispatchSource.SetCancelHandler (() => {
				PrintResult (textView, "Vnode monitor cancelled");
			});

			dispatchSource.Resume ();
		}

		void TestVnodeMonitor ()
		{
			File.Delete (TestFileUrl.Path);
		}

		void StartMemoryMonitor ()
		{
			dispatchSource = new DispatchSource.MemoryPressure (
				MemoryPressureFlags.Critical | MemoryPressureFlags.Warn | MemoryPressureFlags.Normal, 
				DispatchQueue.MainQueue);

			dispatchSource.SetRegistrationHandler (() => {
				PrintResult (textView, "Memory monitor registered");
			});

			dispatchSource.SetEventHandler (() => {
				var pressureLevel = ((DispatchSource.MemoryPressure)dispatchSource).PressureFlags;
				PrintResult (textView, string.Format ("Memory worning of level: {0}", pressureLevel));
				tableView.UserInteractionEnabled = true;
				dispatchSource.Cancel ();
			});

			dispatchSource.SetCancelHandler (() => {
				PrintResult (textView, "Memory monitor cancelled");
			});

			dispatchSource.Resume ();
		}

		void TestMemoryMonitor ()
		{
			if (UIDevice.CurrentDevice.Model.Contains ("Simulator")) {
				PrintResult (textView, "Press: Hardware -> Simulate Memory Warning");
				tableView.UserInteractionEnabled = false;
			} else {
				PrintResult (textView, "This test available on simulator only");
				dispatchSource.Cancel ();
			}
		}

		void StartWriteMonitor ()
		{
			NSUrl fileURL = TestFileUrl;
			var stream = new FileStream (fileURL.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			int fileDescriptor = GetFileDescriptor (stream);

			dispatchSource = new DispatchSource.WriteMonitor (fileDescriptor, DispatchQueue.MainQueue);

			dispatchSource.SetRegistrationHandler (() => {
				PrintResult (textView, "Write monitor registered");
			});

			dispatchSource.SetEventHandler (() => {
				string message = string.Format ("Write monitor: {0} was opened in write mode", fileURL.LastPathComponent);
				PrintResult (textView, message);
				dispatchSource.Cancel ();
				stream.Close ();
			});

			dispatchSource.SetCancelHandler (() => {
				PrintResult (textView, "Write monitor cancelled");
			});

			dispatchSource.Resume ();
		}

		void TestReadMonitor ()
		{
			NSUrl fileURL = TestFileUrl;

			var stream = File.OpenRead (fileURL.Path);
			int fileDescriptor = GetFileDescriptor (stream);

			dispatchSource = new DispatchSource.ReadMonitor (fileDescriptor, DispatchQueue.MainQueue);

			dispatchSource.SetRegistrationHandler (() => {
				PrintResult (textView, "Read monitor registered");
			});

			dispatchSource.SetEventHandler (() => {
				PrintResult (textView, string.Format ("Read monitor: {0} was opened in read mode", fileURL.LastPathComponent));
				dispatchSource.Cancel ();
				stream.Close ();
			});

			dispatchSource.SetCancelHandler (() => {
				PrintResult (textView, "Read monitor cancelled");
			});

			dispatchSource.Resume ();
		}

		int GetFileDescriptor (FileStream stream)
		{
			var safeHandle = stream.SafeFileHandle;
			IntPtr descriptor = safeHandle.DangerousGetHandle ();
			return descriptor.ToInt32 ();
		}
	}
}

 