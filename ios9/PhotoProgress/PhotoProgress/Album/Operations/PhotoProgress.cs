using System;

using Foundation;

namespace PhotoProgress {
	public class PhotoProgress : NSProgress {

		public event EventHandler CompletedUnitCountChanged;
		public event EventHandler TotalUnitCountChanged;
		public event EventHandler FractionCompletedChanged;
		public event EventHandler PhotoProgressCanceled;
		public event EventHandler PhotoProgressPaused;

		public void AddChild (PhotoProgress child, int pendingUnitCount)
		{
			child.CompletedUnitCountChanged += (obj, args) => CompletedUnitCountChanged?.Invoke(obj, args);
			base.AddChild (child, pendingUnitCount);
		}

		public override void WillChangeValue (string forKey)
		{
			base.WillChangeValue (forKey);

			switch (forKey) {
			case "fractionCompleted":
				FractionCompletedChanged?.Invoke (null, null);
				break;
			case "completedUnitCount":
				CompletedUnitCountChanged?.Invoke (null, null);
				break;
			case "totalUnitCount":
				TotalUnitCountChanged?.Invoke (null, null);
				break;
			}
		}

		public override void Cancel ()
		{
			base.Cancel ();
			PhotoProgressCanceled?.Invoke (null, null);
		}

		public override void Pause ()
		{
			base.Pause ();
			PhotoProgressPaused?.Invoke (null, null);
		}
	}
}

