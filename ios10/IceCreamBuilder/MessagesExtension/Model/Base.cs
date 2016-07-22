namespace MessagesExtension {
	public enum BaseType {
		base01,
		base02,
		base03,
		base04
	}

	public class Base : IceCreamPart {
		public static Base[] All { get; } = {
			new Base (BaseType.base01),
			new Base (BaseType.base02),
			new Base (BaseType.base03),
			new Base (BaseType.base04)
		};

		public BaseType Type { get; set; }

		public override string QueryItemKey { get; } = "Base";

		public Base (BaseType type) : base (type.ToString ())
		{
			Type = type;
		}
	}
}

