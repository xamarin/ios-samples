using Foundation;

namespace Emporium
{
	public class Product : NSObject
	{
		public string Name { get; set; }

		new public string Description { get; set; }

		public string Price { get; set; }
	}
}