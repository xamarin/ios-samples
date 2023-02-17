namespace MessagesExtension {
	public enum ToppingType {
		topping01,
		topping02,
		topping03,
		topping04,
		topping05,
		topping06,
		topping07,
		topping08,
		topping09,
		topping10,
		topping11,
		topping12
	}

	public class Topping : IceCreamPart {
		public static Topping [] All { get; } = {
			new Topping (ToppingType.topping01),
			new Topping (ToppingType.topping02),
			new Topping (ToppingType.topping03),
			new Topping (ToppingType.topping04),
			new Topping (ToppingType.topping05),
			new Topping (ToppingType.topping06),
			new Topping (ToppingType.topping07),
			new Topping (ToppingType.topping08),
			new Topping (ToppingType.topping09),
			new Topping (ToppingType.topping10),
			new Topping (ToppingType.topping11),
			new Topping (ToppingType.topping12)
		};

		public ToppingType Type { get; set; }

		public override string QueryItemKey { get; } = "Topping";

		public Topping (ToppingType type) : base (type.ToString ())
		{
			Type = type;
		}
	}
}

