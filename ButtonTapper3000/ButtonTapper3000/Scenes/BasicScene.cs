using System;
using CoreGraphics;

using SpriteKit;
using UIKit;

namespace ButtonTapper3000 {

	public class BasicScene : SKScene {

		protected UIColor SelectedColor { get; private set; }
		protected UIColor UnselectedColor { get; private set; }
		protected UIColor ButtonColor { get; private set; }
		protected UIColor InfoColor { get; private set; }

		protected nfloat FrameMidX { get; private set; }
		protected nfloat FrameMidY { get; private set; }

		SKTransition transition;

		public BasicScene (CGSize size) : base (size)
		{
			ScaleMode = SKSceneScaleMode.AspectFill;

			BackgroundColor = UIColor.FromRGBA (0.15f, 0.15f, 0.3f, 1f);

			UnselectedColor = UIColor.FromRGBA (0f, 0.5f, 0.5f, 1f);
			SelectedColor = UIColor.FromRGBA (0.5f, 1f, 0.99f, 1f);

			ButtonColor = UIColor.FromRGBA (1f, 1f, 0f, 1f);
			InfoColor = UIColor.FromRGBA (1f, 1f, 1f, 1f);

			FrameMidX = Frame.GetMidX ();
			FrameMidY = Frame.GetMidY ();

			transition = SKTransition.MoveInWithDirection (SKTransitionDirection.Up, 0.5);
		}

		public void PresentScene (BasicScene scene)
		{
			View.PresentScene (scene, transition);
		}
	}
}