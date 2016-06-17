using System;
using System.Linq;

using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using GameplayKit;
using UIKit;

namespace FourInARow {
	public partial class ViewController : UIViewController {

		const int NanoSecondsPerSeond = 1000000000;

		GKMinMaxStrategist strategist;
		CAShapeLayer[][] chipLayers;
		Board board;
		UIBezierPath chipPath;

		[Export ("initWithCoder:")]
		public ViewController (NSCoder coder) : base (coder)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			strategist = new GKMinMaxStrategist {
				MaxLookAheadDepth = 7,
				RandomSource = new GKARC4RandomSource ()
			};

			var columns = new CAShapeLayer[Board.Width][];
			for (int column = 0; column < Board.Width; column++)
				columns [column] = new CAShapeLayer[Board.Height];

			chipLayers = columns;
			ResetBoard ();
		}

		public override void ViewDidLayoutSubviews ()
		{
			var button = columnButtons [0];
			nfloat lenght = NMath.Min (button.Frame.Width - 10, button.Frame.Height / 6 - 10);
			var rect = new CGRect (0f, 0f, lenght, lenght);
			chipPath = UIBezierPath.FromOval (rect);

			for (int i = 0; i < chipLayers.Length; i++) {
				for (int j = 0; j < chipLayers [i].Length; j++) {
					CAShapeLayer chip = chipLayers [i] [j];

					if (chip == null)
						continue;

					chip.Path = chipPath.CGPath;
					chip.Frame = chipPath.Bounds;
					chip.Position = PositionForChipLayerAtColumnRow (i, j);
				}
			}
		}

		partial void MakeMove (UIButton sender)
		{
			var column = (int)sender.Tag;
			if (!board.CanMoveInColumn (column))
				return;

			board.AddChipInColumn (board.CurrentPlayer.Chip, column);
			UpdateButton (sender);
			UpdateGame ();
		}

		void UpdateButton (UIControl button)
		{
			var column = (int)button.Tag;
			button.Enabled = board.CanMoveInColumn (column);
			int row = Board.Height;
			var chip = Chip.None;

			while (chip == Chip.None && row > 0)
				chip = board.ChipInColumnRow (column, --row);

			if (chip != Chip.None)
				AddChipLayerAtColumnRowColor (column, row, Player.PlayerForChip (chip).Color);
		}

		CGPoint PositionForChipLayerAtColumnRow (int column, int row)
		{
			UIButton columnButton = columnButtons [column];
			nfloat xOffset = columnButton.Frame.GetMidX ();
			nfloat yStride = chipPath.Bounds.Height + 10;
			nfloat yOffset = columnButton.Frame.GetMaxY () - yStride / 2;
			return new CGPoint (xOffset, yOffset - yStride * row);
		}

		void AddChipLayerAtColumnRowColor (int column, int row, UIColor color)
		{
			int count = chipLayers [column].Count (c => c != null);
			if (count < row + 1) {
				var newChip = (CAShapeLayer)CAShapeLayer.Create ();
				newChip.Path = chipPath.CGPath;
				newChip.Frame = chipPath.Bounds;
				newChip.FillColor = color.CGColor;
				newChip.Position = PositionForChipLayerAtColumnRow (column, row);

				View.Layer.AddSublayer (newChip);
				CABasicAnimation animation = CABasicAnimation.FromKeyPath ("position.y");
				animation.From = NSNumber.FromNFloat (newChip.Frame.Height);
				animation.To = NSNumber.FromNFloat (newChip.Position.Y);
				animation.Duration = 0.5;

				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseIn);
				newChip.AddAnimation (animation, null);
				chipLayers [column] [row] = newChip;
			}
		}

		void ResetBoard ()
		{
			board = new Board ();
			foreach (UIButton button in columnButtons)
				UpdateButton (button);

			UpdateUI ();
			strategist.GameModel = board;

			foreach (var innerArray in chipLayers) {
				for (int j = 0; j < innerArray.Length; j++)
					innerArray [j]?.RemoveFromSuperLayer ();
				Array.Clear (innerArray, 0, innerArray.Length);
			}
		}

		void UpdateGame ()
		{
			string gameOverTitle = string.Empty;

			if (board.IsWin (board.CurrentPlayer))
				gameOverTitle = string.Format ("{0} Wins!", board.CurrentPlayer.Name);
			else if (board.IsFull)
				gameOverTitle = "Draw!";

			if (!string.IsNullOrEmpty (gameOverTitle)) {
				var alert = UIAlertController.Create (gameOverTitle, null, UIAlertControllerStyle.Alert);
				var alertAction = UIAlertAction.Create ("Play Again", UIAlertActionStyle.Default, _ => ResetBoard ());
				alert.AddAction (alertAction);
				PresentViewController (alert, true, null);
				return;
			}

			board.CurrentPlayer = board.CurrentPlayer.Opponent;
			UpdateUI ();
		}

		void UpdateUI ()
		{
			NavigationItem.Title = string.Format ("{0} Turn", board.CurrentPlayer.Name);
			NavigationController.NavigationBar.BackgroundColor = board.CurrentPlayer.Color;

			#if USE_AI_PLAYER
			if (board.CurrentPlayer.Chip != Chip.Black)
				return;

			foreach (UIButton button in columnButtons)
				button.Enabled = false;

			var spinner = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			spinner.StartAnimating ();

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (spinner);

			DispatchQueue.DefaultGlobalQueue.DispatchAsync (() => {
				var startegistTime = DateTime.Now;
				int column = ColumnForAIMove ();
				var delta = DateTime.Now - startegistTime;
				var aiTimeCeiling = TimeSpan.FromSeconds (2);

				var delay = Math.Min ((aiTimeCeiling - delta).Seconds, aiTimeCeiling.Seconds);

				DispatchQueue.MainQueue.DispatchAfter (new DispatchTime (DispatchTime.Now, delay * NanoSecondsPerSeond),
					() => MakeAIMoveInColumn (column));
			});
			#endif
		}

		int ColumnForAIMove ()
		{
			var aiMove = strategist.GetBestMove (board.CurrentPlayer) as Move;
			if (aiMove == null)
				throw new Exception ("AI should always be able to move (detect endgame before invoking AI)");

			return aiMove.Column;
		}

		void MakeAIMoveInColumn (int column)
		{
			NavigationItem.LeftBarButtonItem = null;
			board.AddChipInColumn (board.CurrentPlayer.Chip, column);
			foreach (var button in columnButtons)
				UpdateButton (button);

			UpdateGame ();
		}
	}
}