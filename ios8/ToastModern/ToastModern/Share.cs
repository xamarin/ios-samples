using System;
using CoreGraphics;
using UIKit;

namespace ToastModern
{
	public class Share : Layer
	{
		public Share (Layer parent) : base (parent)
		{
			Layer screen = Screen.GlobalScreen;

			// position at bottom edge of screen
			Y = screen.Height;
			LoadImage ("camera.png");
			new Camera (this);

			var stencil = new Layer (this);
			stencil.Width = stencil.Height = 300;
			stencil.Layer.CornerRadius = stencil.Width / 2;
			stencil.Layer.BorderColor = CreateColor (255, 255, 255, 255);
			stencil.Layer.BorderWidth = 4;
			stencil.Layer.ShadowColor = CreateColor (0, 0, 0, 255);
			stencil.Layer.ShadowOpacity = 1;
			stencil.Layer.ShadowRadius = 1;
			stencil.Layer.ShadowOffset = CGSize.Empty;
			stencil.X = Width / 2 - stencil.Width / 2;
			stencil.Y = Height / 2 - stencil.Height / 2;

			var cover = new Layer (this);
			cover.Width = cover.Height = 330;
			cover.Layer.BackgroundColor = CreateColor (0, 0, 0, 255);
			cover.X = Width / 2 - cover.Width / 2;
			cover.Y = Height / 2 - cover.Height / 2;
			cover.Hidden = true;

			var toast = new Layer (this);
			toast.LoadImage ("toast.jpg");
			toast.Width = toast.Height = screen.Width;
			toast.Y = screen.Height / 2 - toast.Height / 2;
			toast.Layer.Opacity = 0;

			var keyboard = new Layer (screen);
			keyboard.LoadImage ("keyboard.png");
			keyboard.Y = screen.Height;

			var keyboardTyping = new KeyboardTyping (screen);
			keyboardTyping.Hidden = true;

			var postNavBar = new Layer (screen);
			postNavBar.LoadImage ("postNavBar.png");
			postNavBar.Y = -postNavBar.Height;

			OnTouchUp = (touches) => {
				// get position of touch
				UITouch touch = (UITouch)touches.AnyObject;
				CGPoint touchPos = touch.LocationInView (screen);

				if (touchPos.X < 100 && touchPos.Y > screen.Height - 100) {
					UIView.Animate (0.5, () => {
						Y = screen.Height;
					});
				} else if (touchPos.X > 100 && touchPos.Y > screen.Height - 100) {
					UIView.PerformWithoutAnimation (() => {
						cover.Hidden = false;
					});

					UIView.Animate (0.5, () => {
						toast.Layer.Opacity = 1;
					}, () => {
						UIView.Animate (0.5, () => {
							toast.Y = 48;
							postNavBar.Y = 0;
							keyboard.Y = screen.Height - keyboard.Height;
						}, () => {
							var firstKeyboardTypingFrame = (Layer)keyboardTyping.Subviews [0];

							UIView.PerformWithoutAnimation (() => {
								keyboardTyping.Hidden = false;
								firstKeyboardTypingFrame.Layer.Opacity = 0;
							});

							UIView.Animate (0.5, () => {
								firstKeyboardTypingFrame.Layer.Opacity = 1;
							});
						});
					});
				}
			};

			postNavBar.OnTouchUp = (touches) => {
				keyboardTyping.Hidden = true;
				UIView.Animate (0.5, () => {
					Y = screen.Height;
					toast.Y = screen.Height / 2f - toast.Height / 2f;
					toast.Layer.Opacity = 0;
					postNavBar.Y = -postNavBar.Height;
					keyboard.Y = screen.Height;
				}, () => {
					cover.Hidden = true;
				});
			};
		}
	}
}

