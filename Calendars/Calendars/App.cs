using System;
using EventKit;

namespace Calendars
{
	/// <summary>
	/// Singleton class for Application wide objects. In this sample, we use it to
	/// maintain a single instance of the EKEventStore.
	/// </summary>
	public class App
	{
		public static App Current
		{
			get { return current; }
		}
		private static App current;

		/// <summary>
		/// The EKEventStore is intended to be long-lived. It's expensive to new it up
		/// and can be thought of as a database, so we create a single instance of it
		/// and reuse it throughout the app
		/// </summary>
		public EKEventStore EventStore
		{
			get { return eventStore; }
		}
		protected EKEventStore eventStore;


		static App ()
		{
			current = new App();
		}
		protected App () 
		{
			eventStore = new EKEventStore ( );
		}
	}
}

