using Foundation;
using HomeKit;

namespace HomeKitCatalog
{
	/// <summary>
	/// The `HomeStore` class is a simple singleton class which holds a home manager and the current selected home.
	/// </summary>
	public class HomeStore : NSObject, IHMHomeManagerDelegate
	{
		public static readonly HomeStore SharedStore = new HomeStore ();

		// The current 'selected' home.
		public HMHome Home { get; set; }

		readonly HMHomeManager homeManager;
		public HMHomeManager HomeManager {
			get {
				return homeManager;
			}
		}

		HomeStore ()
		{
			homeManager = new HMHomeManager ();
		}
	}
}