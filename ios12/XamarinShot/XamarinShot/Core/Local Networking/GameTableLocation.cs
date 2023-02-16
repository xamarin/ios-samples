
namespace XamarinShot.Models {
	using System.Collections.Generic;

	public class GameTableLocation {
		private static Dictionary<int, GameTableLocation> Locations = new Dictionary<int, GameTableLocation> ();

		private readonly string name;

		public GameTableLocation (int identifier)
		{
			this.Identifier = identifier;
			this.name = $"Table {this.Identifier}";
		}

		public int Identifier { get; private set; }

		public override int GetHashCode ()
		{
			return this.Identifier.GetHashCode ();
		}

		public static GameTableLocation GetLocation (int identifier)
		{
			if (!Locations.TryGetValue (identifier, out GameTableLocation location)) {
				location = new GameTableLocation (identifier);
				Locations [identifier] = location;
			}

			return location;
		}

		public override bool Equals (object obj)
		{
			return this.Identifier == (obj as GameTableLocation)?.Identifier;
		}

		public static bool operator == (GameTableLocation lhs, GameTableLocation rhs)
		{
			return lhs?.Identifier == rhs?.Identifier;
		}

		public static bool operator != (GameTableLocation lhs, GameTableLocation rhs)
		{
			return lhs?.Identifier != rhs?.Identifier;
		}
	}
}
