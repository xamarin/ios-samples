using System;
using Foundation;
using UIKit;

namespace tvRemote
{
	public partial class EventView : UIView
	{
		#region Computed Properties
		public override bool CanBecomeFocused {
			get {
				return true;
			}
		}
		#endregion

		#region 
		public EventView (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void PressesBegan (NSSet<UIPress> presses, UIPressesEvent evt)
		{
			base.PressesBegan (presses, evt);

			foreach (UIPress press in presses) {
				// Was the Touch Surface clicked?
				if (press.Type == UIPressType.Select) {
					BackgroundColor = UIColor.Red;
				}
			}
		}

		public override void PressesCancelled (NSSet<UIPress> presses, UIPressesEvent evt)
		{
			base.PressesCancelled (presses, evt);

			foreach (UIPress press in presses) {
				// Was the Touch Surface clicked?
				if (press.Type == UIPressType.Select) {
					BackgroundColor = UIColor.Clear;
				}
			}
		}

		public override void PressesChanged (NSSet<UIPress> presses, UIPressesEvent evt)
		{
			base.PressesChanged (presses, evt);
		}

		public override void PressesEnded (NSSet<UIPress> presses, UIPressesEvent evt)
		{
			base.PressesEnded (presses, evt);

			foreach (UIPress press in presses) {
				// Was the Touch Surface clicked?
				if (press.Type == UIPressType.Select) {
					BackgroundColor = UIColor.Clear;
				}
			}
		}
		#endregion
	}
}
