using Foundation;
using GameplayKit;

namespace FourInARow {
	public class Move : NSObject, IGKGameModelUpdate {

		int valueBehind;
		public int Value {
			get {
				return valueBehind;
			}
			[Export ("setValue:")]
			private set {
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