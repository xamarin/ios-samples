using System;

using Foundation;
using GameplayKit;

namespace FourInARow {
	public class Move : NSObject, IGKGameModelUpdate {

		nint valueBehind;

		public nint Value {
			[Export ("value")]
			get {
				return valueBehind;
			}
			[Export ("setValue:")]
			set {
				valueBehind = value;
			}
		}

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