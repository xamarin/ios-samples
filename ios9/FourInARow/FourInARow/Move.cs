using System;

using Foundation;
using GameplayKit;

namespace FourInARow {
	public class Move : NSObject, IGKGameModelUpdate {

		public nint Value { get; set; }

		public int Column { get; private set; }

		Move (int column)
		{
			Column = column;
		}

		public static Move MoveInColumn (int column)
		{
			return new Move (column);
		}
	}
}