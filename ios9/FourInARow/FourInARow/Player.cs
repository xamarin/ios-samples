using Foundation;
using GameplayKit;
using UIKit;

namespace FourInARow {
	public enum Chip {
		None = 0,
		Red,
		Black
	}

	public class Player : NSObject, IGKGameModelPlayer {

		public const int CountToWin = 4;

		public static Player RedPlayer {
			get {
				return PlayerForChip (Chip.Red);
			}
		}

		public static Player BlackPlayer {
			get {
				return PlayerForChip (Chip.Black);
			}
		}

		public int PlayerID {
			[Export ("playerId")]
			get {
				return (int)Chip;
			}
		}

		public Chip Chip { get; private set; }

		public UIColor Color {
			get {
				switch (Chip) {
				case Chip.Red:
					return UIColor.Red;
				case Chip.Black:
					return UIColor.Black;
				default:
					return UIColor.Clear;
				}
			}
		}

		public string Name {
			get {
				switch (Chip) {
				case Chip.Red:
					return "Red";
				case Chip.Black:
					return "Black";
				default:
					return string.Empty;
				}
			}
		}

		public Player Opponent { 
			get {
				switch (Chip) {
				case Chip.Red:
					return Player.BlackPlayer;
				case Chip.Black:
					return Player.RedPlayer;
				default:
					return null;
				}
			}
		}

		static Player[] allPlayers;
		public static Player[] AllPlayers {
			get {
				allPlayers = allPlayers ?? new [] {
					new Player (Chip.Red),
					new Player (Chip.Black)
				};
				return allPlayers;
			}
		}

		public Player (Chip chip)
		{
			Chip = chip;
		}

		public static Player PlayerForChip (Chip chip)
		{
			return (chip == Chip.None) ? null : AllPlayers [(int)chip - 1];
		}
	}
}