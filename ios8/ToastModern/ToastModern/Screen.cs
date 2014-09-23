using System;
using UIKit;

namespace ToastModern
{
	public class Screen : Layer
	{
		public static Screen GlobalScreen { get; set; }

		public Screen (UIWindow window) : base (window)
		{
		}

		public static void Init (UIWindow window)
		{
			GlobalScreen = new Screen (window);
			GlobalScreen.Size = window.Frame.Size;
			GlobalScreen.Setup ();
		}

		void Setup ()
		{
			Layer screen = GlobalScreen;

			// create map (see Map.m for map related code)
			var map = new Map (screen);
			map.LoadImage ("map.png");

			// create map cover (masks map behind the list)
			var mapCover = new Layer (screen);
			mapCover.Width = 320f;
			mapCover.Height = screen.Height - 400f;
			mapCover.Y = 400;
			mapCover.Layer.BackgroundColor = CreateColor (220, 220, 220, 255);

			// create list (see List.m for list related code)
			var list = new List (screen);
			list.LoadImage ("list.png");
			list.Y = 300f;

			// create navigation bar
			var navBar = new Layer (screen);
			navBar.LoadImage ("navBar.png");

			// create share sequence (see Share.m)
			var share = new Share (screen);

			// share (plus) button "tap"
			navBar.OnTouchUp = (touches) => {
				UIView.Animate (0.5f, () => {
					share.Y = 0;
				});
			};
		}
	}
}

