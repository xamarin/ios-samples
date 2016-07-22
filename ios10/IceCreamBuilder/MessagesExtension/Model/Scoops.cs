namespace MessagesExtension {
	public enum ScoopsType {
		scoops01,
		scoops02,
		scoops03,
		scoops04,
		scoops05,
		scoops06,
		scoops07,
		scoops08,
		scoops09,
		scoops10
	}

	public class Scoops : IceCreamPart {
		public static Scoops[] All { get; } = {
			new Scoops (ScoopsType.scoops01),
			new Scoops (ScoopsType.scoops02),
			new Scoops (ScoopsType.scoops03),
			new Scoops (ScoopsType.scoops04),
			new Scoops (ScoopsType.scoops05),
			new Scoops (ScoopsType.scoops06),
			new Scoops (ScoopsType.scoops07),
			new Scoops (ScoopsType.scoops08),
			new Scoops (ScoopsType.scoops09),
			new Scoops (ScoopsType.scoops10)
		};

		public ScoopsType Type { get; set; }

		public override string QueryItemKey { get; } = "Scoops";

		public Scoops (ScoopsType type) : base (type.ToString ())
		{
			Type = type;
		}
	}
}

