using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace HelloGoodbye
{
	public class PreviewLabel : UILabel
	{
		public event EventHandler ActivatePreviewLabel;

		public override UIAccessibilityTrait AccessibilityTraits {
			get {
				return base.AccessibilityTraits | UIAccessibilityTrait.Button;
			}
			set {
				base.AccessibilityTraits = value;
			}
		}

		public PreviewLabel ()
		{
			Text = "Preview".LocalizedString("Name of the card preview tab");
			Font = StyleUtilities.LargeFont;
			TextColor = StyleUtilities.PreviewTabLabelColor;
		}

		public override bool AccessibilityActivate ()
		{
			EventHandler handler = ActivatePreviewLabel;
			if (handler != null)
				handler (this, EventArgs.Empty);

			return true;
		}
	}
}

