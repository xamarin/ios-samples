using System;

using UIKit;

namespace HapticFeedback
{
	public partial class ViewController : UIViewController
	{
		#region Constructors
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}
		#endregion

		#region Custom Actions
		partial void ImpactAction(Foundation.NSObject sender)
		{
			// Initialize feedback
			var impact = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
			impact.Prepare();

			// Trigger feedback
			impact.ImpactOccurred();
		}

		partial void NotificationAction(Foundation.NSObject sender)
		{
			// Initialize feedback
			var notification = new UINotificationFeedbackGenerator();
			notification.Prepare();

			// Trigger feedback
			notification.NotificationOccurred(UINotificationFeedbackType.Error);
		}

		partial void SelectionAction(Foundation.NSObject sender)
		{
			// Initialize feedback
			var selection = new UISelectionFeedbackGenerator();
			selection.Prepare();

			// Trigger feedback
			selection.SelectionChanged();
		}
		#endregion
	}
}
