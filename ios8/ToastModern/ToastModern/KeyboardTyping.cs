using System;
using UIKit;

namespace ToastModern
{
	public class KeyboardTyping : Layer
	{
		public int KeyboardTypingFrame { get; private set; }

		public override bool Hidden {
			get {
				return base.Hidden;
			}
			set {
				base.Hidden = value;

				if (value) {
					UIView.PerformWithoutAnimation (() => {
						for (int i = 0; i < Subviews.Length; i++) {
							var frame = (Layer)Subviews [i];
							frame.Hidden = i != 0;
						}
					});

					KeyboardTypingFrame = 0;
				}

			}
		}

		public KeyboardTyping (Layer parent) : base (parent)
		{
			Width = Screen.GlobalScreen.Width;
			Height = Screen.GlobalScreen.Height;

			for (int i = 1; i <= 18; i++) {
				var frame = new Layer (this);

				string imageName = i < 10 ? string.Format ("typing.00{0}.png", i) : string.Format ("typing.0{0}.png", i);
				frame.LoadImage (imageName);

				frame.OnTouchDown = (touches) => {
					NextFrame ();
				};

				frame.Hidden = i != 1;
			}

			KeyboardTypingFrame = 1;
		}

		void NextFrame ()
		{
			if (KeyboardTypingFrame < Subviews.Length) {
				UIView.PerformWithoutAnimation (() => {
					var frame = Subviews [KeyboardTypingFrame];
					frame.Hidden = false;
				});
			}

			KeyboardTypingFrame++;
		}
	}
}

