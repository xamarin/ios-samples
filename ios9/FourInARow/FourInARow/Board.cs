using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using GameplayKit;

namespace FourInARow {
	public class Board : NSObject, IGKGameModel {

		public const int Width = 7;
		public const int Height = 6;

		public Chip[] Cells { get; private set; }

		public Player CurrentPlayer { get; set; }

		public bool IsFull {
			get {
				for (int column = 0; column < Width; column++) {
					if (CanMoveInColumn (column))
						return false;
				}

				return true;
			}
		}

		public Board ()
		{
			CurrentPlayer = Player.RedPlayer;
			Cells = new Chip [Width * Height];
		}

		public Chip ChipInColumnRow (int column, int row)
		{
			var index = row + column * Height;
			return (index >= Cells.Length) ? Chip.None : Cells [index];
		}

		public bool CanMoveInColumn (int column)
		{
			return NextEmptySlotInColumn (column) >= 0;
		}

		public void AddChipInColumn (Chip chip, int column)
		{
			int row = NextEmptySlotInColumn (column);
			if (row >= 0)
				SetChipInColumnRow (chip, column, row);
		}

		public IGKGameModelPlayer[] GetPlayers ()
		{
			return Player.AllPlayers;
		}

		public IGKGameModelPlayer GetActivePlayer ()
		{
			return CurrentPlayer;
		}

		public NSObject Copy (NSZone zone)
		{
			var board = new Board ();
			board.SetGameModel (this);
			return board;
		}

		public void SetGameModel (IGKGameModel gameModel)
		{
			var board = (Board)gameModel;
			UpdateChipsFromBoard (board);
			CurrentPlayer = board.CurrentPlayer;
		}

		public IGKGameModelUpdate[] GetGameModelUpdates (IGKGameModelPlayer player)
		{
			var moves = new List<Move> ();

			for (int column = 0; column < Width; column++) {
				if (CanMoveInColumn (column))
					moves.Add (Move.MoveInColumn (column));
			}

			return moves.ToArray ();
		}

		public void ApplyGameModelUpdate (IGKGameModelUpdate gameModelUpdate)
		{
			AddChipInColumn (CurrentPlayer.Chip, ((Move)gameModelUpdate).Column);
			CurrentPlayer = CurrentPlayer.Opponent;
		}

		[Export ("scoreForPlayer:")]
		public nint ScorForPlayer (Player player)
		{
			var playerRunCounts = RunCountsForPlayer (player);
			int playerTotal = playerRunCounts.Sum ();

			var opponentRunCounts = RunCountsForPlayer (player.Opponent);
			int opponentTotal = opponentRunCounts.Sum ();

			// Return the sum of player runs minus the sum of opponent runs.
			return playerTotal - opponentTotal;
		}

		public bool IsWin (Player player)
		{
			var runCounts = RunCountsForPlayer (player);
			return runCounts.Max () >= Player.CountToWin;
		}

		bool IsLoss (Player player)
		{
			return IsWin (player.Opponent);
		}

		int[] RunCountsForPlayer (Player player)
		{
			var chip = player.Chip;
			var counts = new List<int> ();

			// Detect horizontal runs.
			for (int row = 0; row < Height; row++) {
				int runCount = 0;
				for (int column = 0; column < Width; column++) {
					if (ChipInColumnRow (column, row) == chip) {
						++runCount;
					} else {
						if (runCount > 0)
							counts.Add (runCount);
						runCount = 0;
					}
				}

				if (runCount > 0)
					counts.Add (runCount);
			}

			// Detect vertical runs.
			for (int column = 0; column < Width; column++) {
				int runCount = 0;
				for (int row = 0; row < Height; row++) {
					if (ChipInColumnRow (column, row) == chip) {
						++runCount;
					} else {
						if (runCount > 0)
							counts.Add (runCount);
						runCount = 0;
					}
				}

				if (runCount > 0)
					counts.Add (runCount);
			}

			// Detect diagonal (northeast) runs
			for (int startColumn = -Height; startColumn < Width; startColumn++) {
				int runCount = 0;
				for (int offset = 0; offset < Height; offset++) {
					int column = startColumn + offset;
					if (column < 0 || column > Width)
						continue;

					if (ChipInColumnRow (column, offset) == chip) {
						++runCount;
					} else {
						if (runCount > 0)
							counts.Add (runCount);
						runCount = 0;
					}
				}

				if (runCount > 0)
					counts.Add (runCount);
			}

			// Detect diagonal (northwest) runs
			for (int startColumn = 0; startColumn < Width + Height; startColumn++) {
				int runCount = 0;
				for (int offset = 0; offset < Height; offset++) {
					int column = startColumn - offset;
					if (column < 0 || column > Width)
						continue;

					if (ChipInColumnRow (column, offset) == chip) {
						++runCount;
					} else {
						if (runCount > 0)
							counts.Add (runCount);
						runCount = 0;
					}
				}

				if (runCount > 0)
					counts.Add (runCount);
			}

			return counts.ToArray ();
		}

		void UpdateChipsFromBoard (Board otherBoard)
		{
			Array.Copy (otherBoard.Cells, Cells, Cells.Length);
		}

		void SetChipInColumnRow (Chip chip, int column, int row)
		{
			Cells [row + column * Height] = chip;
		}

		int NextEmptySlotInColumn (int column)
		{
			for (int row = 0; row < Height; row++) {
				if (ChipInColumnRow (column, row) == Chip.None)
					return row;
			}

			return -1;
		}
	}
}