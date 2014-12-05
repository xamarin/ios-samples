using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace ButtonTapper3000 {

	public class GameSetupMenu : BasicScene {

		SKLabelNode [] timeLabels;
		SKLabelNode [] modeLabels;
		SKLabelNode startButton;
		SKLabelNode backButton;
	
		public GameSetupMenu (CGSize size) : base (size)
		{
			SKLabelNode timeLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Time",
				FontSize = 24,
				Position = new CGPoint (FrameMidX, FrameMidY + 70)
			};

			SKLabelNode time15Button = new SKLabelNode ("GillSans-Bold") {
				Text = "15",
				FontSize = 14,
				FontColor = UnselectedColor,
				Position = new CGPoint (FrameMidX - 40, FrameMidY + 40)
			};
			SKLabelNode time30Button = new SKLabelNode ("GillSans-Bold") {
				Text = "30",
				FontSize = 14,
				FontColor = UnselectedColor,
				Position = new CGPoint (FrameMidX, FrameMidY + 40)
			};
			SKLabelNode time45Button = new SKLabelNode ("GillSans-Bold") {
				Text = "45",
				FontSize = 14,
				FontColor = UnselectedColor,
				Position = new CGPoint (FrameMidX + 40, FrameMidY + 40)
			};
			timeLabels = new SKLabelNode[] { time15Button, time30Button, time45Button };

			SKLabelNode modeLabel = new SKLabelNode ("GillSans-Bold") {
				Text = "Mode",
				FontSize = 24,
				Position = new CGPoint (FrameMidX, FrameMidY)
			};
			SKLabelNode modeEasyButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Easy",
				FontSize = 14,
				FontColor = UnselectedColor,
				Position = new CGPoint (FrameMidX - 40, FrameMidY - 40)
			};
			SKLabelNode modeHardButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Hard",
				FontSize = 14,
				FontColor = UnselectedColor,
				Position = new CGPoint (FrameMidX + 40, FrameMidY - 40)
			};
			modeLabels = new [] { modeEasyButton, modeHardButton };

			startButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Start!",
				FontSize = 30,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 100)
			};
			backButton = new SKLabelNode ("GillSans-Bold") {
				Text = "Back",
				FontSize = 18,
				FontColor = ButtonColor,
				Position = new CGPoint (FrameMidX, FrameMidY - 200)
			};

			timeLabels [(int)GameInfo.GameTime].FontColor = SelectedColor;
			modeLabels [(int)GameInfo.GameMode].FontColor = SelectedColor;

			GameInfo.ResetGame ();

			AddChild (timeLabel);
			foreach (var button in timeLabels)
				AddChild (button);

			AddChild (modeLabel);
			foreach (var button in modeLabels)
				AddChild (button);

			AddChild (startButton);
			AddChild (backButton);
		}

		void SelectTime (int time)
		{
			timeLabels [(int)GameInfo.GameTime].FontColor = UnselectedColor;
			GameInfo.GameTime = (GameTime)time;
			timeLabels [time].FontColor = SelectedColor;
		}

		void SelectMode (int mode)
		{
			modeLabels [(int)GameInfo.GameMode].FontColor = UnselectedColor;
			GameInfo.GameMode = (GameMode)mode;
			modeLabels [mode].FontColor = SelectedColor;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches) {
				CGPoint location = (touch as UITouch).LocationInNode (this);

				for (int i = 0; i < (int)GameTime.Max; i++) {
					if (timeLabels [i].ContainsPoint (location)) {
						SelectTime (i);
						break;
					}
				}

				for (int i = 0; i < (int)GameMode.Max; i++) {
					if (modeLabels [i].ContainsPoint (location)) {
						SelectMode (i);
						break;
					}
				}

				if (startButton.ContainsPoint (location)) {
					PresentScene (new MainGame (View.Bounds.Size));
				} else if (backButton.ContainsPoint (location)) {
					PresentScene (new MainMenu (View.Bounds.Size));
				}
			}
		}

		public override void Update (double currentTime)
		{
		}
	}
}