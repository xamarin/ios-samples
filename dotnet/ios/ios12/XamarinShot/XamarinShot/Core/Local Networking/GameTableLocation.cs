namespace XamarinShot.Models;

public class GameTableLocation
{
        static Dictionary<int, GameTableLocation> Locations = new Dictionary<int, GameTableLocation> ();

        readonly string name;

        public GameTableLocation (int identifier)
        {
                Identifier = identifier;
                name = $"Table {Identifier}";
        }

        public int Identifier { get; private set; }

        public override int GetHashCode ()
        {
                return Identifier.GetHashCode ();
        }

        public static GameTableLocation GetLocation (int identifier)
        {
                if (!Locations.TryGetValue (identifier, out GameTableLocation? location))
                {
                        location = new GameTableLocation (identifier);
                        Locations [identifier] = location;
                }

                return location;
        }

        public override bool Equals (object? obj)
        {
                return Identifier == (obj as GameTableLocation)?.Identifier;
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
