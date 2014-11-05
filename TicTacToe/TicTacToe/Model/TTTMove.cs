using System;
using Foundation;

namespace TicTacToe
{
	public enum TTTMovePlayer
	{
		Me,
		Enemy,
		None
	}

	public enum TTTMoveXPosition
	{
		Left = -1,
		Center = 0,
		Right = 1
	}

	public enum TTTMoveYPosition
	{
		Top = -1,
		Center = 0,
		Bottom = 1
	}

	[Serializable]
	public class TTTMove
	{
		public const int SidePositionsCount = 3;
		public TTTMovePlayer Player;
		public TTTMoveXPosition XPosition;
		public TTTMoveYPosition YPosition;
		const string EncodingKeyPlayer = "player";
		const string EncodingKeyXPosition = "xPosition";
		const string EncodingKeyYPosition = "yPosition";

		public TTTMove () : this (TTTMovePlayer.Me, TTTMoveXPosition.Center,
		                          TTTMoveYPosition.Center)
		{
		}

		public TTTMove (TTTMovePlayer player, TTTMoveXPosition xPosition,
		                TTTMoveYPosition yPosition)
		{
			Player = player;
			XPosition = xPosition;
			YPosition = yPosition;
		}

		public override int GetHashCode ()
		{
			int hash = 1;
			hash = 31 * hash + (int)Player;
			hash = 31 * hash + (int)XPosition + 1;
			hash = 31 * hash + (int)YPosition + 1;
			return hash;
		}

		public override bool Equals (object obj)
		{
			if (obj is TTTMove)
				return false;

			TTTMove move = (TTTMove)obj;
			return (Player == move.Player && XPosition == move.XPosition &&
				YPosition == move.YPosition);
		}
	}
}

