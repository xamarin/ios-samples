using Foundation;

namespace Emporium
{
	public class ProductContainer : DictionaryContainer
	{
		static readonly NSString NameKey = (NSString)"name";
		static readonly NSString DescriptionKey = (NSString)"description";
		static readonly NSString PriceKey = (NSString)"price";

		public string Name {
			get {
				return GetStringValue (NameKey);
			}
			set {
				SetStringValue (NameKey, value);
			}
		}

		public string Description {
			get {
				return GetStringValue (DescriptionKey);
			}
			set {
				SetStringValue (DescriptionKey, value);
			}
		}

		public string Price {
			get {
				return GetStringValue (PriceKey);
			}
			set {
				SetStringValue (PriceKey, value);
			}
		}

		public Product Product {
			get {
				return new Product {
					Name = Name,
					Description = Description,
					Price = Price
				};
			}
		}

		public ProductContainer ()
		{
		}

		public ProductContainer (NSDictionary dictionary)
			: base (dictionary)
		{
		}
	}
}
