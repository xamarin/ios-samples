using System;
using System.Drawing;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

namespace ButtonTapper3000 {

	public class ResultsScreen : BasicScene {

		SKLabelNode doneButton;

		public ResultsScreen (SizeF size) : base (size)
		{
			var timeLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Time: " + GameInfo.GameTimeInSeconds,
				FontSize = 24,
				Position = new PointF (FrameMidX, FrameMidY + 120)
			};
			var modeLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Mode: " + GameInfo.GameMode,
				FontSize = 24,
				Position = new PointF (FrameMidX, FrameMidY + 60)
			};
			var scoreLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Score: " + GameInfo.CurrentTaps,
				FontSize = 30,
				Position = new PointF (FrameMidX, FrameMidY)
			};
			doneButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Done",
				FontSize = 24,
				FontColor = ButtonColor,
				Position = new PointF (FrameMidX, FrameMidY - 90)
			};
			AddChild (timeLabel);
			AddChild (modeLabel);
			AddChild (scoreLabel);
			AddChild (doneButton);
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				PointF location = (touch as UITouch).LocationInNode (this);
				if (doneButton.ContainsPoint (location))
					PresentScene (new MainMenu (View.Bounds.Size));
			}
		}
	}
}