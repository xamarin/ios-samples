
namespace XamarinShot.Models {
	using CoreFoundation;
	using System;
	using UIKit;

	public class HapticsGenerator : IDisposable {
		private readonly UIImpactFeedbackGenerator impact = new UIImpactFeedbackGenerator (UIImpactFeedbackStyle.Medium);

		private readonly UISelectionFeedbackGenerator selection = new UISelectionFeedbackGenerator ();

		private readonly UINotificationFeedbackGenerator notification = new UINotificationFeedbackGenerator ();

		public void GenerateImpactFeedback ()
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.impact.ImpactOccurred ();
			});
		}

		public void GenerateSelectionFeedback ()
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.selection.SelectionChanged ();
			});
		}

		public void GenerateNotificationFeedback (UINotificationFeedbackType notificationType)
		{
			DispatchQueue.MainQueue.DispatchAsync (() => {
				this.notification.NotificationOccurred (notificationType);
			});
		}

		#region IDisposable 

		private bool isDisposed = false; // To detect redundant calls

		protected virtual void Dispose (bool disposing)
		{
			if (!this.isDisposed) {
				if (disposing) {
					this.impact.Dispose ();
					this.selection.Dispose ();
					this.notification.Dispose ();
				}

				this.isDisposed = true;
			}
		}

		public void Dispose ()
		{
			this.Dispose (true);
			GC.SuppressFinalize (true);
		}

		#endregion
	}
}
