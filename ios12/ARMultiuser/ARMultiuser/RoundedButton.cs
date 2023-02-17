
namespace ARMultiuser {
	using CoreGraphics;
	using Foundation;
	using UIKit;

	/// <summary>
	/// A custom button that stands out over the camera view.
	/// </summary>
	[Register ("RoundedButton")]
	public class RoundedButton : UIButton {
		[Export ("initWithCoder:")]
		public RoundedButton (NSCoder coder) : base (coder)
		{
			this.Setup ();
		}

		[Export ("initWithFrame:")]
		public RoundedButton (CGRect frame) : base (frame)
		{
			this.Setup ();
		}

		private void Setup ()
		{
			this.BackgroundColor = this.TintColor;
			this.Layer.CornerRadius = 8f;
			this.ClipsToBounds = true;
			this.SetTitleColor (UIColor.White, UIControlState.Normal);
			if (this.TitleLabel != null) {
				this.TitleLabel.Font = UIFont.BoldSystemFontOfSize (17f);
			}
		}

		public override bool Enabled {
			get => base.Enabled;

			set {
				base.Enabled = value;
				this.BackgroundColor = value ? this.TintColor : UIColor.Gray;
			}
		}
	}
}
