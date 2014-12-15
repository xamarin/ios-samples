using System;
using Foundation;
using System.Collections.Generic;

namespace TicTacToe
{
	public enum TTTGameResult
	{
		InProgress,
		Victory,
		Defeat,
		Draw
	}

	[Serializable]
	public class TTTGame
	{
		const string EncodingKeyResult = "result";
		const string EncodingKeyRating = "rating";
		const string EncodingKeyDate = "date";
		const string EncodingKeyMoves = "moves";
		const string EncodingKeyCurrentPlayer = "currentPlayer";
		public TTTGameResult Result;
		public int Rating;
		public List<TTTMove> Moves;
		TTTMovePlayer currentPlayer;
		public DateTime Date;

		public TTTGame ()
		{
			Date = DateTime.Now;
			Moves = new List<TTTMove> ();
		}

		public bool CanAddMove (TTTMoveXPosition xPosition, TTTMoveYPosition yPosition)
		{
			if (Result != TTTGameResult.InProgress)
				return false;

			TTTMovePlayer player = TTTMovePlayer.None;

			return !hasMove (xPosition, yPosition, ref player);
		}

		public void AddMove (TTTMoveXPosition xPosition, TTTMoveYPosition yPosition)
		{
			if (!CanAddMove (xPosition, yPosition))
				return;

			TTTMove move = new TTTMove (currentPlayer, xPosition, yPosition);
			Moves.Add (move);

			currentPlayer = currentPlayer == TTTMovePlayer.Me ?
				TTTMovePlayer.Enemy : TTTMovePlayer.Me;

			updateGameResult ();
		}

		bool hasMove (TTTMoveXPosition xPosition, TTTMoveYPosition yPosition,
		              ref TTTMovePlayer player)
		{
			foreach (var move in Moves) {
				if (move.XPosition == xPosition && move.YPosition == yPosition) {
					player = move.Player;

					return true;
				}
			}
			return false;
		}

		bool GetWinningPlayer (out TTTMovePlayer playerOut, out TTTMoveXPosition startXPosition,
		                       out TTTMoveYPosition startYPosition, out TTTMoveXPosition endXPosition,
		                       out TTTMoveYPosition endYPosition, TTTMoveXPosition[] xPositions,
		                       TTTMoveYPosition[] yPositions)
		{
			bool moveExists = false;
			TTTMovePlayer player = TTTMovePlayer.None;

			for (int n = 0; n < TTTMove.SidePositionsCount; n++) {
				TTTMovePlayer newPlayer = TTTMovePlayer.None;
				bool newHasMove = hasMove (xPositions [n], yPositions [n], ref newPlayer);

				if (newHasMove) {
					if (moveExists) {
						if (player != newPlayer) {
							moveExists = false;
							break;
						}
					} else {
						moveExists = true;
						player = newPlayer;
					}
				} else {
					moveExists = false;
					break;
				}
			}

			if (moveExists) {
				playerOut = player;
				startXPosition = xPositions [0];
				startYPosition = yPositions [0];
				endXPosition = xPositions [TTTMove.SidePositionsCount - 1];
				endYPosition = yPositions [TTTMove.SidePositionsCount - 1];
			} else {
				playerOut = TTTMovePlayer.None;
				startXPosition = TTTMoveXPosition.Center;
				startYPosition = TTTMoveYPosition.Center;
				endXPosition = TTTMoveXPosition.Center;
				endYPosition = TTTMoveYPosition.Center;
			}

			return moveExists;
		}

		bool GetWinningPlayer (out TTTMovePlayer player, out TTTMoveXPosition startXPosition,
		                       out TTTMoveYPosition startYPosition, out TTTMoveXPosition endXPosition,
		                       out TTTMoveYPosition endYPosition, TTTMoveXPosition xPosition)
		{
			TTTMoveXPosition[] xPositions = new TTTMoveXPosition[TTTMove.SidePositionsCount];

			for (int n = 0; n < TTTMove.SidePositionsCount; n++)
				xPositions [n] = xPosition;

			TTTMoveYPosition[] yPositions = new TTTMoveYPosition[TTTMove.SidePositionsCount];
			yPositions [0] = TTTMoveYPosition.Top;
			yPositions [1] = TTTMoveYPosition.Center;
			yPositions [2] = TTTMoveYPosition.Bottom;

			return GetWinningPlayer (out player, out startXPosition, out startYPosition, out endXPosition,
			                         out endYPosition, xPositions, yPositions);
		}

		bool GetWinningPlayer (out TTTMovePlayer player, out TTTMoveXPosition startXPosition,
		                       out TTTMoveYPosition startYPosition, out TTTMoveXPosition endXPosition,
		                       out TTTMoveYPosition endYPosition, TTTMoveYPosition yPosition)
		{
			TTTMoveYPosition[] yPositions = new TTTMoveYPosition[TTTMove.SidePositionsCount];
			for (int n = 0; n < TTTMove.SidePositionsCount; n++)
				yPositions [n] = yPosition;

			TTTMoveXPosition[] xPositions = new TTTMoveXPosition[TTTMove.SidePositionsCount];
			xPositions [0] = TTTMoveXPosition.Left;
			xPositions [1] = TTTMoveXPosition.Center;
			xPositions [2] = TTTMoveXPosition.Right;

			return GetWinningPlayer (out player, out startXPosition, out startYPosition,
			                         out endXPosition, out endYPosition, xPositions, yPositions);
		}

		bool GetWinningPlayer (out TTTMovePlayer player, out TTTMoveXPosition startXPosition,
		                       out TTTMoveYPosition startYPosition, out TTTMoveXPosition endXPosition,
		                       out TTTMoveYPosition endYPosition, int direction)
		{
			TTTMoveXPosition[] xPositions = new TTTMoveXPosition[TTTMove.SidePositionsCount];
			TTTMoveYPosition[] yPositions = new TTTMoveYPosition[TTTMove.SidePositionsCount];
			int n = 0;

			for (var xPosition = TTTMoveXPosition.Left; xPosition <= TTTMoveXPosition.Right; xPosition++) {
				xPositions [n] = xPosition;
				yPositions [n] = (TTTMoveYPosition)((int)xPosition * direction);
				n++;
			}

			return GetWinningPlayer (out player, out startXPosition, out startYPosition,
			                         out endXPosition, out endYPosition, xPositions, yPositions);
		}

		public bool GetWinningPlayer (out TTTMovePlayer player, out TTTMoveXPosition startXPosition,
		                              out TTTMoveYPosition startYPosition, out TTTMoveXPosition endXPosition,
		                              out TTTMoveYPosition endYPosition)
		{
			bool hasWinner;
			for (var xPosition = TTTMoveXPosition.Left; xPosition <= TTTMoveXPosition.Right; xPosition++) {
				hasWinner = GetWinningPlayer (out player, out startXPosition, out startYPosition,
				                              out endXPosition, out endYPosition, xPosition);
				if (hasWinner)
					return hasWinner;
			}

			for (var yPosition = TTTMoveYPosition.Top; yPosition <= TTTMoveYPosition.Bottom; yPosition++) {
				hasWinner = GetWinningPlayer (out player, out startXPosition, out startYPosition,
				                              out endXPosition, out endYPosition, yPosition);

				if (hasWinner)
					return hasWinner;
			}

			hasWinner = GetWinningPlayer (out player, out startXPosition, out startYPosition,
			                              out endXPosition, out endYPosition, 1);
			if (hasWinner)
				return hasWinner;

			hasWinner = GetWinningPlayer (out player, out startXPosition, out startYPosition,
			                              out endXPosition, out endYPosition, -1);

			return hasWinner;
		}

		TTTGameResult calculateGameResult ()
		{
			TTTMovePlayer player;
			TTTMoveXPosition startXPosition, endXPosition;
			TTTMoveYPosition startYPosition, endYPosition;

			bool hasWinner = GetWinningPlayer (out player, out startXPosition,
			                                   out startYPosition, out endXPosition,
			                                   out endYPosition);

			if (hasWinner)
				return player == TTTMovePlayer.Me ? TTTGameResult.Victory : TTTGameResult.Defeat;

			if (Moves.Count == TTTMove.SidePositionsCount * TTTMove.SidePositionsCount)
				return TTTGameResult.Draw;

			return TTTGameResult.InProgress;
		}

		void updateGameResult ()
		{
			if (Result == TTTGameResult.InProgress)
				Result = calculateGameResult ();
		}
	}
}

